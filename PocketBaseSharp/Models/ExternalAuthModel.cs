using PocketBaseSharp.Json;
using System.Text.Json.Serialization;

namespace PocketBaseSharp.Models
{
    public class ExternalAuthModel
    {
        public string? Id { get; set; }

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? Created { get; set; }

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? Updated { get; set; }

        public string? RecordId { get; set; }
        public string? CollectionId { get; set; }
        public string? Provider { get; set; }
        public string? ProviderId { get; set; }
    }
}
