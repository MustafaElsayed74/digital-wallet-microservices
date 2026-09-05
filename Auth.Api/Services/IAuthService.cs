using Auth.Api.Models;

namespace Auth.Api.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
    }
}
