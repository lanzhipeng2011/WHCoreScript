//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using UnityEngine;
namespace SPacket.SocketInstance
{
    public class GC_OP_TELEPORTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_OP_TELEPORT packet = (GC_OP_TELEPORT )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            bool bShowTeleport = packet.IsShow == 1 ? true : false;
            GameObject teleport = GameManager.gameManager.ActiveScene.Teleport;
            if (teleport != null)
            {
                teleport.SetActive(bShowTeleport);
            }
            GameObject teleportCopyScene = GameManager.gameManager.ActiveScene.TeleportCopyScene;
            if (teleportCopyScene != null)
            {
                teleportCopyScene.SetActive(bShowTeleport);
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
