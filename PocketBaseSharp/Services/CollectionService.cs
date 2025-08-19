using PocketBaseSharp.Models.Collection;
using FluentResults;
using PocketBaseSharp.Services.Base;

namespace PocketBaseSharp.Services
{
    /// <summary>
    /// Service for managing database collections and their schemas.
    /// Provides CRUD operations for collection definitions and import/export functionality.
    /// </summary>
    public class CollectionService : BaseCrudService<CollectionModel>
    {
        private readonly PocketBase _client;

        protected override string BasePath(string? url = null) => "/api/collections";

        /// <summary>
        /// Initializes a new instance of the CollectionService class.
        /// </summary>
        /// <param name="client">The PocketBase client instance</param>
        public CollectionService(PocketBase client) : base(client)
        {
            this._client = client;
        }

        /// <summary>
        /// Asynchronously imports collections from a collection array, optionally deleting missing collections.
        /// </summary>
        /// <param name="collections">The collections to import</param>
        /// <param name="deleteMissing">Whether to delete collections not included in the import</param>
        /// <param name="body">Optional additional request body data</param>
        /// <param name="query">Optional query parameters</param>
        /// <param name="headers">Optional HTTP headers</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result indicating success or failure of the import operation</returns>
        public Task<Result> ImportAsync(IEnumerable<CollectionModel> collections, bool deleteMissing = false, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            body ??= new Dictionary<string, object>();
            body.Add("collections", collections);
            body.Add("deleteMissing", deleteMissing);

            var url = $"{BasePath()}/import";
            return _client.SendAsync(url, HttpMethod.Put, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Synchronously imports collections from a collection array, optionally deleting missing collections.
        /// </summary>
        /// <param name="collections">The collections to import</param>
        /// <param name="deleteMissing">Whether to delete collections not included in the import</param>
        /// <param name="body">Optional additional request body data</param>
        /// <param name="query">Optional query parameters</param>
        /// <param name="headers">Optional HTTP headers</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result indicating success or failure of the import operation</returns>
        public Result Import(IEnumerable<CollectionModel> collections, bool deleteMissing = false, IDictionary<string, object>? body = null, IDictionary<string, object?>? query = null, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        {
            body ??= new Dictionary<string, object>();
            body.Add("collections", collections);
            body.Add("deleteMissing", deleteMissing);

            var url = $"{BasePath()}/import";
            return _client.Send(url, HttpMethod.Put, headers: headers, query: query, body: body, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Asynchronously retrieves a collection by its name.
        /// </summary>
        /// <param name="name">The name of the collection to retrieve</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result containing the collection model or an error</returns>
        public Task<Result<CollectionModel>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/{UrlEncode(name)}";
            return _client.SendAsync<CollectionModel>(url, HttpMethod.Get, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Synchronously retrieves a collection by its name.
        /// </summary>
        /// <param name="name">The name of the collection to retrieve</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result containing the collection model or an error</returns>
        public Result<CollectionModel> GetByName(string name, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/{UrlEncode(name)}";
            return _client.Send<CollectionModel>(url, HttpMethod.Get, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Asynchronously deletes a collection by its name.
        /// </summary>
        /// <param name="name">The name of the collection to delete</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result indicating success or failure of the deletion</returns>
        public Task<Result> DeleteAsync(string name, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/{UrlEncode(name)}";
            return _client.SendAsync(url, HttpMethod.Delete, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Synchronously deletes a collection by its name.
        /// </summary>
        /// <param name="name">The name of the collection to delete</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result indicating success or failure of the deletion</returns>
        public Result Delete(string name, CancellationToken cancellationToken = default)
        {
            var url = $"{BasePath()}/{UrlEncode(name)}";
            return _client.Send(url, HttpMethod.Delete, cancellationToken: cancellationToken);
        }

    }
}
