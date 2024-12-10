/********************************************************************************
 *	文件名：	Obj_Character_Attr.cs
 *	全路径：	\Script\Obj\Obj_Character_Attr.cs
 *	创建人：	luoy
 *	创建时间：2013-2-12
 *
 *	功能说明：游戏逻辑Obj_Character类的属性相关部分
 *	修改记录：
 *	李嘉 2014-02-19 将原来的obj基类上移作为有动作行为的obj_character，下层添加基类obj
*********************************************************************************/

using System.Collections.Generic;
using System.Security.Permissions;
using Games.ImpactModle;
using GCGame;
using UnityEngine;
using Games.GlobeDefine;
using Games.Item;

namespace Games.LogicObj
{
    public partial class Obj_Character : Obj
    {
        protected BaseAttr m_BaseAttr;
        public virtual BaseAttr BaseAttr
        {
            get { return m_BaseAttr; }
            set { m_BaseAttr = value; }
        }
        
        public virtual void OptHPChange() //血量变化后的操作
        {
        }
        ///
        public virtual void OptMPChange()//法力变化后的操作
        {
        }
        public virtual void OptXPChange()//XP变化后的操作
        {
        }

        public virtual void OptLevelChange()//等级变化后的操作
        {
        }

        public virtual void OptHeadPicChange()//头像变化后的操作
        {
        }

        public virtual void OptNameChange()//名字变化后的操作
        {
        }

        public virtual void OptForceChange()//势力变化后的操作
        {
            SetNameBoardColor();
        }

        public virtual void OptStealthLevChange() //隐身级别变化后的操作
        {
            if (StealthLev > 0 )
            {
                SetStealthState();
            }
            else if (StealthLev <=0 )
            {
                CancelStealthState();
            }
        }

		public virtual void OptOutLineChange() //隐身级别变化后的操作
		{
			if (OutLine )
			{
				SetOutLine();
			}
			else
			{
				CancelOutLine();
			}
		}
        public virtual void OnExpChange()   // 经验变化后的操作
        {
        }        
        public virtual void UpdateAttrBroadcastPackt(GC_BROADCAST_ATTR packet)
        {
            //bind parent
            if (packet.HasBindparent)
            {
                BindParent = packet.Bindparent;
            }          
            //bind children
            {
                List<int> newlist = new List<int>( packet.bindchildrenList );
                UpdateBindChildren(newlist);
            }
            if (packet.HasName)
            {
                BaseAttr.RoleName = packet.Name;
                OptNameChange();
            }
            if (packet.HasCurForce)
            {
                BaseAttr.Force = packet.CurForce;
                OptForceChange();
            }
            if (packet.HasVipCost)
            {
                if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
                {
                    Obj_MainPlayer mainPlayer = this as Obj_MainPlayer;
                    if (mainPlayer != null)
                    {
                        mainPlayer.VipCost = packet.VipCost;
                        //更新自动挂机
                        if (mainPlayer.BaseAttr.Level >= GlobeVar.MAX_AUTOEQUIT_LIVE && VipData.GetVipLv() >= GlobeVar.USE_AUTOFIGHT_VIPLEVEL)
                        {
                            //vip2的时候选择自动强化
                            mainPlayer.UpdateSelectEquip();
                        }
                    }
                }
                if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
                {
                    Obj_OtherPlayer otherPlayer = this as Obj_OtherPlayer;
                    if (otherPlayer != null)
                    {
                        otherPlayer.VipCost = packet.VipCost;
                    }
                }
            }
            if (packet.HasCombatValue)
            {
                if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
                {
                    Obj_MainPlayer mainPlayer = this as Obj_MainPlayer;
                    if (mainPlayer != null)
                    {
                        mainPlayer.OtherCombatValue = packet.CombatValue;
                        mainPlayer.UpdateCombatValue();
                    }
                }
                if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
                {
                    Obj_OtherPlayer otherPlayer = this as Obj_OtherPlayer;
                    if (otherPlayer != null)
                    {
                        otherPlayer.OtherCombatValue = packet.CombatValue;
                        otherPlayer.UpdateCombatValue();
                    }
                }
            }

            if (packet.HasMoveSpeed)
            {
                BaseAttr.MoveSpeed = packet.MoveSpeed / 100.0f;
                if (NavAgent != null)
                {
                    NavAgent.speed = BaseAttr.MoveSpeed;
                }

                //主角的伙伴需要特殊处理
                if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_FELLOW)
                {
                    Obj_Fellow fellowObj = this as Obj_Fellow;
                    if (null != fellowObj && fellowObj.IsOwnedByMainPlayer())
                    {
                        fellowObj.SetMoveSpeedAsMainPlayer();
                    }
                }
                //主角速度变化把自己伙伴的速度也改变
                if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
                {
                    Obj_MainPlayer mainPlayer = this as Obj_MainPlayer;
                    if (mainPlayer != null)
                    {
                        Obj_Fellow fellowObj = mainPlayer.GetCurFellow();
                        if (fellowObj != null)
                        {
                            fellowObj.SetMoveSpeedAsMainPlayer();
                        }
                    }
                }
            }
            if (packet.HasBDie)
            {
                bool bDie = packet.BDie == 1;
                if (bDie == true)
                {
                    OnDie();
                }
            }
            if (packet.HasStealthLev)
            {
                StealthLev = packet.StealthLev;
                OptStealthLevChange();
            }
        }
        
    }
}
