//********************************************************************
// 文件名: TitleInvestitiveLogic.cs
// 描述: 称号界面
// 作者: WangZhe
//********************************************************************

using UnityEngine;
using System.Collections;
using GCGame;
using Games.TitleInvestitive;
using GCGame.Table;
using Games.LogicObj;
using Module.Log;
using System.Collections.Generic;
using Games.GlobeDefine;
public class TitleInvestitiveLogic : MonoBehaviour {

    private static TitleInvestitiveLogic m_Instance = null;
    public static TitleInvestitiveLogic Instance()
    {
        return m_Instance;
    }

    public GameObject m_TitleInvestitiveListGrid;
    public UILabel m_TitleInvestitiveDescLabel;
    public UILabel m_ActiveTitleNameLabel;
    public GameObject m_UseButton;
    public GameObject m_LockButton;
    public GameObject m_UnLockButton;

    private int m_PlayerChooseIndex = -1;
    private int m_PlayerChooseID = -1;
    private GameObject m_TitleItem = null;
    private List<int> m_TypeCacheList = new List<int>();


	// Use this for initialization
    void OnEnable () 
    {
        // 默认把当前称号滚动到列表中间(改为不移动到中间 只选中)
        m_Instance = this;
        int nNum = 0;
        if (GameManager.gameManager.PlayerDataPool.TitleInvestitive.ActiveTitle != -1)
        {
            TitleInvestitiveItemLogic[] itemArray = gameObject.GetComponentsInChildren<TitleInvestitiveItemLogic>();
            for (int i = 0; i < itemArray.Length; i++ )
            {
                if (itemArray[i].Index == GameManager.gameManager.PlayerDataPool.TitleInvestitive.ActiveTitle)
                {
                    itemArray[i].ChooseTitleInvestitive();
                    //int nItemHeight = item.gameObject.transform.FindChild("Background").gameObject.GetComponent<UISprite>().height;
                    //m_TitleInvestitiveListGrid.transform.localPosition = new Vector3(m_TitleInvestitiveListGrid.transform.localPosition.x, nItemHeight * nNum, 0);
                    break;
                }
                nNum++;
            }
        }
        UpdateLockButton();

        Init();
	}
    
    void OnDisable()
    {
        m_Instance = null;
    }
    public void UpdateLockButton()
    {
        if (GameManager.gameManager.PlayerDataPool.IsLockPriorTitle == false)
        {
            m_LockButton.SetActive(true);
            m_UnLockButton.SetActive(false);
        }
        else
        {
            m_LockButton.SetActive(false);
            m_UnLockButton.SetActive(true);
        }
    }

    public void Init()
    {
        m_PlayerChooseIndex = -1;
        m_PlayerChooseID = -1;
        m_TypeCacheList.Clear();
        LoadTitleItem();
        ShowActiveTitleName();
    }

    void LoadTitleItem()
    {
        if (m_TitleItem == null)
        {
            UIManager.LoadItem(UIInfo.TitleInvestitiveItem, LoadTitleItemOver);
        }
        else
        {
            InitAllTitleInvestitive();
        }
    }

    void LoadTitleItemOver(GameObject resItem, object param)
    {
        if (resItem == null)
        {
            LogModule.ErrorLog("load TitleInvestitiveItem fail");
            return;
        }
        m_TitleItem = resItem;
        InitAllTitleInvestitive();
    }

