/********************************************************************
	filename:	Obj_MainPlayer_Master.cs
	date:		2014-5-7  21-07
	author:		tangyi
	purpose:	师门相关obj逻辑
*********************************************************************/
using UnityEngine;
using System.Collections;
using Games.LogicObj;
using System;
using Games.GlobeDefine;
using GCGame.Table;
using System.Collections.Generic;

namespace Games.LogicObj
{
    public partial class Obj_MainPlayer : Obj_OtherPlayer
    {
        //是否需要请求个人师门信息
        private bool m_bNeedRequestMasterInfo = true;
        public bool NeedRequestMasterInfo
        {
            get { return m_bNeedRequestMasterInfo; }
            set { m_bNeedRequestMasterInfo = value; }
        }
        //是否需要请求全服师门列表
        private bool m_bNeedRequestMasterList = true;
        public bool NeedRequestMasterList
        {
            get { return m_bNeedRequestMasterList; }
            set { m_bNeedRequestMasterList = value; }
        }

        //师门信息更新间隔，包括师门列表和师门信息
        private const int c_MasterRequestCoolDown = 30;
        
        //申请全服师门列表
        public void ReqMasterList()
        {
            if (null != GameManager.gameManager.PlayerDataPool.MasterPreList)
            {
                GameManager.gameManager.PlayerDataPool.MasterPreList.CleanUp();
            }

            CG_MASTER_REQ_LIST msg = (CG_MASTER_REQ_LIST)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MASTER_REQ_LIST);
            msg.Requester = GUID;
            msg.SendPacket();

            m_bNeedRequestMasterList = false;
            StartCoroutine(ResetMasterListFlag());
        }

        IEnumerator ResetMasterListFlag()
        {
            yield return new WaitForSeconds(c_MasterRequestCoolDown);

            m_bNeedRequestMasterList = true;
        }

        //申请师门信息
        public void ReqMasterInfo()
        {
            if (null != GameManager.gameManager.PlayerDataPool.MasterInfo)
            {
                GameManager.gameManager.PlayerDataPool.MasterInfo.CleanUp();
            }

            CG_MASTER_REQ_INFO msg = (CG_MASTER_REQ_INFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MASTER_REQ_INFO);
            msg.Requester = GUID;
            msg.SendPacket();

            //m_fLastRequestMasterInfoTime = Time.time;
            m_bNeedRequestMasterInfo = false;
            StartCoroutine(ResetMasterInfoFlag());
        }

        IEnumerator ResetMasterInfoFlag()
        {
            yield return new WaitForSeconds(c_MasterRequestCoolDown);

            m_bNeedRequestMasterInfo = true;
        }

        //申请创建师门
        public void ReqCreateMaster(string MasterName, string MasterNotice)
        {
            //检测名字长度
            if (MasterName.Length <= 0 || MasterName.Length > GlobeVar.MAX_MASTER_NAME)
            {
                return;
            }
            //检测公告长度
            if (MasterNotice.Length <= 0 || MasterNotice.Length > GlobeVar.MAX_MASTER_NOTICE)
            {
                return;
            }

//            //玩家等级判断
//            if (BaseAttr.Level < GlobeVar.CREATE_MASTER_LEVEL)
//            {
//                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "你的人物等级不足60级，无法创建师门");
//                return;
//            }

            //有师门无法申请
            if (GameManager.gameManager.PlayerDataPool.MasterInfo.MasterGuid != GlobeVar.INVALID_GUID)
            {
                //是待审批成员时 可以创建师门
                if (GameManager.gameManager.PlayerDataPool.IsMasterReserveMember() == false)
                {
                    Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "你已属于一个宗派，不能创建宗派");
                    return;
                }
            }

