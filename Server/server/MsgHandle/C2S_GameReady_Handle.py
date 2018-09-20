import protobuf.PacketOpcode_pb2
import protobuf.Msg_pb2
import random

def HandleMsg(server,conn,proto):
    if not conn.isInRoom():
        return
    room = server.getRoomById(conn.roomId)
    if room:
        room.readyConn(conn,True)
        if room.isStart():
            #发送所有玩家开始战斗消息
            lstConns = room.getAllConn()
            randomSeed = random.randint(0, 999999)
            for oneConn in lstConns:
                sendData = protobuf.Msg_pb2.S2C_StartBattle_Data()
                sendData.seed = randomSeed;
                sendData.userId = oneConn.id;
                oneConn.sendMsg(protobuf.PacketOpcode_pb2.S2C_StartBattle, sendData)
            # 创建所有玩家
            for oneConn in lstConns:
                sendData = protobuf.Msg_pb2.Frame_CreatePlayer_Data()
                sendData.playerId = oneConn.id
                sendData.campId = 0
                sendData.configId = 9001
                for otherConn in lstConns:
                    otherConn.sendMsg(protobuf.PacketOpcode_pb2.Frame_CreatePlayer, sendData)



