using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ExpressUser.Controllers
{

    public class LogoutController : Controller
    {
        [Route("~/logout")]
        public async Task Index(string sid)
        {
            var cp = User as ClaimsPrincipal;
            var claimsid = cp.FindFirst("sid");
            if (claimsid != null && claimsid.Value == sid)
            {
                await HttpContext.SignOutAsync("Cookies");
            }
        }
    }
}