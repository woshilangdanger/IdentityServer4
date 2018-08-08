using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExpressUser.Services.UserInteraction
{
    /// <summary>
    /// 处理创建用户身份信息
    /// </summary>
    public class UserClaimsPrincipal : IUserClaimsPrincipalFactory<IdentityUser>
    {
        private readonly IUserStoreService _storeService;
        public UserClaimsPrincipal(IUserStoreService storeService)
        {
            _storeService = storeService;
        }
        public async Task<ClaimsPrincipal> CreateAsync(IdentityUser user)
        {
            var claims = await _storeService.GetAllClaimsByUser(user);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return await Task.FromResult(claimsPrincipal);

        }
    }
}
