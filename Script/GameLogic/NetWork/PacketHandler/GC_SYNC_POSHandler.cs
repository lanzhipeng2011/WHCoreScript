//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using UnityEngine;
namespace SPacket.SocketInstance
{
    public class GC_SYNC_POSHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SYNC_POS packet = (GC_SYNC_POS)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;

          
            //enter your logic
            Singleton<ObjManager>.GetInstance().SyncObjectPosition(packet.ServerId, packet.PosX, packet.PosZ);

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
