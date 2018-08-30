using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using UnityEngine;
using GameData;

namespace Game
{
    public class GameAudioSys : Singleton<GameAudioSys>
    {
        public int Play(int id)
        {
            return Play(id, Vector3.zero);
        }

        public int Play(int id,Vector3 pos)
        {
            var resInfo = ResCfgSys.Instance.GetCfg<ResAudio>(id);
            if (resInfo == null) return -1;
            AudioChannelType channelType = AudioChannelSource.GetChannelTypeByString(resInfo.type);
            return AudioSys.Instance.Play(resInfo.path, channelType, resInfo.loop, resInfo.priority,pos);
        }

        public int Play(int id,Transform trans)
        {
            var resInfo = ResCfgSys.Instance.GetCfg<ResAudio>(id);
            if (resInfo == null) return -1;
            AudioChannelType channelType = AudioChannelSource.GetChannelTypeByString(resInfo.type);
            return AudioSys.Instance.Play(resInfo.path, channelType, resInfo.loop, resInfo.priority, trans);
        }

        public void SetVolume(float volume)
        {
            AudioSys.Instance.SetVolume(volume);
        }

        public void SetVolume(AudioChannelType channelType, float volume)
        {
            AudioSys.Instance.SetVolume(channelType, volume);
        }

        /// <summary>
        /// 停止音频编号=id的所有音效，如果key>-1，那么停止当前key的音效
        /// </summary>
        public void Stop(int id,int key)
        {
            var resInfo = ResCfgSys.Instance.GetCfg<ResAudio>(id);
            if (resInfo == null) return;
            AudioSys.Instance.Stop(resInfo.path,key);
        }

        /// <summary>
        /// 停止某类音效
        /// </summary>
        /// <param name="channelType"></param>
        public void Stop(AudioChannelType channelType)
        {
            AudioSys.Instance.Stop(channelType);
        }

        public void StopAll()
        {
            AudioSys.Instance.StopAll();
        }
    }
}
