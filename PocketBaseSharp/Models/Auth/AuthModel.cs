using System.Text.Json.Serialization;

namespace PocketBaseSharp.Models.Auth
{
    public abstract class AuthModel
    {
        public string? Token { get; set; }

        [JsonIgnore]
        public abstract IBaseModel? Model { get; }
    }
}
