//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
     public class GC_RET_TEAMPLATFORMINFOHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_RET_TEAMPLATFORMINFO packet = (GC_RET_TEAMPLATFORMINFO )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             TeamPlatformWindow.ClearplayerList();
             TeamPlatformWindow.ClearTeamMap();
             if (DungeonWindow.Instance())
             {
                 DungeonWindow.Instance().OnTeamPlatformOpen();
             }
             for (int i = 0; i < packet.playerGuidCount; i++)
             {
                 TeamPlatformWindow.addplayerList(packet.GetPlayerGuid(i),packet.GetPlayerName(i),packet.GetPlayerLevel(i),packet.GetPlayerProf(i),packet.GetPlayerCombat(i));
             }
             for (int i = 0; i < packet.teamIDCount; i++)
             {
                 TeamPlatformWindow.addTeamList(packet.GetTeamID(i), packet.GetMemberGuid(i), packet.GetMemberName(i), packet.GetMemberLevel(i), packet.GetMemberProf(i), packet.GetMemberCombat(i));
             }
             TeamPlatformWindow.Instance().UpdateTeamItemInfo();
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
