//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using Games.GlobeDefine;
namespace SPacket.SocketInstance
 {
     public class GC_DROPITEM_INFOHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_DROPITEM_INFO packet = (GC_DROPITEM_INFO )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic

             //判断ServerID是否合法
             if (packet.ObjId < 0)
             {
                 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
             }
             //安全措施，如果发现ServerID已经存在，则先移除掉
             if (Singleton<ObjManager>.GetInstance().IsObjExist(packet.ObjId))
             {
                 Singleton<ObjManager>.GetInstance().RemoveObj(packet.ObjId);
             }
             Obj_DroopItemData initData = new Obj_DroopItemData();
             initData.m_nServerID = packet.ObjId;
             initData.m_fX = ((float)packet.Pos_x) / 100;
             initData.m_fZ = ((float)packet.Pos_z) / 100;
             initData.m_nType = packet.Type;
             initData.m_nItemId = packet.DropItemId;
             initData.m_nItemCount = packet.Count;
             initData.m_OwnerGuid = packet.OwnerGuid;            
            
             Singleton<ObjManager>.GetInstance().NewDropObj(GameDefine_Globe.OBJ_TYPE.OBJ_DROP_ITEM, initData);

             

             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
