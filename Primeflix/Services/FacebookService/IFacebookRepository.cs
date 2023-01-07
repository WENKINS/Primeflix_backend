using Primeflix.Models;

namespace Primeflix.Services.FacebookService
{
    public interface IFacebookRepository
    {
        Task<FacebookTokenValidationResult> ValidateAccessToken(string accessToken);
        Task<FacebookUserInfoResult> GetUserInfo(string accessToken);
    }
}
