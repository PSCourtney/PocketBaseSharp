using System.Text.Json.Serialization;

namespace PocketBaseSharp.Models
{
    public class AdminModel : BaseModel
    {
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("avatar")]
        public int? Avatar { get; set; }
    }
}
