using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
namespace Infrastructure.Extensions
{
    /// <summary>
    /// 扩展EFCore动态条件 liyouming add 20180801
    /// </summary>
    public class UosoExpressionParser<T>
    {
        /// <summary>
        /// 定义某个类型的参数（通过反射获取参数表现方式）
        /// </summary>
        ParameterExpression parameter = Expression.Parameter(typeof(T));
        /// <summary>
        /// 转换条件 将条件转化成Expression表达式
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public Expression<Func<T, bool>> ParserConditions(IEnumerable<UosoConditions> conditions)
        {
            //将条件转化成表达是的Body
            var query = ParseExpressionBody(conditions);
            return Expression.Lambda<Func<T, bool>>(query, parameter);
        }
        /// <summary>
        /// And关系条件处理
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Expression<Func<T, bool>> AndGroupAnd(IEnumerable<UosoConditions> conditions)
        {
            var query = ParseExpressionBody(conditions);
            return Expression.Lambda<Func<T, bool>>(query, parameter);
        }

        public Expression<Func<T, bool>> AndGroupOr(IEnumerable<UosoConditions> conditions)
        {
            var query = ParseExpressionBodyOr(conditions);
            return Expression.Lambda<Func<T, bool>>(query, parameter);
        }
        /// <summary>
        /// Or关系条件处理
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Expression<Func<T, bool>> OrGroupAnd( IEnumerable<UosoConditions> conditions)
        {
            var query = ParseExpressionBody(conditions);
            return Expression.Lambda<Func<T, bool>>(query, parameter);
        }
        public Expression<Func<T, bool>> OrGroupOr( IEnumerable<UosoConditions> conditions)
        {
            var query = ParseExpressionBodyOr(conditions);
            return Expression.Lambda<Func<T, bool>>(query, parameter);
        }




        /// <summary>
        /// 递归拼接条件表达式
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public Expression ParseExpressionBody(IEnumerable<UosoConditions> conditions)
        {
            if (conditions == null || conditions.Count() == 0)
            {
                return Expression.Constant(true, typeof(bool));
            }
            else if (conditions.Count() == 1)
            {
                return ParseCondition(conditions.First());
            }
            else
            {
                Expression left = ParseCondition(conditions.First());
                Expression right = ParseExpressionBody(conditions.Skip(1));
                return Expression.AndAlso(left, right);
            }
        }

        public Expression ParseExpressionBodyOr(IEnumerable<UosoConditions> conditions)
        {
            if (conditions == null || conditions.Count() == 0)
            {
                return Expression.Constant(true, typeof(bool));
            }
            else if (conditions.Count() == 1)
            {
                return ParseCondition(conditions.First());
            }
            else
            {
                Expression left = ParseCondition(conditions.First());
                Expression right = ParseExpressionBody(conditions.Skip(1));
                return Expression.Or(left, right);
            }
        }
        /// <summary>
        /// 组装赋值参数转化成表达式
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Expression ParseCondition(UosoConditions condition)
        {
            ParameterExpression p = parameter;
            Expression key = Expression.Property(p, condition.Key);
            Expression value;
            switch (condition.ValueType.ToLower())
            {
                case "int":
                    value = Expression.Constant(int.Parse(condition.Value)); ;
                    break;
                case "float":
                    value = Expression.Constant(float.Parse(condition.Value));
                    break;
                case "double":
                    value = Expression.Constant(double.Parse(condition.Value));
                    break;
                case "decimal":
                    value = Expression.Constant(decimal.Parse(condition.Value));

                    break;
                case "time":
                    value = Expression.Constant(DateTime.Parse(condition.Value));
                    break;
                case "datetime":
                    value = Expression.Constant(DateTime.Parse(condition.Value));
                    break;
                case "string":
                    value = Expression.Constant(condition.Value);
                    break;
                case "enum":
                    value = Expression.Constant(Enum.Parse(typeof(Enum), condition.Value));
                    break;
                case "boolean":
                    value = Expression.Constant(Boolean.Parse(condition.Value));
                    break;
                default:
                    throw new NotSupportedException(string.Format(condition.ValueType, "ValueType类型： {0} 不支持"));
            }



            switch (condition.Operator)
            {
                case UosoOperatorEnum.Contains:
                    return Expression.Call(key, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), value);
                case UosoOperatorEnum.Equal:
                    return Expression.Equal(key, Expression.Convert(value, key.Type));
                case UosoOperatorEnum.Greater:
                    return Expression.GreaterThan(key, Expression.Convert(value, key.Type));
                case UosoOperatorEnum.GreaterEqual:
                    return Expression.GreaterThanOrEqual(key, Expression.Convert(value, key.Type));
                case UosoOperatorEnum.Less:
                    return Expression.LessThan(key, Expression.Convert(value, key.Type));
                case UosoOperatorEnum.LessEqual:
                    return Expression.LessThanOrEqual(key, Expression.Convert(value, key.Type));
                case UosoOperatorEnum.NotEqual:
                    return Expression.NotEqual(key, Expression.Convert(value, key.Type));
                case UosoOperatorEnum.In:
                    return ParaseIn(p, condition);
                case UosoOperatorEnum.Between:
                    return ParaseBetween(p, condition);
                default:
                    throw new NotImplementedException("不支持此操作");
            }
        }

        /// <summary>
        /// 处理特殊的Between 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        private Expression ParaseBetween(ParameterExpression parameter, UosoConditions conditions)
        {
            ParameterExpression p = parameter;
            Expression key = Expression.Property(p, conditions.Key);
            var valueArr = conditions.Value.ToString().Split(',');
            if (valueArr.Length != 2)
            {
                throw new NotImplementedException("ParaseBetween参数错误");
            }
            Expression startvalue = null;
            Expression endvalue = null;
            try
            {
                if (conditions.ValueType.ToLower() == "datetime")
                {
                    startvalue = Expression.Constant(DateTime.Parse(valueArr[0]));
                    endvalue = Expression.Constant(DateTime.Parse(valueArr[1]));
                }
                if (conditions.ValueType.ToLower() == "int")
                {
                    startvalue = Expression.Constant(int.Parse(valueArr[0]));
                    endvalue = Expression.Constant(int.Parse(valueArr[1]));

                }

            }
            catch
            {
                throw new NotImplementedException("ParaseBetween参数不支持此类型");
            }
            Expression expression = Expression.Constant(true, typeof(bool));
            //开始位置

            Expression start = Expression.GreaterThanOrEqual(key, Expression.Convert(startvalue, key.Type));
            Expression end = Expression.GreaterThanOrEqual(key, Expression.Convert(endvalue, key.Type));
            return Expression.AndAlso(start, end);
        }

        /// <summary>
        /// 处理特殊的In条件
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        private Expression ParaseIn(ParameterExpression parameter, UosoConditions conditions)
        {
            ParameterExpression p = parameter;
            Expression key = Expression.Property(p, conditions.Key);
            var valueArr = conditions.Value.ToString().Split(',');
            Expression expression = Expression.Constant(true, typeof(bool));
            foreach (var itemVal in valueArr)
            {
                Expression value = Expression.Constant(itemVal);
                Expression right = Expression.Equal(key, Expression.Convert(value, key.Type));
                Expression.Or(expression, right);
            }
            return expression;
        }






    }
}
