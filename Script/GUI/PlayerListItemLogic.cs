using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Games.LogicObj;
using System;
using GCGame.Table;
using Module.Log;
using GCGame;
public class PlayerListItemLogic : MonoBehaviour 
{

    public enum PlayerListItemType
    {
        Invalid = -1,
        TeamMemberInfo = 0,        //队员信息
        NearByPlayer = 1,        //附近玩家
        NearByTeam = 2,        //附近队伍
        FriendInfo = 3,        //好友列表
        BlackPlayerInfo = 4,        //黑名单
        HatePlayerInfo = 5,         //仇人
    }

    // 玩家列表项信息 视情况再添加
    public UILabel m_PlayerTeamJobLabel;            // 队长/队员
    public UILabel m_PlayerProLabel;                // 职业
    public UILabel m_PlayerNameLabel;               // 名字
    public UILabel m_PlayerLevelLabel;              // 等级
    public UILabel m_PlayerCombatNumLabel;          // 战斗力
    public UILabel m_PlayerStateLable;              // 状态
    
    //按钮
    public GameObject m_SelectSprite;
    public GameObject m_BkSprite;

    static private Color m_SelfColor = new Color(0.0f / 255.0f, 162.0f / 255.0f, 232.0f / 255.0f, 1.0f);
	static private Color m_OtherColor = new Color(254.0f/255.0f, 235.0f/255.0f,183.0f/255.0f, 1.0f);

    //其他非界面相关数据
    public int m_nTeamID = GlobeVar.INVALID_ID;     //队伍ID，只在附近队伍列表中生效
    public int TeamID
    {
        get { return m_nTeamID; }
    }

    private int m_nTeamPosIndex = GlobeVar.INVALID_ID;
    public int TeamPosIndex
    {
        get { return m_nTeamPosIndex; }
    }

    public UInt64 m_Guid = GlobeVar.INVALID_GUID;
    public UInt64 GUID
    {
        get { return m_Guid;  }
    }
    public string m_PlayerName = "";
    private PlayerListItemType m_ItemType = PlayerListItemType.Invalid;
    public PlayerListItemType ItemType
    {
        get { return m_ItemType; }
        set { m_ItemType = value;  }
    }

	private int m_PlayerLvl = GlobeVar.INVALID_ID;
	public int PlayerLvl
	{
		get { return m_PlayerLvl; }
	}

	private int m_PlayerBattle = GlobeVar.INVALID_ID;
	public int PlayerBattle
	{
		get { return m_PlayerBattle; }
	}

    private RelationFriendWindow m_FriendParent = null;
    private RelationTeamWindow m_TeamParent = null;

