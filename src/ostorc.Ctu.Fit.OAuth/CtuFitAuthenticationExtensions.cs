using Microsoft.Extensions.DependencyInjection;

namespace ostorc.Ctu.Fit.OAuth;

public static class CtuFitAuthenticationExtensions
{
    public static AuthenticationBuilder AddCtuFit(
        this AuthenticationBuilder builder, Action<CtuFitAuthenticationOptions> configuration)
    {
        return builder.AddCtuFit(CtuFitAuthenticationDefaults.AuthenticationScheme,
            CtuFitAuthenticationDefaults.DisplayName, configuration);
    }

    public static AuthenticationBuilder AddCtuFit(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        string displayName,
        Action<CtuFitAuthenticationOptions> configuration)
    {
        return builder
            .AddOAuth<CtuFitAuthenticationOptions, CtuFitAuthenticationHandler>(authenticationScheme, displayName,
                configuration);
    }
}