    void InitAllTitleInvestitive()
    {
        Utils.CleanGrid(m_TitleInvestitiveListGrid);
        int number = 0;
        for (int i = 0; i < (int)TITLE_SIZE.TITLE_TOTAL; i++)
        {
            if (i < (int)TITLE_SIZE.USERDEF_START)
            {
                GameSystemTitleInvestitive title = GameManager.gameManager.PlayerDataPool.TitleInvestitive.SystemTitle[i];
                if (title.TitleID != -1)
                {
                    GameObject TitleInvestitiveItem = Utils.BindObjToParent(m_TitleItem,
                        m_TitleInvestitiveListGrid, number < 10 ? ("TitleInvestitiveItem" + "0" + number.ToString()) : ("TitleInvestitiveItem" + number.ToString()));
                    if (null != TitleInvestitiveItem && null != TitleInvestitiveItem.GetComponent<TitleInvestitiveItemLogic>())
                    {
                        TitleInvestitiveItem.GetComponent<TitleInvestitiveItemLogic>().InitSystemTitleInfo(title.TitleID, i);
                        number += 1;
                    }
                }
            }
            else
            {
                GameUserDefTitleInvestitive title = GameManager.gameManager.PlayerDataPool.TitleInvestitive.UserDefTitle[i - (int)TITLE_SIZE.USERDEF_START];
                if (title.TitleID != -1)
                {
                    GameObject TitleInvestitiveItem = Utils.BindObjToParent(m_TitleItem,
                        m_TitleInvestitiveListGrid, number < 10 ? ("TitleInvestitiveItem" + "0" + number.ToString()) : ("TitleInvestitiveItem" + number.ToString()));

                    if (null != TitleInvestitiveItem && null != TitleInvestitiveItem.GetComponent<TitleInvestitiveItemLogic>())
                    {
                        TitleInvestitiveItem.GetComponent<TitleInvestitiveItemLogic>().InitUserDefTitleInfo(title.TitleID, title.StrFullTitleName, i);
                        number += 1;
                    }
                }
            }
        }

        foreach (KeyValuePair<int, List<Tab_TitleData>> pair in TableManager.GetTitleData())
        {
            Tab_TitleData tabTitle = pair.Value[0];
            if (tabTitle == null)
            {
                continue;
            }

            if (!m_TypeCacheList.Contains(tabTitle.Type) &&
                !GameManager.gameManager.PlayerDataPool.TitleInvestitive.IsHaveType(tabTitle.Type))
            {
                m_TypeCacheList.Add(tabTitle.Type);
                GameObject TitleInvestitiveItem = Utils.BindObjToParent(m_TitleItem,
                    m_TitleInvestitiveListGrid, number < 10 ? ("TitleInvestitiveItem" + "0" + number.ToString()) : ("TitleInvestitiveItem" + number.ToString()));
                TitleInvestitiveItem.GetComponent<TitleInvestitiveItemLogic>().InitNoHoldTitleInfo(tabTitle.Id);
                number += 1;
            }
        }
        m_TitleInvestitiveListGrid.GetComponent<UIGrid>().Reposition();

        if (RoleViewLogic.Instance())
        {
            RoleViewLogic.Instance().AfterLoadTitleItem();
        }
    }

    public void ChooseTitleInvestitive(int nIndex, int nTitleID, TitleInvestitiveItemLogic.TITLE_CLASS eClass)
    {
        m_PlayerChooseIndex = nIndex;
        m_PlayerChooseID = nTitleID;
        ShowTitleInvestitiveDesc(nTitleID, eClass);
        TitleInvestitiveItemLogic[] itemArray = gameObject.GetComponentsInChildren<TitleInvestitiveItemLogic>();
        for (int i = 0; i < itemArray.Length; i++ )
        {
            if (null != itemArray[i] && itemArray[i].TitleID != m_PlayerChooseID)
            {
                itemArray[i].ClearChooseState();
            }
        }

        if (eClass == TitleInvestitiveItemLogic.TITLE_CLASS.NOHOLD)
        {
            m_UseButton.SetActive(false);
        }
        else
        {
            m_UseButton.SetActive(true);
        }
    }

    void ShowTitleInvestitiveDesc(int nTitleID, TitleInvestitiveItemLogic.TITLE_CLASS eClass)
    {
        if (eClass == TitleInvestitiveItemLogic.TITLE_CLASS.NOHOLD)
        {
            Tab_TitleData tabTitleData = TableManager.GetTitleDataByID(nTitleID, 0);
            if (tabTitleData != null)
            {
                Tab_TitleType tabTitleType = TableManager.GetTitleTypeByID(tabTitleData.Type, 0);
                if (tabTitleType != null)
                {
                    m_TitleInvestitiveDescLabel.text = tabTitleType.TypeDesc;
                }
            }
        }
        else
        {
            Tab_TitleData tabTitleData = TableManager.GetTitleDataByID(nTitleID, 0);
            if (tabTitleData != null)
            {
                m_TitleInvestitiveDescLabel.text = tabTitleData.InvestitiveDescription;
            }
        }       
    }

