//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using UnityEngine;
using Games.LogicObj;
namespace SPacket.SocketInstance
 {
 public class GC_RET_JUQINGITEM_PLAYEFFECTHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
 GC_RET_JUQINGITEM_PLAYEFFECT packet = (GC_RET_JUQINGITEM_PLAYEFFECT )ipacket;
 if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
 //enter your logic

                string name = packet.Juqingitemname;
                if (name!="") 
               {
				GameObject obj=ObjManager.GetInstance().FindOtherGameObj(name);
				if(obj)
				obj.GetComponent<Obj_JuqingItem>().DelaysendMsg();
               }
 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
