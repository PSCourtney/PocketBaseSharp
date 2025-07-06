using System.Text.Json.Serialization;

namespace PocketBaseSharp.Models.Auth
{
    public class AdminAuthModel : AuthModel
    {
        [JsonIgnore]
        public override IBaseModel? Model => Admin;


        [JsonPropertyName("admin")]
        public AdminModel? Admin { get; set; }
    }
}
