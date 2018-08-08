using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public class UosoOrderConditions
    {
        public string Key { get; set; }
        public OrderSequence Order { get; set; }

      
    }
    public enum OrderSequence
    {
        ASC,
        DESC
    }
}
