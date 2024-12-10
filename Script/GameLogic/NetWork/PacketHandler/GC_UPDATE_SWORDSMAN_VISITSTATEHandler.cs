//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_UPDATE_SWORDSMAN_VISITSTATEHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
     GC_UPDATE_SWORDSMAN_VISITSTATE packet = (GC_UPDATE_SWORDSMAN_VISITSTATE)ipacket;
     if (null == packet)
     {
         return (uint)PACKET_EXE.PACKET_EXE_ERROR;
     }
    //enter your logic
    GameManager.gameManager.PlayerDataPool.SwordsManVisitState = packet.Visitstateid;
    if (SwordsManController.Instance() != null)
    {
        SwordsManController.Instance().UpdateSwordsManVisitState();
    }
    return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
