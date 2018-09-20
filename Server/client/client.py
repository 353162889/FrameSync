import socket
import struct
import protobuf.Msg_pb2
import time

class CustomClient:
    def __init__(self):
        self.socket = socket.socket()
        self.index = 0

    def connect(self, addr):
        self.socket.connect(addr)
        while True:
            inp = input("请输入:")
            #time.sleep(1)
            #inp = "aaaaaa";
            print("发送数据:"+inp,self.index,time.time())
            self.index = self.index + 1
            sendTime = time.time()
            info = protobuf.Msg_pb2.Msg_Test_Data()
            info.msg = inp
            bs = info.SerializeToString()
            self.socket.sendall(struct.pack("=HH"+str(len(bs))+"s",257,len(bs), bs))
            continue
            # self.socket.sendall(struct.pack("i"+str(len(inp))+"s",len(bytes(inp))+4,inp))
            head_bytes = self.socket.recv(4)
            unpack = struct.unpack("=HH",head_bytes)
            id = unpack[0]
            length = unpack[1]
            data = self.socket.recv(length)
            recvInfo = protobuf.Msg_pb2.Msg_Test_Data()
            recvInfo.ParseFromString(data)
            print("receive form server,length:{0} id:{1} msg:{2}".format(length,id,recvInfo.msg))
            #print("ping",(time.time()-sendTime)*1000)
            # print(str(length)+str(id) + str(status)+recvInfo.msg)

if __name__ == "__main__":
    client = CustomClient()
    client.connect(("127.0.0.1",8080))
