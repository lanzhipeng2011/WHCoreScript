//********************************************************************
// 文件名: PopMenuLogic.cs
// 描述: 点击菜单脚本
// 作者: WangZhe
//********************************************************************

using UnityEngine;
using System.Collections;
using GCGame;
using System;
using Games.GlobeDefine;
using GCGame.Table;
using System.Collections.Generic;

public class PopMenuLogic : MonoBehaviour {

    private static PopMenuLogic m_Instance;
    public static PopMenuLogic Instance()
    {
        return m_Instance;
    }

    public GameObject m_PopMenuOffset;              // 菜单位移
    public GameObject m_MenuItemGrid;                // 菜单项格子
    private int m_MenuItemsNum;                           // 菜单项数量
    private UInt64 m_PopMenuSelectGuid;             //弹出菜单的目标GUID，菜单弹出时赋值，这样可以在一些通用接口中直接调用
    private string m_PopMenuSelectName;             //弹出菜单的目标姓名，菜单弹出时赋值，这样可以在一些通用接口中直接调用

    private GameObject m_resMenuItem = null;

    static bool m_bInGuildPop = false;

    void Awake()
    {
        m_Instance = this;
        m_PopMenuSelectGuid = GlobeVar.INVALID_GUID;
        m_PopMenuSelectName = "";
    }
	// Use this for initialization
	void Start () {
        m_MenuItemsNum = 0;
	}


    void OnDestroy()
    {
        m_Instance = null;
    }

    /// <summary>
    /// 显示菜单
    /// </summary>
    /// <param name="strMenuName">菜单名 主要用于区分不同菜单的定制</param>
    /// <param name="vecPos">菜单位置</param>
    public static void ShowMenu(string strMenuName, GameObject destGameObject)
    {
        List<object> initParams = new List<object>();
        initParams.Add(strMenuName);
        initParams.Add(destGameObject);
        UIManager.ShowUI(UIInfo.PopMenuRoot, PopMenuLogic.ShowUIOver, initParams);
        if (strMenuName == "GuildMemberPopMenu")
        {
            m_bInGuildPop = true;
        }
        else
        {
            m_bInGuildPop = false;
        }
    }

    public static void ClosePop()
    {
        if (m_bInGuildPop)
        {
            UIManager.CloseUI(UIInfo.PopMenuRoot);
        }
    }

