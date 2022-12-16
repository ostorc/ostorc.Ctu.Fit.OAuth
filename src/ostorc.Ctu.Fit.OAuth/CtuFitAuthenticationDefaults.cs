namespace ostorc.Ctu.Fit.OAuth;

public static class CtuFitAuthenticationDefaults
{
    public const string AuthenticationScheme = "FitCtu";
    public const string DisplayName = "FIT CTU";
    public const string Issuer = "FIT CTU";
    public const string CallbackPath = "/signin-ctu-fit";
    public const string AuthorizationEndpoint = "https://auth.fit.cvut.cz/oauth/authorize";
    public const string TokenEndpoint = "https://auth.fit.cvut.cz/oauth/token";
    public const string UserInformationEndpoint = "https://auth.fit.cvut.cz/oauth/check_token";
}