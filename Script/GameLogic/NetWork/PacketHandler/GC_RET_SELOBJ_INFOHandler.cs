//This code create by CodeEngine

using System;
using Games.LogicObj;
using Games.SkillModle;
using GCGame.Table;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_RET_SELOBJ_INFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_SELOBJ_INFO packet = (GC_RET_SELOBJ_INFO )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
//            int nObjId = packet.ObjId;
            int nSelObjId = packet.SeleobjId;
            Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
            if (_mainPlayer ==null)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }

            Obj_Character selObj = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(nSelObjId);
			//??????????


			//========?????id?-1???????
//			if (_mainPlayer.SelectTarget != null)
//			{
//				selObj.OutLine=false;
//				selObj.OptOutLineChange();
//			}
            if (selObj)
            {
			Tab_RoleBaseAttr tab = TableManager.GetRoleBaseAttrByID (selObj.BaseAttr.RoleBaseID, 0);
			if ((tab != null) && (tab.IsZA == 1))
			{
				return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
			}
            _mainPlayer.SelectTarget = selObj;
				//??????????
//			selObj.OutLine=true;
//			selObj.OptOutLineChange();
            }
            else
            {
               _mainPlayer.SelectTarget = null;
			   _mainPlayer.OnSelectForClick = false;
            }
            //更新选择目标头像
            _mainPlayer.UpdateTargetFrame(selObj);
            //如果选择的目标在播放技能范围的特效 切换目标时得 修改特效播放的对象
            if (_mainPlayer.CurPressSkillId != -1 && _mainPlayer.SelectTarget)
            {
                Tab_SkillEx _skillEx = TableManager.GetSkillExByID(_mainPlayer.CurPressSkillId, 0);
                if (_skillEx != null)
                {
                    if (_skillEx.RangeEffectType != -1 && _skillEx.RangeEffectTarType == (int)SKILLRANGEEFFECTTAR.SELECTTARGET)
                    {
                        _mainPlayer.SelectTarget.PlaySkillRangeEffect();
                    }
                }
            }

            //enter your logic 
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
