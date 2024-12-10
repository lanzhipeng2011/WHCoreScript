//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_DELETE_TITLEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_DELETE_TITLE packet = (GC_DELETE_TITLE )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            int nDeleteIndex = packet.TitleIndex;
            int nActiveTitle = packet.ActiveTitle;
            GameManager.gameManager.PlayerDataPool.TitleInvestitive.HandleDeleteTitle(nDeleteIndex, nActiveTitle);
            if (Singleton<ObjManager>.Instance.MainPlayer)
            {
                Singleton<ObjManager>.Instance.MainPlayer.ShowPlayerTitleInvestitive();
            }
            if (TitleInvestitiveLogic.Instance())
            {
                TitleInvestitiveLogic.Instance().HandleDeleteTitle();
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