    static void ShowUIOver(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            List<object> initParams = param as List<object>;
            if (PopMenuLogic.Instance() != null && initParams != null)
            {
                PopMenuLogic.Instance().ShowPopMenu((string)initParams[0], (GameObject)initParams[1]);
            }
        }
    }

    void ShowPopMenu(string strMenuName, GameObject destGameObject)
    {
        m_PopMenuOffset.transform.localPosition = transform.parent.InverseTransformPoint(destGameObject.transform.position);

        List<object> initParams = new List<object>();
        initParams.Add(strMenuName);
        UIManager.LoadItem(UIInfo.PopMenuItem, LoadItemOver, initParams); 
    }

    void LoadItemOver(GameObject resObj, object param)
    {
        if (resObj == null)
        {
            return;
        }

        m_resMenuItem = resObj;

        List<object> initParams = param as List<object>;
        if (initParams == null)
        {
            return;
        }

        string strMenuName = (string)initParams[0];

        if (strMenuName == "TargetFramePopMenu")
        {
            ShowTargetFramePopMenu();
        }
        if (strMenuName == "GuildMemberPopMenu")
        {
            ShowGuildMemberPopMenu();
        }

        if (m_MenuItemsNum > 0)
        {
            m_MenuItemGrid.GetComponent<UIGrid>().repositionNow = true;
        }
    }

    /// <summary>
    /// 增加菜单项
    /// </summary>
    /// <param name="ItemId">菜单项ID 主要用于区分命名GameObject 目前无其他作用</param>
    /// <param name="strLabel">菜单项文字</param>
    /// <param name="funcItemOnClicked">菜单项响应函数</param>
    void AddMenuItem(int ItemId, string strLabel, PopMenuItemLogic.MenuItemOnClicked funcItemOnClicked)
    {
        if (null == m_resMenuItem)
            return;

        GameObject newMenuItem = Utils.BindObjToParent(m_resMenuItem, m_MenuItemGrid, ItemId.ToString());
        if (null != newMenuItem && null != newMenuItem.GetComponent<PopMenuItemLogic>())
        {
            newMenuItem.GetComponent<PopMenuItemLogic>().InitMenuItem(strLabel, funcItemOnClicked);
            m_MenuItemsNum++;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    //PopMenu下的一些通用方法，只要在弹出PopMenu的时候记录了弹出目标的Guid与Name即可
    //////////////////////////////////////////////////////////////////////////
    //加好友
    void PopMenuAddFriend()
    {
        //如果非玩家，则不显示
        if (GlobeVar.INVALID_GUID == m_PopMenuSelectGuid)
        {
            return;
        }

        //如果目标是自己也不发送加好友
        if (Singleton<ObjManager>.GetInstance().MainPlayer.GUID == m_PopMenuSelectGuid)
        {
            return;
        }

        CG_ADDFRIEND msg = (CG_ADDFRIEND)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ADDFRIEND);
        msg.Guid = m_PopMenuSelectGuid;
        msg.SendPacket();

        // Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2906}");
    }

    //私聊
    void PopMenuChat()
    {
        //如果非玩家，则无效
        if (GlobeVar.INVALID_GUID == m_PopMenuSelectGuid)
        {
            return;
        }

        //如果目标是自己也不发送加好友
        if (Singleton<ObjManager>.GetInstance().MainPlayer.GUID == m_PopMenuSelectGuid)
        {
            return;
        }

        //未打开过则创建
        if (null == ChatInfoLogic.Instance())
        {
			PlayerFrameLogic.Instance ().SwitchAllWhenPopUIShow (false);//关闭其它主界面
            UIManager.ShowUI(UIInfo.ChatInfoRoot, ShowChatInfoRootOver);
        }
    }

    void ShowChatInfoRootOver(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            //发起私聊
            if (null != ChatInfoLogic.Instance())
            {
                ChatInfoLogic.Instance().BeginChat(m_PopMenuSelectGuid, m_PopMenuSelectName);
            }
        }
    }

    //邀请入队
    void PopMenuInviteTeam()
    {
        //如果非玩家，则无效
        if (GlobeVar.INVALID_GUID == m_PopMenuSelectGuid)
        {
            return;
        }

        //如果目标是自己也不发送加好友
        if (Singleton<ObjManager>.GetInstance().MainPlayer.GUID == m_PopMenuSelectGuid)
        {
            return;
        }

        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //如果有队伍，则判断下队伍是否已满
        if (GlobeVar.INVALID_ID != GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID)
        {
            if (GameManager.gameManager.PlayerDataPool.TeamInfo.IsFull())
            {
				//==========
				Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1145}");
                return;
            }
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.ReqInviteTeam(m_PopMenuSelectGuid);
    }

    //申请加队
    void PopMeunJoinTeam()
    {
        //如果非玩家，则则无效
        if (GlobeVar.INVALID_GUID == m_PopMenuSelectGuid)
        {
            return;
        }

        //如果目标是自己也不发送加好友
        if (Singleton<ObjManager>.GetInstance().MainPlayer.GUID == m_PopMenuSelectGuid)
        {
            return;
        }

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
			MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{4605}"), "", TeamLeave);
		}

        Singleton<ObjManager>.GetInstance().MainPlayer.ReqJoinTeam(m_PopMenuSelectGuid);
    }

	void TeamLeave()
	{
		if (GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID != GlobeVar.INVALID_ID)
		{
			if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
			{
				Singleton<ObjManager>.GetInstance().MainPlayer.ReqLeaveTeam();
			}
		}
	}

    //切磋
    void PopMenuDuel()
    {
        //如果非玩家，则无效
        if (GlobeVar.INVALID_GUID == m_PopMenuSelectGuid)
        {
            return;
        }
        //如果目标是自己也不发送
        if (Singleton<ObjManager>.GetInstance().MainPlayer.GUID == m_PopMenuSelectGuid)
        {
            return;
        }

        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.ReqDuel(m_PopMenuSelectGuid);
    }

    //查看属性
    void PopMenuView()
    {
        //如果非玩家，则无效
        if (GlobeVar.INVALID_GUID == m_PopMenuSelectGuid)
        {
            return;
        }
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //如果目标是自己也不发送
        if (Singleton<ObjManager>.GetInstance().MainPlayer.GUID == m_PopMenuSelectGuid)
        {
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.ReqViewOtherPlayer(m_PopMenuSelectGuid, OtherRoleViewLogic.OPEN_TYPE.OPEN_TYPE_POPMENU);
    }
    //查看寄售行信息
    void PopMenuConsignSaleInfo()
    {
		PlayerFrameLogic.Instance ().SwitchAllWhenPopUIShow (false);//关闭其它主界面
        UIManager.ShowUI(UIInfo.ConsignSaleRoot, BuyItemOpenConsignSale);
    }

    //邀请入帮
    void PopMenuInviteGuild()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        if (GlobeVar.INVALID_GUID == GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid)
        {
            //你当前未加入帮会，无法邀请。
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2686}");
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.ReqInviteGuild(m_PopMenuSelectGuid);
    }

    //申请加帮派
    void PopMeunJoinGuild()
    {
        //如果非玩家，则则无效
        if (GlobeVar.INVALID_GUID == m_PopMenuSelectGuid)
        {
            return;
        }

        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //如果目标是自己也不发送加帮派
        if (Singleton<ObjManager>.GetInstance().MainPlayer.GUID == m_PopMenuSelectGuid)
        {
            return;
        }

        if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid != GlobeVar.INVALID_GUID)
        {
            GuildMember mySelfGuildInfo = GameManager.gameManager.PlayerDataPool.GuildInfo.GetMainPlayerGuildInfo();
            if (null != mySelfGuildInfo)
            {
                if (mySelfGuildInfo.Job == (int)GameDefine_Globe.GUILD_JOB.RESERVE)
                {
                    //只能同时申请一个帮会，将替换原来的请求，是否继续？
                    string dicStr = StrDictionary.GetClientDictionaryString("#{1861}");
                    MessageBoxLogic.OpenOKCancelBox(dicStr, "", AgreeChangeJoinGuildRequest, null);
                    return;
                }
            }
            //你当前已经有帮会，无法申请。
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3094}");
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.ReqJoinOtherPlayerGuild(m_PopMenuSelectGuid, m_PopMenuSelectName);
    }

    void AgreeChangeJoinGuildRequest()
    {
        if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqJoinOtherPlayerGuild(m_PopMenuSelectGuid, m_PopMenuSelectName);
        }
    }

    void BuyItemOpenConsignSale(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            if (ConsignSaleLogic.Instance() != null)
            {
                ConsignSaleLogic.Instance().SearchForAskBuy(m_PopMenuSelectName);
            }
        }
    }
 
    //////////////////////////////////////////////////////////////////////////
    //目标头像PopMenu点击相关函数以及其专属菜单项函数
    //////////////////////////////////////////////////////////////////////////
    void ShowTargetFramePopMenu()
    {
        //如果非玩家，则无效
        if (null == TargetFrameLogic.Instance() || GlobeVar.INVALID_GUID == TargetFrameLogic.Instance().TargetGuid)
        {
            return;
        }

        //如果目标是自己也不发送加好友
        if (Singleton<ObjManager>.GetInstance().MainPlayer.GUID == TargetFrameLogic.Instance().TargetGuid)
        {
            return;
        }

        m_PopMenuSelectGuid = TargetFrameLogic.Instance().TargetGuid;
        m_PopMenuSelectName = TargetFrameLogic.Instance().StrTargetName;

        //添加菜单项
        AddMenuItem(0, StrDictionary.GetClientDictionaryString("#{1299}"), PopMenuAddFriend);    //加好友
        AddMenuItem(1, StrDictionary.GetClientDictionaryString("#{1300}"), PopMenuChat);         //私聊
        AddMenuItem(2, StrDictionary.GetClientDictionaryString("#{1301}"), PopMenuInviteTeam);   //邀请入队
        AddMenuItem(3, StrDictionary.GetClientDictionaryString("#{1302}"), PopMeunJoinTeam);     //申请入队
        //AddMenuItem(4, StrDictionary.GetClientDictionaryString("#{1657}"), PopMenuDuel);         //申请切磋
        AddMenuItem(5, StrDictionary.GetClientDictionaryString("#{2181}"), PopMenuView);         //申请查看信息
        AddMenuItem(6, StrDictionary.GetClientDictionaryString("#{2554}"), PopMenuConsignSaleInfo);//申请查看寄售信息
        AddMenuItem(7, StrDictionary.GetClientDictionaryString("#{1156}"), PopMenuInviteGuild);     //邀请入帮
        AddMenuItem(8, StrDictionary.GetClientDictionaryString("#{3093}"), PopMeunJoinGuild);     //申请入帮
    }
    
    //////////////////////////////////////////////////////////////////////////
    //帮会会员PopMenu以及其专属菜单项函数
    //////////////////////////////////////////////////////////////////////////
    void ShowGuildMemberPopMenu()
    {
        if (null == MasterAndGuildLogic.Instance().m_GuildWindow || GlobeVar.INVALID_GUID == MasterAndGuildLogic.Instance().m_GuildWindow.SelectMemberGuid)
        {
            return;
        }

        //在帮会数据中找到该帮众
        GuildMember member;
        if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildMemberList.TryGetValue(MasterAndGuildLogic.Instance().m_GuildWindow.SelectMemberGuid,out member))
        {
            if (!member.IsValid())
            {
                return;
            }
        }
        else
        {
            return;
        }

        //在帮会数据中找到本人
        GuildMember mySelfGuildInfo;
        if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildMemberList.TryGetValue(Singleton<ObjManager>.GetInstance().MainPlayer.GUID, out mySelfGuildInfo))
        {
            if (!mySelfGuildInfo.IsValid())
            {
                return;
            }
        }
        else
        {
            return;
        }

        //如果本人为预备役会员,不弹出popmenu
        if (mySelfGuildInfo.Job == (int)GameDefine_Globe.GUILD_JOB.RESERVE)
        {
            return;
        }

        m_PopMenuSelectGuid = member.Guid;
        m_PopMenuSelectName = member.MemberName;
        
        //根据自己和member的信息，获得不同的菜单项
        if (member.Job == (int)GameDefine_Globe.GUILD_JOB.RESERVE)
        {
            if (mySelfGuildInfo.Job != (int)GameDefine_Globe.GUILD_JOB.MEMBER)
            {
                //预备役会员打开审批通过按钮
                AddMenuItem(0, StrDictionary.GetClientDictionaryString("#{1860}"), GuildMemberDisagreeReserve);      //拒绝
                AddMenuItem(0, StrDictionary.GetClientDictionaryString("#{1859}"), GuildMemberAgreeReserve);         //同意
            }
        }
        else
        {
            //这种情况下由于popMenu过长，所以固定位置
            m_PopMenuOffset.transform.localPosition = new UnityEngine.Vector3(0, 120, 0);

            //默认选项
            AddMenuItem(0, StrDictionary.GetClientDictionaryString("#{1299}"), PopMenuAddFriend);             //加好友
            AddMenuItem(1, StrDictionary.GetClientDictionaryString("#{1300}"), PopMenuChat);                  //私聊
            AddMenuItem(2, StrDictionary.GetClientDictionaryString("#{1766}"), PopMenuView);                  //查看信息
            AddMenuItem(3, StrDictionary.GetClientDictionaryString("#{1301}"), PopMenuInviteTeam);            //邀请入队
            AddMenuItem(4, StrDictionary.GetClientDictionaryString("#{1302}"), PopMeunJoinTeam);              //申请入队

            //只有帮主可以看到的选项
            if (mySelfGuildInfo.Job == (int)GameDefine_Globe.GUILD_JOB.CHIEF)
            {
                if (member.Job == (int)GameDefine_Globe.GUILD_JOB.VICE_CHIEF)
                {
                    AddMenuItem(5, StrDictionary.GetClientDictionaryString("#{2979}"), GuildMemberCommission);     //是副帮主，则显示撤销
                }
                else
                {
                    AddMenuItem(5, StrDictionary.GetClientDictionaryString("#{2978}"), GuildMemberCommission);     //是普通帮众，显示任命
                }
                AddMenuItem(6, StrDictionary.GetClientDictionaryString("#{1769}"), GuildMemberChangeToMaster); //禅让
                AddMenuItem(7, StrDictionary.GetClientDictionaryString("#{1770}"), GuildMemberExpel);          //逐出
            }
            
            //只有副帮主可以看到的选项
            if (mySelfGuildInfo.Job == (int)GameDefine_Globe.GUILD_JOB.VICE_CHIEF)
            {
                if (member.Job != (int)GameDefine_Globe.GUILD_JOB.CHIEF)
                {
                    AddMenuItem(5, StrDictionary.GetClientDictionaryString("#{1770}"), GuildMemberExpel);     //逐出
                }
            }
        }
    }

    //预备役会员审批通过
    void GuildMemberAgreeReserve()
    {
        Singleton<ObjManager>.GetInstance().MainPlayer.ReqApproveGuildMember(m_PopMenuSelectGuid, 1);
    }

    //预备役会员审批未通过
    void GuildMemberDisagreeReserve()
    {
        Singleton<ObjManager>.GetInstance().MainPlayer.ReqApproveGuildMember(m_PopMenuSelectGuid, 0);
    }
    
    //任命
    void GuildMemberCommission()
    {
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqCommisionGuildMember(m_PopMenuSelectGuid);
        }
    }

    //禅让
    void GuildMemberChangeToMaster()
    {
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqChangeGuildMaster(m_PopMenuSelectGuid);
        }
    }

    //逐出
    void GuildMemberExpel()
    {
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqKickGuildMember(m_PopMenuSelectGuid);
        }
    }

    void ClosePopMenu()
    {
        UIManager.CloseUI(UIInfo.PopMenuRoot);
    }
}
