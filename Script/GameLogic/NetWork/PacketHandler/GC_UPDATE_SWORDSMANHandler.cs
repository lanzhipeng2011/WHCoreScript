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
    public class GC_UPDATE_SWORDSMANHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
     GC_UPDATE_SWORDSMAN packet = (GC_UPDATE_SWORDSMAN)ipacket;
    if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
    int packtype = packet.Packtype;
    SwordsManContainer.PACK_TYPE containertype = (SwordsManContainer.PACK_TYPE)packtype;
    //取得物品容器
    SwordsManContainer Container = GameManager.gameManager.PlayerDataPool.GetSwordsManContainer(containertype);
    if (null == Container)
    {
        return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
    }

	for (int i = 0; i < SwordsManContainer.SWORDSMAN_BACKPACK_SIZE; i++)         
	{
			SwordsMan oSwordsMan = Container.GetSwordsMan(i);
			if (null == oSwordsMan)
			{
				continue;
			}
			oSwordsMan.CleanUp();
	}

     for (int i = 0; i < packet.packindexCount; i++)         
     {
         int packindex = packet.GetPackindex(i);
 
         //取得侠客
         SwordsMan oSwordsMan = Container.GetSwordsMan(packindex);
         if (null == oSwordsMan)
         {
             continue;;
         }
         //ID
         oSwordsMan.DataId = packet.GetDataid(i);
         //GUID
         oSwordsMan.Guid = packet.GetGuid(i);
         if (oSwordsMan.DataId == GlobeVar.INVALID_ID)
         {
             oSwordsMan.CleanUp();
         }
         else
         {
             Tab_SwordsManAttr SwordsManTable = TableManager.GetSwordsManAttrByID(packet.GetDataid(i), 0);
             if (null == SwordsManTable)
             {
                 continue;
             }
             oSwordsMan.Exp = packet.GetExp(i);
             oSwordsMan.Level = packet.GetLevel(i);
             oSwordsMan.Quality = SwordsManTable.Quality;
             oSwordsMan.Name = SwordsManTable.Name;
             oSwordsMan.Locked = packet.GetLock(i)>0 ? true:false;
             Tab_SwordsManLevelUp SwordsLevelUpTable = TableManager.GetSwordsManLevelUpByID(oSwordsMan.DataId, 0);
             if (null == SwordsLevelUpTable)
             {
                 oSwordsMan.MaxExp = oSwordsMan.Exp;
                 LogModule.ErrorLog("SwordsLevelUpTable is null");
             }
             else
             {
                 oSwordsMan.MaxExp = SwordsLevelUpTable.GetExpNeedLvbyIndex(oSwordsMan.Level);
             }
         }
     }
   
     if (containertype == SwordsManContainer.PACK_TYPE.TYPE_EQUIPPACK)
     {
         if (SwordsManController.Instance()!= null)
         {
             SwordsManController.Instance().UpdateSwordsManEquipPack();
         }
         if (SwordsManLevelupController.Instance() != null)
         {
             SwordsManLevelupController.Instance().UpdateSwordsManInfo();
         }
         GameManager.gameManager.PlayerDataPool.SwordsManCombat = Container.GetAllSwordsManCombatValue();

     }
     else if(SwordsManContainer.PACK_TYPE.TYPE_BACKPACK == containertype)
     {
         if (SwordsManController.Instance() != null)
         {
             SwordsManController.Instance().UpdateSwordsManBackPack();
         }
         if (SwordsManLevelupController.Instance() != null)
         {
             SwordsManLevelupController.Instance().UpdateSwordsManBackPack();
             SwordsManLevelupController.Instance().UpdateSwordsManInfo();
         }
     }
    return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;

 }
 }
 }
