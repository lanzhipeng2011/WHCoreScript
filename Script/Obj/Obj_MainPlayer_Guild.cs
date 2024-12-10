/********************************************************************************
 *	文件名：	Obj_MainPlayer_Guild.cs
 *	全路径：	\Script\Obj\Obj_MainPlayer_Guild.cs
 *	创建人：	李嘉
 *	创建时间：2014-04-22
 *
 *	功能说明：游戏主角Obj的帮会逻辑部分
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using Games.LogicObj;
using System;
using Games.GlobeDefine;
using GCGame.Table;

namespace Games.LogicObj
{
    public partial class Obj_MainPlayer : Obj_OtherPlayer
    {
        //玩家的帮会相关标记位，做到申请过一次之后可以保存结果一段时间，减少网络流量
        private bool m_bNeedRequestGuildInfo = true;       //是否需要请求个人帮会信息
        public bool NeedRequestGuildInfo
        {
            get { return m_bNeedRequestGuildInfo; }
            set { m_bNeedRequestGuildInfo = value; }
        }
        private bool m_bNeedRequestGuildList = true;       //是否需要请求全服帮会列表
        public bool NeedRequestGuildList
        {
            get { return m_bNeedRequestGuildList; }
            set { m_bNeedRequestGuildList = value; }
        }
        
        private bool m_bShowGuildNewReserveFlag = false;    //是否显示新审批成员标志
        public bool ShowGuildNewReserveFlag
        {
            get { return m_bShowGuildNewReserveFlag; }
            set { m_bShowGuildNewReserveFlag = value; }
        }
        //更新帮会相关标记位
        private const int c_GuildRequestCoolDown = 30;      //帮会信息更新间隔，包括帮会列表和帮会信息

        private UInt64 m_CacheChangeMasterGuid = GlobeVar.INVALID_GUID;     //缓存待禅让目标会员Guid
        private UInt64 m_CacheKickMemberGuid = GlobeVar.INVALID_GUID;       //缓存待踢出目标会员Guid
        
        //申请全服帮会列表
        public void ReqGuildList()
        {
            //if (null != GameManager.gameManager.PlayerDataPool.guildList)
            //{
            //    GameManager.gameManager.PlayerDataPool.guildList.CleanUp();
            //}

            CG_GUILD_REQ_LIST msg = (CG_GUILD_REQ_LIST)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUILD_REQ_LIST);
            msg.Requester = GUID;
            msg.SendPacket();

            m_bNeedRequestGuildList = false;
            StartCoroutine(ResetGuildListTime());
        }

        IEnumerator ResetGuildListTime()
        {
            yield return new WaitForSeconds(c_GuildRequestCoolDown);

            m_bNeedRequestGuildList = true;
        }

        //申请帮会信息
        public void ReqGuildInfo()
        {
            //if (null != GameManager.gameManager.PlayerDataPool.GuildInfo)
            //{
            //    GameManager.gameManager.PlayerDataPool.GuildInfo.CleanUp();
            //}

            CG_GUILD_REQ_INFO msg = (CG_GUILD_REQ_INFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUILD_REQ_INFO);
            msg.Requester = GUID;
            msg.SendPacket();

            m_bNeedRequestGuildInfo = false;
            StartCoroutine(ResetGuildInfoTime());
        }

        IEnumerator ResetGuildInfoTime()
        {
            yield return new WaitForSeconds(c_GuildRequestCoolDown);

            m_bNeedRequestGuildInfo = true;
        }

        //申请创建帮会
        public void ReqCreateGuild(string guildName)
        {
            //检测名字长度
            if (guildName.Length <= 0 | guildName.Length > GlobeVar.MAX_GUILD_NAME)
            {
                SendNoticMsg(false, "#{1761}");    //请输入帮会名称
                return;
            }

            //玩家等级判断
            if (BaseAttr.Level < GlobeVar.CREATE_GUILD_LEVEL)
            {
                SendNoticMsg(false, "#{1771}");    //你的人物等级不足40级，无法创建帮会
                return;
            }

            //有帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid != GlobeVar.INVALID_GUID)
            {
                SendNoticMsg(false, "#{1772}");        //你已属于一个帮会，不能创建帮会
                return;
            }

            CG_GUILD_CREATE msg = (CG_GUILD_CREATE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUILD_CREATE);
            msg.GuildName = guildName;
            msg.SendPacket();
        }

        //申请加入他人所在的帮会
        public void ReqJoinOtherPlayerGuild(UInt64 PlayerGuid, string strPlayerName)
        {
            //玩家Guid判断
            if (PlayerGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //玩家等级判断
            if (BaseAttr.Level < GlobeVar.JOIN_GUILD_LEVEL)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1780}");    //你的人物等级不足20级，无法加入帮会
                return;
            }

            CG_GUILD_JOIN_OTHERPLAYER msg = (CG_GUILD_JOIN_OTHERPLAYER)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUILD_JOIN_OTHERPLAYER);
            if (msg != null)
            {
                msg.UserGuid = PlayerGuid;
                msg.UserName = strPlayerName;
                msg.SendPacket();
            }
            

            //SendNoticMsg(false, "#{2340}");
        }


        //申请加入帮会
        public void ReqJoinGuild(UInt64 guildGuid)
        {
            //帮会Guid判断
            if (guildGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //玩家等级判断
            if (BaseAttr.Level < GlobeVar.JOIN_GUILD_LEVEL)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1780}");    //你的人物等级不足20级，无法创建帮会
                return;
            }
            
            CG_GUILD_JOIN msg = (CG_GUILD_JOIN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUILD_JOIN);
            msg.GuildGuid = guildGuid;
            msg.SendPacket( );

            if (GameManager.gameManager.PlayerDataPool.GuildInfo.PreserveGuildGuid == guildGuid)
            {
                SendNoticMsg(false, "#{5624}");
                return;
            }

            GameManager.gameManager.PlayerDataPool.GuildInfo.PreserveGuildGuid = guildGuid;
            SendNoticMsg(false, "#{2340}");
        }

        //邀请某个玩家加入帮会
        public void ReqInviteGuild(UInt64 invitedGuid)
        {
            //被邀请者判断
            if (invitedGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            CG_GUILD_INVITE msg = (CG_GUILD_INVITE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUILD_INVITE);
            msg.InvitedGuid = invitedGuid;
            msg.SendPacket();
        }

        //申请离开帮会
        public void ReqLeavGuild()
        {
            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //帮主离开为解散帮会，否则为帮众退出帮会
            if (GameManager.gameManager.PlayerDataPool.IsGuildChief())
            {
                //解散帮会操作不可撤销，确定执行吗？
                MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2359}"), "", MsgBoxLeaveGuildOK, null);
            }
            else
            {
                //你确认要退出{0}帮会吗？
                MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{1788}", GameManager.gameManager.PlayerDataPool.GuildInfo.GuildName),
                                                "", MsgBoxLeaveGuildOK, null);
            }
        }

        //离开帮会MessageBox确认
        private void MsgBoxLeaveGuildOK()
        {
            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            CG_GUILD_LEAVE msg = (CG_GUILD_LEAVE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUILD_LEAVE);
            msg.Requester = GUID;
            msg.SendPacket();
        }

        //任命会员职位
        public void ReqCommisionGuildMember(UInt64 approver)
        {
            //目前只有帮主可以执行，改变只有两种，副帮主->普通帮众 and 普通帮众->副帮主
            if (!GameManager.gameManager.PlayerDataPool.IsGuildChief())
            {
                return;
            }

            if (approver == GlobeVar.INVALID_GUID)
            {
                return;
            }

            GuildMember member;
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildMemberList.TryGetValue(approver, out member))
            {
                if (member.IsValid())
                {
                    if (member.Job == (int)GameDefine_Globe.GUILD_JOB.VICE_CHIEF)
                    {
                        ReqChangeGuildMemberJob(approver, (int)GameDefine_Globe.GUILD_JOB.MEMBER);
                    }
                    else if (member.Job == (int)GameDefine_Globe.GUILD_JOB.MEMBER)
                    {
                        ReqChangeGuildMemberJob(approver, (int)GameDefine_Globe.GUILD_JOB.VICE_CHIEF);
                    }
                }
            }
        }

        //修改会员权限
        private UInt64 m_approverGuid = GlobeVar.INVALID_GUID;
        private int m_jobID = GlobeVar.INVALID_ID;
        public void ReqChangeGuildMemberJob(UInt64 approver, int nJobID)
        {
            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //被修改者GUID判断
            if (approver == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //职位判断
            if (nJobID < 0 || nJobID >= (int)GameDefine_Globe.GUILD_JOB.MAX)
            {
                return;
            }

            m_approverGuid = approver;
            m_jobID = nJobID;
            //确定对该玩家进行任命？
            MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{3220}"), "", MsgBoxChangeGuildMemberJobOK, MsgBoxChangeGuildMemberJobCancel);
        }

        private void MsgBoxChangeGuildMemberJobOK()
        {
            //被修改者GUID判断
            if (m_approverGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //职位判断
            if (m_jobID < 0 || m_jobID >= (int)GameDefine_Globe.GUILD_JOB.MAX)
            {
                return;
            }

            CG_GUILD_JOB_CHANGE msg = (CG_GUILD_JOB_CHANGE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUILD_JOB_CHANGE);
            msg.Approver = m_approverGuid;
            msg.JobID = m_jobID;
            msg.SendPacket();
        }

        private void MsgBoxChangeGuildMemberJobCancel()
        {
            m_approverGuid = GlobeVar.INVALID_GUID;
            m_jobID = GlobeVar.INVALID_ID;
        }


        //批准待审批会员
        public void ReqApproveGuildMember(UInt64 approver, int agree)
        {
            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            CG_GUILD_APPROVE_RESERVE msg = (CG_GUILD_APPROVE_RESERVE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUILD_APPROVE_RESERVE);
            msg.Approver = approver;
            msg.IsAgree = agree;
            msg.SendPacket();
        }

        //踢出某个会员
        public void ReqKickGuildMember(UInt64 kickedGuid)
        {
            if (kickedGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            m_CacheKickMemberGuid = kickedGuid;
            //确定将该玩家从帮会中除名？
            MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2360}"), "", MsgBoxKickGuildMmeberOK, MsgBoxKickGuildMmeberCancel);
        }

        //帮会踢人MessageBox确认函数
        private void MsgBoxKickGuildMmeberOK()
        {
            if (m_CacheKickMemberGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }
            
            CG_GUILD_KICK msg = (CG_GUILD_KICK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUILD_KICK);
            msg.Kicked = m_CacheKickMemberGuid;
            msg.SendPacket();

            m_CacheKickMemberGuid = GlobeVar.INVALID_GUID;
        }

        //帮会踢人MessageBox取消函数
        private void MsgBoxKickGuildMmeberCancel()
        {
            m_CacheKickMemberGuid = GlobeVar.INVALID_GUID;
        }

        //禅让帮主
        public void ReqChangeGuildMaster(UInt64 approver)
        {
            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //必须是帮主
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildChiefGuid != GUID)
            {
                //无权进行此操作
                SendNoticMsg(false, "#{2513}");
                return;
            }

            //禅让和修改会员权限发同样消息包，只是JobID固定为GUILD_JOB.CHIEF
            if (approver == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //目标等级是否达到40
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GetMemberLevel(approver) < GlobeVar.CREATE_GUILD_LEVEL)
            {
                //禅让目标的等级不得低于40级。
                SendNoticMsg(false, "#{2362}");
                return;
            }

            m_CacheChangeMasterGuid = approver;
            //禅让帮主操作不可撤销，确定执行吗？
            MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2361}"), "", MsgBoxChangeGuildMasterOK, MsgBoxChangeGuildMasterCancel);
        }

        //帮会禅让MessageBox确认函数
        private void MsgBoxChangeGuildMasterOK()
        {
            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID ||
                m_CacheChangeMasterGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //必须是帮主
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildChiefGuid != GUID)
            {
                return;
            }

            //目标等级是否达到40
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GetMemberLevel(m_CacheChangeMasterGuid) < GlobeVar.CREATE_GUILD_LEVEL)
            {
                //禅让目标的等级不得低于40级。
                SendNoticMsg(false, "#{2362}");
                return;
            }
            
            CG_GUILD_JOB_CHANGE msg = (CG_GUILD_JOB_CHANGE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUILD_JOB_CHANGE);
            msg.Approver = m_CacheChangeMasterGuid;
            msg.JobID = (int)GameDefine_Globe.GUILD_JOB.CHIEF;
            msg.SendPacket();

            m_CacheChangeMasterGuid = GlobeVar.INVALID_GUID;
        }

        //帮会禅让MessageBox取消函数
        private void MsgBoxChangeGuildMasterCancel()
        {
            m_CacheChangeMasterGuid = GlobeVar.INVALID_GUID;
        }
        
        //帮会升级
        public void ReqGuildLevelUp(int nLevel)
        {
            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //必须是帮主
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildChiefGuid != GUID)
            {
                return;
            }

            CG_GUILD_REQ_LEVELUP msg = (CG_GUILD_REQ_LEVELUP)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUILD_REQ_LEVELUP);
            msg.Level = nLevel;
            msg.SendPacket();
        }

        //帮主分配跑商次数
        public void AssignGuildBusinessTimes() 
        {
            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID ||
                m_CacheChangeMasterGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //必须是帮主
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildChiefGuid != GUID)
            {
                return;
            }

            CG_ASSIGN_GUILDBUSINESS_TIME msg = (CG_ASSIGN_GUILDBUSINESS_TIME)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASSIGN_GUILDBUSINESS_TIME);
            msg.AssionTime = 1;
            msg.SendPacket();
        }


        //修改帮会公告
        public bool ReqChangeGuildNotice(string message)
        {
            //判断字符串的合法性
            if (message.Length <= 0)
            {
                return false;
            }

            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
				return false;
            }

            //目前非帮主和副帮主无法修改
            if (!GameManager.gameManager.PlayerDataPool.IsGuildChief() &&
                !GameManager.gameManager.PlayerDataPool.IsGuildViceChief(GUID))
            {
                //无权进行此操作
                SendNoticMsg(false, "#{2513}");
				return false;
            }

            CG_GUILD_REQ_CHANGE_NOTICE msg = (CG_GUILD_REQ_CHANGE_NOTICE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUILD_REQ_CHANGE_NOTICE);
            msg.GuildNotice = message;
            msg.SendPacket();
			return true;
        }

        public void ReqSendGuildMail(string message)
        {
            //判断字符串的合法性
            if (message.Length <= 0)
            {
                return;
            }

            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //目前非帮主和副帮主无法修改
            if (!GameManager.gameManager.PlayerDataPool.IsGuildChief() &&
                !GameManager.gameManager.PlayerDataPool.IsGuildViceChief(GUID))
            {
                //无权进行此操作
                SendNoticMsg(false, "#{3554}");
                return;
            }

            if (GameManager.gameManager.PlayerDataPool.Money.GetMoney_Coin() < 1000)
            {
                //无权进行此操作
                SendNoticMsg(false, "#{3552}");
                return;
            }

            CG_SEND_GUILDMAIL packetSendMail = (CG_SEND_GUILDMAIL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SEND_GUILDMAIL);
            packetSendMail.SetGuildMail(message);
            packetSendMail.SendPacket();
        }

        //获取帮会跑商信息

        //申请帮会信息
        private bool m_bNeedReqGuildBusinessInfo = true;
        public bool NeedReqGuildBusinessInfo
        {
            get { return m_bNeedReqGuildBusinessInfo; }
            set { m_bNeedReqGuildBusinessInfo = value; }
        }
        public void ReqGuildBusinessInfo()
        {
            // 无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            CG_ASK_GUILDBUSINESSINFO msg = (CG_ASK_GUILDBUSINESSINFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_GUILDBUSINESSINFO);
            msg.AskInfo = 1;
            msg.SendPacket();

            m_bNeedRequestGuildInfo = false;
            StartCoroutine(ResetGuildBusinessInfoTime());
        }

        IEnumerator ResetGuildBusinessInfoTime()
        {
            yield return new WaitForSeconds(c_GuildRequestCoolDown);

            m_bNeedRequestGuildInfo = true;
        }

        //设置自动分配帮会跑商
        public void SetAutoAssign()
        {
            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //目前非帮主
            if (!GameManager.gameManager.PlayerDataPool.IsGuildChief())
            {
                //无权进行此操作
                SendNoticMsg(false, "#{2513}");
                return;
            }

            CG_ASK_SETAUTOASSIGNPSTIMES msg = (CG_ASK_SETAUTOASSIGNPSTIMES)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_SETAUTOASSIGNPSTIMES);
            msg.SetIsAutoAssign(0);
            msg.SendPacket();
            
        }

        //帮主分配跑商
        public void AssignGuildBusiness()
        {
            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //目前非帮主
            if (!GameManager.gameManager.PlayerDataPool.IsGuildChief())
            {
                //无权进行此操作
                SendNoticMsg(false, "#{2513}");
                return;
            }

            CG_ASSIGN_GUILDBUSINESS_TIME msg = (CG_ASSIGN_GUILDBUSINESS_TIME)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASSIGN_GUILDBUSINESS_TIME);
            msg.SetAssionTime(1);
            msg.SendPacket();

        }

        //接受帮会任务
        public void DoAcceptGuildBusiness(int nOptIndex)
        {
            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                //无权进行此操作
                SendNoticMsg(false, "#{3921}");
                return;
            }

            bool flag = true;
            if (GameManager.gameManager.MissionManager.IsHaveMission(GlobeVar.GUILDBUSINESS_MISSIONID_H) || GameManager.gameManager.MissionManager.IsHaveMission(GlobeVar.GUILDBUSINESS_MISSIONID_L))
            {
                flag = false;
            }
            if (!flag)
            {
                SendNoticMsg(false, "#{3926}");
                return;
            }

            Tab_GuildBusiness tab = TableManager.GetGuildBusinessByID(GameManager.gameManager.PlayerDataPool.GuildInfo.GuildLevel, 0);
            if (null == tab )
            {
                return;
            }
            int useTime = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_GUILDBUSINESS_GOTTEN_NUM);
            int curTime = tab.MemTimes - useTime;
            if (curTime <= 0)
            {
                //跑商次数不够
                SendNoticMsg(false, "#{3935}");
                return;
            }

            if ( IsGBCanAccept())
            {
                CG_ASK_GUILDBUSINESS_ACCEPT msg = (CG_ASK_GUILDBUSINESS_ACCEPT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_GUILDBUSINESS_ACCEPT);
                msg.SetOptIndex(nOptIndex);
                msg.SendPacket();
                return;
            }
            
        }

        public void GoToGBNPC() {
            //自动寻路
            int nTargetNpcID = Games.GlobeDefine.GlobeVar.GUILDBUSINESS_ACCEPTNPC;
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

        public void GuildMakeAction(int makeItemID)
        {
            //无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                SendNoticMsg(false, "#{3955}");
                return;
            }

            //目前非帮主
            if (!GameManager.gameManager.PlayerDataPool.IsGuildChief())
            {
                //无权进行此操作
                SendNoticMsg(false, "#{3955}");
                return;
            }

            Tab_GuildMake tab = TableManager.GetGuildMakeByID(makeItemID, 0);
            if(null == tab)
            {
                return;
            }

            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildWeath < tab.MadeCost)
            {
                //财富值不足
                SendNoticMsg(false, "#{3956}");
                return;
            }

            CG_ASK_GUILD_MAKE msg = (CG_ASK_GUILD_MAKE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_GUILD_MAKE);
            msg.SetMakeID(makeItemID);
            msg.SendPacket();
        }

        // 帮会任务相关
        private bool m_bNeedReqGuildMissionInfo = true;
        public bool NeedReqGuildMissionInfo
        {
            get { return m_bNeedReqGuildMissionInfo; }
            set { m_bNeedReqGuildMissionInfo = value; }
        }
        
        IEnumerator ResetGuildMissionInfoTime()
        {
            yield return new WaitForSeconds(c_GuildRequestCoolDown);

            m_bNeedReqGuildMissionInfo = true;
        }
        public void ReqGuildMissionInfo()
        {
            // 无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            CG_ASK_GUILDMISSIONINFO msg = (CG_ASK_GUILDMISSIONINFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_GUILDMISSIONINFO);
            msg.SetAskInfo(1);
            msg.SendPacket();

            NeedReqGuildMissionInfo = false;
            StartCoroutine(ResetGuildMissionInfoTime());
        }

        // 分配帮会任务
        public void AssignGuildMission()
        {
            // 无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                SendNoticMsg(false, "#{5436}");
                return;
            }

            // 目前非帮主
            if (!GameManager.gameManager.PlayerDataPool.IsGuildChief())
            {
                // 只有帮主能够发布任务
                SendNoticMsg(false, "#{5430}");
                return;
            }

            if (!GameManager.gameManager.PlayerDataPool.GuildInfo.CanGMAssign)
            {
                // 没有可分配次数
                SendNoticMsg(false, "#{5431}");
                return;
            }

            CG_ASSIGN_GUILDMISSION msg = (CG_ASSIGN_GUILDMISSION)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASSIGN_GUILDMISSION);
            msg.SetAskAssign(1);
            msg.SendPacket();
        }


        // 分配帮会任务
        public void PartakeGuildMission()
        {
            // 无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                SendNoticMsg(false, "#{5436}");
                return;
            }
            
            Tab_GuildMissionGuild tab = TableManager.GetGuildMissionGuildByID(GameManager.gameManager.PlayerDataPool.GuildInfo.GuildLevel, 0);
            if (null == tab) { return; }

            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GMCanPartakeType >= tab.MemMaxTimesOneDay)
            {
                // 每日可参与次数用完
                SendNoticMsg(false, "#{5437}");
                return;
            }

            if (GameManager.gameManager.PlayerDataPool.GuildInfo.CMMisionID > 0)
            {
                // 已经参与帮会任务
                SendNoticMsg(false, "#{5473}");
                return;
            }

            if (false == GameManager.gameManager.PlayerDataPool.GuildInfo.GuildLeftMissionTime)
            {
                // 没有可参与次数
                SendNoticMsg(false, "#{5438}");
                return;
            }

            CG_PARTAKE_GUILDMISSION msg = (CG_PARTAKE_GUILDMISSION)PacketDistributed.CreatePacket(MessageID.PACKET_CG_PARTAKE_GUILDMISSION);
            msg.SetAskPartake(1);
            msg.SendPacket();
        }

        public void AcceptGuildMission()
        {
            // 无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                SendNoticMsg(false, "#{5436}");
                return;
            }

            if (GameManager.gameManager.PlayerDataPool.GuildInfo.CMMisionID == 0)
            {
                return;
            }

            GameManager.gameManager.MissionManager.AcceptMission(GameManager.gameManager.PlayerDataPool.GuildInfo.CMMisionID);

        }

        public void CompleteGuildMission()
        {
            // 无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                SendNoticMsg(false, "#{5436}");
                return;
            }

            if (GameManager.gameManager.PlayerDataPool.GuildInfo.CMMisionID == 0)
            {
                return;
            }

            GameManager.gameManager.MissionManager.CompleteMission(GameManager.gameManager.PlayerDataPool.GuildInfo.CMMisionID);



        }

        public void AbandonGuildMission()
        {
            // 无帮会无法申请
            if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
            {
                SendNoticMsg(false, "#{5436}");
                return;
            }

            if (GameManager.gameManager.PlayerDataPool.GuildInfo.CMMisionID == 0)
            {
                return;
            }

            GameManager.gameManager.MissionManager.AbandonMission(GameManager.gameManager.PlayerDataPool.GuildInfo.CMMisionID);

        }
    }


}
