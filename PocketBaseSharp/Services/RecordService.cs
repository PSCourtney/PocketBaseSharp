using PocketBaseSharp.Enum;
using FluentResults;
using PocketBaseSharp.Services.Base;

namespace PocketBaseSharp.Services
{
    /// <summary>
    /// Service for managing records within a specific collection.
    /// Provides CRUD operations and file management for collection records.
    /// </summary>
    public class RecordService : BaseSubCrudService
    {

        protected override string BasePath(string? path = null)
        {
            var encoded = UrlEncode(path);
            return $"/api/collections/{encoded}/records";
        }

        private readonly PocketBase _client;
        readonly string _collectionName;

        /// <summary>
        /// Initializes a new instance of the RecordService class for a specific collection.
        /// </summary>
        /// <param name="client">The PocketBase client instance</param>
        /// <param name="collectionName">The name of the collection this service manages</param>
        public RecordService(PocketBase client, string collectionName) : base(client, collectionName)
        {
            this._collectionName = collectionName;
            this._client = client;
        }

        private Uri GetFileUrl(string recordId, string fileName, IDictionary<string, object?>? query = null)
        {
            var url = $"api/files/{UrlEncode(_collectionName)}/{UrlEncode(recordId)}/{fileName}";
            return _client.BuildUrl(url, query);
        }

        /// <summary>
        /// Asynchronously downloads a file associated with a specific record.
        /// </summary>
        /// <param name="recordId">The ID of the record containing the file</param>
        /// <param name="fileName">The name of the file to download</param>
        /// <param name="thumbFormat">Optional thumbnail format specification</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A Result containing the file stream or an error</returns>
        public Task<Result<Stream>> DownloadFileAsync(string recordId, string fileName, ThumbFormat? thumbFormat = null, CancellationToken cancellationToken = default)
        {
            var url = $"api/files/{UrlEncode(_collectionName)}/{UrlEncode(recordId)}/{fileName}";

            //TODO find out how the specify the actual resolution to resize
            var query = new Dictionary<string, object?>()
            {
                { "thumb", ThumbFormatHelper.GetNameForQuery(thumbFormat) }
            };

            return _client.GetStreamAsync(url, query, cancellationToken);
        }

    }
}
