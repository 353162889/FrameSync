import socketserver
import socket
import threading
import queue
import struct
import io

from errno import EALREADY, EINPROGRESS, EWOULDBLOCK, ECONNRESET, EINVAL, \
    ENOTCONN, ESHUTDOWN, EISCONN, EBADF, ECONNABORTED, EPIPE, EAGAIN, \
    errorcode

_DISCONNECTED = frozenset({ECONNRESET, ENOTCONN, ESHUTDOWN, ECONNABORTED, EPIPE,
                           EBADF})


class MsgPacket:
    HeadSize = 2 + 2

    @staticmethod
    def isFramePacket(packetId):
        return packetId > 9999;

    def __init__(self, headBytes=None,packetID = None):
        self.head = headBytes
        self.serializeBytes = None
        self.buff = None
        self.proto = None
        if headBytes is not None:
            unpack = struct.unpack("HH", self.head)
            self.packetID = unpack[0]
            self.packetSize = unpack[1]
        if packetID is not None:
            self.packetID = packetID

    def writeBuff(self, buff):
        self.buff = buff

    def writeProto(self,proto):
        self.proto = proto

    def serializeProto(self):
        if self.proto is not None:
            self.buff = self.proto.SerializeToString()

    def serialize(self):
        if self.serializeBytes is None:
            length = 0
            self.serializeProto()
            if self.buff is not None:
                length = len(self.buff)
                self.packetSize = length
                self.serializeBytes = struct.pack("HH{0}s".format(length), self.packetID,self.packetSize,self.buff)
            else:
                self.packetSize = 0
                self.serializeBytes = struct.pack("HH", self.packetID,self.packetSize)

    def getSerializeBuff(self):
        return self.serializeBytes


class PacketReceiver:
    def __init__(self, conn):
        self.connection = conn
        self.bytesIO = io.BytesIO()
        self.dataLength = 0
        self.packet = None
        #这个队列是线程安全的
        self.packetQueue = queue.Queue()
        self.buffer_size = 1024
        self.mapOpcodeClass = {}
        self.loadOpcodeClass()

    def loadOpcodeClass(self):
        enumOpcodeLib = __import__("protobuf.PacketOpcode_pb2",fromlist=True)
        msgLib = __import__("protobuf.Msg_pb2", fromlist=True)
        packetOpcode = getattr(enumOpcodeLib,"PacketOpcode",None)
        if packetOpcode:
            for item in packetOpcode.items():
                className = item[0] + "_Data"
                protoClass = getattr(msgLib,className,None)
                if protoClass:
                    self.mapOpcodeClass[item[1]] = protoClass

    def recvData(self):
        d = b''
        try:
            data = self.connection.sock.recv(self.buffer_size)
            if not data:
                self.connection.lostConnection()
            else:
                d = data

        except OSError as why:
            if why.args[0] in _DISCONNECTED:
                self.connection.lostConnection()
        finally:
            self.__recv_data(d)

    def __recv_data(self, data):
        if data and len(data) > 0:
            self.bytesIO.write(data)
            self.dataLength += len(data)
            while True:
                newPacket = self.__try_handle_packet()
                if newPacket is not None:
                    self.packetQueue.put(newPacket)
                else:
                    break

    def __try_handle_packet(self):
        if self.packet is not None:
            return self.__handle_packet()
        else:
            if self.dataLength >= MsgPacket.HeadSize:
                self.bytesIO.seek(0)
                packetHead = self.bytesIO.read(MsgPacket.HeadSize)
                self.packet = MsgPacket(packetHead)
                self.bytesIO.seek(self.dataLength)
                return self.__handle_packet()
            return None

    def __handle_packet(self):
        if self.dataLength >= self.packet.packetSize + MsgPacket.HeadSize:
            print("receive from {0} packetID = {1}".format(self.connection.id,self.packet.packetID))
            #小于10000是一般包，反序列化，否则是帧包，不反序列化
            if(not MsgPacket.isFramePacket(self.packet.packetID)) :
                self.bytesIO.seek(MsgPacket.HeadSize)
                buff = self.bytesIO.read(self.packet.packetSize)
                protoClass = self.mapOpcodeClass.get(self.packet.packetID,None)
                if(protoClass):
                    proto = protoClass()
                    proto.ParseFromString(buff)
                    self.packet.writeProto(proto)
            else:
                #帧包的话，将原大小写入buff中
                self.bytesIO.seek(0)
                buff = self.bytesIO.read(self.packet.packetSize + MsgPacket.HeadSize)
                self.packet.writeBuff(buff)

            # 将当前变成packet包IO的buff删除
            self.bytesIO.seek(self.packet.packetSize + MsgPacket.HeadSize)
            leaveBuff = self.bytesIO.read(self.dataLength - self.packet.packetSize - MsgPacket.HeadSize)
            self.bytesIO.seek(0)
            self.bytesIO.write(leaveBuff)
            self.dataLength = len(leaveBuff)
            tempPacket = self.packet
            self.packet = None
            return tempPacket
        else:
            return None


