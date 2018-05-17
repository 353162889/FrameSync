using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NodeEditor
{
    /// <summary>
    ///显示的属性字段的属性
    /// </summary>
    public class NEPropertyAttribute : Attribute
    {
        public string name { get; private set; }
        /// <summary>
        /// 是否显示在节点上
        /// </summary>
        public bool showOnNode { get; private set; }
        public NEPropertyAttribute(string name,bool showOnNode = false)
        {
            this.name = name;
            this.showOnNode = showOnNode;
        }

        public static string GetName(FieldInfo info)
        {
            var arr = info.GetCustomAttributes(typeof(NEPropertyAttribute), false);
            if (arr.Length > 0) return ((NEPropertyAttribute)arr[0]).name;
            return info.Name;
        }
    }
}
