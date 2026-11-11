using FluentResults;
using PocketBaseSharp.Services.Base;

namespace PocketBaseSharp.Services
{
    /// <summary>
    /// Service for the PocketBase logs API.
    /// </summary>
    /// <remarks>
    /// The legacy request log methods (GetRequest, GetRequests, GetRequestsStatistics and their async variants)
    /// were removed in PocketBaseSharp 1.2.0 because they targeted the <c>api/logs/requests</c> route,
    /// which PocketBase removed in v0.23.0. See CHANGELOG.md.
    /// </remarks>
    public class LogService : BaseService
    {
        protected override string BasePath(string? path = null) => "api/logs";

        private readonly PocketBase _client;

        public LogService(PocketBase client)
        {
            this._client = client;
        }

        /// <summary>
        /// Deletes all logs without changing the logs retention setting (PocketBase v0.40.0+, superusers only).
        /// </summary>
        /// <param name="query">The query parameters to send to the API. Default is null.</param>
        /// <param name="headers">The headers to send to the API. Default is null.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation. Default is the default cancellation token.</param>
        public Task<Result> TruncateAsync(IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            return _client.SendAsync(BasePath(), HttpMethod.Delete, headers: headers, query: query, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Deletes all logs without changing the logs retention setting (PocketBase v0.40.0+, superusers only).
        /// </summary>
        /// <param name="query">The query parameters to send to the API. Default is null.</param>
        /// <param name="headers">The headers to send to the API. Default is null.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation. Default is the default cancellation token.</param>
        public Result Truncate(IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            return _client.Send(BasePath(), HttpMethod.Delete, headers: headers, query: query, cancellationToken: cancellationToken);
        }
    }
}
