using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using IdentityServer4.Models;

using IdentityServer4.Stores;

namespace ExpressUser.Services
{
    public class CustomUserClientSecretValidator :ISecretValidator
    {

        private readonly IClientStore _clientStore;
        public CustomUserClientSecretValidator(IClientStore clientStore)
        {
            _clientStore = clientStore;
        }


        public async Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var clientmodel=  await _clientStore.FindEnabledClientByIdAsync(parsedSecret.Id);
            if (clientmodel != null)
            {
                if (clientmodel.ClientSecrets.FirstOrDefault()?.Value == secrets.FirstOrDefault()?.Value)
                {
                    return await Task.FromResult(new SecretValidationResult { Success = true });
                }
                else {
                    return await Task.FromResult(new SecretValidationResult { IsError = true, Error = "客户端密钥错误", ErrorDescription = "客户端密钥错误" });
                }
             

            }

           return await Task.FromResult(new SecretValidationResult {  IsError=true,  Error="不存在客户端信息", ErrorDescription= "不存在客户端信息" });
        }
    }
}
