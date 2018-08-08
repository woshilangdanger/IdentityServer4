using System.Collections.Generic;

namespace ExpressUser.Models
{

    /// <summary>
    /// 主要绑定Consent界面上的一些模型
    /// </summary>
    public class ConsentViewModel
    {
        public string ReturnUrl { get; set; }
        public bool RememberConsent { get; set; }
        public string Button { get; set; }
        public IEnumerable<string> ScopesConsented { get; set; }
    }
}
