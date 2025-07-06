using PocketBaseSharp.Models.Auth;
using PocketBaseSharp.Services.Base;

namespace PocketBaseSharp.Services
{
    public class AdminService : BaseAuthService<AdminAuthModel>
    {

        protected override string BasePath(string? url = null) => "/api/admins";

        public AdminService(PocketBase client) : base(client)
        {
        }

    }
}
