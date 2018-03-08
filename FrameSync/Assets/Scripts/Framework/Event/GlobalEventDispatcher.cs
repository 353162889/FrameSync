using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class GlobalEventDispatcher : EventDispatcher
    {
        private static GlobalEventDispatcher m_cInstance;
        public static GlobalEventDispatcher Instance {
            get {
                if(m_cInstance == null)
                {
                    m_cInstance = new GlobalEventDispatcher();
                }
                return m_cInstance;
            }
        }
    }
}
