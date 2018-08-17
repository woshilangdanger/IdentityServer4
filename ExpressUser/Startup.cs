using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using ExpressUser.Config;
using System.Reflection;
using ExpressUser.Services.UserInteraction;
using ExpressUser.Services;
using IdentityServer4.Services;
using ExpressUser.Extensions;
using Infrastructure;
using Infrastructure.Services;
using Infrastructure.Services.Impl;

namespace ExpressUser
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region 基础配置模块
            string _migrationAssablyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddMvc();
            services.AddOptions();
            services.Configure<IdentityServer4Config>(Configuration.GetSection("IdentityServer4Config"));//配置
            services.Configure<UserDbConfig>(Configuration.GetSection("UserDbConfig"));//配置 
            #endregion

            #region AspNetCore Identity 模块
            var userdbconfig = Configuration.GetSection("UserDbConfig").Get<UserDbConfig>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(userdbconfig.UserConnectString, builder =>
                {
                    builder.MigrationsAssembly(_migrationAssablyName);
                });
            });

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;


            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
               // .AddClaimsPrincipalFactory<UserClaimsPrincipal>();


            #endregion

            #region IdentityServer4 基础设置及配置
            services.AddIdentityServer(options =>
            {
                #region 用户交互配置 主要涉及到入口地址参数等
                options.UserInteraction = new IdentityServer4.Configuration.UserInteractionOptions
                {
                    LoginUrl = "/Account/Login",
                    LogoutUrl = "/Account/Logout",
                    ConsentUrl = "/Account/Consent",
                    ErrorUrl = "/Account/Error",
                    LoginReturnUrlParameter = "ReturnUrl",
                    LogoutIdParameter = "logoutId",
                    ConsentReturnUrlParameter = "ReturnUrl",
                    ErrorIdParameter = "errorId",
                    CustomRedirectReturnUrlParameter = "ReturnUrl",
                    CookieMessageThreshold = 5
                };
                #endregion

            })
            #endregion

            .AddDeveloperSigningCredential()

            #region 使用内存数据测试
            //.AddInMemoryIdentityResources(MemoryClients.GetIdentityResources())
            //.AddInMemoryApiResources(MemoryClients.GetApiResources())
            //.AddInMemoryClients(MemoryClients.GetClients()) 
            #endregion
            #region 添加EF数据库支持 可以使用 SqlServer MySql Sqlite
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                {
                    #region 通过配置 EF 适配各种数据库
                    var config = Configuration.GetSection("IdentityServer4Config").Get<IdentityServer4Config>();
                    switch (config.DbType)
                    {
                        case 1:
                            //适配Migration
                            builder.UseSqlServer(config.Idr4ConnectString, sqlserver =>
                            {
                                sqlserver.MigrationsAssembly(_migrationAssablyName);
                                sqlserver.UseRelationalNulls();
                                sqlserver.UseRowNumberForPaging();
                            });
                            break;
                        case 2:
                            //MySQL支持
                            builder.UseMySql(config.Idr4ConnectString, mysql =>
                            {
                                mysql.MigrationsAssembly(_migrationAssablyName);
                                mysql.UseRelationalNulls();
                            });
                            break;

                        default:
                            break;
                    }
                    #endregion
                };

            })

            .AddOperationalStore(options =>
            {

                options.ConfigureDbContext = builder =>
                {
                    #region 通过配置 EF 适配各种数据库
                    var config = Configuration.GetSection("IdentityServer4Config").Get<IdentityServer4Config>();
                    switch (config.DbType)
                    {
                        case 1:
                            //适配Migration
                            builder.UseSqlServer(config.Idr4ConnectString, sqlserver =>
                            {
                                sqlserver.MigrationsAssembly(_migrationAssablyName);
                                sqlserver.UseRelationalNulls();
                                sqlserver.UseRowNumberForPaging();
                            });
                            break;
                        case 2:
                            //MySQL支持
                            builder.UseMySql(config.Idr4ConnectString, mysql =>
                            {
                                mysql.MigrationsAssembly(_migrationAssablyName);
                                mysql.UseRelationalNulls();
                            });
                            break;

                        default:
                            break;
                    }
                    #endregion

                };
                //允许清理
                options.EnableTokenCleanup = true;
                //清理条数
                // options.TokenCleanupBatchSize = 1000;
                //每隔多长时间清理一次 (单位)s
                options.TokenCleanupInterval = 3600;


            })
            #endregion
            #region 添加授权验证方式
                     //.AddExtensionGrantValidator<CustomExtensionGrantUserServices>()
                   //  .AddResourceOwnerValidator<CustomPasswordOwnerUserServices>()
                     //.AddSecretValidator<CustomUserClientSecretValidator>()
            #endregion
            .AddAspNetIdentity<IdentityUser>();
            services.AddScoped<IUserStoreService, UserStoreService>();
           // services.AddScoped<IProfileService, CustomProfileService>();
            services.AddScoped<IUserServices, UserServices>();
          

            #region 授权登录

            services.AddAuthentication(
              options =>
              {
                  options.DefaultScheme = "Cookies";
                  options.DefaultChallengeScheme = "oidc";
               
              })
          .AddCookie("Cookies")
          .AddOpenIdConnect("oidc", options =>
          {

              options.SignInScheme = "Cookies";
              options.Authority = "http://localhost:20001";
              options.ClientId = "express";
              options.ClientSecret = "express";
              options.RequireHttpsMetadata = false;
              options.SaveTokens = true;
              options.ResponseType = "code id_token";
              options.GetClaimsFromUserInfoEndpoint = true;
              options.Scope.Add("expressapi");
              options.GetClaimsFromUserInfoEndpoint = true;

          });
            #endregion

            #region 
            services.AddTransient<IUserServices, UserServices>();
            #endregion

        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseInitIdr4Data();
            app.UseInitIdentityData();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Body}/{id?}");
                routes.MapRoute(
             name: "areas",
             template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
           );
            });
        }
    }
}
