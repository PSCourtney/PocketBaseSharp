using PocketBaseSharp.Event;
using PocketBaseSharp.Models;
using PocketBaseSharp.Models.Files;
using PocketBaseSharp.Services;
using System.Collections;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using FluentResults;
using PocketBaseSharp.Errors;

namespace PocketBaseSharp
{
    /// <summary>
    /// Main client class for interacting with PocketBase backend services.
    /// Provides access to all PocketBase APIs including authentication, collections, real-time subscriptions, and more.
    /// </summary>
    public class PocketBase
    {
        #region Private Fields
        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        private Dictionary<string, RecordService> recordServices = new Dictionary<string, RecordService>();
        #endregion

        #region Events
        /// <summary>
        /// Event handler delegate for intercepting HTTP requests before they are sent.
        /// </summary>
        /// <param name="sender">The PocketBase instance</param>
        /// <param name="e">Request event arguments containing the request details</param>
        /// <returns>The potentially modified HTTP request message</returns>
        public delegate HttpRequestMessage BeforeSendEventHandler(object sender, RequestEventArgs e);
        
        /// <summary>
        /// Event fired before each HTTP request is sent, allowing for request modification.
        /// </summary>
        public event BeforeSendEventHandler? BeforeSend;

        /// <summary>
        /// Event handler delegate for processing HTTP responses after they are received.
        /// </summary>
        /// <param name="sender">The PocketBase instance</param>
        /// <param name="e">Response event arguments containing the response details</param>
        public delegate void AfterSendEventHandler(object sender, ResponseEventArgs e);
        
        /// <summary>
        /// Event fired after each HTTP response is received, allowing for response processing.
        /// </summary>
        public event AfterSendEventHandler? AfterSend;
        #endregion

        /// <summary>
        /// Gets the authentication store for managing user authentication state.
        /// </summary>
        public AuthStore AuthStore { private set; get; }
        
        /// <summary>
        /// Gets the admin service for managing administrative operations.
        /// </summary>
        public AdminService Admin { private set; get; }
        
        /// <summary>
        /// Gets the user service for managing user operations.
        /// </summary>
        public UserService User { private set; get; }
        
        /// <summary>
        /// Gets the log service for accessing request logs and statistics.
        /// </summary>
        public LogService Log { private set; get; }
        
        /// <summary>
        /// Gets the settings service for managing application settings.
        /// </summary>
        public SettingsService Settings { private set; get; }
        
        /// <summary>
        /// Gets the collection service for managing database collections.
        /// </summary>
        public CollectionService Collections { private set; get; }
        
        /// <summary>
        /// Gets the real-time service for managing WebSocket connections and subscriptions.
        /// </summary>
        public RealTimeService RealTime { private set; get; }
        
        /// <summary>
        /// Gets the health service for checking server health status.
        /// </summary>
        public HealthService Health { private set; get; }
        
        /// <summary>
        /// Gets the backup service for managing database backups.
        /// </summary>
        public BackupService Backup { private set; get; }

        /// <summary>
        /// Gets the batch service for performing multiple operations in a single request.
        /// </summary>
        public BatchService Batch { private set; get; }

        private readonly string _baseUrl;
        private readonly string _language;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the PocketBase client.
        /// </summary>
        /// <param name="baseUrl">The base URL of the PocketBase server (e.g., "http://localhost:8090")</param>
        /// <param name="authStore">Optional authentication store for managing auth state. If null, a new AuthStore will be created</param>
        /// <param name="language">The preferred language for API responses (default: "en-US")</param>
        /// <param name="httpClient">Optional HttpClient instance. If null, a new HttpClient will be created</param>
        public PocketBase(string baseUrl, AuthStore? authStore = null, string language = "en-US", HttpClient? httpClient = null)
        {
            this._baseUrl = baseUrl;
            this._language = language;
            this._httpClient = httpClient ?? new HttpClient();

            AuthStore = authStore ?? new AuthStore();
            Admin = new AdminService(this);
            User = new UserService(this);
            Log = new LogService(this);
            Settings = new SettingsService(this);
            Collections = new CollectionService(this);
            //Records = new RecordService(this);
            RealTime = new RealTimeService(this);
            Health = new HealthService(this);
            Backup = new BackupService(this);
            Batch = new BatchService(this);
        }

        /// <summary>
        /// Creates an authentication service for a specific collection that stores auth records.
        /// </summary>
        /// <typeparam name="T">The type of the auth model, must implement IBaseAuthModel</typeparam>
        /// <param name="collectionName">The name of the collection containing auth records</param>
        /// <returns>A CollectionAuthService instance for the specified collection</returns>
        public CollectionAuthService<T> AuthCollection<T>(string collectionName)
            where T : IBaseAuthModel
        {
            return new CollectionAuthService<T>(this, collectionName);
        }

