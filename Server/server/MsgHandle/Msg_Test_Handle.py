import protobuf.PacketOpcode_pb2
from protobuf.Msg_pb2 import Msg_Test_Data
def HandleMsg(server,conn,proto):
    print(proto.msg)
    conn.sendMsg(protobuf.PacketOpcode_pb2.Msg_Test,proto)