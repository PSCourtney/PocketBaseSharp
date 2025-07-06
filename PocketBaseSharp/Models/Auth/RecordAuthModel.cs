using System.Text.Json.Serialization;

namespace PocketBaseSharp.Models.Auth
{
    public class RecordAuthModel<T> : AuthModel where T : IBaseAuthModel
    {
        [JsonIgnore]
        public override IBaseModel? Model => Record;

        [JsonPropertyName("record")]
        public T? Record { get; set; }

        public IDictionary<string, object?>? meta { get; set; }
    }
}
