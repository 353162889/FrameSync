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
        public List<PvpPlayer> lstPlayer { get { return m_lstPlayer; } }
        private List<PvpPlayer> m_lstPlayer;
        private bool m_bInit;

        public void Init()
        {
            m_bInit = true;
            m_dic = new Dictionary<long, PvpPlayer>();
            m_lstPlayer = new List<PvpPlayer>();
            m_cMainPlayer = null;
            FrameSyncSys.Instance.OnFrameSyncUpdate += FrameUpdate;
        }

        public void SetMainPlayer(PvpPlayer player)
        {
            m_cMainPlayer = player;
        }

        public PvpPlayer CreatePlayer(long id,PvpPlayerData playerData = null)
        {
            PvpPlayer player = new PvpPlayer();
            player.Init(id, playerData);
            m_dic.Add(player.id, player);
            m_lstPlayer.Add(player);
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
            for (int i = m_lstPlayer.Count - 1; i > -1; i--)
            {
                if(m_lstPlayer[i].id == id)
                {
                    m_lstPlayer.RemoveAt(i);
                    break;
                }
            }
            if (m_dic.TryGetValue(id, out player))
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
            if (!m_bInit) return;
            FrameSyncSys.Instance.OnFrameSyncUpdate -= FrameUpdate;
            foreach (var item in m_dic)
            {
                item.Value.Clear();
            }
            m_lstPlayer.Clear();
            m_dic.Clear();
        }

    }
}
