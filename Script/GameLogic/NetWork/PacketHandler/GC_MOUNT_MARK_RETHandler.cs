//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
 {
     public class GC_MOUNT_MARK_RETHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_MOUNT_MARK_RET packet = (GC_MOUNT_MARK_RET )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             int nMountID = packet.MountID;
             // ×øÆï½çÃæ´ò¹³
             GameManager.gameManager.PlayerDataPool.m_objMountParam.AutoFlagMountID = nMountID;
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
