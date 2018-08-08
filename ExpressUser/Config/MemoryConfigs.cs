
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressUser.Config
{
    public class MemoryConfigs
    {
        public static List<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {

                new IdentityResource{
                    Name="openid",
                     Enabled=true,
                      Emphasize=true,
                       Required=true,
                       DisplayName="用户授权认证信息",
                       Description="获取你的授权认证"
        },
                new IdentityResource{
                    Name="profile",
                     Enabled=true,
                      Emphasize=false,
                       Required=true,
                        DisplayName="用户个人信息",
                         Description="获取你的个人基本资料信息，如：姓名、性别、年龄等"
                }

};

        }
        public static List<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
               //普通的通过构造函数限制 指定scope以及displayname 就行了
              new ApiResource("expressapi","快递接口服务")
            };
        }

        public static List<Client> GetClients()
        {

            

            return new List<Client> {

                new Client(){

                    ClientId="express",
                    ClientName="试卷管理系统",
                    ClientUri="http://www.chinanetcore.com",
                    LogoUri="http://img05.tooopen.com/images/20160109/tooopen_sy_153858412946.jpg",
                    ClientSecrets={new Secret("express".Sha256()) },
                    AllowedGrantTypes= GrantTypes.Hybrid,
                    AccessTokenType= AccessTokenType.Jwt,
                    RequireConsent=false,
                    AlwaysIncludeUserClaimsInIdToken=true,
                    RedirectUris={ "http://localhost:20001/signin-oidc" },
                    PostLogoutRedirectUris={"http://localhost:20001/signout-callback-oidc" },
                    AllowedScopes={
                       "openid",
                       "profile",
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                       "expressapi"

                    },
                    BackChannelLogoutUri="http://localhost:20001/logout",
                    BackChannelLogoutSessionRequired=true
                },

                new Client(){

                    ClientId="expressapitest",
                    ClientName="快递接口",
                    //ClientSecrets={new Secret("expressapitest".Sha256()) },
                    AllowedGrantTypes= GrantTypes.Implicit,
                    AccessTokenType= AccessTokenType.Jwt,
                    RequireConsent=false,
                    AllowAccessTokensViaBrowser=true,
                    RedirectUris={ "http://localhost:20002/swagger/oauth2-redirect.html"},
                    AllowedScopes={
                       "openid",
                       "profile",
                         IdentityServerConstants.StandardScopes.OfflineAccess,
                       "expressapi"
                    }
                   
                }
            };
        }
    }
}
