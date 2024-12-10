//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_RET_TRAILHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_TRAIL packet = (GC_RET_TRAIL)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            if (RelationFriendWindow.Instance() != null)
            {
                RelationFriendWindow.Instance().HandleRetTrail(packet.SceneClass, packet.SceneInst, packet.PosX, packet.PosZ);
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
