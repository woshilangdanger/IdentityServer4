using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExpressUser.Extensions;
using ExpressUser.Services.UserInteraction;
using Infrastructure.DynamicQuery;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModel;

namespace ExpressUser.Areas.Identity.Controllers
{

    [Area("Identity")]
    public class UserController : Controller
    {
        private readonly IUserServices _userServices;
        private readonly IRoleServices _RoleServices;
        public UserController(IUserServices userServices, IRoleServices RoleServices)
        {
            _userServices = userServices;
            _RoleServices = RoleServices;
        }

        public ActionResult Demo(int pageindex, int pagesize, string username)
        {
            return View();
        }

        // GET: User
        public ActionResult Index(string username,string phone,string email, int pageindex = 1, int pagesize = 2)
        {
            //查询条件
            Expression<Func<IdentityUser, bool>> filter = u => true;

            if (!string.IsNullOrEmpty(username))
            {
                filter = filter.And(c => c.UserName.Contains(username));
            }
            if (!string.IsNullOrEmpty(phone))
            {
                filter = filter.And(c => c.PhoneNumber.Contains(phone));
            }
            if (!string.IsNullOrEmpty(email))
            {
                filter = filter.And(c => c.Email.Contains(email));
            }
            List<UosoOrderConditions> orderConditions = new List<UosoOrderConditions> {
                 new UosoOrderConditions{
                      Key="Id",
                       Order = OrderSequence.DESC
                 }
            };
            int itemcount = 0;
            var list = _userServices.GetPagedList3(filter, orderConditions, pageindex, pagesize, out itemcount);
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
            ViewBag.Search = new List<string> { username , phone , email };
            return View(list);
        }

        // GET: User/Details/5


        // GET: User/Create
        public ActionResult Add()
        {
            var RoleList = _RoleServices.Roles().ToList();
            return View(RoleList);
        }

        // POST: User/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(UseModel model)
        {
            OperationResult result = new OperationResult();
            var res = await _userServices.FindByName(model.UserName);
            if (res != null)
            {
                result.Message = "用户已经存在";
                return Json(result);
            }
            bool resbool = await _userServices.Create(model);
            if (!resbool)
            {
                result.Message = "添加失败";
                return Json(result);
            }
            result.Code = ResultCode.Success;
            result.Data = "添加成功";
            return Json(result);
        }

        // GET: User/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            //获取用户信息
            var Model = await _userServices.FindByUserId(id);
            //设置默认密码,检测密码是否改变
            ViewBag.Pwd = "q!@*90";
            //所有角色
            var RoleList = _RoleServices.Roles().ToList();
            //该用户所有角色
            var roleres = await _userServices.UserRoles(Model);
            List<string> UserRoles = roleres.ToList();

            //复选框结果集
            List<CheckBoxResult> checkbox = new List<CheckBoxResult>();
            CheckBoxResult check = null;
            foreach (var roles in RoleList)
            {
                check = new CheckBoxResult();
                check.Name = roles.Name;
                foreach (var item in UserRoles)
                {
                    if (item == roles.Name)
                    {
                        check.Checked = true;
                    }
                }
                checkbox.Add(check);
            }
            //复选框结果集
            ViewBag.CheckBox = checkbox;
            //用户已经存在的所有角色
            ViewBag.ExistRole = string.Join(",", UserRoles.ToArray());
            return View(Model);
        }

        // POST: User/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(UseModel user)
        {
            OperationResult result = new OperationResult();
            var res = await _userServices.FindByUserId(user.ID);
            if (res == null)
            {
                result.Message = "用户不存在";
                return Json(result);
            }
            res.UserName = user.UserName.Trim();
            res.PhoneNumber = user.Phone.Trim();
            res.Email = user.Email.Trim();
            //判断用户是否修改过密码
            if (!user.PassWord.Trim().Equals("q!@*90"))
            {
                res.PasswordHash = await _userServices.HashPwd(res, user.PassWord.Trim());
            }
            bool resbool = await _userServices.Edit(res, user.Roles, user.ExistRole);
            if (!resbool)
            {
                result.Message = "修改失败";
                return Json(result);
            }
            result.Code = ResultCode.Success;
            result.Data = "修改成功";
            return Json(result);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(string idstr)
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
                var res = await _userServices.FindByUserId(item);
                if (res != null)
                {
                    await _userServices.Delete(res);
                }
            }
            result.Code = ResultCode.Success;
            result.Data = "删除成功";
            return Json(result);
        }
    }
}