using Newtonsoft.Json;

namespace Primeflix.Models
{
    public class FacebookUserInfoResult
    {
        public string Id { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
