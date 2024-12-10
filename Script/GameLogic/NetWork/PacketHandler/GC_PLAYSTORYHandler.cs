//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
     public class GC_PLAYSTORYHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_PLAYSTORY packet = (GC_PLAYSTORY )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             int nStoryID = packet.StoryID;
			 if (nStoryID != 999)
								
				StoryDialogLogic.ShowStory (nStoryID);
			else 
			{

			
			//if(GameManager.gameManager.PlayerDataPool.IsFirstYeXiDaYing==false)
				{
					GameManager.gameManager.ActiveScene.OnLoad(21);
					GameManager.gameManager.ActiveScene.MainPlayerCreateOver();

					
				}
					
			}

             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
