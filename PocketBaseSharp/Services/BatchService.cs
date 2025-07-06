using FluentResults;
using PocketBaseSharp.Services.Base;
using System.Text.Json.Serialization;

namespace PocketBaseSharp.Services
{
    /// <summary>
    /// Service for handling PocketBase batch operations
    /// </summary>
    public class BatchService : BaseService
    {
        protected override string BasePath(string? path = null) => "api/batch";

        private readonly PocketBase _client;

        public BatchService(PocketBase client)
        {
            this._client = client;
        }

        /// <summary>
        /// Creates a new batch builder for constructing batch requests
        /// </summary>
        /// <returns>A new BatchBuilder instance</returns>
        public BatchBuilder CreateBatch()
        {
            return new BatchBuilder(this);
        }

        /// <summary>
        /// Executes a batch request with the provided batch payload
        /// </summary>
        /// <param name="batchPayload">The batch payload containing all requests</param>
        /// <param name="headers">Optional headers</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing the batch response</returns>
        public async Task<Result<BatchResponse>> SendBatchAsync(
            BatchPayload batchPayload,
            IDictionary<string, string>? headers = null,
            CancellationToken cancellationToken = default)
        {
            var body = new Dictionary<string, object>
            {
                { "requests", batchPayload.Requests.Select(r => new Dictionary<string, object>
                {
                    { "method", r.Method },
                    { "url", r.Url },
                    { "body", r.Body ?? new Dictionary<string, object>() },
                    { "headers", r.Headers ?? new Dictionary<string, string>() }
                }).ToList() }
            };

            return await _client.SendAsync<BatchResponse>(BasePath(), HttpMethod.Post, headers: headers, body: body, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Executes a batch request synchronously
        /// </summary>
        /// <param name="batchPayload">The batch payload containing all requests</param>
        /// <param name="headers">Optional headers</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing the batch response</returns>
        public Result<BatchResponse> SendBatch(
            BatchPayload batchPayload,
            IDictionary<string, string>? headers = null,
            CancellationToken cancellationToken = default)
        {
            var body = new Dictionary<string, object>
            {
                { "requests", batchPayload.Requests.Select(r => new Dictionary<string, object>
                {
                    { "method", r.Method },
                    { "url", r.Url },
                    { "body", r.Body ?? new Dictionary<string, object>() },
                    { "headers", r.Headers ?? new Dictionary<string, string>() }
                }).ToList() }
            };

            return _client.Send<BatchResponse>(BasePath(), HttpMethod.Post, headers: headers, body: body, cancellationToken: cancellationToken);
        }
    }

    /// <summary>
    /// Builder class for constructing batch requests
    /// </summary>
    public class BatchBuilder
    {
        private readonly BatchService _batchService;
        private readonly List<BatchRequest> _requests;

        internal BatchBuilder(BatchService batchService)
        {
            _batchService = batchService;
            _requests = new List<BatchRequest>();
        }

        /// <summary>
        /// Adds a create operation to the batch
        /// </summary>
        /// <typeparam name="T">The type of object to create</typeparam>
        /// <param name="collectionName">The collection name</param>
        /// <param name="data">The object to create</param>
        /// <param name="expand">Optional expand parameter</param>
        /// <returns>This BatchBuilder instance for method chaining</returns>
        public BatchBuilder Create<T>(string collectionName, T data, string? expand = null)
        {
            var url = $"/api/collections/{collectionName}/records";
            if (!string.IsNullOrEmpty(expand))
            {
                url += $"?expand={expand}";
            }

            var body = ConstructBodyFromObject(data);

            _requests.Add(new BatchRequest
            {
                Method = "POST",
                Url = url,
                Body = body
            });

            return this;
        }

        /// <summary>
        /// Adds an update operation to the batch
        /// </summary>
        /// <typeparam name="T">The type of object to update</typeparam>
        /// <param name="collectionName">The collection name</param>
        /// <param name="recordId">The ID of the record to update</param>
        /// <param name="data">The data to update</param>
        /// <param name="expand">Optional expand parameter</param>
        /// <returns>This BatchBuilder instance for method chaining</returns>
        public BatchBuilder Update<T>(string collectionName, string recordId, T data, string? expand = null)
        {
            var url = $"/api/collections/{collectionName}/records/{recordId}";
            if (!string.IsNullOrEmpty(expand))
            {
                url += $"?expand={expand}";
            }

            var body = ConstructBodyFromObject(data);

            _requests.Add(new BatchRequest
            {
                Method = "PATCH",
                Url = url,
                Body = body
            });

            return this;
        }

        /// <summary>
        /// Adds an upsert operation to the batch
        /// </summary>
        /// <typeparam name="T">The type of object to upsert</typeparam>
        /// <param name="collectionName">The collection name</param>
        /// <param name="data">The object to upsert (must have an id field)</param>
        /// <param name="expand">Optional expand parameter</param>
        /// <returns>This BatchBuilder instance for method chaining</returns>
        public BatchBuilder Upsert<T>(string collectionName, T data, string? expand = null)
        {
            var url = $"/api/collections/{collectionName}/records";
            if (!string.IsNullOrEmpty(expand))
            {
                url += $"?expand={expand}";
            }

            var body = ConstructBodyFromObject(data);

            _requests.Add(new BatchRequest
            {
                Method = "PUT",
                Url = url,
                Body = body
            });

            return this;
        }

        /// <summary>
        /// Adds a delete operation to the batch
        /// </summary>
        /// <param name="collectionName">The collection name</param>
        /// <param name="recordId">The ID of the record to delete</param>
        /// <returns>This BatchBuilder instance for method chaining</returns>
        public BatchBuilder Delete(string collectionName, string recordId)
        {
            var url = $"/api/collections/{collectionName}/records/{recordId}";

            _requests.Add(new BatchRequest
            {
                Method = "DELETE",
                Url = url,
                Body = new Dictionary<string, object>()
            });

            return this;
        }

        /// <summary>
        /// Executes the batch request asynchronously
        /// </summary>
        /// <param name="headers">Optional headers</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing the batch response</returns>
        public async Task<Result<BatchResponse>> SendAsync(
            IDictionary<string, string>? headers = null,
            CancellationToken cancellationToken = default)
        {
            if (!_requests.Any())
            {
                return Result.Fail<BatchResponse>("No requests added to batch");
            }

            var batchPayload = new BatchPayload { Requests = _requests };
            return await _batchService.SendBatchAsync(batchPayload, headers, cancellationToken);
        }

        /// <summary>
        /// Executes the batch request synchronously
        /// </summary>
        /// <param name="headers">Optional headers</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing the batch response</returns>
        public Result<BatchResponse> Send(
            IDictionary<string, string>? headers = null,
            CancellationToken cancellationToken = default)
        {
            if (!_requests.Any())
            {
                return Result.Fail<BatchResponse>("No requests added to batch");
            }

            var batchPayload = new BatchPayload { Requests = _requests };
            return _batchService.SendBatch(batchPayload, headers, cancellationToken);
        }

        /// <summary>
        /// Constructs a body dictionary from an object using reflection
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>Dictionary representation of the object</returns>
        private Dictionary<string, object> ConstructBodyFromObject(object? obj)
        {
            if (obj == null) return new Dictionary<string, object>();

            var body = new Dictionary<string, object>();
            var type = obj.GetType();

            foreach (var prop in type.GetProperties())
            {
                var value = prop.GetValue(obj);
                if (value != null)
                {
                    var propertyName = GetPropertyName(prop);

                    // Handle DateTime serialization to ISO format
                    if (value is DateTime dateTime)
                    {
                        body[propertyName] = dateTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff'Z'");
                    }
                    else
                    {
                        body[propertyName] = value;
                    }
                }
            }

            return body;
        }

        /// <summary>
        /// Gets the property name, considering JsonPropertyName attributes
        /// </summary>
        /// <param name="property">The property info</param>
        /// <returns>The property name to use in JSON</returns>
        private string GetPropertyName(System.Reflection.PropertyInfo property)
        {
            var jsonPropertyNameAttribute = property.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false)
                .FirstOrDefault() as JsonPropertyNameAttribute;

            if (jsonPropertyNameAttribute != null)
            {
                return jsonPropertyNameAttribute.Name;
            }

            // Convert to camelCase
            var name = property.Name;
            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
    }

    /// <summary>
    /// Represents a single batch request
    /// </summary>
    public class BatchRequest
    {
        public string Method { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public Dictionary<string, object>? Body { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
    }

    /// <summary>
    /// Represents the batch payload containing all requests
    /// </summary>
    public class BatchPayload
    {
        public List<BatchRequest> Requests { get; set; } = new List<BatchRequest>();
    }

    /// <summary>
    /// Represents a single item in the batch response
    /// PocketBase returns an array of these directly
    /// </summary>
    public class BatchResponseItem
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("body")]
        public Dictionary<string, object>? Body { get; set; }
    }

    /// <summary>
    /// Type alias for the batch response (which is just an array)
    /// </summary>
    public class BatchResponse : List<BatchResponseItem>
    {
    }
}