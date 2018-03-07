using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class UpdateScheduler : Scheduler
    {
        private static UpdateScheduler m_cInstance;
        public static UpdateScheduler Instance {
            get {
                return m_cInstance;
            }
        }

        protected override void Awake()
        {
            m_cInstance = this;
            base.Awake();
        }
    }
}
