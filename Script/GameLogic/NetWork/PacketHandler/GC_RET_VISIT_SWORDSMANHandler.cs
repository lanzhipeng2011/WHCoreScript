//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using UnityEngine;
namespace SPacket.SocketInstance
{
    public class GC_RET_VISIT_SWORDSMANHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_VISIT_SWORDSMAN packet = (GC_RET_VISIT_SWORDSMAN)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            if (packet.Success > 0)
            {
                if (BackCamerControll.Instance() != null)
                {
                    BackCamerControll.Instance().PlaySceneEffect(144);
                }
                GameManager.gameManager.SoundManager.PlaySoundEffect(117); //box
            }
            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
