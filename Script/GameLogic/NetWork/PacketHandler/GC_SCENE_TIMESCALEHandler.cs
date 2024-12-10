//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_SCENE_TIMESCALEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SCENE_TIMESCALE packet = (GC_SCENE_TIMESCALE)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            GameManager.gameManager.ActiveScene.SceneTimeScale(packet.TimeScaleType);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
