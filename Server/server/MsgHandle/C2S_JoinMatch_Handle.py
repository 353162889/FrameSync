import protobuf.PacketOpcode_pb2
import protobuf.Msg_pb2

def HandleMsg(server,conn,proto):
    result = server.joinMatch(conn)
    sendData = protobuf.Msg_pb2.S2C_JoinMatchResult_Data()
    sendData.status =result
    conn.sendMsg(protobuf.PacketOpcode_pb2.S2C_JoinMatchResult, sendData)
    #检查匹配是否成功
    matchConns = server.getMatchConns(1)
    if matchConns:
        for matchConn in matchConns:
            server.leaveMatch(matchConn)
        joinRoom = server.createRoom()
        if joinRoom:
            for matchConn in matchConns:
                joinRoom.add(matchConn)
            sendData = protobuf.Msg_pb2.S2C_MatchResult_Data()
            sendData.status = True
            conn.sendMsg(protobuf.PacketOpcode_pb2.S2C_MatchResult, sendData)
