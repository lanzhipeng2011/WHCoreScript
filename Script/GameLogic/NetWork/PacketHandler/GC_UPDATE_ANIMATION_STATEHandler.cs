//This code create by CodeEngine

using System;
using Games.GlobeDefine;
using Games.LogicObj;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using GCGame.Table;
using GCGame;
namespace SPacket.SocketInstance
 {
    public class GC_UPDATE_ANIMATION_STATEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_UPDATE_ANIMATION_STATE packet = (GC_UPDATE_ANIMATION_STATE )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
          
            Obj_Character _objChar = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(packet.ObjId);
            if (_objChar ==null)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }
			_objChar.CurObjAnimState = (GameDefine_Globe.OBJ_ANIMSTATE)packet.AnimationState;
			// by dsy
			if (_objChar.CurObjAnimState == GameDefine_Globe.OBJ_ANIMSTATE.STATE_DEATH) 
			{
				Tab_RoleBaseAttr  attr=TableManager.GetRoleBaseAttrByID(_objChar.BaseAttr.RoleBaseID,0);
				if((attr!=null)&&attr.IsZA==1)
				{
					CZLanimation ani=_objChar.gameObject.GetComponentInChildren<CZLanimation>();
					if(ani!=null)
					{
						ani.playanimation();
					}
					_objChar.PlayEffect(301);
				}
				//_objChar.PlayEffect (50);
				_objChar.PlayEffect((int)GameDefine_Globe.EffectID.BEHIT);
				if (_objChar.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC)
				{
					_objChar.SetShanBai();
					_objChar.PlaySoundAtPos(_objChar.ObjType, 4, _objChar.Position);
				}
				if(attr!=null&&attr.IsPlayMdt==1)
				{
					GameManager.gameManager.ChangeTimeScal(0.2f);
					_objChar.m_IsMJ=true;
				}
			}
			if (_objChar.CurObjAnimState == GameDefine_Globe.OBJ_ANIMSTATE.STATE_ATTACKFLY) 
			{
				Tab_RoleBaseAttr  attr=TableManager.GetRoleBaseAttrByID(_objChar.BaseAttr.RoleBaseID,0);
				if((attr!=null)&&attr.IsZA==1)
				{
//					if(_objChar.BaseAttr.RoleBaseID!=190209)
//					{
//					_objChar.AnimLogic.Play(1100);
//					}
//					else
					{
						CZLanimation ani=_objChar.gameObject.GetComponentInChildren<CZLanimation>();
						if(ani!=null)
						{
							ani.playanimation();
						}
					}
					_objChar.PlayEffect(301);
					_objChar.PlayEffect(304);
				}
				Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
				_mainPlayer.OnSelectTarget(null);
			}
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }

    }
 }
