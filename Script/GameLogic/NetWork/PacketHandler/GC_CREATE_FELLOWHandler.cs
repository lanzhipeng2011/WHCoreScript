//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using Games.GlobeDefine;
namespace SPacket.SocketInstance
{
    public class GC_CREATE_FELLOWHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_CREATE_FELLOW packet = (GC_CREATE_FELLOW)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

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

            Obj_Init_Data initData = new Obj_Init_Data();
            initData.m_ServerID = packet.ServerId;
            initData.m_fX = ((float)packet.PosX) / 100;
            initData.m_fZ = ((float)packet.PosZ) / 100;
            initData.m_RoleBaseID = packet.DataId;
            initData.m_Force = packet.Curforce;
            initData.m_StrName = packet.Name;
            initData.m_Guid = packet.Guid;
            initData.m_OwnerObjId = packet.Ownerobjid;
            initData.m_MoveSpeed = ((float)packet.Movespeed) / 100;
            initData.m_fDir = (float)packet.Facedir / 100;
            initData.m_StealthLev = packet.StealthLev;
            initData.m_FellowQuality = packet.Quality;
            if (packet.HasBindparent)
            {
                initData.m_BindParent = packet.Bindparent;
            }
            initData.m_BindChildren.Clear();
            for (int nindex = 0; nindex < GlobeVar.BIND_CHILDREN_MAX; ++nindex)
            {
                if (nindex < packet.bindchildrenCount)
                {
                    initData.m_BindChildren.Add(packet.GetBindchildren(nindex));
                }
                else
                {
                    initData.m_BindChildren.Add(-1);
                }
            }
            Singleton<ObjManager>.GetInstance().NewCharacterObj(GameDefine_Globe.OBJ_TYPE.OBJ_FELLOW, initData);
            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