class PacketSender:
    def __init__(self, conn):
        self.connection = conn
        self.bytesIO = io.BytesIO()
        self.packetQueue = queue.Queue()
        self.dataLength = 0

    def sendPacket(self,packet):
        if packet is not None:
            self.packetQueue.put(packet)

    def serializePacket(self):
        self.__handle_packet()

    def sendData(self):
        if not self.packetQueue.empty():
            pack = self.packetQueue.get()
            pack.serialize()
            print("send to {0} packetID = {1}".format(self.connection.id,pack.packetID))
            serializeBuff = pack.getSerializeBuff()
            self.__send_buff(serializeBuff)

    def __send_buff(self, buff):
        sendLen = 0
        totalLen = len(buff)
        while sendLen < totalLen:
            l = self.connection.sock.send(buff)
            if l == 0:
                self.connection.lostConnection()
                break
            sendLen += l

#不处理逻辑功能
class Connection:
    def __init__(self, sock, id):
        self.id = id
        self.roomId = -1
        self.sock = sock
        assert isinstance(self.sock, socket.socket)
        self.connected = True
        self.sock.setblocking(0)
        self.buffer_size = 1024
        self.receiver = PacketReceiver(self)
        self.sender = PacketSender(self)

    def isInRoom(self):
        return self.roomId > -1

    def joinRoom(self,roomId):
        self.roomId = roomId

    def leaveRoom(self):
        self.roomId = -1

    def addFramePacket(self,pack):
        return

    def lostConnection(self):
        print("lostConnection[id] "+str(self.id))
        if self.sock is not None:
            try:
                self.sock.close()
            except OSError:
                print("lostConnect,OSError")
                pass
            finally:
                self.roomId = -1
                self.sock = None
                self.connected = False

    def recvData(self):
        if self.connected:
            self.receiver.recvData()

    def sendData(self):
        if self.connected:
            self.sender.sendData()

    def dequeueMsg(self):
        if not self.receiver.packetQueue.empty():
            pack = self.receiver.packetQueue.get()
            if pack is not None:
                if pack.proto is not None:
                    return pack
        return None

    def sendPacket(self,pack):
        self.sender.sendPacket(pack)

    def sendMsg(self,opcode, proto):
        pack = MsgPacket(packetID=opcode)
        pack.writeProto(proto)
        self.sendPacket(pack)
class Room:
    def __init__(self,id):
        self.id = id
        self.__mapConn = {}
        self.__delConn = []

    def add(self,conn):
        if self.getConn(conn.id) is None:
            self.__mapConn[conn.id] = conn;
            conn.joinRoom(self.id)
            return True
        return False

    def remove(self,conn):
        if self.getConn(conn.id):
            del self.__mapConn[conn.id];
            conn.leaveRoom()
            return True
        return False

    def getCount(self):
        return  len(self.__mapConn)

    def getConn(self,connId):
        return self.__mapConn.get(connId,None)

    def update(self):
        self.__delConn.clear()
        for conn in self.__mapConn.values():
            if not conn.connected:
                self.__delConn.append(conn.id)
        for connId in self.__delConn:
            del self.__mapConn[connId]
        if len(self.__mapConn) <= 0:
            return False
        return True
    def clear(self):
        self.__mapConn.clear()
        self.__delConn.clear()

class CustomRequestHandler(socketserver.BaseRequestHandler):
    def handle(self):
        self.server.addConnection(self.request)


