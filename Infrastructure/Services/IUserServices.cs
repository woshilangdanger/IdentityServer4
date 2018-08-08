using Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IUserServices
    {

        IList<IdentityUser> GetPagedList3(Expression<Func<IdentityUser, bool>> whereLambda, IEnumerable<UosoOrderConditions> orderConditions, int pageIndex, int pageSize, out int itemcount);
        IList<IdentityUser> GetPagedList2(List<UosoConditions> conditionname,  IEnumerable<UosoOrderConditions> orderConditions, int pageIndex, int pageSize, out int itemcount);
        /// <summary>  
        /// 带分页的查询   
        /// </summary>  
        /// <typeparam name="TKey"></typeparam>  
        /// <param name="pageIndex">页码</param>  
        /// <param name="pageSize">页容量</param>  
        /// <param name="whereLambda">条件 lambda表达式</param>  
        /// <param name="orderBy">排序 lambda表达式</param>  
        /// <returns></returns>  
        IQueryable<IdentityUser> GetPagedList<TKey>(int pageIndex, int pageSize, Expression<Func<IdentityUser, bool>> whereLambda, Expression<Func<IdentityUser, TKey>> orderByLambda, out int itemcount, bool isAsc = true);
        /// <summary>
        /// 添加用户类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> Create(UseModel model);
        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> Edit(IdentityUser model, string Roles, string ExistRole);
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> Delete(IdentityUser model);
        /// <summary>
        /// 根据UserId查看用户是否存在
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<IdentityUser> FindByUserId(string userid);
        /// <summary>
        /// 根据UserName查看用户是否存在
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<IdentityUser> FindByName(string username);
        /// <summary>
        /// 根据用户获取所有角色
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> UserRoles(IdentityUser user);

        Task<string> HashPwd(IdentityUser user, string pwd);
    }
}
