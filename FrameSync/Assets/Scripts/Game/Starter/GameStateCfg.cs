using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public enum GameStateType
    {
        Root,
        GameOut,
        GameIn,
        GameDebugStandAloneInit,//测试单机下参数初始化
    }

    public class GameStateData
    {
        public GameStateType mStateType;
        public Type mClassType;
        public bool mDefaultState;
        public GameStateData[] mSubStateData;
        public GameStateData(GameStateType stateType,Type classType,bool defaultState ,GameStateData[] subStateData = null)
        {
            this.mStateType = stateType;
            this.mClassType = classType;
            this.mDefaultState = defaultState;
            this.mSubStateData = subStateData;
        }
    }

    public partial class GameStateCfg
    {
        public static GameStateData GetConfig(GameVersionMode versionMode,GameNetMode netMode)
        {
            if(versionMode == GameVersionMode.Release)
            {
                if(netMode == GameNetMode.Network)
                {
                    return ReleaseNetwork;
                }
                else if(netMode == GameNetMode.StandAlone)
                {
                    return ReleaseNetwork;
                }
            }
            else if(versionMode == GameVersionMode.Debug)
            {
                if (netMode == GameNetMode.Network)
                {
                    return ReleaseNetwork;
                }
                else if (netMode == GameNetMode.StandAlone)
                {
                    return DebugStandAlone;
                }
            }
            return null;
        }
    }

    public partial class GameStateCfg
    {
        public static GameStateData ReleaseNetwork = new GameStateData(GameStateType.Root,typeof(StateContainerBase),false,
            new GameStateData[] {
                new GameStateData(GameStateType.GameOut,typeof(GameOut),true),
                new GameStateData(GameStateType.GameIn,typeof(GameIn),false),
            });
    }

    public partial class GameStateCfg
    {
        public static GameStateData ReleaseStandAlone = new GameStateData(GameStateType.Root, typeof(StateContainerBase), false,
            new GameStateData[] {
                new GameStateData(GameStateType.GameOut,typeof(GameOut),false),
                new GameStateData(GameStateType.GameIn,typeof(GameIn),false),
                new GameStateData(GameStateType.GameDebugStandAloneInit,typeof(GameDebugStandAloneInit),true),
            });
    }

    public partial class GameStateCfg
    {
        public static GameStateData DebugStandAlone = new GameStateData(GameStateType.Root, typeof(StateContainerBase), false,
            new GameStateData[] {
                new GameStateData(GameStateType.GameOut,typeof(GameOut),false),
                new GameStateData(GameStateType.GameIn,typeof(GameIn),false),
                new GameStateData(GameStateType.GameDebugStandAloneInit,typeof(GameDebugStandAloneInit),true),
            });
    }
}
