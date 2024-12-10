using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Module.Log;
using GCGame;

public class PlayerFindWindow : MonoBehaviour
{
    private static PlayerFindWindow m_Instance = null;
    public static PlayerFindWindow Instance()
    {
        return m_Instance;
    }

    public GameObject m_PlayerFindListGrid;         //列表控件
    public UIInput m_NameInput;                     //姓名输入控件
    public UIImageButton m_AddFreindButton;
    public UIImageButton m_TrailButton;

    public ShowLable m_TrailShowLable;
    public ShowLable m_FriendShowLable;

    private PlayerFindItemLogic m_SelectItem;

    void Awake()
    {
        m_Instance = this;
    }

    void OnEnable()
    {
        GUIData.delPlayerFindResult += UpdatePlayerFindResult;
        ClearPleyrFindItem();
        SetSelectPlayerItem(null);
        m_AddFreindButton.isEnabled = false;
        m_FriendShowLable.ChooseShowLable(0);
        m_TrailButton.isEnabled = false;
        m_TrailShowLable.ChooseShowLable(0);
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    void OnDisable()
    {
        GUIData.delPlayerFindResult -= UpdatePlayerFindResult;
    }
    
    //收到服务器返回结果之后，更新玩家查找结果
    public void UpdatePlayerFindResult(GC_FINDPLAYER packet)
    {
        if (packet.HasRet)
        {
            if (packet.Ret == 1)
            {
                GUIData.AddNotifyData2Client(false,"#{2746}");
                return;
            }
            else if (packet.Ret == 2)
            {
				GUIData.AddNotifyData2Client(false,"#{2745}");
                return;
            }
			else if(packet.Ret == 3)
			{
				GUIData.AddNotifyData2Client(false,"#{5611}");
				return;
			}
            UIManager.LoadItem(UIInfo.PlayerFindItem, OnLoadPlayerFindItem, packet);
			m_AddFreindButton.isEnabled = true;
			m_TrailButton.isEnabled = true;
        }
        else
        {
            UIManager.LoadItem(UIInfo.PlayerFindItem, OnLoadPlayerFindItem, packet);
			m_AddFreindButton.isEnabled = true;
			m_TrailButton.isEnabled = true;
        }
        m_TrailShowLable.ChooseShowLable(1);
        m_FriendShowLable.ChooseShowLable(1);
    }

    void OnLoadPlayerFindItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("can not load player find item");
            return;
        }

        GC_FINDPLAYER packet = param as GC_FINDPLAYER;
        //清空表格内容
        ClearPleyrFindItem();

        //填充数据
        int idCount = packet.guidCount;
        for (int i = 0; i < idCount; i++)
        {
            Relation player = new Relation();
            player.Guid = packet.GetGuid(i);
            player.Name = packet.GetName(i);
            player.Level = packet.GetLevel(i);
            player.Profession = packet.GetProf(i);
            player.CombatNum = packet.GetCombat(i);

            //GameObject newPlayerFindItem = Utils.BindObjToParent(resItem, m_PlayerFindListGrid, i.ToString());
            //newPlayerFindItem.GetComponent<PlayerFindItemLogic>().InitPlayerFindItem(player);
            PlayerFindItemLogic PlayerItem = PlayerFindItemLogic.CreateItem(m_PlayerFindListGrid, resItem, i.ToString(), this);
            if (PlayerItem == null)
            {
                continue;
            }
            PlayerItem.InitPlayerFindItem(player);
            if (m_SelectItem == null)
            {
                SetSelectPlayerItem(PlayerItem);
            }
        }

        m_PlayerFindListGrid.GetComponent<UIGrid>().repositionNow = true;
        m_PlayerFindListGrid.GetComponent<UITopGrid>().recenterTopNow = true;
    }

    //点击玩家查找按钮
    void OnClickSearch()
    {
        //if (null == m_NameInput)
        //{
        //    return;
        //}

        //清空表格内容
        ClearPleyrFindItem();

        //向服务器发送超找好友包
		if(m_NameInput.value != LoginData.GetPlayerRoleData(PlayerPreferenceData.LastRoleGUID).name)
		{
			CG_FINDPLAYER msg = (CG_FINDPLAYER)PacketDistributed.CreatePacket(MessageID.PACKET_CG_FINDPLAYER);
			msg.Guid = GlobeVar.INVALID_GUID;
			msg.Name = m_NameInput.value;
			msg.SendPacket();
		}
    }

    //清空列表框
    public void ClearPleyrFindItem()
    {
        if (null == m_PlayerFindListGrid)
        {
            return;
        }

        //PlayerFindItemLogic[] item = m_PlayerFindListGrid.GetComponentsInChildren<PlayerFindItemLogic>();
        //for (int i = 0; i < item.Length; ++i)
        //{
        //    Destroy(item[i].gameObject);
        //}

        //m_PlayerFindListGrid.transform.DetachChildren();
        Utils.CleanGrid(m_PlayerFindListGrid);
        SetSelectPlayerItem(null);

    }

    public void OpenNearByTab()
    {

    }

    void SetSelectPlayerItem(PlayerFindItemLogic SelectItem)
    {
        if (SelectItem == null)
        {
            if ( m_AddFreindButton !=null)
            {
                m_AddFreindButton.isEnabled = false;
                m_FriendShowLable.ChooseShowLable(0);
            }
            if (m_TrailButton != null)
            {
                m_TrailButton.isEnabled = false;
                m_TrailShowLable.ChooseShowLable(0);
            }
            m_SelectItem = SelectItem;
            return;
        }
        if ( m_SelectItem != null)
        {
            m_SelectItem.OnCancelSelectItem();
        }
        m_SelectItem = SelectItem;
        m_SelectItem.OnSelectItem();
        if (m_AddFreindButton != null)
        {
            m_AddFreindButton.isEnabled = true;
            m_FriendShowLable.ChooseShowLable(1);
        }
        if (m_TrailButton != null)
        {
            m_TrailButton.isEnabled = true;
            m_TrailShowLable.ChooseShowLable(1);
        }
    }

    public void OnClickPlayerItem(PlayerFindItemLogic SelectItem)
    {
        if (SelectItem == null)
        {
            LogModule.ErrorLog("OnClickPlayerItem::SelectItem id null");
            return;
        }
        SetSelectPlayerItem(SelectItem);
    }

    public void OnClickAddFriend()
    {
        //检查GUID合法性
        if (m_SelectItem == null)
        {
			GUIData.AddNotifyData2Client(false,"#{2750}");
            return;
        }
        if (m_SelectItem.Guid == GlobeVar.INVALID_GUID)
        {
			GUIData.AddNotifyData2Client(false,"#{2750}");
		
            return;
        }
        //是否已经是好友
        if (true == GameManager.gameManager.PlayerDataPool.FriendList.IsExist(m_SelectItem.Guid))
        {
			GUIData.AddNotifyData2Client(false,"#{2750}");
            return;
        }

        //添加好友
        Singleton<ObjManager>.GetInstance().MainPlayer.ReqAddFriend(m_SelectItem.Guid);
    }

    void OnClickTrail()
    {
        //检查GUID合法性
        if (m_SelectItem == null)
        {
			GUIData.AddNotifyData2Client(false,"#{2750}");
            return;
        }
        if (m_SelectItem.Guid == GlobeVar.INVALID_GUID)
        {
			GUIData.AddNotifyData2Client(false,"#{2750}");
            return;
        }
        if (m_SelectItem.Guid == Singleton<ObjManager>.Instance.MainPlayer.GUID)
        {
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.ReqTrailPlayer(m_SelectItem.Guid);
    }
}
