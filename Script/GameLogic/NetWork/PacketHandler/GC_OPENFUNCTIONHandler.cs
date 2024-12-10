//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_OPENFUNCTIONHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_OPENFUNCTION packet = (GC_OPENFUNCTION )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            if (MenuBarLogic.Instance() != null)
            {
                MenuBarLogic.Instance().OpenFunction(packet.Type);
            }
            if (FunctionButtonLogic.Instance() != null)
            {
                FunctionButtonLogic.Instance().OpenFunction(packet.Type);
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
