//This code create by CodeEngine

using System;
using Games.GlobeDefine;
using Games.LogicObj;
using Games.SkillModle;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
    public class GC_DAMAGEBOARD_INFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_DAMAGEBOARD_INFO packet = (GC_DAMAGEBOARD_INFO )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            int objID = packet.ObjId;
            Obj_Character _objChar = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(objID);
            if (_objChar ==null)
            {
               return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }

            int nType = packet.Type;
            int nValue = packet.Value;
            //冒伤害板数字
            int nShowTimes = -1;
            float fShowInter = 0.0f;
            if (packet.HasShowtimes && packet.HasShowinter)
            {
                nShowTimes = packet.Showtimes;
                fShowInter = packet.Showinter/1000.0f;
            }
            //需要多次冒泡的特殊处理
            if (nShowTimes>0) 
            {
                MultiShowDamageBoard _damageBoardInfo =new MultiShowDamageBoard();
                _damageBoardInfo.CleanUp();
                _damageBoardInfo.ShowTimes =nShowTimes;
                _damageBoardInfo.ShowInter =fShowInter;
                _damageBoardInfo.DamageType =(GameDefine_Globe.DAMAGEBOARD_TYPE)(nType);
                _damageBoardInfo.ShowValue = nValue/nShowTimes;
                _objChar.MultiShowDamageInfo.Add(_damageBoardInfo);
                _objChar.UpdateShowMultiShowDamageBoard();
            }
            else //不需要分多次冒泡的
            {
               _objChar.ShowDamageBoard((GameDefine_Globe.DAMAGEBOARD_TYPE)(nType), nValue);
            }
            //连击点的模拟显示
            if (packet.Type == (int)GameDefine_Globe.DAMAGEBOARD_TYPE.TARGET_HPDOWN_PLAYER ||
                packet.Type == (int)GameDefine_Globe.DAMAGEBOARD_TYPE.PLAYER_ATTACK_CRITICAL)
            {
                Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
                if (_mainPlayer)
                {
                    bool bIsCritical=(packet.Type == (int)GameDefine_Globe.DAMAGEBOARD_TYPE.PLAYER_ATTACK_CRITICAL ? true : false);
                    int incHitPoint =1;
                    _mainPlayer.ChangeHit(incHitPoint, bIsCritical);
                }  
            }
			//_objChar.PlayEffect (50);
			_objChar.PlayEffect((int)GameDefine_Globe.EffectID.BEHIT);
			if (_objChar.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC)
			{
				_objChar.SetShanBai();
				_objChar.PlaySoundAtPos(_objChar.ObjType, 4, _objChar.Position);
			}

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