    public static PlayerListItemLogic CreateItem(GameObject grid, GameObject resItem, string name, RelationFriendWindow parent)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("PlayerListItemLogic Create resItem is null_1");
            return null;
        }

        GameObject curItem = Utils.BindObjToParent(resItem, grid, name);
        if (null != curItem)
        {
            PlayerListItemLogic curItemComponent = curItem.GetComponent<PlayerListItemLogic>();
            if (null != curItemComponent)
                curItemComponent.SetParent(parent);

            return curItemComponent;
        }

        return null;
    }

    public static PlayerListItemLogic CreateItem(GameObject grid, GameObject resItem, string name, RelationTeamWindow parent)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("PlayerListItemLogic Create resItem is null_2");
            return null;
        }

        GameObject curItem = Utils.BindObjToParent(resItem, grid, name);
        if (null != curItem)
        {
            PlayerListItemLogic curItemComponent = curItem.GetComponent<PlayerListItemLogic>();
            if (null != curItemComponent)
                curItemComponent.SetParent(parent);

            return curItemComponent;
        }

        return null;
    }

    public void SetParent(RelationFriendWindow parent)
    {
        m_FriendParent = parent;
        m_TeamParent = null;
    }

    public void SetParent(RelationTeamWindow parent)
    {
        m_TeamParent = parent;
        m_FriendParent = null;
    }

    //隐藏所有控件
    void HideAllUI()
    {
        m_PlayerTeamJobLabel.gameObject.SetActive(false);
        m_PlayerProLabel.gameObject.SetActive(false);
        m_PlayerNameLabel.gameObject.SetActive(false);
        m_PlayerLevelLabel.gameObject.SetActive(false);
        m_PlayerCombatNumLabel.gameObject.SetActive(false);
        m_PlayerStateLable.gameObject.SetActive(false);

        m_PlayerTeamJobLabel.color = m_OtherColor;
        m_PlayerProLabel.color = m_OtherColor;
        m_PlayerNameLabel.color = m_OtherColor;
        m_PlayerLevelLabel.color = m_OtherColor;
        m_PlayerCombatNumLabel.color = m_OtherColor;
        m_PlayerStateLable.color = m_OtherColor;

        m_SelectSprite.SetActive(false);
        m_ItemType = PlayerListItemType.Invalid;
        m_nTeamPosIndex = GlobeVar.INVALID_ID;
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
                UIManager.ShowUI(UIInfo.ChatInfoRoot, ShowChatInfoRootOver);
            }
        }
    }

    void ShowChatInfoRootOver(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            //发起私聊
            if (null != ChatInfoLogic.Instance())
            {
                ChatInfoLogic.Instance().BeginChat(m_Guid, m_PlayerName);
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
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqViewOtherPlayer(m_Guid, OtherRoleViewLogic.OPEN_TYPE.OPEN_TYPE_FRIEND);
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
                Singleton<ObjManager>.GetInstance().MainPlayer.ReqDelFriend(m_Guid);
                return;
            }

            //如果是黑名单，则删除黑名单
            if (GameManager.gameManager.PlayerDataPool.BlackList.IsExist(m_Guid))
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.ReqDelBlack(m_Guid);
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
    
    //显示本队队员信息
    public void InitPlayerListItemInfo(TeamMember member, int nTeamPosIndex)
    {
        //首先隐藏掉全部控件，在需要set的时候再生效
        HideAllUI();

        m_nTeamPosIndex = nTeamPosIndex;

        m_ItemType = PlayerListItemType.TeamMemberInfo;
        // 设置GUID
        m_Guid = member.Guid;
        // 队长/队员
        SetTeamJob(member.TeamJob);
        // 职业
        SetPro(member.Profession);
        // 名字
        SetName(member.MemberName);
        // 等级
        SetLevel(member.Level);
        // 战斗力
        SetCombatNum(member.CombatNum);
        
        //如果是自己
        if (m_Guid == Singleton<ObjManager>.GetInstance().MainPlayer.GUID)
        {
            m_PlayerTeamJobLabel.color = m_SelfColor;
            m_PlayerProLabel.color = m_SelfColor;
            m_PlayerNameLabel.color = m_SelfColor;
            m_PlayerLevelLabel.color = m_SelfColor;
            m_PlayerCombatNumLabel.color = m_SelfColor;
            m_PlayerStateLable.color = m_SelfColor;
        }
    }

    //显示附近玩家信息
    public void InitPlayerListItemInfo(NearbyPlayer info)
    {
        HideAllUI();

        if (info != null)
        {
            m_ItemType = PlayerListItemType.NearByPlayer;
            //设置GUID
            m_Guid = info.Guid;
            //姓名
            SetName(info.Name);
            //职业
            SetPro(info.Profession);
            //等级
            SetLevel(info.Level);
            //战斗力
            SetCombatNum(info.CombatNum);
        }
    }

    //显示附近队伍信息
    public void InitPlayerListItemInfo(NearbyTeam info)
    {
        HideAllUI();

        if (info != null)
        {
            m_ItemType = PlayerListItemType.NearByTeam;
            //设置GUID
            m_Guid = info.Guid;
            //姓名
            SetName(info.Name);
            //职业
            SetPro(info.Profession);
            //等级
            SetLevel(info.Level);
            //战斗力
            SetCombatNum(info.CombatNum);
            //队长/队员
            SetTeamJob(0);
            //队伍ID
            m_nTeamID = info.TeamID;
        }
    }

    //显示好友
    public void InitPlayerListItemInfoFriend(Relation relation)
    {
        HideAllUI();

        if (relation != null)
        {
            m_ItemType = PlayerListItemType.FriendInfo;
            //设置GUID
            m_Guid = relation.Guid;
            //姓名
            SetName(relation.Name);
            //职业
            SetPro(relation.Profession);
            //等级
            SetLevel(relation.Level);
            //战斗力
            SetCombatNum(relation.CombatNum);
            //状态
            SetState(relation.State);
        }
    }

    //显示黑名单
    public void InitPlayerListItemInfoBlack(Relation relation)
    {
        HideAllUI();

        if (relation != null)
        {
            m_ItemType = PlayerListItemType.BlackPlayerInfo;
            //设置GUID
            m_Guid = relation.Guid;
            //姓名
            SetName(relation.Name);
            //职业
            SetPro(relation.Profession);
            //等级
            SetLevel(relation.Level);
            //战斗力
            SetCombatNum(relation.CombatNum);
            //状态
            SetState(relation.State);
        }
    }

    public void InitPlayerListItemInfoHate(Relation relation)
    {
        HideAllUI();

        if (relation != null)
        {
            m_ItemType = PlayerListItemType.HatePlayerInfo;
            //设置GUID
            m_Guid = relation.Guid;
            //姓名
            SetName(relation.Name);
            //职业
            SetPro(relation.Profession);
            //等级
            SetLevel(relation.Level);
            //战斗力
            SetCombatNum(relation.CombatNum);
            //状态
            SetState(relation.State);
        }
    }
    
    void SetTeamJob(int nJob)
    {
        if (m_PlayerTeamJobLabel == null)
        {
            return;
        }
        m_PlayerTeamJobLabel.gameObject.SetActive(true);
        if (0 == nJob)
        {
            //m_PlayerTeamJobLabel.text = "队长";
            m_PlayerTeamJobLabel.text = StrDictionary.GetClientDictionaryString("#{2870}");
        }
        else
        {
            //m_PlayerTeamJobLabel.text = "队员";
            m_PlayerTeamJobLabel.text = StrDictionary.GetClientDictionaryString("#{2871}");
        }
    }

    void SetPro(int nProfession)
    {
        if (m_PlayerProLabel == null)
        {
            return;
        }
        m_PlayerProLabel.gameObject.SetActive(true);
        //判断职业是否合法
        if (nProfession < 0 || nProfession >= (int)CharacterDefine.PROFESSION.MAX)
        {
            //非法职业ID，则强制为0
            nProfession = 0;
        }

        // 玩家职业
       m_PlayerProLabel.text = StrDictionary.GetClientDictionaryString("#{" + CharacterDefine.PROFESSION_DICNUM[nProfession].ToString() + "}");
    }

    void SetName(string szName)
    {
        // 记录名字
        m_PlayerName = szName;
        if ( m_PlayerNameLabel != null)
        {
            m_PlayerNameLabel.gameObject.SetActive(true);
            m_PlayerNameLabel.text = szName;
        }
    }

    void SetLevel(int nLevel)
    {
        if (m_PlayerLevelLabel != null)
        {
            m_PlayerLevelLabel.gameObject.SetActive(true);
            //m_PlayerLevelLabel.text = StrDictionary.GetClientDictionaryString("#{1063}", nLevel);
            m_PlayerLevelLabel.text = nLevel.ToString();
			m_PlayerLvl = nLevel;
        }
    }

    void SetCombatNum(int nCombatNum)
    {
        if ( m_PlayerCombatNumLabel != null)
        {
            m_PlayerCombatNumLabel.gameObject.SetActive(true);
            //m_PlayerCombatNumLabel.text = StrDictionary.GetClientDictionaryString("#{1064}", nCombatNum);
            m_PlayerCombatNumLabel.text = nCombatNum.ToString();
			m_PlayerBattle = nCombatNum;
        }
    }

    void SetState(int nState)
    {
        //1-在线
        if ( m_PlayerStateLable == null)
        {
            return;
        }
        if ((int)CharacterDefine.RELATION_TYPE.ONLINE == nState)
        {
            m_PlayerStateLable.gameObject.SetActive(true);
            m_PlayerStateLable.text = StrDictionary.GetClientDictionaryString("#{1260}");   //在线
        }
        //离线
        else
        {
            m_PlayerStateLable.gameObject.SetActive(true);
            m_PlayerStateLable.text = StrDictionary.GetClientDictionaryString("#{1261}");   //离线
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnClickPlayerListItem()
    {
        if (m_FriendParent != null)
        {
            m_FriendParent.OnClickPlayerListItem(this);
        }
        else if (m_TeamParent != null)
        {
            m_TeamParent.OnClickPlayerListItem(this);
        }
        else
        {
            LogModule.ErrorLog("OnClickPlayerListItem parent is null");
        }
    }

    public void OnSelectItem()
    {
        if (m_SelectSprite != null )
        {
            m_SelectSprite.SetActive(true);
        }
        if (m_BkSprite != null)
        {
            m_BkSprite.SetActive(false);
        }
    }

    public void OnCancelSelectItem()
    {
        if (m_SelectSprite != null)
        {
            m_SelectSprite.SetActive(false);
        }
        if (m_BkSprite != null)
        {
            m_BkSprite.SetActive(true);
        }
    }
}
