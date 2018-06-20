using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    public class BTTimeDecoratorData
    {
        public FP time;
    }
    [NENode(typeof(BTTimeDecoratorData))]
    public class BTTimeDecorator : BTDecorator, IBTTimeLineNode
    {
        public FP time
        {
            get
            {
                return ((BTTimeDecoratorData)m_cData).time;
            }
        }
    }
}
