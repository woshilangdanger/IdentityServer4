using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    public class RoleModel
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string Claims { get; set; }
        public string ExistClaim { get; set; }
    }
}
