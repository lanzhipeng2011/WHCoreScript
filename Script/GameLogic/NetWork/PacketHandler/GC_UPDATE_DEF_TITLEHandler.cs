//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
     public class GC_UPDATE_DEF_TITLEHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
            GC_UPDATE_DEF_TITLE packet = (GC_UPDATE_DEF_TITLE )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            int nTitleID = packet.TitleID;
            string strUserDef = packet.UserDefFullTitleName;

            GameManager.gameManager.PlayerDataPool.TitleInvestitive.HandleUpdateDefTitle(nTitleID, strUserDef);
            Singleton<ObjManager>.GetInstance().MainPlayer.ShowPlayerTitleInvestitive();
            if (TitleInvestitiveLogic.Instance() != null)
            {
                TitleInvestitiveLogic.Instance().HandleGainTitle();
            }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