class CustomServer(socketserver.TCPServer):
    def __init__(self, server_address, RequestHandlerClass, bind_and_activate=True):
        self._connections = {}
        self._delConnection = []
        self._connectionId = 0
        self._rooms = {}
        self._delRoom = []
        self._roomId = 0
        self._mapMsgHandle = {}
        self.loadMsgHandle()
        super(CustomServer, self).__init__(server_address, RequestHandlerClass, bind_and_activate)

    def loadMsgHandle(self):
        enumOpcodeLib = __import__("protobuf.PacketOpcode_pb2", fromlist=True)
        packetOpcode = getattr(enumOpcodeLib, "PacketOpcode", None)
        if packetOpcode:
            for item in packetOpcode.items():
                moduleName = item[0] + "_Handle"
                fullModelName = "MsgHandle."+ moduleName
                module = None
                try:
                    module = __import__(fullModelName,fromlist=True)
                except:
                    continue;
                if module:
                    self._mapMsgHandle[item[1]] = getattr(module,"HandleMsg")

    def process_request(self, request, client_address):
        self.finish_request(request, client_address)

    def shutdown_request(self, request):
        super(CustomServer, self).shutdown_request(request)
        print("CustomServer,shutdown_request")

    def handle_timeout(self):
        super(CustomServer, self).handle_timeout()
        print("CustomServer,handle_timeout")

    def addConnection(self, request):
        if not self.getConnectionById(self._connectionId + 1):
            self._connectionId += 1
            conn = Connection(request, self._connectionId)
            self.lockRecv.acquire()
            self.lockSend.acquire()
            self.lockLogic.acquire()
            self._connections[conn.id] = conn
            print("addConnection,clientId:" + str(conn.id))
            self.lockLogic.release()
            self.lockSend.release()
            self.lockRecv.release()
            return True
        return False

    def getConnectionById(self, connId):
        return self._connections.get(id, None)

    def lostConnection(self, connId):
        conn = self.getConnectionById(connId)
        if conn:
            conn.lostConnection()

    def __delConnection(self, connId):
        self.lockRecv.acquire()
        self.lockSend.acquire()
        self.lockLogic.acquire()
        if self.getConnectionById(connId):
            del self._connections[connId]
        self.lockLogic.release()
        self.lockSend.release()
        self.lockRecv.release()

    #逻辑线程中调用
    def createRoom(self):
        if not self.getRoomById(self._roomId + 1):
            self._roomId += 1
            room = Room(self._roomId)
            self._rooms[room.id] = room;
            return room
        return None
    #逻辑线程中调用
    def getRoomById(self,roomId):
        return self._rooms.get(roomId,None)

    #逻辑线程中调用
    def removeRoom(self,roomId):
        if self.getRoomById(roomId):
            self._rooms[roomId].clear()
            del self._rooms[roomId]

    def service_actions(self):
        self._delConnection.clear()
        for conn in self._connections.values():
            if not conn.connected:
                self._delConnection.append(conn)
        for conn in self._delConnection:
            self.__delConnection(conn.id)


    def serve_forever(self, poll_interval=0.5):
        #       开启线程读取或发送数据
        self.lockRecv = threading.Lock()
        self.lockSend = threading.Lock()
        self.lockLogic = threading.Lock()
        self.recvThread = threading.Thread(target=self.recvData_thread,args=(self.lockRecv,))
        self.sendThread = threading.Thread(target=self.sendData_thread,args=(self.lockSend,))
        self.logicThread = threading.Thread(target=self.logic_thread,args=(self.lockLogic,))
        self.recvThread.daemon = False
        self.sendThread.daemon = False
        self.recvThread.start()
        self.sendThread.start()
        self.logicThread.start()
        super(CustomServer, self).serve_forever(poll_interval)

    def recvData_thread(self,lock):
        while True:
            lock.acquire()
            for conn in self._connections.values():
                try:
                    conn.recvData()
                except ConnectionResetError:
                    conn.lostConnection()
            lock.release()

    def sendData_thread(self,lock):
        while True:
            lock.acquire()
            for conn in self._connections.values():
                try:
                    conn.sendData()
                except:
                    conn.lostConnection()
            lock.release()

    def logic_thread(self,lock):
        while True:
            lock.acquire()
            for conn in self._connections.values():
                pack = conn.dequeueMsg()
                while pack:
                    self.handMsg(conn,pack)
                    pack = conn.dequeueMsg()

            self._delRoom.clear()
            for room in self._rooms.values():
                if not room.update():
                    self._delRoom.append(room.id)
            for roomId in self._delRoom:
                self.removeRoom(roomId)
            lock.release()

    def handMsg(self,conn,pack):
        if MsgPacket.isFramePacket(pack.packetID):
            if conn.isInRoom():
                room = self.getRoomById(conn.roomId)
                if room:
                    room.addFramePacket(pack)
        else:
            handle = self._mapMsgHandle.get(pack.packetID, None)
            if handle:
                handle(self, conn, pack.proto)


if __name__ == '__main__':
    # info = protobuf.Msg_pb2.Info()
    # info.msg = "你好"
    # s = info.SerializeToString()
    # print(s)
    # info1 = protobuf.Msg_pb2.Info()
    # info1.ParseFromString(s)
    # print(info1.msg)
    print("开始启动服务器...")
    addr = ("127.0.0.1", 8080)
    server = CustomServer(addr, CustomRequestHandler)
    print("服务器启动...")
    server.serve_forever()
    print("服务器完毕...")

