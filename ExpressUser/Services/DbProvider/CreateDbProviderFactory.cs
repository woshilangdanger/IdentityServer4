using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using ExpressUser.Config;
using Microsoft.EntityFrameworkCore;

namespace ExpressUser.Services.DbProvider
{
    public class CreateDbProviderFactory : IDbProviderFactory
    {
        private readonly UserDbConfig _config;
        public CreateDbProviderFactory(IOptions<UserDbConfig> options)
        {
      
            _config = options.Value;
        }
     
        public IDbConnection GetConnection()
        {
           return   new SqlConnection(_config.UserConnectString);
     
        }
    }
}
