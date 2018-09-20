import protobuf.PacketOpcode_pb2
from protobuf.Msg_pb2 import Msg_Test_Data
import  time
index = 0
def HandleMsg(server,conn,proto):
    global index
    print("HandleMsg",index,time.time())
    index = index + 1
    conn.sendMsg(protobuf.PacketOpcode_pb2.Msg_Test,proto)