            CG_MASTER_CREATE msg = (CG_MASTER_CREATE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MASTER_CREATE);
            msg.MasterName = MasterName;
            msg.MasterNotice = MasterNotice;
            msg.SendPacket();
        }

        //申请加入师门
        public void ReqJoinMaster(UInt64 MasterGuid)
        {
            //师门Guid判断
            if (MasterGuid == GlobeVar.INVALID_GUID || MasterGuid == 0)
            {
                return;
            }

            //有师门无法申请
            if (GameManager.gameManager.PlayerDataPool.MasterInfo.MasterGuid != GlobeVar.INVALID_GUID &&
                GameManager.gameManager.PlayerDataPool.IsMasterReserveMember() == false)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2801}");
                return;
            }

            if (GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr.Level < 10)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2971}");
                return;
            }

            CG_MASTER_JOIN msg = (CG_MASTER_JOIN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MASTER_JOIN);
            msg.MasterGuid = MasterGuid;
            msg.SendPacket();
        }

        //申请离开师门
        public void ReqLeavMaster()
        {
            //无师门无法申请
            if (GameManager.gameManager.PlayerDataPool.MasterInfo.MasterGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            CG_MASTER_LEAVE msg = (CG_MASTER_LEAVE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MASTER_LEAVE);
            msg.Requester = GUID;
            msg.SendPacket();
        }

        //批准待审批会员
        public void ReqApproveMasterMember(UInt64 approver, bool agree)
        {
            //无师门无法申请
            if (GameManager.gameManager.PlayerDataPool.MasterInfo.MasterGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            CG_MASTER_APPROVE_RESERVE msg = (CG_MASTER_APPROVE_RESERVE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MASTER_APPROVE_RESERVE);
            msg.ReserveMember = approver;
            msg.IsAgree = agree? 1: 0;
            msg.SendPacket();
        }

        //踢出某个会员
        public void ReqKickMasterMember(UInt64 kickedGuid)
        {
            if (kickedGuid == GlobeVar.INVALID_GUID || kickedGuid == 0)
            {
                return;
            }

            //无师门无法申请
            if (GameManager.gameManager.PlayerDataPool.MasterInfo.MasterGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //非掌门不能申请
            if (GameManager.gameManager.PlayerDataPool.IsMasterChief() == false)
            {
                return;
            }

            if (kickedGuid == GUID)
            {
                return;
            }

            CG_MASTER_KICK msg = (CG_MASTER_KICK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MASTER_KICK);
            msg.Kicked = kickedGuid;
            msg.SendPacket();
        }

        //请求激活技能
        public void ReqActiveMasterSkill(string skillname, int skillid)
        {
            //技能ID
            if (skillid < 0)
            {
                return;
            }

            //技能名称字数
            if (skillname.Length <= 0 || skillname.Length > GlobeVar.MAX_MASTER_SKILL_NAME)
            {
                return;
            }

            //掌门才能激活
            if (GameManager.gameManager.PlayerDataPool.IsMasterChief() == false)
            {
                return;
            }

            //激活师门技能需要师门薪火达到要求
            int SkillNum = GameManager.gameManager.PlayerDataPool.GetMasterSkillActiveNum();
            int MasterTorch = GameManager.gameManager.PlayerDataPool.MasterInfo.MasterTorch;
            Tab_MasterSkillLimit line = TableManager.GetMasterSkillLimitByID(SkillNum + 1, 0);
            if (line != null)
            {
                if (MasterTorch < line.MinMasterTorch)
                {
                    string str = StrDictionary.GetClientDictionaryString("#{3275}", line.MinMasterTorch, line.ActiveConsumeNum);
                    Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, str);
                    return;
                }
                if (MasterTorch < line.ActiveConsumeNum)
                {
                    string str = StrDictionary.GetClientDictionaryString("#{3275}", line.MinMasterTorch, line.ActiveConsumeNum);
                    Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, str);
                    return;
                }
            }

            CG_MASTER_ACTIVE_SKILL msg = (CG_MASTER_ACTIVE_SKILL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MASTER_ACTIVE_SKILL);
            msg.SkillId = skillid;
            msg.SkillName = skillname;
            msg.SendPacket();
        }

        //请求学习技能
        public void ReqLearnMasterSkill(int skillid)
        {
            //技能ID
            if (skillid < 0)
            {
                return;
            }

            //没有师门不能学习
            if (GameManager.gameManager.PlayerDataPool.MasterInfo.MasterGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            CG_MASTER_LEARN_SKILL msg = (CG_MASTER_LEARN_SKILL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MASTER_LEARN_SKILL);
            msg.SkillId = skillid;
            msg.SendPacket();
        }

        //请求遗忘技能
        public void ReqForgetMasterSkill(int skillid)
        {
            //技能ID
            if (skillid < 0)
            {
                return;
            }

            //没有师门不能遗忘
            if (GameManager.gameManager.PlayerDataPool.IsHaveMaster() == false)
            {
                return;
            }

            CG_MASTER_FORGET_SKILL msg = (CG_MASTER_FORGET_SKILL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MASTER_FORGET_SKILL);
            msg.SkillId = skillid;
            msg.SendPacket();
        }

        //请求修改公告
        public void ReqChangeNotice(string notice)
        {
            //检测公告长度
            if (notice.Length <= 0 || notice.Length > GlobeVar.MAX_MASTER_NOTICE)
            {
                return;
            }

            //只有掌门才能修改
            if (GameManager.gameManager.PlayerDataPool.IsMasterChief() == false)
            {
                return;
            }

            CG_MASTER_REQ_CHANGE_NOTICE msg = (CG_MASTER_REQ_CHANGE_NOTICE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MASTER_REQ_CHANGE_NOTICE);
            msg.MasterNotice = notice;
            msg.SendPacket();
            //把本地的也改掉
            GameManager.gameManager.PlayerDataPool.MasterInfo.MasterNotice = notice;
        }

        public void ReqUpdateTorch()
        {
            //请求更新薪火值
            CG_ASK_TORCH_VALUE packet = (CG_ASK_TORCH_VALUE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_TORCH_VALUE);
            packet.SetType(1);
            packet.SendPacket();
        }

        //师门技能是否可用
        public bool MasterSkillCanUse(int skillid)
        {
            Tab_MasterSkill line = TableManager.GetMasterSkillByID(skillid, 0);
            if (line == null)
            {
                return false;
            }
            int baseskill = line.BaseSkill;

            //有师门
            if (GameManager.gameManager.PlayerDataPool.IsHaveMaster())
            {
                //师门激活了这个技能
                Master masterinfo = GameManager.gameManager.PlayerDataPool.MasterInfo;
                foreach (KeyValuePair<int, string> skill in masterinfo.MasterSkillList)
                {
                    if (skill.Key > 0 && skill.Key == baseskill)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void GoToMasterMissionNPC()
        {
            //自动寻路
            int nTargetNpcID = Games.GlobeDefine.GlobeVar.MASTERMISSION_ACCEPTNPC_DATAID;
            Tab_SceneNpc tab = TableManager.GetSceneNpcByID(nTargetNpcID, 0);
            if (tab == null)
            {
                return;
            }

            AutoSearchPoint point = new AutoSearchPoint(tab.SceneID, tab.PosX, tab.PosZ);
            if (GameManager.gameManager && GameManager.gameManager.AutoSearch)
            {
                GameManager.gameManager.AutoSearch.BuildPath(point);
                Tab_RoleBaseAttr RoleBase = TableManager.GetRoleBaseAttrByID(tab.DataID, 0);
                if (null != RoleBase && null != GameManager.gameManager.AutoSearch.Path)
                {
                    GameManager.gameManager.AutoSearch.Path.AutoSearchTargetName = RoleBase.Name;
                }
            }
        }

        public void MasterRecruit()
        {
            if (null == GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr)
            {
                return;
            }
            string chatInfo = "#{5623}";
            if (GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr.Level >= 65)
            {
                chatInfo = "#{5622}";
            }

            CG_CHAT msg = (CG_CHAT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CHAT);
            msg.SetChatinfo(chatInfo);
            msg.SetChattype((int)CG_CHAT.CHATTYPE.CHAT_TYPE_MASTER);
            msg.SendPacket();
        }
    }

}
