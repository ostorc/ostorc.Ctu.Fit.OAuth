using System.Security.Claims;

namespace ostorc.Ctu.Fit.OAuth;

public class CtuFitAuthenticationOptions: OAuthOptions
{
    public CtuFitAuthenticationOptions()
    {
        AuthorizationEndpoint = CtuFitAuthenticationDefaults.AuthorizationEndpoint;
        TokenEndpoint = CtuFitAuthenticationDefaults.TokenEndpoint;
        UserInformationEndpoint = CtuFitAuthenticationDefaults.UserInformationEndpoint;
        
        ClaimsIssuer = CtuFitAuthenticationDefaults.Issuer;
        CallbackPath = CtuFitAuthenticationDefaults.CallbackPath;
        
        ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "user_name");
        ClaimActions.MapJsonKey(ClaimTypes.Name, "user_name");
        ClaimActions.MapCustomJson(ClaimTypes.Email, e =>
        {
            var userName = e.GetString("user_name");
            if (userName is null)
                return null;

            return $"{userName}@cvut.cz";
        });
        
    }
}