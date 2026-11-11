namespace PocketBaseSharp.Models.Collection
{
    /// <summary>
    /// Result of <c>POST /api/collections/meta/dry-run-view</c> (PocketBase v0.37.0+).
    /// </summary>
    public class DryRunViewResultModel
    {
        /// <summary>
        /// The generated view collection fields (raw field definitions, e.g. "id", "name", "type", "required", ...).
        /// </summary>
        public List<IDictionary<string, object?>> Fields { get; set; } = new();

        /// <summary>
        /// A sample of the resulting records.
        /// </summary>
        public List<IDictionary<string, object?>> Sample { get; set; } = new();
    }
}
