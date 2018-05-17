using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeEditor
{
    /// <summary>
    /// 节点的描述
    /// </summary>
    public class NENodeDescAttribute : Attribute
    {
        public string desc { get; private set; }
        public NENodeDescAttribute(string desc)
        {
            this.desc = desc;
        }

        public static string GetDesc(Type type)
        {
            var arr = type.GetCustomAttributes(typeof(NENodeDescAttribute), false);
            if (arr.Length > 0) return ((NENodeDescAttribute)arr[0]).desc;
            return "";
        }
    }
}
