using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ExpressUser.Models;
using Microsoft.AspNetCore.Identity;


namespace ExpressUser.Services.UserInteraction
{
    public class UserStoreService : IUserStoreService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserStoreService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {

            _userManager = userManager;
            _roleManager = roleManager;
        }


        /// <summary>
        /// 根据用户获取Claim信息 liyouming
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IList<Claim>> GetUserClaimsByUser(IdentityUser user)
        {
            return await _userManager.GetClaimsAsync(user);

        }
        public async Task<IList<Claim>> GetRoleClaimsByRole(IdentityRole role)
        {
            return await _roleManager.GetClaimsAsync(role);

        }
        public async Task<IList<Claim>> GetAllClaimsByUser(IdentityUser user)
        {
            var allclaims = new List<Claim>();
            var userclaims = await _userManager.GetClaimsAsync(user);
            allclaims.AddRange(userclaims);

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var roleval in roles)
            {
                var roleinfo = await _roleManager.FindByNameAsync(roleval);
                var roleclaims = await _roleManager.GetClaimsAsync(roleinfo);
                allclaims.AddRange(roleclaims);
            }
            return allclaims;
        }

        /// <summary>
        /// 验证用户密码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> ValidatorUser(IdentityUser user, string password)
        {

            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IdentityUser> FindByName(string username)
        {

            return await _userManager.FindByNameAsync(username);
        }

        public async Task<IdentityUser> FindByUserId(string userid)
        {
            return await _userManager.FindByIdAsync(userid);
        }

        public async Task<IdentityUser> FindByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public IQueryable<IdentityUser> Users()
        {
            var reslist = _userManager.Users;
            return reslist;
        }
    }
}
