import  protobuf.PacketOpcode_pb2;
import  sys
if __name__ == '__main__' :
    dd =__import__("protobuf.PacketOpcode_pb2",fromlist=True)
    dd1 = getattr(dd,"_PACKETOPCODE")
    for v in dd1.values:
        print(v.name)

    className =  getattr(dd,"PacketOpcode").Name(1000) + "_Data"
    cc = __import__("protobuf.Msg_pb2",fromlist=True)
    tt = getattr(cc, className)()
    tt.msg = 'aa'
    print(tt.msg)

    for item in protobuf.PacketOpcode_pb2.PacketOpcode.items():
        print(item)

    ttt = __import__("server.MsgHandle.Msg_Test_Handle",fromlist=True)
    print(ttt)
    print(sys.modules.get("server.MsgHandle.Msg_Test_Handle",None))
    modele =sys.modules.get("server.MsgHandle.Msg_Test_Handle",None)
    modele.HandleMsg(None,None)

    mode = sys.modules.get("server.MsgHandle.Msg_Test_Handle", None)
    print(mode)
    #print(ttt.__all__)
