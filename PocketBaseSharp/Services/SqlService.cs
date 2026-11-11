using FluentResults;
using PocketBaseSharp.Models.Sql;
using PocketBaseSharp.Services.Base;

namespace PocketBaseSharp.Services
{
    /// <summary>
    /// Service for the raw SQL console API (PocketBase v0.39.0+).
    /// This operation is allowed only for superusers.
    /// </summary>
    public class SqlService : BaseService
    {
        private readonly PocketBase _client;

        protected override string BasePath(string? path = null) => "api/sql";

        public SqlService(PocketBase client)
        {
            this._client = client;
        }

        /// <summary>
        /// Executes the specified raw SQL query (superusers only).
        /// ⚠️ Use with care - any statement (including destructive ones) will be executed.
        /// </summary>
        /// <param name="sql">The raw SQL query to execute.</param>
        /// <param name="query">The query parameters to send to the API. Default is null.</param>
        /// <param name="headers">The headers to send to the API. Default is null.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SqlResultModel"/> with the resulting columns, rows, affected rows and execution time.</returns>
        public Task<Result<SqlResultModel>> RunAsync(string sql, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            var body = new Dictionary<string, object> { { "query", sql } };
            return _client.SendAsync<SqlResultModel>(BasePath(), HttpMethod.Post, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Executes the specified raw SQL query (superusers only).
        /// ⚠️ Use with care - any statement (including destructive ones) will be executed.
        /// </summary>
        /// <param name="sql">The raw SQL query to execute.</param>
        /// <param name="query">The query parameters to send to the API. Default is null.</param>
        /// <param name="headers">The headers to send to the API. Default is null.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SqlResultModel"/> with the resulting columns, rows, affected rows and execution time.</returns>
        public Result<SqlResultModel> Run(string sql, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            var body = new Dictionary<string, object> { { "query", sql } };
            return _client.Send<SqlResultModel>(BasePath(), HttpMethod.Post, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }
    }
}
