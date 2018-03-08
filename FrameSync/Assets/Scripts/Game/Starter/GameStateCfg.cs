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
        Start,
        Loading,
        Gaming
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
                    return ReleaseNetwork;
                }
            }
            return null;
        }
    }

    public partial class GameStateCfg
    {
        public static GameStateData ReleaseNetwork = new GameStateData(GameStateType.Root,typeof(StateContainerBase),false,
            new GameStateData[] {
                new GameStateData(GameStateType.Start,typeof(GS_Start),true),
                new GameStateData(GameStateType.Loading,typeof(GS_Loading),false),
                new GameStateData(GameStateType.Gaming,typeof(GS_Gaming),false),
            });
    }
}
