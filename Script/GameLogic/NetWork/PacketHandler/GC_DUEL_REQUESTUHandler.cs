//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using Games.LogicObj;
namespace SPacket.SocketInstance
 {
 public class GC_DUEL_REQUESTUHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_DUEL_REQUESTU packet = (GC_DUEL_REQUESTU )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic
 //ֱ�ӵ���MainPlayer����Ӳ���
 if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
 {
     Singleton<ObjManager>.GetInstance().MainPlayer.DuelWithMe(packet.Guid, packet.Name);
 }
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
