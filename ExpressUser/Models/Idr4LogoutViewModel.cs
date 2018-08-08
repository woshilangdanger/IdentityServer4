using System;

namespace ExpressUser.Models
{
    public class Idr4LogoutViewModel
    {
        public string LogoutId { get; set; }

        public bool ShowLogoutPrompt { get; set; }


        /// <summary>
        /// 下面两个字段为退出直接跳转做处理
        /// </summary>
        public string PostLogoutRedirectUri { get; set; }

        public string SignOutIFrameUrl { get; set; }
        /// <summary>
        /// 得到退出转到地址
        /// </summary>
        public string RedirectUri
        {
            get
            {
                return new Uri(PostLogoutRedirectUri).GetLeftPart(UriPartial.Authority);
            }
        }



    }
}
