using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    /// <summary>
    /// 复选框结果
    /// </summary>
    public class CheckBoxResult
    {
        public CheckBoxResult()
        {
            Checked = false;
        }
        /// <summary>
        /// ID
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Checked { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
