using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.DynamicQuery
{
    public class PagerResult
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public object Data { get; set; }
    }
}
