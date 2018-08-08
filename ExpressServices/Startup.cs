using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExpressServices.Config;
using ExpressServices.QueryServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;


namespace ExpressServices
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddOptions();
            services.Configure<AuthorityConfig>(Configuration.GetSection("AuthorityConfig"));


            #region JwtBearer授权
            var _authorityconfig = Configuration.GetSection("AuthorityConfig").Get<AuthorityConfig>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddJwtBearer(options =>
                  {
                      options.Authority = _authorityconfig.Authority;
                      options.RequireHttpsMetadata = _authorityconfig.RequireHttpsMetadata;
                      options.Audience = "expressapi"; //scope;


                  })
                  ;
            #endregion
            #region SwaggerUI 配置
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1.0",
                    Title = "快递服务接口"
                });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "ExpressServices.xml");
                options.IncludeXmlComments(xmlPath);


                #region Swagger添加授权验证服务

                //options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                //{
                //    Description = "JWT Bearer 授权 \"Authorization: Bearer+空格+token\"",
                //    Name = "Authorization",
                //    In = "header",
                //    Type = "apiKey"
                //});
               
                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = _authorityconfig.Authority + "/connect/authorize",
                    Scopes = new Dictionary<string, string>
                        {
                            { "expressapi", "快递接口服务" }
                        }
                });
                options.OperationFilter<IdentityServer4OAuth2OperationFilter>();
                #endregion

            });
            #endregion
            #region  跨域策略
            services.AddCors(options =>
            {
                options.AddPolicy("mypolicy", builder =>
                 builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials()
                 );
            });
            #endregion
            services.AddScoped<IDbProviderFactory, CreateDbProviderFactory>();
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("mypolicy");
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi");
                c.OAuthClientId("expressapitest");
                c.OAuthAppName("expressapi");
            });
        }
    }
}
