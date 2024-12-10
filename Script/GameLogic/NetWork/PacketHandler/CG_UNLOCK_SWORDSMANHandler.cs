//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.SwordsMan;
using GCGame.Table;
using Games.GlobeDefine;

namespace SPacket.SocketInstance
 {
    public class CG_UNLOCK_SWORDSMANHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
     CG_UNLOCK_SWORDSMAN packet = (CG_UNLOCK_SWORDSMAN)ipacket;
     if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
     //enter your logic
     return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
