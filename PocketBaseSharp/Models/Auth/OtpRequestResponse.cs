using System.Text.Json.Serialization;

namespace PocketBaseSharp.Models.Auth
{
    /// <summary>
    /// Response model for OTP request endpoint.
    /// Contains the OTP ID that must be used for authentication.
    /// </summary>
    public class OtpRequestResponse
    {
        /// <summary>
        /// Unique identifier for this OTP request.
        /// Must be provided along with the OTP code during authentication.
        /// </summary>
        [JsonPropertyName("otpId")]
        public string? OtpId { get; set; }
    }
}
