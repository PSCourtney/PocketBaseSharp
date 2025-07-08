using PocketBaseSharp.Json;
using System.Text.Json.Serialization;

namespace PocketBaseSharp.Models.Log
{
    public class LogRequestStatistic
    {
        public int? Total { get; set; }

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? Date { get; set; }
    }
}
