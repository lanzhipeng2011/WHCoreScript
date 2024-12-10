//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.LogicObj;
namespace SPacket.SocketInstance
 {
 public class GC_SYNC_PLAYER_STATEHandler : Ipacket
 {
     public uint Execute(PacketDistributed ipacket)
     {
         GC_SYNC_PLAYER_STATE packet = (GC_SYNC_PLAYER_STATE )ipacket;
         if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
         //enter your logic
         PlayerData playerDataPool = GameManager.gameManager.PlayerDataPool;
         if (playerDataPool != null)
         {
			if (packet.ObjId == Singleton<ObjManager>.Instance.MainPlayer.GUID)
			{
				for (int i = 0; i < packet.stateTypeCount; i++)
				{
					if (packet.GetStateType(i) == 16)
					{
						Singleton<ObjManager>.Instance.MainPlayer.GuildBusinessState = packet.GetStateValue(i);
                        Singleton<ObjManager>.Instance.MainPlayer.UpadatePlayerGBState();
					}
				}
			}
			else
			{
				Obj_OtherPlayer player = Singleton<ObjManager>.GetInstance().FindOtherPlayerInScene(packet.ObjId);
				if (player != null)
				{
					for (int i = 0; i < packet.stateTypeCount; i++)
					{
						if (packet.GetStateType(i) == 16)
						{
							player.GuildBusinessState = packet.GetStateValue(i);
							player.UpdateGBNameBoard();
						}
					}
				}
			}     
             
                 
         }
        
         
         return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
     }
 }
 }
