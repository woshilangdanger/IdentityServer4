using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressUser.Services.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizeCodeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public AuthorizeCodeAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var claims = context.HttpContext.User.Claims;
            if (!claims.ToList().Select(c => c.Value).Contains(Name))
            {
                context.Result = new ForbidResult();
            }
            await Task.CompletedTask;
        }
    }
}
