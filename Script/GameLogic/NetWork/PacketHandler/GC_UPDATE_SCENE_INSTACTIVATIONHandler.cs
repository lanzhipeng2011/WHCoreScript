//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_UPDATE_SCENE_INSTACTIVATIONHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_UPDATE_SCENE_INSTACTIVATION packet = (GC_UPDATE_SCENE_INSTACTIVATION)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            SceneData.UpdateSceneInst(packet);
            
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
