using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressServices.Config
{
    public class UserDbConfig
    {
        public string UserConnectString { get; set; }
        public int DbType { get; set; }
    }
}
