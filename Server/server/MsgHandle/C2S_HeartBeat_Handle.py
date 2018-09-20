import protobuf.PacketOpcode_pb2
import protobuf.Msg_pb2
import time
def HandleMsg(server,conn,proto):
    sendData = protobuf.Msg_pb2.S2C_HeartBeat_Data()
    conn.sendMsg(protobuf.PacketOpcode_pb2.S2C_HeartBeat, sendData)
    print("S2C_HeartBeat",time.time())
