//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.Item;
using Games.GlobeDefine;
namespace SPacket.SocketInstance
{
    public class GC_SHOW_EQUIPREMINDHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SHOW_EQUIPREMIND packet = (GC_SHOW_EQUIPREMIND)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            GameItem item = new GameItem();
            item.Guid = packet.EquipGUID;
            item.DataID = packet.EquipID;

            BaseAttr attr = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr;
            int Profession =  Singleton<ObjManager>.GetInstance().MainPlayer.Profession;
            if (item.GetEquipLevel() > attr.Level || (item.GetProfessionRequire() != -1 && item.GetProfessionRequire() != Profession)) 
            {
             
                return 0;
            }

            int tabindex = item.GetSubClass()-1;
            GameItem eqitem = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(tabindex);
           
            int eqfightValue =  eqitem.GetCombatValue();
            int fightValue = item.GetCombatValue();
            if (eqitem != null && eqfightValue >= fightValue)
            {
                return 0;
            } 

            if (BackPackLogic.Instance() != null)
            {
                if (packet.EquipGUID != BackPackLogic.Instance().TakeOffGuid)
                {
                    EquipRemindLogic.InitEquipInfo(item);
                }
                else
                {
                    BackPackLogic.Instance().TakeOffGuid = GlobeVar.INVALID_GUID;
                }
            }
            else if (RoleViewLogic.Instance() != null)
            {
                if (packet.EquipGUID != RoleViewLogic.Instance().TakeOffGuid)
                {
                    EquipRemindLogic.InitEquipInfo(item);
                }
                else
                {
                    RoleViewLogic.Instance().TakeOffGuid = GlobeVar.INVALID_GUID;
                }
            }
            else
            {
                EquipRemindLogic.InitEquipInfo(item);
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
