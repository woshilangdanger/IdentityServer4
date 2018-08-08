using System.Collections.Generic;

namespace ExpressUser.Models
{
    /// <summary>
    /// 主要绑定Idr4中关于Consent界面交互的实体字段
    /// </summary>
    public class Idr4ConsentViewModel : ConsentViewModel
    {
        public string ClientName { get; set; }
        public string ClientUrl { get; set; }

        public string ClientLogoUrl { get; set; }

        public bool AllowRememberConsent { get; set; }

        public IEnumerable<Idr4ScopeViewModel> IdentityScopes { get; set; }
        public IEnumerable<Idr4ScopeViewModel> ResouceScopes { get; set; }
    }
}
