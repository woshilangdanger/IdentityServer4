using ExpressUser.Config;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ExpressUser.Extensions
{
    public static class ExpressExtenstions
    {
        /// <summary>
        /// 初始化Idr4数据
        /// </summary>
        /// <param name="app"></param>
        public static void UseInitIdr4Data(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {



                //初始化数据库结构
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var configContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configContext.Database.Migrate();

                //初始数据
                if (!configContext.Clients.Any())
                {
                    foreach (var client in MemoryConfigs.GetClients())
                    {
                        configContext.Clients.Add(client.ToEntity());
                    }
                    configContext.SaveChanges();
                }

                if (!configContext.IdentityResources.Any())
                {
                    foreach (var resource in MemoryConfigs.GetIdentityResources())
                    {
                        configContext.IdentityResources.Add(resource.ToEntity());
                    }
                    configContext.SaveChanges();
                }

                if (!configContext.ApiResources.Any())
                {
                    foreach (var resource in MemoryConfigs.GetApiResources())
                    {
                        configContext.ApiResources.Add(resource.ToEntity());
                    }
                    configContext.SaveChanges();
                }
            }
        }
        public static void UseInitIdentityData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {

                var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                applicationDbContext.Database.Migrate();
                var userservices = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                
                //添加角色
                if (!applicationDbContext.Roles.Any())
                {
                    List<IdentityRole> identityRoles = new List<IdentityRole>
                    {
                    new IdentityRole
                    {
                            Name="Administrator",
                            NormalizedName="超级管理员",
                            ConcurrencyStamp=""
                    },
                    new IdentityRole{
                            Name="PJY",
                            NormalizedName="派件员",
                            ConcurrencyStamp=""
                    }
                    };

                    applicationDbContext.Roles.AddRange(identityRoles);
                    applicationDbContext.SaveChanges();

                }
                

                    //添加用户
                    if (!applicationDbContext.Users.Any())
                 {
                    var user = new IdentityUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = "admin",
                        NormalizedUserName = "admin"

                    };
                    string pwd = userservices.PasswordHasher.HashPassword(user, "admin");
                    user.PasswordHash = pwd;
                    applicationDbContext.Users.Add(user);
                    applicationDbContext.SaveChanges();
                }
            }
        }

    }
}
