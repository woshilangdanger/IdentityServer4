using System.ComponentModel.DataAnnotations;

namespace ExpressUser.Models
{
    /// <summary>
    /// liyouming add 2018-04-09  
    /// </summary>
    public class LoginViewModel
    {
         /// <summary>
         /// 用户名
         /// </summary>
        //[Required]
        public string username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        //[Required]
        public string password { get; set; }
        /// <summary>
        /// 界面上的选择框  选择是否记住登录
        /// </summary>
        public bool RememberLogin { get; set; }
        /// <summary>
        /// 回调授权验证地址 这个地址与Redirect地址不一样
        /// 登录成功后会转到 ReturnUrl  然后验证授权登录后 获取到客户端的信息 然后根据Client配置中的RedirectUrl转到对应的系统
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
