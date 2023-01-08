using Newtonsoft.Json;
using Primeflix.Models;

namespace Primeflix.Services.FacebookService
{
    public class FacebookRepository : IFacebookRepository
    {
        private const string TokenValidationUrl = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}|{2}";
        private const string UserInfoUrl = "https://graph.facebook.com/me?fields=first_name,last_name,email&access_token={0}";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public FacebookRepository(
            IHttpClientFactory httpClientFactory, 
            IConfiguration configuration
            )
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<FacebookUserInfoResult> GetUserInfo(string accessToken)
        {
            // Get the Facebook user's first name, last name and email.
            var formattedUrl = string.Format(UserInfoUrl, accessToken);

            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();

            var responseAsString = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FacebookUserInfoResult>(responseAsString);
        }

        public async Task<FacebookTokenValidationResult> ValidateAccessToken(string accessToken)
        {
            // Checks whether the access_token was a token that was sent to Primeflix's frontend
            var formattedUrl = string.Format(TokenValidationUrl, accessToken, _configuration.GetSection("Facebook:AppId").Value, _configuration.GetSection("Facebook:AppSecret").Value);

            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();

            var responseAsString = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FacebookTokenValidationResult>(responseAsString);
        }
    }
}
