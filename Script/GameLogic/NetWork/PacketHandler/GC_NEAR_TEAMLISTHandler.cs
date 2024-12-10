//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_NEAR_TEAMLISTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_NEAR_TEAMLIST packet = (GC_NEAR_TEAMLIST)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            //enter your logic
            if (null != GUIData.delNearbyTeampUpdate) GUIData.delNearbyTeampUpdate(packet);
           // RelationLogic.Instance().UpdateNearbyTeam(packet);

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
