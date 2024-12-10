//This code create by CodeEngine

using System;
using Games.GlobeDefine;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_CREATE_SNAREHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_CREATE_SNARE packet = (GC_CREATE_SNARE )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            //判断ServerID是否合法
            if (packet.ServerId < 0)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }
            //安全措施，如果发现ServerID已经存在，则先移除掉
            if (Singleton<ObjManager>.GetInstance().IsObjExist(packet.ServerId))
            {
                Singleton<ObjManager>.GetInstance().RemoveObj(packet.ServerId);
            }

            ObjSnare_Init_Data initData = new ObjSnare_Init_Data();
            initData.m_ServerID = packet.ServerId;
            initData.m_OwerGuid = packet.Owerguid;
            initData.m_OwnerObjId = packet.OwerId;
            initData.m_fX = ((float)packet.PosX) / 100;
            initData.m_fZ = ((float)packet.PosZ) / 100;
            initData.m_SnareID = packet.SnareId;
            Singleton<ObjManager>.GetInstance().CreateSnareObj(initData);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
