using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    /// <summary>
    /// 自定义扩展IQueryable操作 实现自定义条件查询 liyouming add 20180801
    /// </summary>
    public static class UosoQueryExtensions
    {
        /// <summary>
        /// 扩展query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="conditions"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public static IQueryable<T> QueryConditions<T>(this IQueryable<T> query, IEnumerable<UosoConditions> conditions)
        {
            if (conditions == null)
                return query;
            var parser = new UosoExpressionParser<T>();
            var filter = parser.ParserConditions(conditions);
            return query.Where(filter);
        }
        /// <summary>
        /// 扩展组合And条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static IQueryable<T> QueryAndGroupAnd<T>(this IQueryable<T> query, IEnumerable<UosoConditions> conditions)
        {
            if (conditions == null)
                return query;
            var parser = new UosoExpressionParser<T>();
            var filter = parser.AndGroupAnd( conditions);
            return query.Where(filter);
        }
        /// <summary>
        /// 扩展单个And条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IQueryable<T> QueryAndSingleAnd<T>(this IQueryable<T> query, UosoConditions condition)
        {
            if (condition == null)
                return query;
            var parser = new UosoExpressionParser<T>();
            var filter = parser.AndGroupAnd(new List<UosoConditions> { condition });
            return query.Where(filter);
        }


        public static IQueryable<T> QueryAndGroupOr<T>(this IQueryable<T> query, IEnumerable<UosoConditions> conditions)
        {
            if (conditions == null)
                return query;
            var parser = new UosoExpressionParser<T>();
            var filter = parser.AndGroupOr( conditions);
            return query.Where(filter);
        }
        /// <summary>
        /// 扩展单个And条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IQueryable<T> QueryAndSingleOr<T>(this IQueryable<T> query, UosoConditions condition)
        {
            if (condition == null)
                return query;
            var parser = new UosoExpressionParser<T>();
            var filter = parser.AndGroupOr( new List<UosoConditions> { condition });
            return query.Where(filter);
        }


        public static IQueryable<T> QueryOrGroupAnd<T>(this IQueryable<T> query, IEnumerable<UosoConditions> conditions)
        {
            if (conditions == null)
                return query;
            var parser = new UosoExpressionParser<T>();
            var filter = parser.OrGroupAnd(conditions);
            return query.Where(filter);
        }
        /// <summary>
        /// 扩展单个And条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IQueryable<T> QueryOrSingleAnd<T>(this IQueryable<T> query, UosoConditions condition)
        {
            if (condition == null)
                return query;
            var parser = new UosoExpressionParser<T>();
            var filter = parser.OrGroupAnd(new List <UosoConditions> { condition });
            return query.Where(filter);
        }



        public static IQueryable<T> QueryOrGroupOr<T>(this IQueryable<T> query, IEnumerable<UosoConditions> conditions)
        {
            if (conditions == null)
                return query;
            var parser = new UosoExpressionParser<T>();
            var filter = parser.OrGroupOr( conditions);
            return query.Where(filter);
        }
        /// <summary>
        /// 扩展单个And条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IQueryable<T> QueryOrSingleOr<T>(this IQueryable<T> query, UosoConditions condition)
        {
            if (condition == null)
                return query;
            var parser = new UosoExpressionParser<T>();
            var filter = parser.OrGroupOr( new List<UosoConditions> { condition });
            return query.Where(filter);
        }



        /// <summary>
        /// 分页处理 liyouming add 20180801 这个需要反正排序之后处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public static IQueryable<T> Pager<T>(this IQueryable<T> query, int pageindex, int pagesize, out int itemCount)
        {
            itemCount = query.Count();
            return query.Skip((pageindex - 1) * pagesize).Take(pagesize);
        }
        public static IQueryable<T> OrderConditions<T>(this IQueryable<T> query, IEnumerable<UosoOrderConditions> orderConditions)
        {
            if (orderConditions != null)
            {
                foreach (var orderinfo in orderConditions)
                {
                    var t = typeof(T);
                    var propertyInfo = t.GetProperty(orderinfo.Key);
                    var parameter = Expression.Parameter(t);
                    Expression propertySelector = Expression.Property(parameter, propertyInfo);
                    if (propertyInfo.PropertyType.ToString().ToLower() == "system.int32")
                    {
                        var orderby = Expression.Lambda<Func<T, int>>(propertySelector, parameter);

                        if (orderinfo.Order == OrderSequence.DESC)
                            query = query.OrderByDescending(orderby);
                        else
                            query = query.OrderBy(orderby);
                    }
                    else {
                        var orderby = Expression.Lambda<Func<T, object>>(propertySelector, parameter);

                        if (orderinfo.Order == OrderSequence.DESC)
                            query = query.OrderByDescending(orderby);
                        else
                            query = query.OrderBy(orderby);
                    }
                    

                }
            }
            return query;
        }

    }
}
