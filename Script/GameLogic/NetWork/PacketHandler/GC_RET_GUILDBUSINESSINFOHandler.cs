//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
     public class GC_RET_GUILDBUSINESSINFOHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_RET_GUILDBUSINESSINFO packet = (GC_RET_GUILDBUSINESSINFO )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
            if (null != GameManager.gameManager.PlayerDataPool.GuildInfo)
            {
                GameManager.gameManager.PlayerDataPool.GuildInfo.UpdataBusinessDate(packet);
            }
            if (MenuBarLogic.Instance() != null)
            {
                MenuBarLogic.Instance().UpdateGuildAndMasterReserveMember();
            }
            if (PlayerFrameLogic.Instance() != null)
            {
                PlayerFrameLogic.Instance().UpdateRemainNum();
            }

            if (GuildWindow.Instance() != null)
            {
                GuildWindow.Instance().ShowGuildBusiness();
            }
            
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
