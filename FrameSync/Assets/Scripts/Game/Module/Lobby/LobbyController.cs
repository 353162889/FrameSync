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
            BattleInfo.userId = 1;
            BattleInfo.levelId = GameConst.Instance.GetInt("default_level_id");
            var levelResInfo = ResCfgSys.Instance.GetCfg<ResLevel>(BattleInfo.levelId);
            BattleInfo.sceneId = levelResInfo.scene_id;
            BattleInfo.standAlone = true;
            GameStarter.GameGlobalState.SwitchState((int)GameStateType.GameIn);
        }

        public void JoinMulti()
        {

        }
    }
}