using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

using System.Data;
using Microsoft.EntityFrameworkCore;
using ExpressServices.Config;

namespace ExpressServices.QueryServices
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
                return new MySqlConnection(_config.UserConnectString);
            }
        }
    }

