namespace PocketBaseSharp.Models.Collection
{
    /// <summary>
    /// A configurable OAuth2 provider returned by <c>GET /api/collections/meta/oauth2-providers</c>
    /// (PocketBase v0.37.0+).
    /// </summary>
    public class OAuth2ProviderInfoModel
    {
        /// <summary>
        /// The provider identifier (e.g. "google").
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The human readable provider name.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// The provider logo as an inlined SVG string.
        /// </summary>
        public string? Logo { get; set; }
    }
}
