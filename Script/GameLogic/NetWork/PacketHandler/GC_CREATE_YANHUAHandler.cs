//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.LogicObj;
using UnityEngine;
using Games.GlobeDefine;
using Games.Scene;

namespace SPacket.SocketInstance
{
    public class GC_CREATE_YANHUAHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_CREATE_YANHUA packet = (GC_CREATE_YANHUA)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic 
            //安全措施，如果发现ServerID已经存在，则先移除掉
            if (Singleton<ObjManager>.GetInstance().IsObjExist(packet.ServerId))
            {
                Singleton<ObjManager>.GetInstance().RemoveObj(packet.ServerId);
            }

            ObjYanHua_Init_Data initData = new ObjYanHua_Init_Data();
            initData.m_ServerID = packet.ServerId;
            initData.m_OwerGuid = packet.Owerguid;
            initData.m_OwnerObjId = packet.OwerId;
            initData.m_fX = ((float)packet.PosX) / 100;
            initData.m_fZ = ((float)packet.PosZ) / 100;
            initData.m_nYanHuaID = packet.SnareId;
            Singleton<ObjManager>.GetInstance().CreateYanHuaObj(initData);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
