using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;

namespace NodeEditor
{

    public class NETreeComposeType
    {
        public Type rootType { get; private set; }
        public List<Type> lstNodeAttribute { get; private set; }
        public string filePre { get; private set; }//前缀
        public string fileExt { get; private set; }//后缀

        public NETreeComposeType(Type rootType, List<Type> lstNodeAttr, string filePre, string fileExt)
        {
            this.rootType = rootType;
            this.lstNodeAttribute = lstNodeAttr;
            this.filePre = filePre;
            this.fileExt = fileExt;
        }
    }

    public class NEConfig
    {
        public static NETreeComposeType[] arrTreeComposeData = new NETreeComposeType[] {
        //一般数据
        new NETreeComposeType(typeof(BTRoot),new List<Type> { typeof(NENodeAttribute) },"","bytes"),
        };

        public static string[] arrTreeComposeTypeDesc = new string[] {
        "节点",
    };
    }
}
