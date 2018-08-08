using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;


namespace ExpressUser.Extensions
{

    public class UosoPagerTagHelper : TagHelper
    {
        public UosoPagerOption UosoPagerOption { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            if (UosoPagerOption.CountNum < 1)
            {
                UosoPagerOption.CountNum = 5;
            }
            if (UosoPagerOption.PageIndex < 1)
            {
                UosoPagerOption.PageIndex = 1;
            }
            if (UosoPagerOption.PageIndex > UosoPagerOption.TotalPage)
            {
                UosoPagerOption.PageIndex = UosoPagerOption.TotalPage;
            }
            if (UosoPagerOption.TotalPage <= 1)
            {
                return;
            }
            var queryarr = UosoPagerOption.Query.Where(c => c.Key != "pageindex" && c.Key != "pagesize").ToList();
            string queryurl = string.Empty;
            foreach (var item in queryarr)
            {
                queryurl += "&" + item.Key + "=" + item.Value;
            }

            output.Content.AppendFormat("<a class=\"prev\" href=\"{0}?pageindex={1}&pagesize={2}{3}\">首页</a>", UosoPagerOption.Url, 1, UosoPagerOption.PageSize, queryurl);
            if (UosoPagerOption.PageIndex == 1)
            {
                output.Content.AppendFormat("<a class=\"prev\" href=\"{0}?pageindex={1}&pagesize={2}{3}\">上一页</a>", UosoPagerOption.Url, 1, UosoPagerOption.PageSize, queryurl);
            }
            else {
                output.Content.AppendFormat("<a class=\"prev\" href=\"{0}?pageindex={1}&pagesize={2}{3}\">上一页</a>", UosoPagerOption.Url, UosoPagerOption.PageIndex - 1, UosoPagerOption.PageSize, queryurl);

            }
               
           
            #region 分页逻辑
            if (UosoPagerOption.PageIndex == 1)
            {
                for (int i = UosoPagerOption.PageIndex; i <= UosoPagerOption.PageIndex + UosoPagerOption.CountNum - 1; i++)
                {
                    if (i <= UosoPagerOption.TotalPage)
                    {
                        if (UosoPagerOption.PageIndex == i)
                        {
                            output.Content.AppendFormat("<span class=\"current\">{0}</span>", i);
                        }
                        else
                        {
                            output.Content.AppendFormat("<a class=\"num\" href=\"{0}?pageindex={1}&pagesize={2}{3}\">{1}</a>", UosoPagerOption.Url, i, UosoPagerOption.PageSize, queryurl);

                        }
                    }
                }
            }

            else if (UosoPagerOption.PageIndex % UosoPagerOption.CountNum == 0)
            {
               
                for (int i = UosoPagerOption.PageIndex - (UosoPagerOption.CountNum / 2); i <= UosoPagerOption.PageIndex + UosoPagerOption.CountNum / 2; i++)
                {
                    if (i <= UosoPagerOption.TotalPage)
                    {
                        if (UosoPagerOption.PageIndex == i)
                        {
                            output.Content.AppendFormat("<span class=\"current\">{0}</span>", i);
                        }
                        else
                        {
                            output.Content.AppendFormat("<a class=\"num\" href=\"{0}?pageindex={1}&pagesize={2}{3}\">{1}</a>", UosoPagerOption.Url, i, UosoPagerOption.PageSize, queryurl);

                        }
                    }
                }
            }
            else
            {
              
                int startindex = UosoPagerOption.CountNum * (UosoPagerOption.PageIndex / UosoPagerOption.CountNum) + 1;
                for (int i = startindex; i <= startindex + UosoPagerOption.CountNum - 1; i++)
                {
                    if (i <= UosoPagerOption.TotalPage)
                    {
                        if (UosoPagerOption.PageIndex == i)
                        {
                            output.Content.AppendFormat("<span class=\"current\">{0}</span>", i);
                        }
                        else
                        {
                            output.Content.AppendFormat("<a class=\"num\" href=\"{0}?pageindex={1}&pagesize={2}{3}\">{1}</a>", UosoPagerOption.Url, i, UosoPagerOption.PageSize, queryurl);

                        }
                    }
                }

               

            }

            #endregion
            if (UosoPagerOption.PageIndex == UosoPagerOption.TotalPage)
            {
                output.Content.AppendFormat("<a class=\"next\" href=\"{0}?pageindex={1}&pagesize={2}{3}\">下一页</a>", UosoPagerOption.Url, UosoPagerOption.TotalPage, UosoPagerOption.PageSize, queryurl);
            }
            else
            {
                output.Content.AppendFormat("<a class=\"next\" href=\"{0}?pageindex={1}&pagesize={2}{3}\">下一页</a>", UosoPagerOption.Url, UosoPagerOption.PageIndex + 1, UosoPagerOption.PageSize, queryurl);
            }
            output.Content.AppendFormat("<a class=\"next\" href=\"{0}?pageindex={1}&pagesize={2}{3}\">尾页</a>", UosoPagerOption.Url, UosoPagerOption.TotalPage, UosoPagerOption.PageSize, queryurl);

            base.Process(context, output);
        }
    }
}
