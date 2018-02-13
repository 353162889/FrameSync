using Proto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Framework
{
    //一般网络包
    public class MsgPacket
    {
        public static int PacketOpcodeBit = 2;
        public static int PacketDataLenBit = 2;
        public static int PacketHeadSize = PacketOpcodeBit + PacketDataLenBit;

        public bool isFrame { get; private set; }
        public short opcode { get; private set; }
        public int length { get; private set; }
        public object protoBuf { get; private set; }
        private Type m_cType;

        //发送构造
        public MsgPacket(short opcode,object protoBuf)
        {
            this.isFrame = false;
            this.length = 0;
            this.opcode = opcode;
            this.protoBuf = protoBuf;
            InitType();
        }
        //接收构造
        public MsgPacket(byte[] headBytes)
        {
            this.opcode = BitConverter.ToInt16(headBytes, 0);
            this.length = BitConverter.ToInt16(headBytes, PacketOpcodeBit);
            this.protoBuf = null;
            InitType();
        }
        private void InitType()
        {
            string strEnum = Enum.GetName(typeof(PacketOpcode), opcode);
            m_cType = Type.GetType("Proto." + strEnum + "Data");
            if(m_cType == null)
            {
                CLog.LogError("找不数据包opcode=" + opcode+"的解析类");
            }
        }

        public void WriteToBuf(ByteBuf buf, MemoryStream helpStream)
        {
            buf.SetIndex(0, 0);
            buf.WriteShortLE(this.opcode);
            buf.WriteShortLE((short)this.length);
            helpStream.Position = 0;
            if(this.protoBuf != null)
            {
                try {
                    ProtoBuf.Serializer.NonGeneric.Serialize(helpStream, this.protoBuf);
                    buf.WriteBytes(helpStream.GetBuffer(), 0, (int)helpStream.Length);
                }
                catch(Exception e)
                {
                    CLog.LogError("序列化数据包opcode=" + this.opcode + ",类型为"+ (m_cType == null ? "Null" : m_cType.ToString()) +"失败");
                }
            }
            else
            {
                CLog.LogError("序列化数据包opcode="+this.opcode + ",类型为" + (m_cType == null ? "Null" : m_cType.ToString()) + "的数据为空");
            }
        }

        public void Deserizlize(byte[] bytes,MemoryStream helpStream)
        {
            try
            {
                helpStream.Position = 0;
                helpStream.Write(bytes, 0, bytes.Length);
                this.protoBuf = ProtoBuf.Serializer.NonGeneric.Deserialize(m_cType, helpStream);
            }
            catch (Exception e)
            {
                CLog.LogError("反序列化数据包opcode=" + this.opcode + ",类型为" + (m_cType == null ? "Null" : m_cType.ToString()) + "的数据失败");
            }
        }
    }
}
