//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_NOTICEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_NOTICE packet = (GC_NOTICE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            if (packet.HasNotice)
            {
                bool IsFilterRepeat = false;
                if(packet.HasFilterRepeat)
                {
                    IsFilterRepeat = packet.FilterRepeat == 1? true:false;
                }

                GUIData.AddNotifyData(packet.Notice, IsFilterRepeat);
                if (packet.Notice == "#{2129}")
                {
                    UIManager.CloseUI(UIInfo.RankRoot);
                }
            }
            
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
