using Microsoft.AspNetCore.Identity;

namespace NZWalks.Models.Repositories;

public interface ITokenRepository
{
    string CreateJWTToken(IdentityUser user, List<string> roles);
}