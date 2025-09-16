using FluentResults;
using PocketBaseSharp.Services.Base;

namespace PocketBaseSharp.Services
{
    /// <summary>
    /// Service for managing application settings and configuration.
    /// Provides functionality to retrieve and update PocketBase application settings.
    /// </summary>
    public class SettingsService : BaseService
    {

        protected override string BasePath(string? path = null) => "api/settings";

        private readonly PocketBase _client;

        /// <summary>
        /// Initializes a new instance of the SettingsService class.
        /// </summary>
        /// <param name="client">The PocketBase client instance</param>
        public SettingsService(PocketBase client)
        {
            this._client = client;
        }

        /// <summary>
        /// Asynchronously retrieves all application settings.
        /// </summary>
        /// <param name="query">Optional query parameters</param>
        /// <param name="headers">Optional HTTP headers</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result containing a dictionary of all settings</returns>
        public Task<Result<IDictionary<string, object>>> GetAllAsync(IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            return _client.SendAsync<IDictionary<string, object>>(BasePath(), HttpMethod.Get, headers: headers, query: query, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Synchronously retrieves all application settings.
        /// </summary>
        /// <param name="query">Optional query parameters</param>
        /// <param name="headers">Optional HTTP headers</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result containing a dictionary of all settings</returns>
        public Result<IDictionary<string, object>> GetAll(IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            return _client.Send<IDictionary<string, object>>(BasePath(), HttpMethod.Get, headers: headers, query: query, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Asynchronously updates application settings with the provided values.
        /// </summary>
        /// <param name="body">The settings to update</param>
        /// <param name="query">Optional query parameters</param>
        /// <param name="headers">Optional HTTP headers</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result containing the updated settings</returns>
        public Task<Result<IDictionary<string, object>>> UpdateAsync(IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            return _client.SendAsync<IDictionary<string, object>>(BasePath(), HttpMethod.Patch, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Synchronously updates application settings with the provided values.
        /// </summary>
        /// <param name="body">The settings to update</param>
        /// <param name="query">Optional query parameters</param>
        /// <param name="headers">Optional HTTP headers</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result containing the updated settings</returns>
        public Result<IDictionary<string, object>> Update(IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            return _client.Send<IDictionary<string, object>>(BasePath(), HttpMethod.Patch, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Asynchronously tests the S3 configuration settings.
        /// </summary>
        /// <param name="body">Optional test data</param>
        /// <param name="query">Optional query parameters</param>
        /// <param name="headers">Optional HTTP headers</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result indicating whether the S3 test was successful</returns>
        public Task<Result> TestS3Async(IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/test/s3";
            return _client.SendAsync(url, HttpMethod.Post, headers: headers, body: body, query: query, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Synchronously tests the S3 configuration settings.
        /// </summary>
        /// <param name="body">Optional test data</param>
        /// <param name="query">Optional query parameters</param>
        /// <param name="headers">Optional HTTP headers</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result indicating whether the S3 test was successful</returns>
        public Result TestS3(IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/test/s3";
            return _client.Send(url, HttpMethod.Post, headers: headers, body: body, query: query, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Asynchronously tests the email configuration by sending a test email.
        /// </summary>
        /// <param name="toEmail">The email address to send the test email to</param>
        /// <param name="template">The email template to use for the test</param>
        /// <param name="body">Optional test data</param>
        /// <param name="query">Optional query parameters</param>
        /// <param name="headers">Optional HTTP headers</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result indicating whether the email test was successful</returns>
        public Task<Result> TestEmailAsync(string toEmail, string template, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            query ??= new Dictionary<string, object?>();
            query.Add("email", toEmail);
            query.Add("template", template);

            var url = $"{BasePath()}/test/email";
            return _client.SendAsync(url, HttpMethod.Post, headers: headers, body: body, query: query, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Synchronously tests the email configuration by sending a test email.
        /// </summary>
        /// <param name="toEmail">The email address to send the test email to</param>
        /// <param name="template">The email template to use for the test</param>
        /// <param name="body">Optional test data</param>
        /// <param name="query">Optional query parameters</param>
        /// <param name="headers">Optional HTTP headers</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result indicating whether the email test was successful</returns>
        public Result TestEmail(string toEmail, string template, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            query ??= new Dictionary<string, object?>();
            query.Add("email", toEmail);
            query.Add("template", template);

            var url = $"{BasePath()}/test/email";
            return _client.Send(url, HttpMethod.Post, headers: headers, body: body, query: query, cancellationToken: cancellationToken);
        }

    }
}
