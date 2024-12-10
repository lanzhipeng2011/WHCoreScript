//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_RET_COPYSCENE_REWARDHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
         GC_RET_COPYSCENE_REWARD packet = (GC_RET_COPYSCENE_REWARD )ipacket;
         if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
         //enter your logic
            
         if (Singleton<ObjManager>.Instance.MainPlayer)
         {
             if (packet.Result == 1)
             {
                 //Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false,"奖励已经发放,请离开副本");   //稍后替换           
                 //UIManager.CloseUI(UIInfo.VictoryScoreRoot);
                 VictoryScoreRoot.Instance().UpdateButtonState(true);
             }
             else
             {
                 //Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "领取失败");   //稍后替换
                 VictoryScoreRoot.Instance().ClearSend();
             }
         }

         return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
