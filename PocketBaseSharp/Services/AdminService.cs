using PocketBaseSharp.Models.Auth;
using PocketBaseSharp.Services.Base;

namespace PocketBaseSharp.Services
{
    /// <summary>
    /// Service for managing administrative operations and authentication.
    /// Provides functionality for admin user management in PocketBase.
    /// </summary>
    public class AdminService : BaseAuthService<AdminAuthModel>
    {

        protected override string BasePath(string? url = null) => "/api/admins";

        /// <summary>
        /// Initializes a new instance of the AdminService class.
        /// </summary>
        /// <param name="client">The PocketBase client instance</param>
        public AdminService(PocketBase client) : base(client)
        {
        }

    }
}
