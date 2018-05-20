using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class RemoteNodeAttribute : NENodeAttribute
    {
        public RemoteNodeAttribute(Type nodeDataType) : base(nodeDataType)
        {
        }
    }
}
