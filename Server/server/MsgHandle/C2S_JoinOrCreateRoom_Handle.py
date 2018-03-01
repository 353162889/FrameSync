import protobuf.PacketOpcode_pb2
from protobuf.Msg_pb2 import S2C_JoinOrCreateRoom_Data
def HandleMsg(server,conn,proto):
    roomId = proto.roomId
    joinRoom = None
    if not conn.isInRoom():
        if roomId > 0:
            room = server.getRoomById(roomId)
            if room:
                if room.add(conn):
                    joinRoom = room
        if not joinRoom:
            joinRoom = server.createRoom()
            if joinRoom:
                joinRoom.add(conn)
    sendData = S2C_JoinOrCreateRoom_Data()
    if joinRoom:
        sendData.status = True
        sendData.roomId = joinRoom.id
    else:
        sendData.status = False
    conn.sendMsg(protobuf.PacketOpcode_pb2.S2C_JoinOrCreateRoom,sendData)