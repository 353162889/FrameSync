using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class GameEvent
    {
        //系统消息
        public static int ResolutionUpdate = "ResolutionUpdate".GetHashCode();//分辨率改变

        //战斗部分
        public static int StartBattle = "StartBattle".GetHashCode();//开始战斗消息
        public static int PvpPlayerCreate = "PvpPlayerCreate".GetHashCode(); ///战斗玩家创建消息
        public static int UnitAdd = "UnitAdd".GetHashCode();//单位添加
        public static int UnitRemove = "UnitRemove".GetHashCode();//单位移除
        public static int UnitHurt = "UnitHurt".GetHashCode();
        public static int UnitRecovery = "UnitRecovery".GetHashCode();
        public static int UnitDie = "UnitDie".GetHashCode();
        public static int AddUnitDestory = "AddUnitDestory".GetHashCode();
        public static int PvpPlayerUnitDie = "PlayerUnitDie".GetHashCode();
        //这个只用作与选择器是否选择到目标装饰使用(BTG_IsSelectDecorator使用)，合理的设计方式是这个不应该放在全局消息（因为这个是某个技能或远程抛出的）
        public static int BTNodeSelectCompositeSelectTarget = "BTNodeSelectCompositeSelectTarget".GetHashCode();//选择器选择到目标时会抛出此消息
    }
}
