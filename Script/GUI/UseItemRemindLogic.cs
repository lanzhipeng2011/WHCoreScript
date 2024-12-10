using UnityEngine;
using System.Collections;
using Games.Item;
using System.Collections.Generic;
using GCGame.Table;
using Games.GlobeDefine;

public class UseItemRemindLogic : MonoBehaviour
{

    private static UseItemRemindLogic m_Instance = null;
    public static UseItemRemindLogic Instance()
    {
        return m_Instance;
    }

    public GameObject m_ItemRemind;
    public UISprite m_IconSprite;
    public UISprite m_QualitySprite;
    public UILabel m_TitleNameLabel;
    List<GameItem> m_UseItemBuffer = new List<GameItem>();
    const int m_UseItemBufferSize = 4;
    bool m_bOnShow = false;
    float m_fStartShowTime = 0.0f;
    const float ShowTime = 10.0f;
    bool m_bOnHide = true;
    float m_fStartHideTime = 0.0f;
    const float HideTime = 2.0f;

    void Awake()
    {
        m_Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        if (ItemRemindLogic.Instance() != null)
        {
            ItemRemindLogic.Instance().HandleEquipRemind(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShowRemind();
    }

    void OnDestroy()
    {
        m_Instance = this;
    }

    public static void InitUseItemInfo(GameItem newUseItem)
    {
        if (newUseItem == null || !newUseItem.IsValid())
        {
            return;
        }

        List<object> initParams = new List<object>();
        initParams.Add(newUseItem);
        UIManager.ShowUI(UIInfo.UseItemRemindRoot,ShowUIOver, initParams);
    }

    static void ShowUIOver(bool bSuccess, object param)
    {
        if (bSuccess)
        {
            List<object> initParams = param as List<object>;
            if (UseItemRemindLogic.Instance() != null)
            {
                UseItemRemindLogic.Instance().Init(initParams[0] as GameItem);
            }
        }
    }

    void Init(GameItem newUseItem)
    {
        if (newUseItem == null || !newUseItem.IsValid())
        {
            return;
        }

        int canuse = TableManager.GetCommonItemByID(newUseItem.DataID, 0).CanUse;
        if (canuse != 1)
        {
            return;
        }
        // 是否正在显示提示
        if (m_UseItemBuffer.Count == 0)
        {
            m_UseItemBuffer.Add(newUseItem);
            ShowRemind();
            return;
        }
        // 如果正在显示 检测是否可以进缓冲池
        else
        {
            if (m_UseItemBuffer.Count <= m_UseItemBufferSize)
            {
                m_UseItemBuffer.Add(newUseItem);
            }
        }
    }

    void ShowRemind()
    {
        if (m_UseItemBuffer.Count > 0)
        {
            // 处在显示状态且时间已达到10秒
            if (Time.fixedTime - m_fStartShowTime >= ShowTime && m_bOnShow)
            {
                m_bOnShow = false;
                m_bOnHide = true;
                m_fStartShowTime = 0.0f;
                m_fStartHideTime = Time.fixedTime;
                m_ItemRemind.SetActive(false);
                if (ItemRemindLogic.Instance() != null)
                {
                    ItemRemindLogic.Instance().HandleEquipRemind(false);
                }
                m_UseItemBuffer.RemoveAt(0);
            }
            // 未处在显示10秒和隐藏2秒状态 且自身需要显示
            else if (!m_ItemRemind.activeInHierarchy && !m_bOnHide)
            {
                //装备提醒 在的时候暂时不显示
                if (EquipRemindLogic.Instance() == null ||
                    (EquipRemindLogic.Instance() != null && EquipRemindLogic.Instance().m_EquipRemind.activeInHierarchy ==false))
                {
                    m_fStartShowTime = Time.fixedTime;
                    m_ItemRemind.SetActive(true);
                    if (ItemRemindLogic.Instance() != null)
                    {
                        ItemRemindLogic.Instance().HandleEquipRemind(true);
                    }

                    if (m_UseItemBuffer[0] == null || !m_UseItemBuffer[0].IsValid())
                    {
                        return;
                    }

                    Tab_CommonItem tabItem = TableManager.GetCommonItemByID(m_UseItemBuffer[0].DataID, 0);
                    if (tabItem != null)
                    {
                        m_IconSprite.spriteName = tabItem.Icon;
                        m_IconSprite.MakePixelPerfect();
                        m_QualitySprite.spriteName = GlobeVar.QualityColorGrid[tabItem.Quality - 1];
                        m_TitleNameLabel.text = tabItem.Name;
                        m_bOnShow = true;
                    }
                }
            }
            // 处在隐藏状态且时间已达到2秒
            if (Time.fixedTime - m_fStartHideTime >= HideTime && m_bOnHide)
            {
                m_bOnHide = false;
                m_fStartHideTime = 0.0f;
            }
        }
        else
        {
            UIManager.CloseUI(UIInfo.UseItemRemindRoot);
        }
    }

    void OnUseBtClick()
    {
        if (m_UseItemBuffer == null ||
            m_UseItemBuffer.Count <= 0 || 
            m_UseItemBuffer[0] == null || 
            !m_UseItemBuffer[0].IsValid())
        {
            return;
        }

        int canuse = TableManager.GetCommonItemByID(m_UseItemBuffer[0].DataID, 0).CanUse;
        if (canuse == 1)
        {
            CG_USE_ITEM useitem = (CG_USE_ITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_USE_ITEM);
            useitem.SetItemguid(m_UseItemBuffer[0].Guid);
            useitem.SendPacket();
        }
        m_bOnShow = false;
        m_bOnHide = false;
        m_fStartShowTime = 0.0f;
        m_fStartHideTime = 0.0f;
        m_UseItemBuffer.RemoveAt(0);
        m_ItemRemind.SetActive(false);
        if (ItemRemindLogic.Instance() != null)
        {
            ItemRemindLogic.Instance().HandleEquipRemind(false);
        }
    }
    
}
