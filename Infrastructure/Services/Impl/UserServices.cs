using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.DynamicQuery;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.ViewModel;

namespace Infrastructure.Services.Impl
{
    public class UserServices : IUserServices
    {

        private readonly UserManager<IdentityUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;
        public UserServices(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }


        public IList<IdentityUser> GetPagedList3(Expression<Func<IdentityUser, bool>> whereLambda, IEnumerable<UosoOrderConditions> orderConditions, int pageIndex, int pageSize, out int itemcount)
        {
            return  _userManager.Users.AsNoTracking()
                .Where(whereLambda)
                .OrderConditions(orderConditions)
                .Pager(pageIndex, pageSize, out itemcount)
                .ToList();
        }
        public IList<IdentityUser> GetPagedList2(List<UosoConditions> whereLambda,  IEnumerable<UosoOrderConditions> orderConditions, int pageIndex, int pageSize, out int itemcount)
        {
                return _userManager.Users.AsNoTracking()
                .QueryAndSingleOr(whereLambda.Find(c => c.Key == "UserName"))
                .QueryAndSingleOr(whereLambda.Find(c => c.Key == "Email"))
                .QueryAndSingleOr(whereLambda.Find(c => c.Key == "PhoneNumber"))
                .OrderConditions(orderConditions)
                .Pager(pageIndex, pageSize, out itemcount)
                .ToList();
          
            
        }
        public IQueryable<IdentityUser> GetPagedList<TKey>(int pageIndex, int pageSize, Expression<Func<IdentityUser, bool>> whereLambda, Expression<Func<IdentityUser, TKey>> orderByLambda,out int itemcount, bool isAsc = true)
        {
            var query = _userManager.Users.AsNoTracking().Where(whereLambda).OrderBy(orderByLambda);
            itemcount = query.Count();
            if (isAsc) query = query.OrderBy(orderByLambda);
            else query = query.OrderByDescending(orderByLambda);

            return query.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
        }
        public async Task<bool> Create(UseModel model)
        {
            try
            {
                var user = new IdentityUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = model.UserName,
                    NormalizedUserName = model.UserName,
                    Email=model.Email,
                    PhoneNumber=model.Phone
                };
                //传入Password并转换成PasswordHash
                IdentityResult result = await _userManager.CreateAsync(user, model.PassWord);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrWhiteSpace(model.Roles))
                    {
                        model.Roles = model.Roles.TrimStart(',');
                        var NameList = new List<string>(model.Roles.Split(','));
                        if (NameList.Count == 0)
                        {
                            return true;
                        }
                        var resultRole = await _userManager.AddToRolesAsync(user, NameList);
                        if (resultRole.Succeeded)
                        {
                            return true;
                        }
                        else { return false; }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> Delete(IdentityUser model)
        {
            try
            {
                IdentityResult result = await _userManager.DeleteAsync(model);
                if (result.Succeeded)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> Edit(IdentityUser model,string Roles,string ExistRole)
        {
            try
            {
                IdentityResult result = await _userManager.UpdateAsync(model);

                if (result.Succeeded)
                {
                    //新角色集合
                    List<string> NameList = null;
                    if (!string.IsNullOrWhiteSpace(Roles))
                    {
                        Roles = Roles.TrimStart(',');
                        NameList = new List<string>(Roles.Split(','));
                    }
                    //已经拥有的角色集合
                    List<string> ExistRoles = null;
                    if (!string.IsNullOrWhiteSpace(ExistRole))
                    {
                        ExistRoles = new List<string>(ExistRole.Split(','));
                    }
                    //判断该用户是否包含角色,有则移除
                    if (ExistRoles!= null)
                    {
                        await _userManager.RemoveFromRolesAsync(model, ExistRoles);
                    }
                    if (NameList != null)
                    {
                        var resultRole = await _userManager.AddToRolesAsync(model, NameList);
                        if (resultRole.Succeeded)
                        {
                            return true;
                        }
                        else { return false; }
                    }
                    else { return true; }
                    
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IdentityUser> FindByName(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<IdentityUser> FindByUserId(string userid)
        {
            return await _userManager.FindByIdAsync(userid);
        }
        public async Task<IEnumerable<string>> UserRoles(IdentityUser user)
        {
            var list = await _userManager.GetRolesAsync(user);
            return list.ToList();
        }

        public async Task<string> HashPwd(IdentityUser user,string pwd)
        {
            //删除用户密码
            await _userManager.RemovePasswordAsync(user);
            return _userManager.PasswordHasher.HashPassword(user,pwd);
        }
    }
}