    void ShowActiveTitleName()
    {
        m_ActiveTitleNameLabel.text =  GameManager.gameManager.PlayerDataPool.TitleInvestitive.GetCurrentTitle();
    }

    void UseTitleInvestitive()
    {
        int nCurTitleID = GameManager.gameManager.PlayerDataPool.TitleInvestitive.GetCurrentTitleID();
        Tab_TitleData tabTitleData = TableManager.GetTitleDataByID(nCurTitleID, 0);
        if (tabTitleData != null)
        {
            if (tabTitleData.IsImmotile == 1)
            {
                LogModule.DebugLog("头顶称号强制显示");
                return;
            }
        }
        // 发包给服务器
        CG_ACTIVE_TITLE packet = (CG_ACTIVE_TITLE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ACTIVE_TITLE);
		if (m_PlayerChooseIndex >= 64)
		{
			m_PlayerChooseIndex-=64;
		}
        packet.TitleIndex = m_PlayerChooseIndex;
        packet.SendPacket();  
    }

    public void HandleActiveTitle()
    {
        ShowActiveTitleName();
    }

    void DeleteTitleInvestitive()
    {
        Tab_TitleData tabTitleData = TableManager.GetTitleDataByID(m_PlayerChooseID, 0);
        if (tabTitleData != null)
        {
            //MessageBoxLogic.OpenOKCancelBox("确定要删除称号吗？", "", DeleteTitleInvestitiveOK, DeleteTitleInvestitiveCancel);
            MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2877}"), "", DeleteTitleInvestitiveOK, DeleteTitleInvestitiveCancel);
        
        }
    }

    void DeleteTitleInvestitiveOK()
    {
        if (TableManager.GetTitleDataByID(m_PlayerChooseID, 0).CanDeleteByPlayer == 0)
        {
            if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1598}");
                return;
            }
        }
        // 发包给服务器
        CG_DELETE_TITLE packet = (CG_DELETE_TITLE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_DELETE_TITLE);
        packet.TitleIndex = m_PlayerChooseIndex;
        packet.SendPacket();
    }

    void DeleteTitleInvestitiveCancel()
    {
    }

    public void HandleDeleteTitle()
    {
        /*TitleInvestitiveItemLogic[] itemArray = m_TitleInvestitiveListGrid.GetComponentsInChildren<TitleInvestitiveItemLogic>();
        for (int i = 0; i < itemArray.Length; i++ )
        {
            if (itemArray[i].Index == m_PlayerChooseIndex)
            {
                Destroy(itemArray[i].gameObject);
                break;
            }
        }
        ShowActiveTitleName();
        Singleton<ObjManager>.GetInstance().MainPlayer.ShowPlayerTitleInvestitive();
        // 还原
        m_PlayerChooseIndex = -1;
        m_PlayerChooseID = -1;
        m_TitleInvestitiveDescLabel.text = "";
        m_TitleInvestitiveListGrid.GetComponent<UIGrid>().repositionNow = true;*/
        Init();
    }

    public void OnCloseBackPack()
    {
    }

    public void HandleGainTitle()
    {
        Init();
    }

    void LockTitle()
    {
        if (GameManager.gameManager.PlayerDataPool.IsLockPriorTitle == false)
        {
            MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2764}"), "", LockTitleOK, LockTitleCancel);
        }
        else
        {
            LockTitleOK();
        }
    }

    void LockTitleOK()
    {
        CG_ASK_LOCK_TITLE packet = (CG_ASK_LOCK_TITLE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_LOCK_TITLE);
        packet.NoParam = -1;
        packet.SendPacket();
    }

    void LockTitleCancel()
    {

    }
}
