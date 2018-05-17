using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeEditor
{
    /// <summary>
    /// 节点种类
    /// </summary>
    public class NENodeCategoryAttribute : Attribute
    {
        public string category { get; private set; }
        public NENodeCategoryAttribute(string category)
        {
            this.category = category;
        }
    }
}
