using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public enum UosoOperatorEnum
    {
        Contains=0,
        Equal = 1,
        NotEqual = 2,
        Greater = 3,
        GreaterEqual = 4,
        Less = 5,
        LessEqual = 6,
        Between = 7,
        In = 8
    }
}
