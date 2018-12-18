using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Identity.Client;
using SelfKey.Login.Client.Helpers;

namespace Microsoft.AspNetCore.Authentication
{
    public static class AzureAdAuthenticationBuilderExtensions
    {
        private const string ObjectIdentifierType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        private const string TenantIdType = "http://schemas.microsoft.com/identity/claims/tenantid";

        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder) => builder.AddAzureAd(_ => { });

        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder, Action<AzureAdOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureAzureOptions>();
            builder.AddOpenIdConnect();
            return builder;
        }

        public class ConfigureAzureOptions : IConfigureNamedOptions<OpenIdConnectOptions>
        {
            private readonly AzureAdOptions _azureOptions;

            public AzureAdOptions GetAzureAdOptions() => _azureOptions;

            public ConfigureAzureOptions(IOptions<AzureAdOptions> azureOptions) => _azureOptions = azureOptions.Value;

            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.ClientId = _azureOptions.ClientId;
                options.Authority = $"{_azureOptions.Instance}common/v2.0";
                options.UseTokenLifetime = true;
                options.CallbackPath = _azureOptions.CallbackPath;
                options.RequireHttpsMetadata = false;
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                foreach (string scope in $"{_azureOptions.Scopes}".Split(new[] { ' ' }))
                {
                    options.Scope.Add(scope);
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Ensure that User.Identity.Name is set correctly after login.
                    NameClaimType = "name",

                    // Instead of using the default validation (validating against a single issuer value, as we do in line of business apps),
                    // we inject our own multitenant validation logic.
                    ValidateIssuer = false

                    // If the app is meant to be accessed by entire organizations, add your issuer validation logic here.
                    //IssuerValidator = (issuer, securityToken, validationParameters) => {
                    //    if (myIssuerValidationLogic(issuer)) return issuer;
                    //}
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = c => Task.CompletedTask,
                    OnAuthenticationFailed = c =>
                    {
                        c.Response.Redirect("/Home/Error");
                        c.HandleResponse(); // Suppress the exception.
                        return Task.CompletedTask;
                    },
                    OnAuthorizationCodeReceived = async c =>
                    {
                        var cca = new ConfidentialClientApplication(
                            _azureOptions.ClientId, 
                            _azureOptions.BaseUrl + _azureOptions.CallbackPath,
                            new ClientCredential(_azureOptions.ClientSecret),
                            new SessionTokenCache(c.Principal.FindFirst(ObjectIdentifierType).Value, c.HttpContext.RequestServices.GetRequiredService<IMemoryCache>()).GetCacheInstance(), 
                            null);

                        AuthenticationResult result = await cca.AcquireTokenByAuthorizationCodeAsync(c.ProtocolMessage.Code, _azureOptions.Scopes.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

                        // Check whether the login is from the MSA tenant. 
                        // The sample uses this attribute to disable UI buttons for unsupported operations when the user is logged in with an MSA account.
                        if (c.Principal.FindFirst(TenantIdType).Value == "9188040d-6c67-4c5b-b112-36a304b66dad")
                        {
                            // MSA (Microsoft Account) is used to log in.
                        }
                        
                        c.HandleCodeRedemption(result.AccessToken, result.IdToken);
                    },
                    // If your application needs to do authenticate single users, add your user validation below.
                    //OnTokenValidated = context =>
                    //{
                    //    return myUserValidationLogic(context.Ticket.Principal);
                    //}
                };
            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
