using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace ExpressUser.Services
{
    public class CustomIdentityServerInteractionService : IIdentityServerInteractionService
    {
        public Task<string> CreateLogoutContextAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Consent>> GetAllUserConsentsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AuthorizationRequest> GetAuthorizationContextAsync(string returnUrl)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorMessage> GetErrorContextAsync(string errorId)
        {
            throw new NotImplementedException();
        }

        public Task<LogoutRequest> GetLogoutContextAsync(string logoutId)
        {
            throw new NotImplementedException();
        }

        public Task GrantConsentAsync(AuthorizationRequest request, ConsentResponse consent, string subject = null)
        {
            throw new NotImplementedException();
        }

        public bool IsValidReturnUrl(string returnUrl)
        {
            throw new NotImplementedException();
        }

        public Task RevokeTokensForCurrentSessionAsync()
        {
            throw new NotImplementedException();
        }

        public Task RevokeUserConsentAsync(string clientId)
        {
            throw new NotImplementedException();
        }
    }
}
