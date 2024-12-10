using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Games.LogicObj;
using System;
using GCGame.Table;
using Module.Log;

public class RelationButtionWindowLogic : MonoBehaviour 
{

    //按钮
    public UIImageButton m_LeaderImgBtn;            // 任命为队长按钮
    public UIImageButton m_KickImgBtn;              // 请离队伍按钮
    public UIImageButton m_LeaveImgBtn;             // 离开队伍按钮
    public UIImageButton m_InviteTeamImgBtn;        // 邀请入队按钮
    public UIImageButton m_JoinTeamImgBtn;          // 申请入队按钮
    public UIImageButton m_FollowTeamImgBtn;        // 组队跟随按钮
    public UIImageButton m_DismissTeamImgBtn;       // 解散队伍按钮
    public UIImageButton m_AddFriendImgBtn;         // 加为好友按钮

    public UIImageButton m_RelationChatBtn;         // 与关系人聊天按钮
    public UIImageButton m_RelationViewBtn;         // 查看关系人信息按钮
    public UIImageButton m_RelationTeamBtn;         // 与关系人组队按钮
    public UIImageButton m_RelationDelBtn;          // 删除关系人按钮

    public UIImageButton m_BlackDelBtn;             // 删除黑名单按钮
    public UIImageButton m_PlayerTrail;             // 追踪按钮
    public UIImageButton m_TrailDel;                // 仇人删除
    
    public UILabel m_RelationDelLabel;              // 删除关系人按钮文字

    public UIGrid m_ButtonGrid;                     // 按钮Grid，进行横向排序使用

    //其他非界面相关数据
    public int m_nTeamID = GlobeVar.INVALID_ID;     //队伍ID，只在附近队伍列表中生效
    public UInt64 m_Guid = GlobeVar.INVALID_GUID;
    public string m_PlayerName = "";
    private PlayerListItemLogic.PlayerListItemType m_Type;

    //隐藏所有控件
    void HideAllUI()
    {
        m_LeaderImgBtn.gameObject.SetActive(false);
        m_KickImgBtn.gameObject.SetActive(false);
        m_LeaveImgBtn.gameObject.SetActive(false);
        m_InviteTeamImgBtn.gameObject.SetActive(false);
        m_JoinTeamImgBtn.gameObject.SetActive(false);
        m_DismissTeamImgBtn.gameObject.SetActive(false);
        m_FollowTeamImgBtn.gameObject.SetActive(false);
        m_RelationChatBtn.gameObject.SetActive(false);
        m_RelationViewBtn.gameObject.SetActive(false);
        m_RelationTeamBtn.gameObject.SetActive(false);
        m_RelationDelBtn.gameObject.SetActive(false);
        m_AddFriendImgBtn.gameObject.SetActive(false);
        m_BlackDelBtn.gameObject.SetActive(false);
        m_PlayerTrail.gameObject.SetActive(false);
        m_TrailDel.gameObject.SetActive(false);
    }

    //点击任命为队长按钮
    void OnClickLeaderBtn()
    {
        //如果自己不是队长，则返回
        if (false == Singleton<ObjManager>.GetInstance().MainPlayer.IsTeamLeader())
        {
            return;
        }

        //guid检测
        if (GlobeVar.INVALID_GUID == m_Guid ||
            Singleton<ObjManager>.GetInstance().MainPlayer.GUID == m_Guid)
        {
            return;
        }

        //发送任命队长消息
        Singleton<ObjManager>.GetInstance().MainPlayer.ReqChangeTeamLeader(m_Guid);
    }

