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
    /// 扩展的登录  根据 设置grant_type
    /// 当客户端使用granttype来请求
    /// </summary>
    public class CustomExtensionGrantUserServices : IExtensionGrantValidator
    {
        string IExtensionGrantValidator.GrantType => "custom_grant";

        private IUserStoreService _userStoreServices;
        public CustomExtensionGrantUserServices(IUserStoreService userStoreServices)
        {
            _userStoreServices = userStoreServices;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            //var row= context.Request.Raw;
            //var keyusername = row.Get("keyusername");
            //var keypassword = row.Get("keypassword");
            //if (await _userStoreServices.ValidatorUser(keyusername, keypassword))
            //{
            //    var localuser = await _userStoreServices.GetUserByName(keyusername);
            //    context.Result = new GrantValidationResult(subject: localuser.Id.ToString(), authenticationMethod: "custom_grant");
              
            //}
            //else {
            //    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "");
            //}
           
        }
    }
}
