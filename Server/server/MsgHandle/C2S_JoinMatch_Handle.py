import protobuf.PacketOpcode_pb2
import protobuf.Msg_pb2

matchCount = 1;
def HandleMsg(server,conn,proto):
    result = server.joinMatch(conn)
    sendData = protobuf.Msg_pb2.S2C_JoinMatchResult_Data()
    sendData.status =result
    conn.sendMsg(protobuf.PacketOpcode_pb2.S2C_JoinMatchResult, sendData)
    #检查匹配是否成功
    global matchCount
    #匹配暂时以发送过来的最大人数作为人数上限
    if proto.matchCount > matchCount:
        matchCount = proto.matchCount;
    matchConns = server.getMatchConns(matchCount)
    if matchConns:
        for matchConn in matchConns:
            server.leaveMatch(matchConn)
        joinRoom = server.createRoom()
        if joinRoom:
            for matchConn in matchConns:
                joinRoom.add(matchConn)
            allConns = joinRoom.getAllConn()
            for roomConn in allConns:
                sendData = protobuf.Msg_pb2.S2C_MatchResult_Data()
                sendData.status = True
                roomConn.sendMsg(protobuf.PacketOpcode_pb2.S2C_MatchResult, sendData)

