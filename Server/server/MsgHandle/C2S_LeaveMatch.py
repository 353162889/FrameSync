import protobuf.PacketOpcode_pb2
import protobuf.Msg_pb2

def HandleMsg(server,conn,proto):
    result = server.leaveMatch(conn)
    sendData = protobuf.Msg_pb2.S2C_LeaveMatchResult_Data()
    sendData.status = result
    conn.sendMsg(protobuf.PacketOpcode_pb2.S2C_LeaveMatchResult, sendData)