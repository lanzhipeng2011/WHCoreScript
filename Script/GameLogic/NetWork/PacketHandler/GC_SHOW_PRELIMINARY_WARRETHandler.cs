//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_SHOW_PRELIMINARY_WARRETHandler : Ipacket
    {
        public int m_isWin = 0;
        public int m_warType =-1;
        public int m_nExp =0;
        public int m_nContribute =0;
        public int m_nCoin =0;
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SHOW_PRELIMINARY_WARRET packet = (GC_SHOW_PRELIMINARY_WARRET )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            m_isWin =packet.IsWin;
            m_warType = packet.Wartype;
            m_nCoin = packet.CoinValue;
            m_nExp =packet.IncExp;
            m_nContribute =packet.IncGuildContribute;
            UIManager.ShowUI(UIInfo.GuilWarRetWindow, OnLoadGuilWarRetWindow);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }

        void OnLoadGuilWarRetWindow(bool bSuccess, object param)
        {
            if (GuildWarRetInfoLogic.Instance())
            {
                GuildWarRetInfoLogic.Instance().UpdateInfo(m_isWin,m_warType,m_nExp,m_nContribute,m_nCoin);
            }
        }
    }
 }
