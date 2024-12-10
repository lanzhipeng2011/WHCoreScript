//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.LogicObj;
using Games.GlobeDefine;
using GCGame.Table;
using GCGame;
using Games.TitleInvestitive;

namespace SPacket.SocketInstance
{
    public class GC_SYNC_ACTIVETITLEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SYNC_ACTIVETITLE packet = (GC_SYNC_ACTIVETITLE )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            if (packet.Serverid != Singleton<ObjManager>.GetInstance().MainPlayer.ServerID)
            {
                Obj_Character obj = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(packet.Serverid);
                if (obj != null)
                {
                    if (obj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
                        obj.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER)
                    {
                        Obj_OtherPlayer objOtherPlayer = (Obj_OtherPlayer)obj;
                        if (objOtherPlayer != null)
                        {
                            Tab_TitleData tabTitleData = TableManager.GetTitleDataByID(packet.Titleid, 0);
                            if (tabTitleData != null)
                            {
                                string strTitle = Utils.GetTitleColor(tabTitleData.ColorLevel);
                                if (tabTitleData.Type >= (int)USERDEF_SUBTYPE.TITLE_DEFTYPE_NUM)
                                {
                                    // 系统称号
                                    strTitle += tabTitleData.InvestitiveName;
                                }
                                else if ((int)USERDEF_SUBTYPE.TITLE_DEFTYPE_INVALID < tabTitleData.Type && tabTitleData.Type < (int)USERDEF_SUBTYPE.TITLE_DEFTYPE_NUM)
                                {
                                    // 自定义称号
                                    if (packet.HasTitlename)
                                    {
                                        strTitle += packet.Titlename;
                                    }
                                }
                                objOtherPlayer.CurTitleID = packet.Titleid;
                                objOtherPlayer.StrTitleInvestitive = strTitle;
                                objOtherPlayer.ShowTitleInvestitive();
                            }
                            else
                            {
                                objOtherPlayer.CurTitleID = GlobeVar.INVALID_ID;
                                objOtherPlayer.StrTitleInvestitive = "";
                                objOtherPlayer.ShowTitleInvestitive();
                            }
                        }
                    }
                }                
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
