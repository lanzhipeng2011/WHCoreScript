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
                 //Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false,"�����Ѿ�����,���뿪����");   //�Ժ��滻           
                 //UIManager.CloseUI(UIInfo.VictoryScoreRoot);
                 VictoryScoreRoot.Instance().UpdateButtonState(true);
             }
             else
             {
                 //Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "��ȡʧ��");   //�Ժ��滻
                 VictoryScoreRoot.Instance().ClearSend();
             }
         }

         return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
