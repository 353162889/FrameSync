using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class PvpPlayer
    {
        private long m_lId;
        public long id { get { return m_lId; } }
        public void Init(long id,PvpPlayerData playerData = null)
        {
            m_lId = id;
        }

        public void FrameUpdate(FP deltaTime)
        {

        }

        public void Clear()
        {

        }
    }
}
