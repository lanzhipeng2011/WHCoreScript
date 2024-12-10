using UnityEngine;
using System.Collections;
using Games.Item;
using Games.GlobeDefine;

public class ChargeActivityLogic_SC : MonoBehaviour {

    private int[] EquipId = { 59007, 59008, 59009, 59010 }; //橙色武器ID（少林 天山 逍遥 大理）
    const int PrizeItemId1 = 23;     //玄铁ID 
    const int PrizeItemNum1 = 5;     //玄铁数量
    const int PrizeItemId2 = 24;     //打星石ID
    const int PrizeItemNum2 = 5;     //打星石数量

    public ItemSlotLogic m_ItemSlot1;   //橙色武器
    public ItemSlotLogic m_ItemSlot2;   //元宝
    public ItemSlotLogic m_ItemSlot3;   //玄铁
    public ItemSlotLogic m_ItemSlot4;   //打星石

    private static ChargeActivityLogic_SC m_Instance = null;
    public static ChargeActivityLogic_SC Instance()
    {
        return m_Instance;
    }

    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start () {
        InitPrize();
	}

    public void InitPrize()
    {
        m_ItemSlot1.InitInfo_Item(GetPrizeEquipId(), ItemSlotLogic.OnClickOpenTips);
        m_ItemSlot2.InitInfo_YuanbaoBind();
        m_ItemSlot3.InitInfo_Item(PrizeItemId1, ItemSlotLogic.OnClickOpenTips, PrizeItemNum1.ToString(), true);
        m_ItemSlot4.InitInfo_Item(PrizeItemId2, ItemSlotLogic.OnClickOpenTips, PrizeItemNum2.ToString(), true);
    }

    int GetPrizeEquipId()
    {
        int profession = GameManager.gameManager.PlayerDataPool.Profession;
        int PrizeEquipId = EquipId[0];
        switch ((CharacterDefine.PROFESSION)profession)
        {
            case CharacterDefine.PROFESSION.SHAOLIN:
                PrizeEquipId = EquipId[0];
                break;
            case CharacterDefine.PROFESSION.TIANSHAN:
                PrizeEquipId = EquipId[1];
                break;
            case CharacterDefine.PROFESSION.XIAOYAO:
                PrizeEquipId = EquipId[2];
                break;
            case CharacterDefine.PROFESSION.DALI:
                PrizeEquipId = EquipId[3];
                break;
            default:
                PrizeEquipId = EquipId[0];
                break;
        }
        return PrizeEquipId;
    }

    
}
