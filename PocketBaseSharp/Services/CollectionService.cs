using PocketBaseSharp.Models.Collection;
using FluentResults;
using PocketBaseSharp.Services.Base;

namespace PocketBaseSharp.Services
{
    public class CollectionService : BaseCrudService<CollectionModel>
    {
        private readonly PocketBase _client;

        protected override string BasePath(string? url = null) => "/api/collections";

        public CollectionService(PocketBase client) : base(client)
        {
            this._client = client;
        }

        public Task<Result> ImportAsync(IEnumerable<CollectionModel> collections, bool deleteMissing = false, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            body ??= new Dictionary<string, object>();
            body.Add("collections", collections);
            body.Add("deleteMissing", deleteMissing);

            var url = $"{BasePath()}/import";
            return _client.SendAsync(url, HttpMethod.Put, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }

        public Result Import(IEnumerable<CollectionModel> collections, bool deleteMissing = false, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            body ??= new Dictionary<string, object>();
            body.Add("collections", collections);
            body.Add("deleteMissing", deleteMissing);

            var url = $"{BasePath()}/import";
            return _client.Send(url, HttpMethod.Put, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }

        public Task<Result<CollectionModel>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/{UrlEncode(name)}";
            return _client.SendAsync<CollectionModel>(url, HttpMethod.Get, cancellationToken: cancellationToken);
        }

        public Result<CollectionModel> GetByName(string name, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/{UrlEncode(name)}";
            return _client.Send<CollectionModel>(url, HttpMethod.Get, cancellationToken: cancellationToken);
        }

        public Task<Result> DeleteAsync(string name, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/{UrlEncode(name)}";
            return _client.SendAsync(url, HttpMethod.Delete, cancellationToken: cancellationToken);
        }

        public Result Delete(string name, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/{UrlEncode(name)}";
            return _client.Send(url, HttpMethod.Delete, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Returns a list with all configurable OAuth2 providers (PocketBase v0.37.0+, superusers only).
        /// </summary>
        public Task<Result<IEnumerable<OAuth2ProviderInfoModel>>> GetAllOAuth2ProvidersAsync(IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/meta/oauth2-providers";
            return _client.SendAsync<IEnumerable<OAuth2ProviderInfoModel>>(url, HttpMethod.Get, headers: headers, query: query, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Returns a list with all configurable OAuth2 providers (PocketBase v0.37.0+, superusers only).
        /// </summary>
        public Result<IEnumerable<OAuth2ProviderInfoModel>> GetAllOAuth2Providers(IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/meta/oauth2-providers";
            return _client.Send<IEnumerable<OAuth2ProviderInfoModel>>(url, HttpMethod.Get, headers: headers, query: query, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Executes the specified view query and returns the generated fields and a sample of the resulting records
        /// without creating the collection (PocketBase v0.37.0+, superusers only).
        /// </summary>
        /// <param name="viewQuery">The SELECT statement of the view collection.</param>
        public Task<Result<DryRunViewResultModel>> DryRunViewQueryAsync(string viewQuery, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            var body = new Dictionary<string, object> { { "query", viewQuery } };
            var url = $"{BasePath()}/meta/dry-run-view";
            return _client.SendAsync<DryRunViewResultModel>(url, HttpMethod.Post, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Executes the specified view query and returns the generated fields and a sample of the resulting records
        /// without creating the collection (PocketBase v0.37.0+, superusers only).
        /// </summary>
        /// <param name="viewQuery">The SELECT statement of the view collection.</param>
        public Result<DryRunViewResultModel> DryRunViewQuery(string viewQuery, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            var body = new Dictionary<string, object> { { "query", viewQuery } };
            var url = $"{BasePath()}/meta/dry-run-view";
            return _client.Send<DryRunViewResultModel>(url, HttpMethod.Post, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }

    }
}
