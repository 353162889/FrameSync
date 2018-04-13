using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class PvpPlayerMgr : Singleton<PvpPlayerMgr>
    {
        private PvpPlayer m_cMainPlayer;
        public PvpPlayer mainPlayer { get { return m_cMainPlayer; } }
        private Dictionary<long, PvpPlayer> m_dic;

        public void Init()
        {
            m_dic = new Dictionary<long, PvpPlayer>();
            m_cMainPlayer = null;
            FrameSyncSys.Instance.OnFrameSyncUpdate += FrameUpdate;
        }

        public void SetMainPlayer(PvpPlayer player)
        {
            m_cMainPlayer = player;
        }

        public PvpPlayer CreatePlayer(long id)
        {
            PvpPlayer player = new PvpPlayer();
            player.Init(id);
            m_dic.Add(player.id, player);
            return player;
        }

        public PvpPlayer GetPlayer(long id)
        {
            PvpPlayer player = null;
            m_dic.TryGetValue(id, out player);
            return player;
        }

        public bool RemovePlayer(long id)
        {
            PvpPlayer player = null;
            if(m_dic.TryGetValue(id, out player))
            {
                player.Clear();
            }
            return m_dic.Remove(id);
        }

        public void FrameUpdate(FP deltaTime)
        {
            foreach (var item in m_dic)
            {
                item.Value.FrameUpdate(deltaTime);
            }
        }

        public void Clear()
        {
            FrameSyncSys.Instance.OnFrameSyncUpdate -= FrameUpdate;
            foreach (var item in m_dic)
            {
                item.Value.Clear();
            }
            m_dic.Clear();
        }

    }
}
