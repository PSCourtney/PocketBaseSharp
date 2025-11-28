using PocketBaseSharp.Models;
using FluentResults;

namespace PocketBaseSharp.Services.Base
{
    /// <summary>
    /// Abstract base class for services that provide CRUD (Create, Read, Update, Delete) operations.
    /// Provides standard implementation for common database operations on typed models.
    /// </summary>
    /// <typeparam name="T">The type of model this service manages</typeparam>
    public abstract class BaseCrudService<T> : BaseService
    {
        private readonly PocketBase _client;

        /// <summary>
        /// Initializes a new instance of the BaseCrudService class.
        /// </summary>
        /// <param name="client">The PocketBase client instance</param>
        protected BaseCrudService(PocketBase client)
        {
            this._client = client;
        }
        
        /// <summary>
        /// Synchronously retrieves a paginated list of records.
        /// </summary>
        /// <param name="page">The page number to retrieve (default: 1)</param>
        /// <param name="perPage">The number of records per page (default: 30)</param>
        /// <param name="filter">Optional filter expression to apply</param>
        /// <param name="sort">Optional sort expression to apply</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result containing the paginated collection or an error</returns>
        public virtual Result<PagedCollectionModel<T>> List(int page = 1, int perPage = 30, string? filter = null, string? sort = null, CancellationToken cancellationToken = default)
        {
            var path = BasePath();
            var query = new Dictionary<string, object?>()
            {
                { "filter", filter },
                { "page", page },
                { "perPage", perPage },
                { "sort", sort }
            };
        
            return _client.Send<PagedCollectionModel<T>>(path, HttpMethod.Get, query: query, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Asynchronously retrieves a paginated list of records.
        /// </summary>
        /// <param name="page">The page number to retrieve (default: 1)</param>
        /// <param name="perPage">The number of records per page (default: 30)</param>
        /// <param name="filter">Optional filter expression to apply</param>
        /// <param name="sort">Optional sort expression to apply</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result containing the paginated collection or an error</returns>
        public virtual Task<Result<PagedCollectionModel<T>>> ListAsync(int page = 1, int perPage = 30, string? filter = null, string? sort = null, CancellationToken cancellationToken = default)
        {
            var path = BasePath();
            var query = new Dictionary<string, object?>()
            {
                { "filter", filter },
                { "page", page },
                { "perPage", perPage },
                { "sort", sort }
            };

            return _client.SendAsync<PagedCollectionModel<T>>(path, HttpMethod.Get, query: query, cancellationToken: cancellationToken);
        }

        public virtual Result<IEnumerable<T>> GetFullList(int batch = 100, string? filter = null, string? sort = null, CancellationToken cancellationToken = default)
        {
            List<T> result = new();
            int currentPage = 1;
            Result<PagedCollectionModel<T>> lastResponse;
            do
            {
                lastResponse = List(currentPage, perPage: batch, filter: filter, sort: sort, cancellationToken: cancellationToken);
                if (lastResponse.IsSuccess && lastResponse.Value.Items is not null)
                {
                    result.AddRange(lastResponse.Value.Items);
                }
                currentPage++;
            } while (lastResponse.IsSuccess && lastResponse.Value.Items?.Count > 0 && lastResponse.Value.TotalItems > result.Count);

            return result;
        }

        public virtual async Task<IEnumerable<T>> GetFullListAsync(int batch = 100, string? filter = null, string? sort = null, CancellationToken cancellationToken = default)
        {
            List<T> result = new();
            int currentPage = 1;
            Result<PagedCollectionModel<T>> lastResponse;
            do
            {
                lastResponse = await ListAsync(currentPage, perPage: batch, filter: filter, sort: sort, cancellationToken: cancellationToken);
                if (lastResponse.IsSuccess && lastResponse.Value.Items is not null)
                {
                    result.AddRange(lastResponse.Value.Items);
                }
                currentPage++;
            } while (lastResponse.IsSuccess && lastResponse.Value.Items?.Count > 0 && lastResponse.Value.TotalItems > result.Count);

            return result;
        }

        public virtual Result<T> GetOne(string id)
        {
            string url = $"{BasePath()}/{UrlEncode(id)}";
            return _client.Send<T>(url, HttpMethod.Get);
        }
        
        public virtual Task<Result<T>> GetOneAsync(string id)
        {
            string url = $"{BasePath()}/{UrlEncode(id)}";
            return _client.SendAsync<T>(url, HttpMethod.Get);
        }

    }
}
