//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
 public class GC_GUILD_RET_LEVELUPHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_GUILD_RET_LEVELUP packet = (GC_GUILD_RET_LEVELUP )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
