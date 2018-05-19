using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeEditor
{
    public class NETreeComposeType
    {
        public Type rootType { get; private set; }
        public List<Type> lstNodeAttribute { get; private set; }
        public string filePre { get; private set; }//前缀
        public string fileExt { get; private set; }//后缀
        public string desc { get; private set; }

        public NETreeComposeType(Type rootType, List<Type> lstNodeAttr, string filePre, string fileExt,string desc)
        {
            this.rootType = rootType;
            this.lstNodeAttribute = lstNodeAttr;
            this.filePre = filePre;
            this.fileExt = fileExt;
            this.desc = desc;
        }
    }

}
