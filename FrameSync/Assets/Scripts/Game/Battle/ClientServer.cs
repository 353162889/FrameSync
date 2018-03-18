using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    //客户端模拟的服务器，用来做单机的一些处理
    public class ClientServer : Singleton<ClientServer>
    {
        public void Init()
        {
            //监听进入战斗消息，给客户端发送初始化玩家的消息
        }
    }
}
