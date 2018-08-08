using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace ExpressServices.QueryServices
{
    public interface IDbProviderFactory
    {
        IDbConnection GetConnection();
       
    }
}
