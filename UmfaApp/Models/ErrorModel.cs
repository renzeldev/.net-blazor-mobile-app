using System.Text.Json.Serialization;

namespace UmfaApp.Models
{
    public class ErrorModel
    {
        [JsonPropertyName("detail")]
        public string Detail { get; set; }
    }
}
