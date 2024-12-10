using UnityEngine;
using System.Collections;
using GCGame.Table;
using System.Collections.Generic;
using Games.GlobeDefine;
using System;
using Games.Item;

public class QianKunDaiLogic : MonoBehaviour {

    public enum FORMULA_TYPE
    {
        TYPE_GEMLEVELUP = 1,
        TYPE_GEMREFRESH = 2,
        TYPE_NORMAL = 3,
    }

    private static QianKunDaiLogic m_Instance = null;
    public static QianKunDaiLogic Instance()
    {
        return m_Instance;

    }
    const int StuffSlotMax = 6;
    public ItemSlotLogic[] m_StuffSlotArray;
	public GameObject m_CombineBtn;
    public GameObject m_ChooseButton;
    public GameObject m_CancelChooseButton;
    public GameObject m_FormulaRoot;
    public GameObject m_FormulaContent;
    public GameObject m_NumChoose;
    public UIInput m_NumChooseInput;
    public UILabel m_MaxCombineCountLabel;
    public ItemSlotLogic m_ProductSlot;

    private UILabel m_FormulaContentLabel;
    private BoxCollider m_FormulaContentBox;
    private Dictionary<int, int> m_ChooseStuffKinds = new Dictionary<int, int>();
    private List<UInt64> m_ChooseStuffGuid = new List<UInt64>();
    private int m_CurCombineNum;
    private Tab_QianKunDaiFormula m_RightFormula = null;
    private int m_MaxCombineCount = 1;
    private bool m_CombineCD = false;
    private List<UInt64> deleGuid = new List<UInt64>();     // update_item包会调用 防止多次new 一开始new一个缓存住

	public GameObject  m_bagSprite;
	public GameObject  m_EquipPack;
	// 新手指引相关
	private int m_NewPlayerGuideFlag_Step = -1;
	public int NewPlayerGuideFlag_Step
	{
		get { return m_NewPlayerGuideFlag_Step; }
		set { m_NewPlayerGuideFlag_Step = value; }
	}

    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start () {
        
	}

    void OnEnable()
    {
        m_Instance = this;
        //m_ChooseButton.SetActive(true);
        //m_CancelChooseButton.SetActive(false);
        m_FormulaRoot.SetActive(false);
        m_NumChoose.SetActive(false);

        m_FormulaContentLabel = m_FormulaContent.GetComponent<UILabel>();
        m_FormulaContentBox = m_FormulaContent.GetComponent<BoxCollider>();

        for (int i = 0; i < StuffSlotMax; i++)
        {
            m_StuffSlotArray[i].ClearInfo();
        }

        ClearNumChoose();
        ClearProductSlot();
		m_bagSprite.SetActive (false);
		m_EquipPack.SetActive (false);
    }

    void OnDisable()
    {
        m_Instance = null;
        ClearData();
        CancelInvoke("CombineCDWait");
		m_bagSprite.SetActive (true);
		m_EquipPack.SetActive (true);
    }

    void ClearData()
    {
        m_ChooseStuffKinds.Clear();
        m_ChooseStuffGuid.Clear();
        m_CurCombineNum = 0;
        m_RightFormula = null;
        m_MaxCombineCount = 1;
        m_CombineCD = false;
    }

    void ClearProductSlot()
    {
        m_ProductSlot.ClearInfo();
        m_ProductSlot.SetIconDirect("wenhao");
        m_ProductSlot.SetOnClickDirect(ProductSlotOnClick);
    }

    void ProductSlotOnClick(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
    {
        if (nItemID == GlobeVar.INVALID_ID)
        {
            ShowFormula();
        }
        else
        {
            GameItem item = new GameItem();
            item.DataID = nItemID;
            if (item.IsValid())
            {
                if (item.IsEquipMent())
                {
                    EquipTooltipsLogic.ShowEquipTooltip(item, EquipTooltipsLogic.ShowType.UnEquiped);
                }
                else
                {
                    ItemTooltipsLogic.ShowItemTooltip(item, ItemTooltipsLogic.ShowType.Normal);
                }            
            }
        }
    }

    /*void BeginChoose()
    {
        m_ChooseButton.SetActive(false);
        m_CancelChooseButton.SetActive(true);
    }

    void CancelChoose()
    {
        m_ChooseButton.SetActive(true);
        m_CancelChooseButton.SetActive(false);
    }*/

    void CombineFormula()
    {
        if (m_ChooseStuffGuid.Count <= 0)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2079}");
            return;
        }

