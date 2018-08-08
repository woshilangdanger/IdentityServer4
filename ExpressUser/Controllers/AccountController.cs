using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Services;
using IdentityServer4.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using IdentityServer4.Stores;
using System.Security.Principal;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using ExpressUser.Services.UserInteraction;
using ExpressUser.Models;
using Microsoft.AspNetCore.Identity;

namespace ExpressUser.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _identityServerInteractionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IResourceStore _resourceStore;
        private readonly IClientStore _clientStore;
        private readonly IEventService _events;
        private readonly IUserStoreService _testUserStore;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(IIdentityServerInteractionService identityServerInteractionService, IUserStoreService testUserStore, IEventService events, IHttpContextAccessor httpContextAccessor, IAuthenticationSchemeProvider schemeProvider, IClientStore clientStore, IResourceStore resourceStore, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _identityServerInteractionService = identityServerInteractionService;
            _testUserStore = testUserStore;
            _events = events;
            _httpContextAccessor = httpContextAccessor;
            _schemeProvider = schemeProvider;
            _clientStore = clientStore;
            _resourceStore = resourceStore;


        }

        #region 扩展登陆 黎又铭 20180410
        /// <summary>
        /// 展示扩展登录页面 提供来之其他客户端的扩展登录界面
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
        {
            var props = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("ExternalLoginCallback"),
                Items =
                {
                    { "returnUrl", returnUrl }
                }
            };

            //windows授权需要特殊处理，原因是windows没有对回调跳转地址的处理，所以当我们调用授权请求的时候需要再次触发URL跳转
            if (AccountOptions.WindowsAuthenticationSchemeName == provider)
            {
                var result = await HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
                if (result?.Principal is WindowsPrincipal wp)
                {
                    props.Items.Add("scheme", AccountOptions.WindowsAuthenticationSchemeName);
                    var id = new ClaimsIdentity(provider);
                    id.AddClaim(new Claim(JwtClaimTypes.Subject, wp.Identity.Name));
                    id.AddClaim(new Claim(JwtClaimTypes.Name, wp.Identity.Name));

                    //将授权认证的索赔信息添加进去 注意索赔信息的大小
                    if (AccountOptions.IncludeWindowsGroups)
                    {
                        var wi = wp.Identity as WindowsIdentity;
                        var groups = wi.Groups.Translate(typeof(NTAccount));
                        var roles = groups.Select(x => new Claim(JwtClaimTypes.Role, x.Value));
                        id.AddClaims(roles);
                    }

                    await HttpContext.SignInAsync(
                        IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme,
                        new ClaimsPrincipal(id),
                        props);
                    return Redirect(props.RedirectUri);
                }
                else
                {

                    return Challenge(AccountOptions.WindowsAuthenticationSchemeName);
                }
            }
            else
            {

                props.Items.Add("scheme", provider);
                return Challenge(props, provider);
            }
        }


        /// <summary>
        /// 扩展授权
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {

            var result = await HttpContext.AuthenticateAsync(IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("外部授权错误");
            }

            // 获取外部登录的Claims信息
            var externalUser = result.Principal;
            var claims = externalUser.Claims.ToList();

            //尝试确定外部用户的唯一ID（由提供者发出）
            //最常见的索赔，索赔类型分，nameidentifier
            //取决于外部提供者，可能使用其他一些索赔类型
            var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
            if (userIdClaim == null)
            {
                userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            }
            if (userIdClaim == null)
            {
                throw new Exception("未知用户");
            }

            //从集合中移除用户ID索赔索赔和移动用户标识属性还设置外部身份验证提供程序的名称。
            claims.Remove(userIdClaim);
            var provider = result.Properties.Items["scheme"];
            var userId = userIdClaim.Value;

            // 这是最有可能需要自定义逻辑来匹配您的用户的外部提供者的身份验证结果，并为用户提供您所认为合适的结果。
            //  检查外部用户已经设置
            var user = "";// _users.FindByExternalProvider(provider, userId);
            if (user == null)
            {
                //此示例只是自动提供新的外部用户，另一种常见的方法是首先启动注册工作流
                //user = _users.AutoProvisionUser(provider, userId, claims);
            }

            var additionalClaims = new List<Claim>();

            // 如果外部系统发送了会话ID请求，请复制它。所以我们可以用它进行单点登录
            var sid = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                additionalClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            //如果外部供应商发出id_token，我们会把它signout
            AuthenticationProperties props = null;
            var id_token = result.Properties.GetTokenValue("id_token");
            if (id_token != null)
            {
                props = new AuthenticationProperties();
                props.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
            }

            // 为用户颁发身份验证cookie
            //   await _events.RaiseAsync(new UserLoginSuccessEvent(provider, userId, user.SubjectId, user.Username));
            // await HttpContext.SignInAsync(user.SubjectId, user.Username, provider, props, additionalClaims.ToArray());

            // 删除外部验证期间使用的临时cookie
            await HttpContext.SignOutAsync(IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // 验证返回URL并重定向回授权端点或本地页面
            var returnUrl = result.Properties.Items["returnUrl"];
            if (_identityServerInteractionService.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }
        #endregion

        #region 登录处理  黎又铭 20180410
        /// <summary>
        /// 构造下Idr4登陆界面显示视图模型
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        private async Task<Idr4LoginViewModel> CreateIdr4LoginViewModelAsync(string ReturnUrl)
        {
            Idr4LoginViewModel vm = new Idr4LoginViewModel();
            var context = await _identityServerInteractionService.GetAuthorizationContextAsync(ReturnUrl);
            if (context != null)
            {
                if (context?.IdP != null)
                {
                    // 扩展外部扩展登录模型处理
                    vm.EnableLocalLogin = false;
                    vm.ReturnUrl = ReturnUrl;
                    vm.username = context?.LoginHint;
                    vm.ExternalProviders = new ExternalProvider[] { new ExternalProvider { AuthenticationScheme = context.IdP } };

                }

            }
            //外部登陆 获取所有授权信息 并查找当前可用的授权信息
            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;

            if (context?.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;
                    vm.ClientName = client.ClientName;
                    vm.ClientUrl = client.ClientUri;
                    vm.ClientLogoUrl = client.LogoUri;
                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }
            vm.AllowRememberLogin = AccountOptions.AllowRememberLogin;
            vm.EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin;
            vm.ReturnUrl = ReturnUrl;
            vm.username = context?.LoginHint;
            vm.ExternalProviders = providers.ToArray();

            return vm;
        }

        /// <summary>
        /// Idr4授权访问登录并获取授权回调地址
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Login(string ReturnUrl)
        {
            //创建视图模型
            var vm = await CreateIdr4LoginViewModelAsync(ReturnUrl);
            //判断来之其他客户端的登录
            if (vm.IsExternalLoginOnly)
            {
                return await ExternalLogin(vm.ExternalLoginScheme, ReturnUrl);
            }
            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Idr4LoginViewModel model)
        {

            #region  Idr4验证处理 这里主要对ReturnUrl处理

            var context = await _identityServerInteractionService.GetAuthorizationContextAsync(model.ReturnUrl);
            if (context == null)
            {
                //不存在客户端信息
                Redirect("~/");

            }
            #endregion
            #region 基础验证
            if (string.IsNullOrEmpty(model.username))
            {
                ModelState.AddModelError("", "请输入用户名");

            }
            if (string.IsNullOrEmpty(model.password))
            {
                ModelState.AddModelError("", "请输入密码");

            }
            #endregion
            if (ModelState.IsValid)
            {
                var user = await _testUserStore.FindByName(model.username);
                if (user == null)
                {
                    ModelState.AddModelError("", "用户不存在");
                }
                else if (await _testUserStore.ValidatorUser(user, model.password))
                {

                    //查询用户信息

                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));
                    //记住登录
                    AuthenticationProperties authenticationProperties = null;
                    if (AccountOptions.AllowRememberLogin && model.RememberLogin)
                    {
                        authenticationProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                        };
                    }
                    //SignIn
                    var userClaims = await _testUserStore.GetAllClaimsByUser(user);
                    await HttpContext.SignInAsync(user.Id, user.UserName, authenticationProperties, userClaims.ToArray());

                    if (_identityServerInteractionService.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");

                }
                else
                {

                    await _events.RaiseAsync(new UserLoginFailureEvent(model.username, "登录失败"));
                    ModelState.AddModelError("", AccountOptions.InvalidCredentialsErrorMessage);
                }
            }
            //防止验证失败后返回视图后 界面模型参数不存在 所以这里需要构建一次模型
            var vm = await CreateIdr4LoginViewModelAsync(model.ReturnUrl);
            return View("Login", vm);
        }
        #endregion

        #region 同意授权页面处理 黎又铭 20180410

        private async Task<Idr4ConsentViewModel> CreateIdr4ConsentViewModelAsync(string ReturnUrl)
        {
            var request = await _identityServerInteractionService.GetAuthorizationContextAsync(ReturnUrl);
            if (request != null)
            {
                //通过客户端id获取客户端信息
                var clientModel = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);
                if (clientModel != null)
                {
                    //获取资源Scope信息 这里包括了两种 一种是IdentityResource 和ApiResource

                    var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
                    //获取所有的权限

                    // var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(clientModel.AllowedScopes);

                    if (resources != null && (resources.ApiResources.Any() || resources.IdentityResources.Any()))
                    {
                        //构造界面需要的模型


                        var vm = new Idr4ConsentViewModel();

                        //界面初始化时候
                        vm.RememberConsent = true; //默认true
                        vm.ScopesConsented = Enumerable.Empty<string>();
                        vm.ReturnUrl = ReturnUrl;
                        //构建关于Client的信息
                        vm.ClientName = clientModel.ClientName;
                        vm.ClientUrl = clientModel.ClientUri;
                        vm.ClientLogoUrl = clientModel.LogoUri;
                        vm.AllowRememberConsent = clientModel.AllowRememberConsent;
                        vm.IdentityScopes = resources.IdentityResources.Select(x => new Idr4ScopeViewModel
                        {
                            Name = x.Name,
                            DisplayName = x.DisplayName,
                            Description = x.Description,
                            Emphasize = x.Emphasize,
                            Required = x.Required,
                            Checked = vm.ScopesConsented.Contains(x.Name) || x.Required
                        }).ToArray();
                        vm.ResouceScopes = resources.ApiResources.SelectMany(x => x.Scopes).Select(k => new Idr4ScopeViewModel
                        {
                            Name = k.Name,
                            DisplayName = k.DisplayName,
                            Description = k.Description,
                            Emphasize = k.Emphasize,
                            Required = k.Required,
                            Checked = vm.ScopesConsented.Contains(k.Name) || k.Required

                        }).ToArray();
                        //离线
                        if (ConsentOptions.EnableOfflineAccess && resources.OfflineAccess)
                        {
                            vm.ResouceScopes = vm.ResouceScopes.Union(new Idr4ScopeViewModel[] {
                                new Idr4ScopeViewModel{

                                    Name = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
                                    DisplayName = ConsentOptions.OfflineAccessDisplayName,
                                    Description = ConsentOptions.OfflineAccessDescription,
                                    Emphasize = true,
                                    Checked = vm.ScopesConsented.Contains(IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess)
                                }
                            });
                        }
                        return vm;
                    }
                    else
                    {
                        //客户端Scope不存在 可以在界面提示并记录日志
                        return null;
                    }

                }
                else
                {
                    //客户端不存在 可以在界面提示并记录日志
                    return null;

                }

            }
            return null;
        }


        [HttpGet]
        public async Task<IActionResult> Consent(string ReturnUrl)
        {
            //获取请求授权信息
            var vm = await CreateIdr4ConsentViewModelAsync(ReturnUrl);
            if (vm != null)
            {
                return View(vm);
            }
            return View();
        }
        /// <summary>
        /// 接受模型
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Consent(Idr4ConsentViewModel model)
        {

            ConsentResponse consentResponse = null;

            if (model == null)
            {
                ModelState.AddModelError("", "数据发送异常");
            }
            //有没有选择授权

            if (model.ScopesConsented == null || model.ScopesConsented.Count() == 0)
            {
                ModelState.AddModelError("", "请至少选择一个权限");
            }


            //同意授权
            if (model.Button == "yes")
            {
                //选择了授权Scope
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    var scopes = model.ScopesConsented;
                    if (ConsentOptions.EnableOfflineAccess == false)
                    {
                        scopes = scopes.Where(x => x != IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess);
                    }

                    consentResponse = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesConsented = scopes
                    };




                }
            }
            //不同意授权
            else if (model.Button == "no")
            {
                consentResponse = ConsentResponse.Denied;
            }
            else
            {
                var vm1 = await CreateIdr4ConsentViewModelAsync(model.ReturnUrl);
                return View(vm1);

            }

            //无论同意还是不同意都是需要跳转
            if (consentResponse != null)
            {

                var request = await _identityServerInteractionService.GetAuthorizationContextAsync(model.ReturnUrl);
                if (request == null)
                {
                    ModelState.AddModelError("", "客户端登录验证不匹配");
                }
                //if (consentResponse == ConsentResponse.Denied)
                //{
                //    string url = new Uri(request.RedirectUri).Authority;
                //    return Redirect(url);
                //}

                //沟通Idr4服务端实现授权
                await _identityServerInteractionService.GrantConsentAsync(request, consentResponse);

                return Redirect(model.ReturnUrl);


            }


            var vm = await CreateIdr4ConsentViewModelAsync(model.ReturnUrl);
            if (vm != null)
            {
                return View(vm);
            }

            return View();
        }
        #endregion

        #region 错误页面 黎又铭 20180410

        public async Task<IActionResult> Error(string errorId)
        {
            var model = await _identityServerInteractionService.GetErrorContextAsync(errorId);

            Idr4ErrorViewModel idr4ErrorViewModel = new Idr4ErrorViewModel
            {
                Error = model.Error,
                RequestId = model.RequestId,
                ErrorDescription = model.ErrorDescription

            };

            return View(idr4ErrorViewModel);
        }

        #endregion

        #region 退出处理 黎又铭 20180410



        /// <summary>
        /// 构建退出视图模型
        /// </summary>
        /// <param name="logoutId"></param>
        /// <returns></returns>
        private async Task<Idr4LogoutViewModel> CreateIdr4LogoutViewModelAsync(string logoutId)
        {

            var vm = new Idr4LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            var user = _httpContextAccessor.HttpContext.User;
            if (user?.Identity.IsAuthenticated != true)
            {
                //没有授权展示已退出相关业务处理页面
                vm.ShowLogoutPrompt = false;

            }

            var context = await _identityServerInteractionService.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                //用户处理退出  安全退出到退出业务处理页面
                vm.ShowLogoutPrompt = false;

            }
            if (!vm.ShowLogoutPrompt)
            {
                string clientid = context.ClientIds.FirstOrDefault();
                var clientModel = await _clientStore.FindClientByIdAsync(clientid);
                vm.PostLogoutRedirectUri = clientModel?.PostLogoutRedirectUris.FirstOrDefault();
                vm.SignOutIFrameUrl = context.SignOutIFrameUrl;
            }

            return vm;
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            //这里做下改造
            var vm = await CreateIdr4LogoutViewModelAsync(logoutId);
            if (vm.ShowLogoutPrompt == false)
            {
                //不需要二次确认的时候做特殊处理 转到已经退出界面
                //我们在 LoggedOut 处理 使用Js自动跳转

                var user = HttpContext.User;
                if (user?.Identity.IsAuthenticated == true)
                {
                    //删除本地授权Cookies
                    await HttpContext.SignOutAsync();
                    await _events.RaiseAsync(new UserLogoutSuccessEvent(user.GetSubjectId(), user.GetDisplayName()));
                }

                return View("LoggedOut", vm);


            }
            return View(vm);


        }
        [HttpGet]
        public IActionResult LoggedOut(Idr4LogoutViewModel model)
        {
            return View(model);
        }
        /// <summary>
        /// 主要是到页面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(Idr4LogoutViewModel model)
        {
            var vm = await _identityServerInteractionService.GetLogoutContextAsync(model.LogoutId);
            return View();
        }




        #endregion


    }
}