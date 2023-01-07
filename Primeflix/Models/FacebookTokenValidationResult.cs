using Newtonsoft.Json;

namespace Primeflix.Models
{
    public class FacebookTokenValidationResult
    {
        public FacebookTokenValidationData Data { get; set; }
    }

    public class FacebookTokenValidationData
    {
        [JsonProperty("app_id")]
        public string AppId { get; set; }
        public string Type { get; set; }
        public string Application { get; set; }

        [JsonProperty("data_access_expires_at")]
        public long DataAccessExpiresAt { get; set; }

        [JsonProperty("expires_at")]
        public long ExpiresAt { get; set; }

        [JsonProperty("is_valid")]
        public bool IsValid { get; set; }

        public string[] Scopes { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}