    //组队跟随按钮
    void OnClickFollowLeaderBtn()
    {
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.EnterTeamFollow();
        }
    }

    //点击请离队伍按钮
    void OnClickKickBtn()
    {
        //如果自己不是队长，则返回
        if (false == Singleton<ObjManager>.GetInstance().MainPlayer.IsTeamLeader())
        {
            return;
        }

        //guid检测
        if (GlobeVar.INVALID_GUID == m_Guid)
        {
            return;
        }

        //发送踢人消息
        Singleton<ObjManager>.GetInstance().MainPlayer.ReqKickTeamMember(m_Guid);
    }

    void OnClickLeaveBtn()
    {
        if (GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID != GlobeVar.INVALID_ID)
        {
            if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.ReqLeaveTeam();
            }
        }
    }

    void OnClickDismissTeamBtn()
    {
        //非队长无法解散队伍
        if (false == Singleton<ObjManager>.GetInstance().MainPlayer.IsTeamLeader())
        {
            return;
        }
        
        //队长自己踢自己，服务器则认为是解散队伍
        Singleton<ObjManager>.GetInstance().MainPlayer.ReqKickTeamMember(Singleton<ObjManager>.GetInstance().MainPlayer.GUID);
    }

    //点击邀请入队按钮
    void OnClickInviteBtn()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //如果有队伍，则判断下队伍是否已满
        if (GlobeVar.INVALID_ID != GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID)
        {
            if (GameManager.gameManager.PlayerDataPool.TeamInfo.IsFull())
            {
				// 队伍满tips
				Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1145}");
                return;
            }
        }

        if (GlobeVar.INVALID_GUID != m_Guid)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqInviteTeam(m_Guid);
        }
    }

    //点击申请入队按钮
    void OnClickJoinBtn()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }
        /*ReqJoinTeam会检查
        if (GlobeVar.INVALID_ID != GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID)
        {
            return;
        }*/
		if (GameManager.gameManager.PlayerDataPool.IsHaveTeam ()) 
		{
			MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{4605}"), "", OnClickLeaveBtn);
		}

        if (GlobeVar.INVALID_GUID != m_Guid)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqJoinTeam(m_Guid);
        }
    }

    //点击与关系人私聊
    
    void OnClickRelationChat()
    {
        if (GlobeVar.INVALID_GUID != m_Guid)
        {
            //发起私聊
            //未打开过则创建
            if (null == ChatInfoLogic.Instance())
            {
                UIManager.ShowUI(UIInfo.ChatInfoRoot, ShowChatInfoRootOver, m_PlayerName);
            }
        }
    }

    void ShowChatInfoRootOver(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            //发起私聊
            string PlayerName = param as string;
            if (null != ChatInfoLogic.Instance())
            {
                ChatInfoLogic.Instance().BeginChat(m_Guid, PlayerName);
            }
        }
    }

    //点击查看关系人信息
    void OnClickRelationView()
    {
        //
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }
        if (GlobeVar.INVALID_GUID != m_Guid)
        {
			if(null != this.GetComponentInParent<RelationFriendWindow>())
			{
				Singleton<ObjManager>.GetInstance().MainPlayer.ReqViewOtherPlayer(m_Guid, OtherRoleViewLogic.OPEN_TYPE.OPEN_TYPE_FRIEND);
			}
			else if(null != this.GetComponentInParent<RelationTeamWindow>())
			{
				Singleton<ObjManager>.GetInstance().MainPlayer.ReqViewOtherPlayer(m_Guid, OtherRoleViewLogic.OPEN_TYPE.OPEN_TYPE_TEAM);
			}
        }
    }

    //点击与关系人组队
    void OnClickRelationTeam()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //如果有队伍，则判断下队伍是否已满
        if (GlobeVar.INVALID_ID != GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID)
        {
            if (GameManager.gameManager.PlayerDataPool.TeamInfo.IsFull())
            {
                return;
            }
        }

        if (GlobeVar.INVALID_GUID != m_Guid)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqInviteTeam(m_Guid);
        }
    }

    //点击删除关系人
    void OnClickRelationDel()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        if (GlobeVar.INVALID_GUID != m_Guid)
        {
            //如果是好友，则申请删除好友
            if (GameManager.gameManager.PlayerDataPool.FriendList.IsExist(m_Guid))
            {
				MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{5251}"), "", RelationDel);
                //Singleton<ObjManager>.GetInstance().MainPlayer.ReqDelFriend(m_Guid);
                return;
            }

            ////如果是黑名单，则删除黑名单
            //if (GameManager.gameManager.PlayerDataPool.BlackList.IsExist(m_Guid))
            //{
            //    Singleton<ObjManager>.GetInstance().MainPlayer.ReqDelBlack(m_Guid);
            //    return;
            //}
        }
    }

	void RelationDel()
	{
		Singleton<ObjManager>.GetInstance ().MainPlayer.ReqDelFriend (m_Guid);
	}

    public void OnClickBlackDel()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        if (GlobeVar.INVALID_GUID != m_Guid)
        {
            ////如果是好友，则申请删除好友
            //if (GameManager.gameManager.PlayerDataPool.FriendList.IsExist(m_Guid))
            //{
            //    Singleton<ObjManager>.GetInstance().MainPlayer.ReqDelFriend(m_Guid);
            //    return;
            //}

            //如果是黑名单，则删除黑名单
            if (GameManager.gameManager.PlayerDataPool.BlackList.IsExist(m_Guid))
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.ReqDelBlack(m_Guid);
                return;
            }
        }
    }

    void OnClickHateDel()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        if (GlobeVar.INVALID_GUID != m_Guid)
        {
            if (GameManager.gameManager.PlayerDataPool.HateList.IsExist(m_Guid))
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.ReqDelHate(m_Guid);
                return;
            }
        }
    }

    public void OnClickAddFriendBtn()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }
        if (GlobeVar.INVALID_GUID != m_Guid)
        {
            //如果不是好友也不是黑名单，则添加好友
            if (false == GameManager.gameManager.PlayerDataPool.FriendList.IsExist(m_Guid))
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.ReqAddFriend(m_Guid);
                return;
            }
            else
            {
                //该玩家已经是您的好友
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(true, "#{2394}");
            }
        }
    }

    public void SetPlayerListItemInfo(UInt64 guid, int nTeamID, string PlayerName, PlayerListItemLogic.PlayerListItemType showButtonType)
    {
        HideAllUI();
        m_nTeamID = GlobeVar.INVALID_ID;
        m_Type = PlayerListItemLogic.PlayerListItemType.Invalid;      
        if (guid == GlobeVar.INVALID_GUID)
        {           
            return;
        }
        SetName(PlayerName);
        // 设置GUID
        m_Guid = guid;
        m_Type = showButtonType;
       
        switch (showButtonType)
        {
            case PlayerListItemLogic.PlayerListItemType.TeamMemberInfo:
                {
                    UpdateTeamMemberInfo();
                }
                break;
            case PlayerListItemLogic.PlayerListItemType.NearByPlayer:
                {
                    UpdateNearPlayerInfo();
                }
                break;
            case PlayerListItemLogic.PlayerListItemType.NearByTeam:
                {
                    UpdateNearTeamInfo(nTeamID);
                }
                break;
            case PlayerListItemLogic.PlayerListItemType.FriendInfo:
                {
                    UpdateFriendInfo();
                }
                break;
            case PlayerListItemLogic.PlayerListItemType.BlackPlayerInfo:
                {
                    UpdateBlackInfo();
                }
                break;
            case PlayerListItemLogic.PlayerListItemType.HatePlayerInfo:
                {
                    UpdateHateInfo();
                }
                break;
            default:
                {
                    HideAllUI();
                }
                break;
        }
    }
    //显示本队队员信息
    public void UpdateTeamMemberInfo(/*TeamMember member*/)
    {
        //显示按钮
        if (true == Singleton<ObjManager>.GetInstance().MainPlayer.IsTeamLeader())
        {
            if (m_Guid != Singleton<ObjManager>.GetInstance().MainPlayer.GUID)
            {
                //如果是队长，并且显示的不是自己，则显示踢人和提升为队长按钮
                m_LeaderImgBtn.gameObject.SetActive(true);
                m_KickImgBtn.gameObject.SetActive(true);
            }
            else
            {
                //自己显示解散队伍按钮
                m_DismissTeamImgBtn.gameObject.SetActive(true);
            }
        }

        //如果不是队长，则在队长上显示组队跟随
        if (false == Singleton<ObjManager>.GetInstance().MainPlayer.IsTeamLeader() &&
            true == Singleton<ObjManager>.GetInstance().MainPlayer.IsTeamLeader(m_Guid))
        {
            m_FollowTeamImgBtn.gameObject.SetActive(true);
        }

        //如果是自己
        if (m_Guid == Singleton<ObjManager>.GetInstance().MainPlayer.GUID)
        {
            m_LeaveImgBtn.gameObject.SetActive(true);
        }
        else
        {
            //显示加为好友按钮
            m_AddFriendImgBtn.gameObject.SetActive(true);
            //显示查看信息按钮
            m_RelationViewBtn.gameObject.SetActive(true);
        }
        m_ButtonGrid.Reposition();
    }

    //显示附近玩家信息
    public void UpdateNearPlayerInfo(/*NearbyPlayer info*/)
    {
        HideAllUI();       
       //显示按钮
        m_InviteTeamImgBtn.gameObject.SetActive(true);
        m_ButtonGrid.Reposition();       
    }

    //显示附近队伍信息
    public void UpdateNearTeamInfo(/*NearbyTeam info*/int nTeamID)
    {
          
        //队伍ID
        m_nTeamID = nTeamID;
        //显示按钮
        m_JoinTeamImgBtn.gameObject.SetActive(true);

        m_ButtonGrid.Reposition();       
    }

    //显示好友
    public void UpdateFriendInfo(/*Relation relation*/)
    {
        
        m_RelationChatBtn.gameObject.SetActive(true);
        m_RelationViewBtn.gameObject.SetActive(true);
        m_RelationTeamBtn.gameObject.SetActive(true);
        m_RelationDelBtn.gameObject.SetActive(true);
        //m_RelationDelLabel.text = StrDictionary.GetClientDictionaryString("#{2355}");
        m_ButtonGrid.Reposition();
        
    }

    //显示黑名单
    public void UpdateBlackInfo(/*Relation relation*/)
    {
        m_BlackDelBtn.gameObject.SetActive(true);        
        //m_RelationDelLabel.text = StrDictionary.GetClientDictionaryString("#{2356}");
        m_ButtonGrid.Reposition();       
    }

    void UpdateHateInfo()
    {
        m_PlayerTrail.gameObject.SetActive(true);
        m_TrailDel.gameObject.SetActive(true);
        m_ButtonGrid.Reposition();       
    }

    void SetName(string szName)
    {
        // 记录名字
        m_PlayerName = szName;
    }

    void OnClickPlayerTrail()
    {
        if (m_Guid == GlobeVar.INVALID_GUID)
        {
            return;
        }
        if (m_Guid == Singleton<ObjManager>.Instance.MainPlayer.GUID)
        {
            return;
        }

        Singleton<ObjManager>.Instance.MainPlayer.ReqTrailPlayer(m_Guid);
    }

    void OnClickTrailDel()
    {
        if (m_Guid == GlobeVar.INVALID_GUID)
        {
            return;
        }
        if (m_Guid == Singleton<ObjManager>.Instance.MainPlayer.GUID)
        {
            return;
        }

        CG_DELHATELIST msg = (CG_DELHATELIST)PacketDistributed.CreatePacket(MessageID.PACKET_CG_DELHATELIST);
        msg.Guid = m_Guid;
        msg.SendPacket();
    }
}
