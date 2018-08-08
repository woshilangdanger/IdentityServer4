using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Http;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace ExpressServices
{
    /// <summary>
    /// IdentityServer4认证处理
    /// </summary>
    public class IdentityServer4OAuth2OperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {


             
            #region Swagger授权过期器处理
            if (operation.Security == null)
                operation.Security = new List<IDictionary<string, IEnumerable<string>>>();
            var oAuthRequirements = new Dictionary<string, IEnumerable<string>>
                                        {

                                              {"oauth2", new List<string> { "openid", "profile", "expressapi" }}
                                        };
            operation.Security.Add(oAuthRequirements);
            #endregion


            #region Swagger 文件上传处理

            var files = context.ApiDescription.ActionDescriptor.Parameters.Where(n => n.ParameterType == typeof(IFormFile)).ToList();
            if (files.Count > 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    if (i == 0)
                    {
                        operation.Parameters.Clear();
                    }
                    operation.Parameters.Add(new NonBodyParameter
                    {
                        Name = files[i].Name,
                        In = "formData",
                        Description = "Upload File",
                        Required = true,
                        Type = "file"
                    });

                }
                operation.Consumes.Add("multipart/form-data");
            }




            #endregion




        }
    }
}
