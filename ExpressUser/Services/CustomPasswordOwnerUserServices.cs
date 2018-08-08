using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using IdentityServer4.Models;
using ExpressUser.Services.UserInteraction;

namespace ExpressUser.Services
{
    /// <summary>
    /// 实现密码模式
    /// </summary>
    public class CustomPasswordOwnerUserServices : IResourceOwnerPasswordValidator 
    {
        /// <summary>
        /// 注入用户服务
        /// </summary>
        private IUserStoreService _userStoreServices;
        public CustomPasswordOwnerUserServices(IUserStoreService userStoreServices)
        {
            _userStoreServices = userStoreServices;
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userStoreServices.FindByName(context.UserName);
            if (user == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "用户不存在");
            }
           
            if (await _userStoreServices.ValidatorUser(user, context.Password))
            {
                var localuser= await _userStoreServices.FindByName(context.UserName);
                context.Result = new GrantValidationResult(subject: localuser.Id.ToString(), authenticationMethod: "custom_password");
            }
            else {

                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "用户名密码错误");

            }
            
           
        }
    }
}
