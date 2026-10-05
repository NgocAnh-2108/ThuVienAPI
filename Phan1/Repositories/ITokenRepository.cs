using Microsoft.AspNetCore.Identity;

namespace Phan1.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(
            IdentityUser user,
            List<string> roles);
    }
}
