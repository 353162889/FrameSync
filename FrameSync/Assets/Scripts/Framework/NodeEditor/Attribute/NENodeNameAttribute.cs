using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NodeEditor
{
    /// <summary>
    /// 节点的名称显示
    /// </summary>
    public class NENodeNameAttribute : Attribute
    {
        public string name { get; private set; }
        public NENodeNameAttribute(string name)
        {
            this.name = name;
        }

        public static string GetName(Type type)
        {
            var arr = type.GetCustomAttributes(typeof(NEPropertyAttribute), false);
            if (arr.Length > 0) return ((NEPropertyAttribute)arr[0]).name;
            return type.Name;
        }
    }
}

