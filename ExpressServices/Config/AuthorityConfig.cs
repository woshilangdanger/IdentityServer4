using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressServices.Config
{
    public class AuthorityConfig
    {
        public string Authority { get; set; }
        public bool RequireHttpsMetadata { get; set; }
    }
}
