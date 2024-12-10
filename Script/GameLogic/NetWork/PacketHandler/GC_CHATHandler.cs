//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;

namespace SPacket.SocketInstance
{
    public class GC_CHATHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_CHAT packet = (GC_CHAT )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic   

            if (null == GameManager.gameManager.PlayerDataPool.BlackList)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }

            if (packet.HasSenderguid)
            {
                if (GameManager.gameManager.PlayerDataPool.BlackList.IsExist(packet.Senderguid))
                {
                    return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
                }               
            }

            GameManager.gameManager.PlayerDataPool.ChatHistory.OnReceiveChat(packet);
            if (ChatFrameLogic.Instance() != null)
            {
                ChatFrameLogic.Instance().OnReceiveChat(packet);
            }
            if (ChatInfoLogic.Instance() != null)
            {
                ChatInfoLogic.Instance().OnReceiveChat();
            }            
            if (LastSpeakerChatLogic.Instance() != null)
            {
                LastSpeakerChatLogic.Instance().OnReceiveChat();
            }
            
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
