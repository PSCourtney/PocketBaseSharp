namespace PocketBaseSharp.Models.Log
{
    public class AuthMethodsList
    {
        public bool? UsernamePassword { get; set; }
        public bool? EmailPassword { get; set; }
        public IEnumerable<AuthMethodProvider>? AuthProviders { get; set; }
    }
}