        /// <summary>
        /// Gets or creates a record service for the specified collection.
        /// Services are cached and reused for the same collection name.
        /// </summary>
        /// <param name="collectionName">The name of the collection</param>
        /// <returns>A RecordService instance for the specified collection</returns>
        public RecordService Collection(string collectionName)
        {
            if (recordServices.ContainsKey(collectionName))
            {
                return recordServices[collectionName];
            }
            var newService = new RecordService(this, collectionName);
            recordServices[collectionName] = newService;
            return newService;
        }

        /// <summary>
        /// Creates a new batch builder for fluent batch operations
        /// </summary>
        /// <returns>A new BatchBuilder instance</returns>
        public BatchBuilder CreateBatch()
        {
            return Batch.CreateBatch();
        }

        
        /// <summary>
        /// Sends an asynchronous HTTP request to the PocketBase API.
        /// </summary>
        /// <param name="path">The API endpoint path (without base URL)</param>
        /// <param name="method">The HTTP method to use for the request</param>
        /// <param name="headers">Optional HTTP headers to include with the request</param>
        /// <param name="query">Optional query parameters to append to the URL</param>
        /// <param name="body">Optional request body data</param>
        /// <param name="files">Optional files to upload with the request</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result indicating success or failure of the operation</returns>
        public async Task<Result> SendAsync(string path, HttpMethod method, IDictionary<string, string>? headers = null, IDictionary<string, object?>? query = null, IDictionary<string, object>? body = null, IEnumerable<IFile>? files = null, CancellationToken cancellationToken = default)
        {
            headers ??= new Dictionary<string, string>();
            query ??= new Dictionary<string, object?>();
            body ??= new Dictionary<string, object>();
            files ??= new List<IFile>();

            Uri url = BuildUrl(path, query);

            HttpRequestMessage request = CreateRequest(url, method, headers: headers, query: query, body: body, files: files);

            try
            {
                if (BeforeSend is not null)
                {
                    request = BeforeSend.Invoke(this, new RequestEventArgs(url, request));
                }

                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (AfterSend is not null)
                {
                    AfterSend.Invoke(this, new ResponseEventArgs(url, response));
                }

#if DEBUG
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
#endif

                if ((int)response.StatusCode >= 400)
                {
                    var error = new ClientError(method, url.ToString(), (int)response.StatusCode);
                    return Result.Fail(error);
                }
                
                return Result.Ok();
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException requestException)
                {
                    ClientError error = new ClientError(method, url.ToString(), (int)requestException.StatusCode!);
                    return Result.Fail(error);
                }

                return Result.Fail(new Error(ex.Message));
            }
        }

        /// <summary>
        /// Sends a synchronous HTTP request to the PocketBase API.
        /// </summary>
        /// <param name="path">The API endpoint path (without base URL)</param>
        /// <param name="method">The HTTP method to use for the request</param>
        /// <param name="headers">Optional HTTP headers to include with the request</param>
        /// <param name="query">Optional query parameters to append to the URL</param>
        /// <param name="body">Optional request body data</param>
        /// <param name="files">Optional files to upload with the request</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result indicating success or failure of the operation</returns>
        public Result Send(string path, HttpMethod method, IDictionary<string, string>? headers = null, IDictionary<string, object?>? query = null, IDictionary<string, object>? body = null, IEnumerable<IFile>? files = null, CancellationToken cancellationToken = default)
        {
            //RETURN RESULT
            
            headers ??= new Dictionary<string, string>();
            query ??= new Dictionary<string, object?>();
            body ??= new Dictionary<string, object>();
            files ??= new List<IFile>();

            Uri url = BuildUrl(path, query);

            HttpRequestMessage request = CreateRequest(url, method, headers: headers, query: query, body: body, files: files);

            try
            {
                if (BeforeSend is not null)
                {
                    request = BeforeSend.Invoke(this, new RequestEventArgs(url, request));
                }

                var response = _httpClient.Send(request, cancellationToken);

                if (AfterSend is not null)
                {
                    AfterSend.Invoke(this, new ResponseEventArgs(url, response));
                }

                if ((int)response.StatusCode >= 400)
                {
                    var error = new ClientError(method, url.ToString(), (int)response.StatusCode);
                    return Result.Fail(error);
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException requestException)
                {
                    ClientError error = new ClientError(method, url.ToString(), (int)requestException.StatusCode!);
                    return Result.Fail(error);
                }

                return Result.Fail(new Error(ex.Message));
            }
        }

