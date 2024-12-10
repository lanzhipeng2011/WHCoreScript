/********************************************************************************
 *	文件名：	Reputation.cs
 *	全路径：	\Player\Reputation.cs
 *	创建人：	李嘉
 *	创建时间：2013-11-14
 *
 *	功能说明：Obj阵营势力相关接口
 *	修改记录：将参数改为传入一个Obj_Character 客户端只要跟主角判断敌对友好关系
*********************************************************************************/

using Games.Scene;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Games.GlobeDefine;
using Games.LogicObj;

namespace Games.LogicObj
{
    public class Reputation
    {
        public static CharacterDefine.REPUTATION_TYPE GetObjReputionType(Obj_Character objChar)
        {
           
            Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
            if (_mainPlayer == null)
            {
                return CharacterDefine.REPUTATION_TYPE.REPUTATION_INVALID;
            }
            if (objChar == null)
            {
                return CharacterDefine.REPUTATION_TYPE.REPUTATION_INVALID;
            }
            CharacterDefine.REPUTATION_TYPE type = CharacterDefine.REPUTATION_TYPE.REPUTATION_INVALID;
            Tab_Relation reputation = TableManager.GetRelationByID(objChar.BaseAttr.Force, 0);
            if (null != reputation)
            {
                type = (CharacterDefine.REPUTATION_TYPE)reputation.GetRelationbyIndex((int)_mainPlayer.BaseAttr.Force);
            }
            //自己跟自己 永远是友好关系
            if (objChar.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
            {
                return CharacterDefine.REPUTATION_TYPE.REPUTATION_FRIEND;
            }
            //是玩家 且是友好关系 则进一步判断是否是PK状态
            if (objChar.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER &&
                type ==CharacterDefine.REPUTATION_TYPE.REPUTATION_FRIEND) 
            {
                Obj_OtherPlayer _Player = objChar as Obj_OtherPlayer;
                if (_Player)
                {
                    //同队伍和同帮会 伴侣 不能互相PK 不受PK模式影响 
                    if ((GameManager.gameManager.PlayerDataPool.IsHaveGuild()==false||GameManager.gameManager.PlayerDataPool.IsGuildMember(_Player) ==false)&&
                        (GameManager.gameManager.PlayerDataPool.IsHaveTeam()==false||GameManager.gameManager.PlayerDataPool.IsTeamMem(_Player.GUID) == false) &&
                        GameManager.gameManager.PlayerDataPool.LoverGUID !=_Player.GUID &&//是否是伴侣
                        GameManager.gameManager.ActiveScene.IsCopyScene() ==false //是否是副本
                        )
                    {
                        //对方是杀戮模式且在反击列表为敌对关系
                        if (_Player != null && _Player.PkModle == (int)CharacterDefine.PKMODLE.KILL && _Player.IsInMainPlayerPKList)
                        {
                            return CharacterDefine.REPUTATION_TYPE.REPUTATION_HOSTILE;
                        }
                        //主角是杀戮模式 为敌对关系
                        if (_mainPlayer.PkModle == (int)CharacterDefine.PKMODLE.KILL)
                        {
                            return CharacterDefine.REPUTATION_TYPE.REPUTATION_HOSTILE;
                        }
                        if (_Player.IsWildEnemyForMainPlayer)
                        {
                            return CharacterDefine.REPUTATION_TYPE.REPUTATION_HOSTILE;
                        }
                    }
                }
            }
            return type;
        }

        //是否为敌对
        public static bool IsEnemy(Obj_Character objChar)
        {
          
            if (CharacterDefine.REPUTATION_TYPE.REPUTATION_HOSTILE == GetObjReputionType(objChar))
            {
                return true;
            }
           
            return false;
        }

        //是否为友好
        public static bool IsFriend(Obj_Character objChar)
        {
          
            if (CharacterDefine.REPUTATION_TYPE.REPUTATION_FRIEND == GetObjReputionType(objChar))
            {
                return true;
            }

            return false;
        }

        //是否为中立
        public static bool IsNeutral(Obj_Character objChar)
        {
           
            if (CharacterDefine.REPUTATION_TYPE.REPUTATION_NEUTRAL == GetObjReputionType(objChar))
            {
                return true;
            }

            return false;
        }
    }
}