        bool bAllGem = IsAllGem();

        foreach (KeyValuePair<int, List<Tab_QianKunDaiFormula>> pair in TableManager.GetQianKunDaiFormula())
        {
            Tab_QianKunDaiFormula tabFormula = pair.Value[0];
            if (tabFormula == null)
            {
                continue;
            }

            if (IsAllStuffRight(tabFormula, bAllGem))
            {
                m_RightFormula = tabFormula;
                break;
            }
        }

        if (m_RightFormula == null)
        {
            // 不满足任何配方
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2083}");
        }
        else
        {
            for (int i = 0; i < m_ChooseStuffGuid.Count; i++ )
            {
                GameItem item = GameManager.gameManager.PlayerDataPool.BackPack.GetItemByGuid(m_ChooseStuffGuid[i]);
                if (item != null && item.IsValid())
                {
                    if (item.BindFlag)
                    {
                        MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2070}"), "", CombineFormulaOK, CombineFormulaCancel);
						//=====应策划需求去掉此处新手引导
						//MessageBoxLogic.Instance().NewPlayerGuide(1);
						break;
                    }
                    if (i == m_ChooseStuffGuid.Count - 1)
                    {
                        CombineFormulaOK();
                    }
                }
            }           
        }
    }

    bool IsAllGem()
    {
        for (int i = 0; i < m_ChooseStuffGuid.Count; i++ )
        {
            GameItem item = GameManager.gameManager.PlayerDataPool.BackPack.GetItemByGuid(m_ChooseStuffGuid[i]);
            if (item != null && item.IsValid())
            {
                if (!item.IsGem())
                {
                    return false;
                }
            }
        }
        return true;
    }

    void CombineFormulaOK()
    {
        // 确认可合成最大数量 若大于1 则弹出数量确认框
        // 最大合成数量 即为各种素材的最小组数
		
		if (MessageBoxLogic.Instance () != null && MessageBoxLogic.Instance ().NewPlayerGuideFlag_Step != -1) 
		{
			NewPlayerGuidLogic.CloseWindow();
			MessageBoxLogic.Instance ().NewPlayerGuideFlag_Step = -1;
			m_NewPlayerGuideFlag_Step = -1;
		}

        int nMinHoldGroup = 999;

        if (m_RightFormula.Type == (int)FORMULA_TYPE.TYPE_GEMLEVELUP)
        {
            if (m_ChooseStuffGuid.Count > 0)
            {
                GameItem item = GameManager.gameManager.PlayerDataPool.BackPack.GetItemByGuid(m_ChooseStuffGuid[0]);
                if (item != null && item.IsValid())
                {
                    if (m_ChooseStuffKinds.ContainsKey(item.DataID))
                    {
                        if (m_RightFormula.GetStuffCountbyIndex(0) > 0)
                        {
                            nMinHoldGroup = m_ChooseStuffKinds[item.DataID] / m_RightFormula.GetStuffCountbyIndex(0);
                        }
                        else
                        {
                            nMinHoldGroup = m_ChooseStuffKinds[item.DataID] / 3;
                        }
                    }
                }
            }            
        }
        else if (m_RightFormula.Type == (int)FORMULA_TYPE.TYPE_GEMREFRESH)
        {
            foreach(KeyValuePair<int, int> pair in m_ChooseStuffKinds)
            {
                if (nMinHoldGroup > pair.Value)
                {
                    nMinHoldGroup = pair.Value;
                }                
            }
        }
        else
        {
            // 查找配方时已经确定 玩家选择的材料种类数和配方中的种类数必须相等
            for (int i = 0; i < m_RightFormula.getStuffIDCount(); i++)
            {
                int nStuffID = m_RightFormula.GetStuffIDbyIndex(i);
                int nStuffCount = m_RightFormula.GetStuffCountbyIndex(i);
                if (nStuffID != GlobeVar.INVALID_ID)
                {
                    int nPlayerHoldCount = m_ChooseStuffKinds[nStuffID];
                    int nPlayerHoldGroup = nPlayerHoldCount / nStuffCount;
                    if (nMinHoldGroup > nPlayerHoldGroup)
                    {
                        nMinHoldGroup = nPlayerHoldGroup;
                    }
                }
            }
        }

        if (nMinHoldGroup > 1 && nMinHoldGroup < 999)
        {
            m_MaxCombineCount = nMinHoldGroup;
            m_NumChoose.SetActive(true);
            m_NumChooseInput.value = "1";
            m_MaxCombineCountLabel.text = StrDictionary.GetClientDictionaryString("#{2098}", nMinHoldGroup);
        }
        else
        {
            MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2996}", m_RightFormula.Money), "", MakeCombinePacket);
        }
    }

    void CombineFormulaCancel()
    {
        m_RightFormula = null;
    }

    void ShowFormula()
    {
        m_FormulaRoot.SetActive(true);

        /*m_FormulaContentLabel.text = "";
        foreach (KeyValuePair<string, List<Tab_QianKunDaiFormula>> pair in TableManager.GetQianKunDaiFormula())
        {
            Tab_QianKunDaiFormula tabFormula = pair.Value[0];
            if (tabFormula == null)
            {
                continue;
            }

            m_FormulaContentLabel.text += tabFormula.Name;
            m_FormulaContentLabel.text += "\n";
        }*/
        m_FormulaContentLabel.text = StrDictionary.GetClientDictionaryString("#{2118}").Replace("#r", "\n");

        m_FormulaContentBox.size = new Vector3(m_FormulaContentBox.size.x, m_FormulaContentLabel.printedSize.y, 0);
        m_FormulaContentBox.center = new Vector3(m_FormulaContentBox.center.x, -m_FormulaContentLabel.printedSize.y / 2, 0);
    }

    void CloseFormulaRoot()
    {
        m_FormulaRoot.SetActive(false);
    }

    public void ChooseStuff(GameItem chooseitem, ItemSlotLogic slot)
    {
        if (chooseitem == null || !chooseitem.IsValid())
        {
            return;
        }
        // 格子数量上限9
        int nCurStuffNum = m_ChooseStuffGuid.Count;
        if (nCurStuffNum >= StuffSlotMax)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2076}");
            return;
        }

        // 不可重复放入物品
        for (int i = 0; i < m_ChooseStuffGuid.Count; i++)
        {
            GameItem item = GameManager.gameManager.PlayerDataPool.BackPack.GetItemByGuid(m_ChooseStuffGuid[i]);
            if (item != null && item.IsValid())
            {
                if (item.Guid == chooseitem.Guid)
                {
                    Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2173}");
                    return;
                }
            }
        }

        if (m_ChooseStuffKinds.ContainsKey(chooseitem.DataID))
        {
            m_ChooseStuffKinds[chooseitem.DataID] += chooseitem.StackCount;
        }
        else
        {
            m_ChooseStuffKinds.Add(chooseitem.DataID, chooseitem.StackCount);
        }

        m_StuffSlotArray[nCurStuffNum].InitInfo_Item(chooseitem.DataID, StuffSlotOnClick, chooseitem.StackCount.ToString());
        m_StuffSlotArray[nCurStuffNum].ItemSlotEnable();
        m_ChooseStuffGuid.Add(chooseitem.Guid);

        slot.SetItemSlotChoose(true);

        ClearProductSlot();
    }

    void StuffSlotOnClick(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
    {
        int nDeleteIndex = GlobeVar.INVALID_ID;
        for (int i = 0; i < StuffSlotMax; i++ )
        {
            if (i < m_ChooseStuffGuid.Count)
            {
                if (m_StuffSlotArray[i].gameObject.name == strSlotName)
                {
                    GameItem item = GameManager.gameManager.PlayerDataPool.BackPack.GetItemByGuid(m_ChooseStuffGuid[i]);
                    if (item != null && item.IsValid())
                    {
                        if (m_ChooseStuffKinds.ContainsKey(item.DataID))
                        {
                            m_ChooseStuffKinds[item.DataID] -= item.StackCount;
                            if (m_ChooseStuffKinds[item.DataID] == 0)
                            {
                                m_ChooseStuffKinds.Remove(item.DataID);
                            }
                            if (BackPackLogic.Instance() != null)
                            {
                                BackPackLogic.Instance().CancelItemSlotChoose(item.Guid);
                            }
                            m_ChooseStuffGuid.Remove(item.Guid);
                            nDeleteIndex = i;
                            break;
                        }
                    }
                }               
            }
        }

        if (nDeleteIndex != GlobeVar.INVALID_ID)
        {
            for (int i = nDeleteIndex; i < StuffSlotMax; i++ )
            {
                if (i < m_ChooseStuffGuid.Count)
                {
                    GameItem item = GameManager.gameManager.PlayerDataPool.BackPack.GetItemByGuid(m_ChooseStuffGuid[i]);
                    if (item != null && item.IsValid())
                    {
                        m_StuffSlotArray[i].InitInfo_Item(item.DataID, StuffSlotOnClick, item.StackCount.ToString());
                        m_StuffSlotArray[i].ItemSlotEnable();
                    }                    
                }
                else
                {
                    m_StuffSlotArray[i].ClearInfo();
                }
            }            
        }

        ClearProductSlot();
    }

    void OnClickDelNum()
    {
        if (m_CurCombineNum >= 1)
        {
            m_CurCombineNum -= 1;
            m_NumChooseInput.value = m_CurCombineNum.ToString();
        }
		if (m_CurCombineNum < 1)
		{
			m_CurCombineNum = m_MaxCombineCount;
			m_NumChooseInput.value = m_CurCombineNum.ToString();
		}
    }

    void OnClickAddNum()
    {
        if (m_CurCombineNum <= m_MaxCombineCount)
        {
            m_CurCombineNum += 1;
            m_NumChooseInput.value = m_CurCombineNum.ToString();
        }
		if (m_CurCombineNum >m_MaxCombineCount)
		{
			m_CurCombineNum = 1;
			m_NumChooseInput.value = m_CurCombineNum.ToString();
		}
    }

    public void NumChooseInput()
    {
        bool bCanParse = int.TryParse(m_NumChooseInput.value, out m_CurCombineNum);
        if (bCanParse)
        {
            if (m_CurCombineNum > m_MaxCombineCount)
            {
                m_CurCombineNum = m_MaxCombineCount;
                m_NumChooseInput.value = m_CurCombineNum.ToString();
            }            
        }
    }

    void NumChooseCancel()
    {
        m_RightFormula = null;
        ClearNumChoose();
    }

    void NumChooseOK()
    {
        bool bCanParse = int.TryParse(m_NumChooseInput.value, out m_CurCombineNum);
        if (bCanParse)
        {
			if (m_CurCombineNum > m_MaxCombineCount||m_CurCombineNum<=0)
            {
                Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2082}");
            }  
            else
            {
                MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2996}", m_RightFormula.Money * m_CurCombineNum), "", MakeCombinePacket);
            }
        }
    }

    bool IsGemFormulaSameLevel(Tab_QianKunDaiFormula tabFormula)
    {
        if (tabFormula == null)
        {
            return false;
        }

        int nStuffLevel = tabFormula.GetStuffIDbyIndex(0);
        if (nStuffLevel == GlobeVar.INVALID_ID)
        {
            return false;
        }

        if (GetChooseGemLevelCount() != 1)
        {
            return false;
        }

        if (m_ChooseStuffGuid.Count <= 0)
        {
            return false;
        }

        GameItem item = GameManager.gameManager.PlayerDataPool.BackPack.GetItemByGuid(m_ChooseStuffGuid[0]);
        if (item == null || !item.IsValid())
        {
            return false;
        }

        Tab_GemAttr tabGem = TableManager.GetGemAttrByID(item.DataID, 0);
        if (tabGem == null)
        {
            return false;
        }

        if (nStuffLevel != tabGem.Level)
        {
            return false;
        }

        return true;
    }

    bool IsAllStuffRight(Tab_QianKunDaiFormula tabFormula, bool bAllGem)
    {

        // 暂时宝石参与的乾坤袋配方 只有升级和洗宝石两种 全宝石不考虑普通合成
        if (tabFormula.Type == (int)FORMULA_TYPE.TYPE_GEMLEVELUP)
        {
            // 只可能3个同级 同属性宝石
            if (!bAllGem)
            {
                return false;
            }

            // 先判断同等级 选择宝石等级数量为1 且需要等于配方填的等级
            if (false == IsGemFormulaSameLevel(tabFormula))
            {
                return false;
            }

            // 判断同属性
            if (GetChooseGemClassCount() != 1)
            {
                return false;
            }

            return true;
        }
        else if (tabFormula.Type == (int)FORMULA_TYPE.TYPE_GEMREFRESH)
        {
            // 只可能3个同级 属性互不相同宝石
            if (!bAllGem)
            {
                return false;
            }

            // 先判断同等级 选择宝石等级数量为1 且需要等于配方填的等级
            if (false == IsGemFormulaSameLevel(tabFormula))
            {
                return false;
            }

            // 判断属性互不相同
            if (GetChooseGemClassCount() != tabFormula.GetStuffCountbyIndex(0))
            {
                return false;
            }

            return true;
        }
        else
        {
            int nStuffKinds = 0;
            for (int i = 0; i < tabFormula.getStuffIDCount(); i++)
            {
                if (tabFormula.GetStuffIDbyIndex(i) != GlobeVar.INVALID_ID)
                {
                    nStuffKinds += 1;
                }
            }

            if (nStuffKinds != m_ChooseStuffKinds.Count || nStuffKinds <= 0)
            {
                return false;
            }

            for (int i = 0; i < tabFormula.getStuffIDCount(); i++)
            {
                if (!m_ChooseStuffKinds.ContainsKey(tabFormula.GetStuffIDbyIndex(i)) &&
                    tabFormula.GetStuffIDbyIndex(i) != GlobeVar.INVALID_ID)
                {
                    return false;
                }
            }

            return true;
        }
    }

    int GetChooseGemLevelCount()
    {
        List<int> levelkinds = new List<int>();
        foreach(KeyValuePair<int, int> pair in m_ChooseStuffKinds)
        {
            int nGemDataID = pair.Key;
            Tab_GemAttr tabGem = TableManager.GetGemAttrByID(nGemDataID, 0);
            if (tabGem != null)
            {
                if (!levelkinds.Contains(tabGem.Level))
                {
                    levelkinds.Add(tabGem.Level);
                } 
            }
        }
        return levelkinds.Count;
    }

    int GetChooseGemClassCount()
    {
        List<int> classkinds = new List<int>();
        foreach (KeyValuePair<int, int> pair in m_ChooseStuffKinds)
        {
            int nGemDataID = pair.Key;
            Tab_GemAttr tabGem = TableManager.GetGemAttrByID(nGemDataID, 0);
            if (tabGem != null)
            {
                if (!classkinds.Contains(tabGem.AttrClass))
                {
                    classkinds.Add(tabGem.AttrClass);
                }
            }
        }
        return classkinds.Count;
    }

    void ClearNumChoose()
    {
        m_CurCombineNum = 1;
        m_NumChooseInput.value = "";
        m_NumChoose.SetActive(false);
    }

    void MakeCombinePacket()
    {
        if (m_CombineCD)
        {
            return;
        }

        CG_QIANKUNDAI_COMBINE packet = (CG_QIANKUNDAI_COMBINE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_QIANKUNDAI_COMBINE);
		packet.TableID=m_RightFormula.Id;
        for (int i = 0; i < m_ChooseStuffGuid.Count; i++)
        {
            GameItem item = GameManager.gameManager.PlayerDataPool.BackPack.GetItemByGuid(m_ChooseStuffGuid[i]);

            if (item != null && item.IsValid())
            {
				ulong guid=item.Guid;
				packet.AddStuffGUID(guid);
			}
		}

        if (m_CurCombineNum >= 1)
        {
			packet.MakeCount = (uint)m_CurCombineNum;
        }        
        packet.SendPacket();

        m_RightFormula = null;
        ClearNumChoose();

        m_CombineCD = true;
        InvokeRepeating("CombineCDWait", 1, 999);
    }

    void CombineCDWait()
    {
        m_CombineCD = false;
        CancelInvoke("CombineCDWait");
    }

    public void HandleUpdateItemPacket()
    {
        deleGuid.Clear();

        for (int i = 0; i < m_ChooseStuffGuid.Count; i++)
        {
            GameItem item = GameManager.gameManager.PlayerDataPool.BackPack.GetItemByGuid(m_ChooseStuffGuid[i]);
            if (item == null || !item.IsValid())
            {
                deleGuid.Add(m_ChooseStuffGuid[i]);
            }
        }

        for (int i = 0; i < deleGuid.Count; i++ )
        {
            m_ChooseStuffGuid.Remove(deleGuid[i]);
        }

        for (int i = 0; i < StuffSlotMax; i++ )
        {
            if (i < m_ChooseStuffGuid.Count)
            {
                GameItem item = GameManager.gameManager.PlayerDataPool.BackPack.GetItemByGuid(m_ChooseStuffGuid[i]);
                if (item != null && item.IsValid())
                {
                    m_StuffSlotArray[i].InitInfo_Item(item.DataID, StuffSlotOnClick, item.StackCount.ToString());
                    m_StuffSlotArray[i].ItemSlotEnable();
                }
            }
            else
            {
                m_StuffSlotArray[i].ClearInfo();
            }
        }

        ResetChooseStuffKinds();
	
    }

    void ResetChooseStuffKinds()
    {
        m_ChooseStuffKinds.Clear();
        for (int i = 0; i < m_ChooseStuffGuid.Count; i++)
        {
            GameItem item = GameManager.gameManager.PlayerDataPool.BackPack.GetItemByGuid(m_ChooseStuffGuid[i]);
            if (item != null && item.IsValid())
            {
                if (m_ChooseStuffKinds.ContainsKey(item.DataID))
                {
                    m_ChooseStuffKinds[item.DataID] += item.StackCount;
                }
                else
                {
                    m_ChooseStuffKinds.Add(item.DataID, item.StackCount);
                }
            }
        }
    }

    public bool IsInStuffChoose(GameItem chooseitem)
    {
        if (chooseitem == null || !chooseitem.IsValid())
        {
            return false;
        }

        if (chooseitem.Guid == GlobeVar.INVALID_GUID)
        {
            return false;
        }

        for (int i = 0; i < m_ChooseStuffGuid.Count; ++i)
        {
            GameItem item = GameManager.gameManager.PlayerDataPool.BackPack.GetItemByGuid(m_ChooseStuffGuid[i]);
            if (item != null && item.IsValid())
            {
                if (chooseitem.Guid == item.Guid)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void HandleQianKunDaiProduct(int nProductDataID)
    {
        m_ProductSlot.InitInfo_Item(nProductDataID, ProductSlotOnClick);
    }

	public void NewPlayerGuide(int nIndex)
	{
		if (nIndex < 0) 
		{
			return;		
		}
		
		NewPlayerGuidLogic.CloseWindow();
		
		m_NewPlayerGuideFlag_Step = nIndex;
		
		switch (m_NewPlayerGuideFlag_Step)
		{
		case 1:
			if(m_CombineBtn != null)
			{
				NewPlayerGuidLogic.OpenWindow(m_CombineBtn, 180, 64, "", "right", 2, true, true);
			}
			break;
		}
	}
}
