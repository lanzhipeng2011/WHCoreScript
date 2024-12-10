//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_GAIN_TITLEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_GAIN_TITLE packet = (GC_GAIN_TITLE )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            int nTitleIndex = packet.TitleIndex;
            int nTitleID = packet.TitleID;
            int nActiveTitle = packet.ActiveTitle;
            string strUserDef = "";
            if(packet.HasUserDefFullTitleName)
            {
                strUserDef = packet.UserDefFullTitleName;
            }
            GameManager.gameManager.PlayerDataPool.TitleInvestitive.HandleGainTitle(nTitleIndex, nTitleID, strUserDef, nActiveTitle);
            Singleton<ObjManager>.GetInstance().MainPlayer.ShowPlayerTitleInvestitive();
            if(TitleInvestitiveLogic.Instance() != null)
            {
                TitleInvestitiveLogic.Instance().HandleGainTitle();
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
