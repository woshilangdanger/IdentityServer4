using ExpressUser.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExpressUser.Services.UserInteraction
{
    /// <summary>
    /// 这里
    /// </summary>
    public interface IUserStoreService
    {
        /// <summary>
        /// 获取用户Claims信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IList<Claim>> GetUserClaimsByUser(IdentityUser user);
        Task<IList<Claim>> GetRoleClaimsByRole(IdentityRole role);


      Task<IList<Claim>> GetAllClaimsByUser(IdentityUser user);

        /// <summary>
        /// 验证用户密码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> ValidatorUser(IdentityUser user, string password);
        Task<IdentityUser> FindByName(string username);
        Task<IdentityUser> FindByUserId(string userid);
        Task<IdentityUser> FindByEmail(string email);
        IQueryable<IdentityUser> Users();

    }
}
