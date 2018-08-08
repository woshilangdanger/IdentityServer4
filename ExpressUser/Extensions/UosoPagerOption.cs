using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressUser.Extensions
{
    public class UosoPagerOption
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public int CountNum { get; set; }
        public int ItemCount { get; set; }
        public int TotalPage
        {
            get
            {
                return ItemCount % PageSize > 0 ? (ItemCount / PageSize + 1) : ItemCount / PageSize;
            }
        }
        public string Url { get; set; }

        public IQueryCollection Query { get; set; }
    }
}
