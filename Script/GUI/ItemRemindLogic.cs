using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.GlobeDefine;
using GCGame.Table;

public class ItemRemindLogic : MonoBehaviour {

    private static ItemRemindLogic m_Instance = null;
    public static ItemRemindLogic Instance()
    {
        return m_Instance;
    }

    public Transform m_Trans;
    public ItemSlotLogic m_ItemSlot;
    public UILabel m_NameLabel;

    private List<int> m_ItemDataIDBuffer = new List<int>();
    private bool m_bOnShow = false;
    private float m_fStartShowTime = GlobeVar.INVALID_ID;

    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        ShowRemind();
	}

    void OnDestroy()
    {
        m_Instance = this;
    }

    public static void InitItemInfo(int nDataID)
    {
        List<object> initParams = new List<object>();
        initParams.Add(nDataID);
        UIManager.ShowUI(UIInfo.ItemRemindRoot, ShowUIOver, initParams);   
    }

    static void ShowUIOver(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            List<object> initParams = param as List<object>;
            int nDataID = (int)initParams[0];
            if (ItemRemindLogic.Instance() != null)
            {
                if (EquipRemindLogic.Instance() != null)
                {
                    ItemRemindLogic.Instance().m_Trans.localPosition = new Vector3(210, 400, 0);
                }
                ItemRemindLogic.Instance().m_ItemDataIDBuffer.Add(nDataID);
            }
        }
    }

    void ShowRemind()
    {
        if (m_ItemDataIDBuffer.Count > 0)
        {
            if (!m_bOnShow)
            {
                m_ItemSlot.InitInfo_Item(m_ItemDataIDBuffer[0]);
                Tab_CommonItem tabItem = TableManager.GetCommonItemByID(m_ItemDataIDBuffer[0], 0);
                if (tabItem != null)
                {
                    m_NameLabel.text = tabItem.Name;
                }
                m_bOnShow = true;
                m_fStartShowTime = Time.fixedTime;
            }
            else if (Time.fixedTime - m_fStartShowTime >= GetShowTime())
            {
                m_bOnShow = false;
                m_fStartShowTime = GlobeVar.INVALID_ID;
                m_ItemDataIDBuffer.RemoveAt(0);
            }
        }
        else
        {
            UIManager.CloseUI(UIInfo.ItemRemindRoot);
        }
    }

    float GetShowTime()
    {
        if (m_ItemDataIDBuffer.Count > 1)
        {
            return 0.5f;
        }
        else
        {
            return 1.0f;
        }
    }

    public void HandleEquipRemind(bool isShow)
    {
        if (isShow)
        {
            m_Trans.localPosition = new Vector3(210, 400, 0);
        }
        else
        {
            m_Trans.localPosition = new Vector3(210, 250, 0);
        }
    }
}
