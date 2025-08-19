using FluentResults;
using PocketBaseSharp.Models;
using PocketBaseSharp.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBaseSharp.Services
{
    /// <summary>
    /// Service for managing database backups and backup operations.
    /// Provides functionality to retrieve and manage PocketBase backup files.
    /// </summary>
    public class BackupService : BaseService
    {
        readonly PocketBase _pocketBase;
        
        /// <summary>
        /// Initializes a new instance of the BackupService class.
        /// </summary>
        /// <param name="pocketBase">The PocketBase client instance</param>
        public BackupService(PocketBase pocketBase)
        {
            this._pocketBase = pocketBase;
        }

        protected override string BasePath(string? path = null)
        {
            return path ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the complete list of available backups from the server.
        /// </summary>
        /// <returns>A Result containing an enumerable of BackupModel objects representing all available backups</returns>
        public async Task<Result<IEnumerable<BackupModel>>> GetFullListAsync()
        {
            var b = await _pocketBase.SendAsync<IEnumerable<BackupModel>>("/api/backups", HttpMethod.Get);
            return b;
        }

    }
}
