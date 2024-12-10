//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
 {
     public class GC_MOUNTCOLLECTED_FLAGHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_MOUNTCOLLECTED_FLAG packet = (GC_MOUNTCOLLECTED_FLAG )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             GameManager.gameManager.PlayerDataPool.m_objMountParam.SyncMoutCollectedFlag(packet);
             if (MountAndFellowLogic.Instance() != null)
             {
                 MountAndFellowLogic.Instance().MountButton();
             }
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