        /// <summary>
        /// Sends an asynchronous HTTP request to the PocketBase API and deserializes the response to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response to</typeparam>
        /// <param name="path">The API endpoint path (without base URL)</param>
        /// <param name="method">The HTTP method to use for the request</param>
        /// <param name="headers">Optional HTTP headers to include with the request</param>
        /// <param name="query">Optional query parameters to append to the URL</param>
        /// <param name="body">Optional request body data</param>
        /// <param name="files">Optional files to upload with the request</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result containing the deserialized response of type T or an error</returns>
        public async Task<Result<T>> SendAsync<T>(string path, HttpMethod method, IDictionary<string, string>? headers = null, IDictionary<string, object?>? query = null, IDictionary<string, object>? body = null, IEnumerable<IFile>? files = null, CancellationToken cancellationToken = default)
        {
            headers ??= new Dictionary<string, string>();
            query ??= new Dictionary<string, object?>();
            body ??= new Dictionary<string, object>();
            files ??= new List<IFile>();

            Uri url = BuildUrl(path, query);

            HttpRequestMessage request = CreateRequest(url, method, headers: headers, query: query, body: body, files: files);

            try
            {
                if (BeforeSend is not null)
                {
                    request = BeforeSend.Invoke(this, new RequestEventArgs(url, request));
                }

                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (AfterSend is not null)
                {
                    AfterSend.Invoke(this, new ResponseEventArgs(url, response));
                }

#if DEBUG
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
#endif
                
                if ((int)response.StatusCode >= 400)
                {
                    ClientError error = new ClientError(method, url.ToString(), (int)response.StatusCode);
                    return Result.Fail(error);
                }

                var parsedResponse =
                    await response.Content.ReadFromJsonAsync<T>(jsonSerializerOptions, cancellationToken);
                return Result.Ok(parsedResponse!);
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException requestException)
                {
                    ClientError error = new ClientError(method, url.ToString(), (int)requestException.StatusCode!);
                    return Result.Fail(error);
                }

                return Result.Fail(new Error(ex.Message));
            }
        }
        
        /// <summary>
        /// Sends a synchronous HTTP request to the PocketBase API and deserializes the response to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response to</typeparam>
        /// <param name="path">The API endpoint path (without base URL)</param>
        /// <param name="method">The HTTP method to use for the request</param>
        /// <param name="headers">Optional HTTP headers to include with the request</param>
        /// <param name="query">Optional query parameters to append to the URL</param>
        /// <param name="body">Optional request body data</param>
        /// <param name="files">Optional files to upload with the request</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result containing the deserialized response of type T or an error</returns>
        public Result<T> Send<T>(string path, HttpMethod method, IDictionary<string, string>? headers = null, IDictionary<string, object?>? query = null, IDictionary<string, object>? body = null, IEnumerable<IFile>? files = null, CancellationToken cancellationToken = default)
        {
            headers ??= new Dictionary<string, string>();
            query ??= new Dictionary<string, object?>();
            body ??= new Dictionary<string, object>();
            files ??= new List<IFile>();

            Uri url = BuildUrl(path, query);

            HttpRequestMessage request = CreateRequest(url, method, headers: headers, query: query, body: body, files: files);

            try
            {
                if (BeforeSend is not null)
                {
                    request = BeforeSend.Invoke(this, new RequestEventArgs(url, request));
                }

                var response = _httpClient.Send(request, cancellationToken);

                if (AfterSend is not null)
                {
                    AfterSend.Invoke(this, new ResponseEventArgs(url, response));
                }

                if ((int)response.StatusCode >= 400)
                {
                    ClientError error = new ClientError(method, url.ToString(), (int)response.StatusCode);
                    return Result.Fail(error);
                }

                using var stream = response.Content.ReadAsStream();
                var parsedResponse = JsonSerializer.Deserialize<T>(stream, jsonSerializerOptions);
                return Result.Ok(parsedResponse!);
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException requestException)
                {
                    ClientError error = new ClientError(method, url.ToString(), (int)requestException.StatusCode!);
                    return Result.Fail(error);
                }

                return Result.Fail(new Error(ex.Message));
            }
        }

        public async Task<Result<Stream>> GetStreamAsync(string path, IDictionary<string, object?>? query = null, CancellationToken cancellationToken = default)
        {
            query ??= new Dictionary<string, object?>();

            Uri url = BuildUrl(path, query);

            try
            {
                var stream = await _httpClient.GetStreamAsync(url, cancellationToken);
                return Result.Ok(stream);
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException requestException)
                {
                    ClientError error = new ClientError(HttpMethod.Get, url.ToString(), (int)requestException.StatusCode!);
                    return Result.Fail(error);
                }

                return Result.Fail(new Error(ex.Message));
            }
        }

        private HttpRequestMessage CreateRequest(Uri url, HttpMethod method, IDictionary<string, string> headers, IDictionary<string, object?> query, IDictionary<string, object> body, IEnumerable<IFile> files)
        {
            HttpRequestMessage request;

            if (files.Any())
            {
                request = BuildFileRequest(method, url, headers, body, files);
            }
            else
            {
                request = BuildJsonRequest(method, url, headers, body);
            }

            if (!headers.ContainsKey("Authorization") && AuthStore.IsValid)
            {
                request.Headers.Add("Authorization", AuthStore.Token);
            }

            if (!headers.ContainsKey("Accept-Language"))
            {
                request.Headers.Add("Accept-Language", _language);
            }

            return request;
        }

        /// <summary>
        /// Builds a complete URL for the specified API path with optional query parameters.
        /// </summary>
        /// <param name="path">The API endpoint path to append to the base URL</param>
        /// <param name="queryParameters">Optional query parameters to append to the URL</param>
        /// <returns>A complete URI for the API request</returns>
        public Uri BuildUrl(string path, IDictionary<string, object?>? queryParameters = null)
        {
            var url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/");

            if (!string.IsNullOrWhiteSpace(path))
            {
                url += path.StartsWith("/") ? path.Substring(1) : path;
            }

            if (queryParameters is not null)
            {
                var query = NormalizeQueryParameters(queryParameters);

                List<string> urlSegments = new();
                foreach (var kvp in query)
                {
                    var encodedKey = HttpUtility.UrlEncode(kvp.Key);
                    foreach (var item in kvp.Value)
                    {
                        var encodedValue = HttpUtility.UrlEncode(item.ToString());
                        urlSegments.Add($"{encodedKey}={encodedValue}");
                    }
                }

                var queryString = string.Join("&", urlSegments);

                if (!string.IsNullOrWhiteSpace(queryString))
                {
                    url = url + "?" + queryString;
                }
            }

            return new Uri(url, UriKind.RelativeOrAbsolute);
        }

        private IDictionary<string, IEnumerable> NormalizeQueryParameters(IDictionary<string, object?>? parameters)
        {
            Dictionary<string, IEnumerable> result = new();

            if (parameters is null)
            {
                return result;
            }

            foreach (var item in parameters)
            {
                List<string> normalizedValue = new();
                IEnumerable valueAsList;

                if (item.Value is IEnumerable && item.Value is not string)
                {
                    valueAsList = (IEnumerable)item.Value;
                }
                else
                {
                    valueAsList = new List<object?>() { item.Value };
                }

                foreach (var subItem in valueAsList)
                {
                    if (subItem is null)
                    {
                        continue;
                    }
                    normalizedValue.Add(subItem.ToString() ?? "");
                }

                if (normalizedValue.Count > 0)
                {
                    result[item.Key] = normalizedValue;
                }
            }

            return result;
        }

        private HttpRequestMessage BuildJsonRequest(HttpMethod method, Uri url, IDictionary<string, string>? headers = null, IDictionary<string, object>? body = null)
        {
            var request = new HttpRequestMessage(method, url);
            if (body is not null && body.Count > 0)
            {
                request.Content = JsonContent.Create(body);
            }

            if (headers is not null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            return request;
        }

        private HttpRequestMessage BuildFileRequest(HttpMethod method, Uri url, IDictionary<string, string>? headers, IDictionary<string, object>? body, IEnumerable<IFile> files)
        {
            var request = new HttpRequestMessage(method, url);

            if (headers is not null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var form = new MultipartFormDataContent();
            foreach (var file in files)
            {
                var stream = file.GetStream();
                if (stream is null || string.IsNullOrWhiteSpace(file.FieldName) || string.IsNullOrWhiteSpace(file.FileName))
                {
                    continue;
                }

                var fileContent = new StreamContent(stream);
                var mimeType = GetMimeType(file);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                form.Add(fileContent, file.FieldName, file.FileName);
            }


            if (body is not null && body.Count > 0)
            {
                Dictionary<string, string> additionalBody = new Dictionary<string, string>();
                foreach (var item in body)
                {
                    if (item.Value is IList valueAsList && item.Value is not string)
                    {
                        for (int i = 0; i < valueAsList.Count; i++)
                        {
                            var listValue = valueAsList[i]?.ToString();
                            if (string.IsNullOrWhiteSpace(listValue))
                            {
                                continue;
                            }
                            additionalBody[$"{item.Key}{i}"] = listValue;
                        }
                    }
                    else
                    {
                        var value = item.Value?.ToString();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            continue;
                        }
                        additionalBody[item.Key] = value;
                    }
                }

                foreach (var item in additionalBody)
                {
                    var content = new StringContent(item.Value);
                    form.Add(content, item.Key);
                }
            }

            request.Content = form;
            return request;
        }

        private string GetMimeType(IFile file)
        {
            if (file is FilepathFile filePath)
            {
                var fileName = Path.GetFileName(filePath.FilePath);
                return MimeMapping.MimeUtility.GetMimeMapping(fileName);
            }
            return MimeMapping.MimeUtility.UnknownMimeType;
        }

    }
}