namespace ExpressUser.Models
{
    /// <summary>
    /// 承载一些Idr4中Scope模型交互属性
    /// </summary>
    public class Idr4ScopeViewModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Emphasize { get; set; }
        public bool Required { get; set; }
        public bool Checked { get; set; }
    }
}
