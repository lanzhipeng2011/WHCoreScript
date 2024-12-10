using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using GCGame;
using Module.Log;
using GCGame.Table;
using System.Collections.Generic;

public class GuildMakeLogic : MonoBehaviour {


    public GameObject m_GuildMakeGrid;
    public GameObject m_GuildMakeItem;

    public UILabel m_MakeNameLable;
    public UILabel m_MakeLvlLable;
    public UILabel m_MakeNumLable;
    public UILabel m_MakeCostNumLable;
    public UILabel m_MakeDescLable;

    private int m_MakeID = -1;

    private static GuildMakeLogic m_Instance = null;
    public static GuildMakeLogic Instance()
    {
        return m_Instance;
    }
    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start () {
        InitGuildMake();
	}

    void InitGuildMake()
    {
        if (null == m_GuildMakeGrid)
        {
            return;
        }

        for (int i = 1; i <= TableManager.GetGuildMake().Count; i++)
        {
            Tab_GuildMake tab = TableManager.GetGuildMakeByID(i, 0);
            if (tab == null || tab.CommonItemId <= 0 || tab.MadeNumber <= 0)
            {
                continue;
            }

            Tab_CommonItem ItemInfo = TableManager.GetCommonItemByID(tab.CommonItemId, 0);
            if (null == ItemInfo)
            {
                continue;
            }

            // 加载对应生产物品
            GameObject GuildMakeItem = Utils.BindObjToParent(m_GuildMakeItem, m_GuildMakeGrid, i.ToString());
            if (GuildMakeItem == null)
            {
                continue;
            }


            if (null != GuildMakeItem.GetComponent<GuildMakeItemLogic>())
                GuildMakeItem.GetComponent<GuildMakeItemLogic>().Init(ItemInfo, tab.Id);

        }
        m_GuildMakeGrid.GetComponent<UIGrid>().Reposition();

        UpdateMakeInfo(1);
        
    }

   
    void CleanUp()
    {
        m_MakeID = -1;
        m_MakeNameLable.text = "";
        m_MakeLvlLable.text = "";
        m_MakeNumLable.text = "";
        m_MakeDescLable.text = "";
    }

    void OnClose()
    {
        this.gameObject.SetActive(false);
    }

    void OnMake()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        OnClose();

        Singleton<ObjManager>.GetInstance().MainPlayer.GuildMakeAction(m_MakeID);
    }

    public void UpdateMakeInfo(int makeID)
    {
        if (makeID < 1 || makeID > TableManager.GetGuildMake().Count)
        {
            return;
        }

        Tab_GuildMake tab = TableManager.GetGuildMakeByID(makeID, 0);
        if (null == tab)
        {
            return;
        }

        Tab_CommonItem itemInfo = TableManager.GetCommonItemByID(tab.CommonItemId, 0);
        if (null == itemInfo)
        {
            return;
        }

        CleanUp();
        m_MakeID = makeID;
        m_MakeNameLable.text = itemInfo.Name;
        m_MakeLvlLable.text = itemInfo.MinLevelRequire.ToString() + "级绿色腰带";
        m_MakeNumLable.text = tab.MadeNumber.ToString();
        m_MakeCostNumLable.text = tab.MadeCost.ToString();
        m_MakeDescLable.text = itemInfo.Tips;

        GuildMakeItemLogic[] guildMake = m_GuildMakeGrid.GetComponentsInChildren<GuildMakeItemLogic>();
        for (int i = 0; i < guildMake.Length; i++)
        {
            guildMake[i].IsShowPressBg(i == (makeID - 1));
        }
    }

}
