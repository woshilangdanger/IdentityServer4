using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace ExpressUser.Services.DbProvider
{
    public interface IDbProviderFactory
    {
        IDbConnection GetConnection();
       
    }
}
