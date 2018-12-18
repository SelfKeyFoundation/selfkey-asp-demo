using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;

namespace SelfKey.Login.Client.Helpers
{
    public class AuthenticationHelper : IAuthenticationHelper
    {
        private readonly IMemoryCache _memoryCache;

        public AuthenticationHelper(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Gets an access token. First tries to get the access token from the token cache using password (secret) to authenticate. Production apps should use a certificate.
        /// </summary>
        public async Task<string> GetUserAccessTokenAsync(string userId)
        {
            var cca = new ConfidentialClientApplication(
                AzureAdOptions.Settings.ClientId,
                AzureAdOptions.Settings.BaseUrl + AzureAdOptions.Settings.CallbackPath,
                new ClientCredential(AzureAdOptions.Settings.ClientSecret),
                new SessionTokenCache(userId, _memoryCache).GetCacheInstance(),
                null);

            IAccount[] accounts = (await cca.GetAccountsAsync()).ToArray();

            return accounts.Any() ? (await cca.AcquireTokenSilentAsync(AzureAdOptions.Settings.Scopes.Split(new[] { ' ' }), accounts.First())).AccessToken : null;
        }
    }

    public interface IAuthenticationHelper
    {
        Task<string> GetUserAccessTokenAsync(string userId);
    }
}
