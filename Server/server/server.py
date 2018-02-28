import socketserver
import socket
import threading
import queue
import struct
import io
import protobuf.Msg_pb2

from errno import EALREADY, EINPROGRESS, EWOULDBLOCK, ECONNRESET, EINVAL, \
    ENOTCONN, ESHUTDOWN, EISCONN, EBADF, ECONNABORTED, EPIPE, EAGAIN, \
    errorcode

_DISCONNECTED = frozenset({ECONNRESET, ENOTCONN, ESHUTDOWN, ECONNABORTED, EPIPE,
                           EBADF})


class MsgPacket:
    HeadSize = 2 + 2

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
        self.packetQueue = queue.Queue()
        self.buffer_size = 1024

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
            self.bytesIO.seek(MsgPacket.HeadSize)
            buff = self.bytesIO.read(self.packet.packetSize)
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

class Connection:
    def __init__(self, sock, id):
        self.id = id
        self.sock = sock
        assert isinstance(self.sock, socket.socket)
        self.connected = True
        self.sock.setblocking(0)
        self.buffer_size = 1024
        self.receiver = PacketReceiver(self)
        self.sender = PacketSender(self)

    def lostConnection(self):
        print("lostConnection[id] "+str(self.id))
        if self.sock is not None:
            try:
                self.sock.close()
            except OSError:
                print("lostConnect,OSError")
                pass
            finally:
                self.sock = None
                self.connected = False

    def recvData(self):
        if self.connected:
            self.receiver.recvData()

    def sendData(self):
        if self.connected:
            self.sender.sendData()

    def handMsg(self):
        while not self.receiver.packetQueue.empty():
            pack = self.receiver.packetQueue.get()
            msg = ""
            packetID = 1
            if pack.buff is not None:
                # msg = str(struct.unpack(str(len(pack.buff)) + "s", pack.buff)[0],encoding="utf-8")
                # print("receive from{0}:{1}".format(self.id, msg))
                info = protobuf.Msg_pb2.Msg_Test_Data()
                print("receive buff len{0}".format(len(pack.buff)))
                info.ParseFromString(pack.buff)
                msg = info.msg
                packetID = pack.packetID
                print("receive from{0},id:{1} data:{2}".format(self.id,pack.packetID, info.msg))
                #发送消息给客户端
                sendPack = MsgPacket(packetID=packetID)
                # returnMsg = "[server return]"+msg
                returnProto = protobuf.Msg_pb2.Msg_Test_Data()
                returnProto.msg = "[server return]"+msg
                returnProto.SerializeToString()
                # sendPack.writeBuff(bytes(returnMsg,encoding="utf-8"))
                sendPack.writeProto(returnProto)
                self.sendMsg(sendPack)

    def sendMsg(self,pack):
        self.sender.sendPacket(pack)


class CustomRequestHandler(socketserver.BaseRequestHandler):
    def handle(self):
        self.server.addConnection(self.request)


class CustomServer(socketserver.TCPServer):
    def __init__(self, server_address, RequestHandlerClass, bind_and_activate=True):
        self._connections = {}
        self._delConnection = []
        self._connectionId = 0
        super(CustomServer, self).__init__(server_address, RequestHandlerClass, bind_and_activate)

    def process_request(self, request, client_address):
        self.finish_request(request, client_address)

    def shutdown_request(self, request):
        super(CustomServer, self).shutdown_request(request)
        print("CustomServer,shutdown_request")

    def handle_timeout(self):
        super(CustomServer, self).handle_timeout()
        print("CustomServer,handle_timeout")

    def addConnection(self, request):
        self._connectionId += 1
        conn = Connection(request, self._connectionId)
        if not conn.id in self._connections:
            self.lockRecv.acquire()
            self.lockSend.acquire()
            self.lockLogic.acquire()
            self._connections[conn.id] = conn
            print("addConnection,clientId:"+str(conn.id))
            self.lockLogic.release()
            self.lockSend.release()
            self.lockRecv.release()

    def getConnectionById(self, connId):
        return self._connections.get(id, None)

    def lostConnection(self, connId):
        conn = self.getConnectionById(connId)
        if conn:
            conn.lostConnection()

    def delConnection(self, connId):
        self.lockRecv.acquire()
        self.lockSend.acquire()
        self.lockLogic.acquire()
        if connId in self._connections:
            del self._connections[connId]
        self.lockLogic.release()
        self.lockSend.release()
        self.lockRecv.release()

    def service_actions(self):
        self._delConnection.clear()
        for conn in self._connections.values():
            if not conn.connected:
                self._delConnection.append(conn)
        for conn in self._delConnection:
            self.delConnection(conn.id)


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
                conn.handMsg()
            lock.release()


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
