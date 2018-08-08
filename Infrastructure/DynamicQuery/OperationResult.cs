using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.DynamicQuery
{
    /// <summary>
    /// 操作结果
    /// </summary>
    public class OperationResult
    {
        public OperationResult()
        {
            //默认操作失败
            Code = ResultCode.Fault;
        }
        /// <summary>
        /// 编码  0:失败 , 1:成功
        /// </summary>
        public ResultCode Code { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }

    }
    /// <summary>
    /// 结果枚举
    /// </summary>
    public enum ResultCode
    {
        /// <summary>
        /// 失败
        /// </summary>
        Fault,
        /// <summary>
        /// 成功
        /// </summary>
        Success

    }
}
