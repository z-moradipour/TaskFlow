using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}