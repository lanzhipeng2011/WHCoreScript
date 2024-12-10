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
    public class CG_LOCK_SWORDSMANHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
     CG_LOCK_SWORDSMAN packet = (CG_LOCK_SWORDSMAN)ipacket;
    if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
    return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;

 }
 }
 }
