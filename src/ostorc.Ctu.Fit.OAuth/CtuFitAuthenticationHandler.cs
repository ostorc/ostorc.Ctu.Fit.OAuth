using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ostorc.Ctu.Fit.OAuth;

public class CtuFitAuthenticationHandler : OAuthHandler<CtuFitAuthenticationOptions>
{
    public CtuFitAuthenticationHandler(IOptionsMonitor<CtuFitAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        TimeProvider timeProvider) : base(options, logger, encoder)
    {
        options.OnChange(authenticationOptions => authenticationOptions.TimeProvider = timeProvider);
    }

    protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
    {
        var tokenRequestParameters = new Dictionary<string, string>()
        {
            { "redirect_uri", context.RedirectUri },
            { "code", context.Code },
            { "grant_type", "authorization_code" },
        };

        var credentials =
            Convert.ToBase64String(Encoding.UTF8.GetBytes(Options.ClientId + ":" + Options.ClientSecret));

        var requestContent = new FormUrlEncodedContent(tokenRequestParameters);

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, Options.TokenEndpoint);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        requestMessage.Content = requestContent;
        requestMessage.Version = Backchannel.DefaultRequestVersion;
        var response = await Backchannel.SendAsync(requestMessage, Context.RequestAborted);
        var body = await response.Content.ReadAsStringAsync(Context.RequestAborted);

        return response.IsSuccessStatusCode switch
        {
            true => OAuthTokenResponse.Success(JsonDocument.Parse(body)),
            false => OAuthTokenResponse.Failed(new Exception())
        };
    }

    protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity,
        AuthenticationProperties properties, OAuthTokenResponse tokens)
    {
        var uriBuilder = new UriBuilder(Options.UserInformationEndpoint)
        {
            Query = $"token={tokens.AccessToken}"
        };

        using var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response =
            await Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, Context.RequestAborted);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("An error occurred while retrieving the user profile.");
        }

        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync(Context.RequestAborted));

        var principal = new ClaimsPrincipal(identity);
        var context = new OAuthCreatingTicketContext(principal, properties, Context, Scheme, Options, Backchannel,
            tokens, payload.RootElement);
        context.RunClaimActions();

        await Events.CreatingTicket(context);
        return new AuthenticationTicket(context.Principal!, context.Properties, Scheme.Name);
    }
}