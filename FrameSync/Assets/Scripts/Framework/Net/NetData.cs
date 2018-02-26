using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public struct NetSendData
    {
        public short sendOpcode;
        public object data;
    }
    //帧包和一般包通用这个结构（收到帧包后，记得处理后面几个包，是属于当前帧包的）
    public struct NetRecvData
    {
        public short recvOpcode;
        public short len;
        public object data;
    }
}
