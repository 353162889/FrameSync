using UnityEngine;
using System.Collections;
using Framework;
using GameData;

namespace Game
{
    public class LobbyController : Singleton<LobbyController>
    {
        public void JoinSingle()
        {
            BattleInfo.Clear();
            BattleInfo.levelId = GameConst.Instance.GetInt("default_level_id");
            var levelResInfo = ResCfgSys.Instance.GetCfg<ResLevel>(BattleInfo.levelId);
            BattleInfo.sceneId = levelResInfo.scene_id;
            BattleInfo.standAlone = true;
            BattleInfo.ip = "127.0.0.1";
            BattleInfo.port = 8080;
            BattleInfo.matchCount = 1;
            GameStarter.GameGlobalState.SwitchState((int)GameStateType.GameIn);
        }

        public void JoinMulti()
        {
            BattleInfo.Clear();
            BattleInfo.levelId = GameConst.Instance.GetInt("default_level_id");
            var levelResInfo = ResCfgSys.Instance.GetCfg<ResLevel>(BattleInfo.levelId);
            BattleInfo.sceneId = levelResInfo.scene_id;
            BattleInfo.standAlone = false;
            BattleInfo.ip = "192.168.0.103";
            BattleInfo.port = 8080;
            BattleInfo.matchCount = 2;
            GameStarter.GameGlobalState.SwitchState((int)GameStateType.GameIn);
        }
    }
}