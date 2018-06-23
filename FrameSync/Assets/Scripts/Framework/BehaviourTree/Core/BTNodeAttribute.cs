using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    public class BTNodeAttribute : NENodeAttribute
    {
        public BTNodeAttribute(Type nodeDataType) : base(nodeDataType)
        {
        }
    }
}
