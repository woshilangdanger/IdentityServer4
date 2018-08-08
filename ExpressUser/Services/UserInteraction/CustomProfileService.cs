using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using System.Security.Claims;

namespace ExpressUser.Services.UserInteraction
{
    public class CustomProfileService : IProfileService
    {
        IUserStoreService _userStoreServices;
        public CustomProfileService(IUserStoreService userStoreServices)
        {
            _userStoreServices = userStoreServices;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            //设置允许的Claim信息
            var id = context.Subject.Claims.FirstOrDefault(c => c.Type == "sub");
            var user = await _userStoreServices.FindByUserId(id.Value);
            var userclaims = await _userStoreServices.GetAllClaimsByUser(user);
            context.IssuedClaims = userclaims.ToList();



        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = false;
            var id = context.Subject.Claims.FirstOrDefault(c => c.Type == "sub");
            var user = await _userStoreServices.FindByUserId(id.Value);
            context.IsActive = user != null;


        }
    }
}
