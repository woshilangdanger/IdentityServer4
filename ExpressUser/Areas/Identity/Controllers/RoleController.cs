using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExpressUser.Services.Authorization;
using ExpressUser.Config;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Services;
using Infrastructure.DynamicQuery;
using Models.ViewModel;
using Infrastructure.Extensions;
using ExpressUser.Extensions;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;

namespace ExpressUser.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class RoleController : Controller
    {
        private readonly IRoleServices _RoleServices;
        public RoleController(IRoleServices RoleServices)
        {
            _RoleServices = RoleServices;
        }
        // GET: Role
        public ActionResult Index(string username, int pageindex = 1, int pagesize = 2)
        {
            //查询条件
            Expression<Func<IdentityRole, bool>> filter = u => true;
            if (!string.IsNullOrEmpty(username))
            {
                filter = filter.And(c => c.Name.Contains(username));
            }
            List<UosoOrderConditions> orderConditions = new List<UosoOrderConditions> {
                 new UosoOrderConditions{
                      Key="Id",
                       Order = OrderSequence.DESC
                 }
            };
            int itemcount = 0;
            var list = _RoleServices.GetPagedList(filter, orderConditions, pageindex, pagesize, out itemcount);
            ViewBag.Option = new UosoPagerOption()
            {
                ItemCount = itemcount,
                PageSize = pagesize,
                PageIndex = pageindex,
                CountNum = 5,
                Url = Request.Path.Value,
                Query = Request.Query
            };
            //选择框的值
            ViewBag.Search = username;
            return View(list);
        }


       // [AuthorizeCode(PermissionsConfig.RoleAdd)]
        // GET: Role/Create
        public ActionResult Add()
        {
            return View();
        }

        // POST: Role/Create
        //IFormCollection collection
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string RoleName,string Claims)
        {
            OperationResult result = new OperationResult();
            if (string.IsNullOrWhiteSpace(RoleName))
            {
                result.Message = "请输入添加内容";
                return Json(result);
            }
            var res = await _RoleServices.FindByName(RoleName);
            if (res != null)
            {
                result.Message = "角色已经存在";
                return Json(result);
            }
            bool resbool = await _RoleServices.Create(RoleName,Claims);
            if (!resbool)
            {
                result.Message = "添加失败";
                return Json(result);
            }
            
            result.Code = ResultCode.Success;
            result.Data = "添加成功";
            return Json(result);
        }

        // GET: Role/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var Model = await _RoleServices.FindById(id);
            //所有角色
            List<DictionaryResule> RoleList = new List<DictionaryResule> { 
                new DictionaryResule(){
                    Name="用户列表",
                    Value="Role.List"
                },
                new DictionaryResule(){
                    Name="用户新增",
                    Value="Role.Add"
                },
                new DictionaryResule(){
                    Name="用户查看",
                    Value="Role.Read"
                },
                new DictionaryResule(){
                    Name="用户修改",
                    Value="Role.Update"
                },
                new DictionaryResule(){
                    Name="用户删除",
                    Value="Role.Delete"
                }
            };
            //该角色所有的操作
            var ress = await _RoleServices.RoleClaims(Model);

            List<string> RoleClaims =new List<string>();
            foreach (var item in ress)
            {
                RoleClaims.Add(item.Value);
            }


            //复选框结果集
            List<CheckBoxResult> checkbox = new List<CheckBoxResult>();
            CheckBoxResult check = null;
            foreach (var roles in RoleList)
            {
                check = new CheckBoxResult();
                check.Name = roles.Name;
                check.Value = roles.Value;
                foreach (var item in RoleClaims)
                {
                    if (item == roles.Value)
                    {
                        check.Checked = true;
                    }
                }
                checkbox.Add(check);
            }
            //复选框结果集
            ViewBag.CheckBox = checkbox;
            //用户已经存在的所有角色
            ViewBag.ExistRole = string.Join(",", RoleClaims.ToArray());
            
            ViewBag.Claim = ress;
            return View(Model);
        }

        // POST: Role/Edit/5
        [HttpPost]
      //  [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RoleModel role)
        {
            OperationResult result = new OperationResult();
            var res = await _RoleServices.FindById(role.id);
            if (res == null)
            {
                result.Message = "未找到角色信息";
                return Json(result);
            }
            res.Name = role.Name;
            bool resbool = await _RoleServices.Edit(res, role.Claims,role.ExistClaim);
            if (!resbool)
            {
                result.Message = "修改失败";
                return Json(result);
            }
            result.Code = ResultCode.Success;
            result.Data = "修改成功";
            return Json(result);
            //try
            //{
            //    // TODO: Add update logic here
            //    return RedirectToAction(nameof(Index));
            //}
            //catch
            //{
            //return View();
            //}
        }

        // GET: Role/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Role/Delete/5
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string idstr)
        {
            OperationResult result = new OperationResult();
            if (idstr == null)
            {
                result.Message = "请输入删除的内容";
                return Json(result);
            }
            //将字符串转换为集合
            List<string> list = new List<string>(idstr.Split(','));
            if (list.Count <= 0)
            {
                result.Message = "请输入删除的内容";
                return Json(result);
            }
            foreach (var item in list)
            {
                var res = await _RoleServices.FindById(item);
                if (res != null)
                {
                    await _RoleServices.Delete(res);
                }
            }
            result.Code = ResultCode.Success;
            result.Data = "删除成功";
            return Json(result);
        }
    }
}