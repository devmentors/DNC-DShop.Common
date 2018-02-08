using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace DShop.Common.Authentication
{
    public interface IJwtHandler
    {
        JsonWebToken CreateToken(Guid userId, string role);
        JsonWebTokenPayload GetTokenPayload(string accessToken);
    }
}