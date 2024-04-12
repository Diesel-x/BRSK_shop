namespace Front.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class JwtToken
{
    public string UserName { get; set; }
    public string UserRole { get; set; }
    public string UserLogin { get; set; }
}

public class JwtHelper
{
    public static JwtToken DecodeJwtToken(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

        if (jsonToken == null)
        {
            throw new Exception("Invalid JWT token");
        }

        var userName = jsonToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
        var userRole = jsonToken.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;
        var userLogin = jsonToken.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;

        return new JwtToken
        {
            UserName = userName,
            UserRole = userRole,
            UserLogin = userRole
        };
    }
}
