using UnityEngine;
using System.Collections;
using Games.Fellow;
using GCGame.Table;
using System.Collections.Generic;
using GCGame;
using Games.Item;
using Module.Log;

public class PartnerFrameLogic_Skill : MonoBehaviour {

    public GameObject m_PartnerSkillListGrid;
    public UISprite m_PartnerChooseSprite;
    public UILabel m_PartnerChooseName;
    public UISprite m_PartnerChooseQualitySprite;
    public UISprite[] m_OwnSkill;
    public UISprite[] m_OwnSkillBg;

    public GameObject m_SkillInfo;

    public UISprite m_SkillIconSprite;
    public UISprite m_SkillQualityFrame;
    public UILabel m_SkillNameLabel;
    public UILabel m_SkillEffectLabel;
    public UILabel m_NextSkillEffectLabel;
    public ItemSlotLogic m_LevelUpItem;
    public UILabel m_LevelUpItemLabel;
    public UILabel m_ActiveItemLabel;
    public UILabel m_NextSkillLabel;

    public GameObject m_NextSkillEffect;
    public GameObject m_CurSkillEffect;

    public UIImageButton m_EquipButton;
    public UIImageButton m_UnEquipButton;
    public UIImageButton m_LevelUpButton;
    public UIImageButton m_ActiveButton;

    private Fellow m_Fellow = null;
    private int m_SkillIndex = 0;
    private int m_SkillId = -1;
    //private bool m_SkillActive = false;
    private List<int> m_allSkillList = null;

    public static int GetPartnerSkillCanActiveNum()
    {
        int result = 0;
        List<int> totalSkillList = GetTotalSkillList();
        List<int> activeSkillList = GameManager.gameManager.PlayerDataPool.ActiveFellowSkill;
        List<int> unActiveSkillList = GetUnActiveList(totalSkillList, activeSkillList);
        for (int index = 0; index < unActiveSkillList.Count; index++)
        {
            Tab_FellowSkill line = TableManager.GetFellowSkillByID(unActiveSkillList[index], 0);
            if (line != null)
            {
                int needNum = line.ActiveConsumeNum;
                int realNum = GameManager.gameManager.PlayerDataPool.BackPack.GetItemCountByDataId(line.ActiveConsumeSubType);
                if (realNum >= needNum)
                {
                    FellowContainer fellowContainer = GameManager.gameManager.PlayerDataPool.FellowContainer;
                    if ( (line.SkillClass == (int)FELLOWCLASS.HUNMAN && fellowContainer.IsHaveHumanFellow()) ||
                         (line.SkillClass == (int)FELLOWCLASS.ANIMAL && fellowContainer.IsHaveAnimalFellow()) )
                    {
                        result += 1;
                    }
                }
            }
        }
        return result;
    }

    private static PartnerFrameLogic_Skill m_Instance = null;
    public static PartnerFrameLogic_Skill Instance()
    {
        return m_Instance;
    }
	
    void OnEnable()
    {
        m_Instance = this;

        if (m_Fellow == null)
        {
            UpdateEmpty_Skill();
        }
    }

    void OnDisable()
    {
        m_Instance = null;
    }

    public void UpdateEmpty_Skill()
    {
        m_PartnerChooseSprite.gameObject.SetActive(false);
        m_PartnerChooseQualitySprite.gameObject.SetActive(false);
        m_LevelUpItemLabel.gameObject.SetActive(false);
        m_ActiveItemLabel.gameObject.SetActive(false);
        m_LevelUpItem.gameObject.SetActive(false);
        m_CurSkillEffect.gameObject.SetActive(false);
        m_NextSkillEffect.gameObject.SetActive(false);
        //按钮全部隐藏
        m_EquipButton.gameObject.SetActive(false);
        m_UnEquipButton.gameObject.SetActive(false);
        m_LevelUpButton.gameObject.SetActive(false);
        m_ActiveButton.gameObject.SetActive(false);
    }

    public void UpdateFellow_Skill(Fellow fellow)
    {
        UpdatePartnerChoose(fellow);
        InitPartnerSkillList();
    }

    public void InitPartnerSkillList()
    {
        UIManager.LoadItem(UIInfo.PartnerSkillItem, OnLoadPartnerSkillItem);
    }

    void OnLoadPartnerSkillItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load partner skill item error");
            return;
        }
        if (m_PartnerSkillListGrid != null)
        {
            m_LevelUpItem.ClearInfo();
            //先清空
            PartnerSkillItemLogic[] fellowSkill = m_PartnerSkillListGrid.GetComponentsInChildren<PartnerSkillItemLogic>();
            for (int i = 0; i < fellowSkill.Length; ++i)
            {
                Destroy(fellowSkill[i].gameObject);
            }
            m_PartnerSkillListGrid.transform.DetachChildren();

            List<int> totalSkillList = new List<int>();
            List<int> activeSkillList = GameManager.gameManager.PlayerDataPool.ActiveFellowSkill;
            //遍历表格找出所有一级技能
            //int tableCount = TableManager.GetFellowSkill().Count;
            foreach (KeyValuePair<int, List<Tab_FellowSkill>> kvp in TableManager.GetFellowSkill())
            {
                Tab_FellowSkill line = kvp.Value[0];
                if (line != null)
                {
                    if (line.Level == 1)
                    {
                        totalSkillList.Add(line.Id);
                    }
                }
            }
            //符合当前伙伴分类的技能
            totalSkillList = GetSameClassList(totalSkillList, m_Fellow);
            //实际应该显示的技能ID
            List<int> realSkillList = GetRealTotalList(totalSkillList, activeSkillList);
            m_allSkillList = realSkillList;
            //所有技能中尚未激活的ID
            List<int> unActiveSkillList = GetUnActiveList(totalSkillList, activeSkillList);

            bool bFirst = true;
            for (int index = 0; index < realSkillList.Count; index++ )
            {
                GameObject SkillObject = Utils.BindObjToParent(resItem, m_PartnerSkillListGrid, (index + 100).ToString());
                if (SkillObject != null)
                {
                    int skillId = realSkillList[index];
                    if (unActiveSkillList.Contains(skillId) == false)
                    {
                        //已激活
                        SkillObject.GetComponent<PartnerSkillItemLogic>().UpdatePartnerSkill(skillId, true, m_Fellow);
                    }
                    else
                    {
                        //未激活
                        SkillObject.GetComponent<PartnerSkillItemLogic>().UpdatePartnerSkill(skillId, false, m_Fellow);
                    }
                    //选中第一个
                    if (bFirst)
                    {
                        if (m_SkillIndex >= 0 && m_SkillIndex < m_allSkillList.Count)
                        {
                            if (index == m_SkillIndex)
                            {
                                SkillObject.GetComponent<PartnerSkillItemLogic>().SetChoosed();
                                bFirst = false;
                            }
                        }
                        else
                        {
                            SkillObject.GetComponent<PartnerSkillItemLogic>().SetChoosed();
                            bFirst = false;
                        }
                    }
                }
            }
            m_PartnerSkillListGrid.GetComponent<UIGrid>().repositionNow = true;
        }
    }

    void UpdatePartnerChoose(Fellow fellow)
    {
        m_PartnerChooseSprite.spriteName = fellow.GetIcon();
        m_PartnerChooseSprite.MakePixelPerfect();
        m_PartnerChooseSprite.gameObject.SetActive(true);
        m_PartnerChooseName.text = fellow.Name;
        m_PartnerChooseQualitySprite.spriteName = FellowTool.GetFellowSkillQualityFrame(fellow.Quality);
        m_PartnerChooseQualitySprite.gameObject.SetActive(true);

        for (int index = 0; index < Fellow.FELLOW_MAXOWNSKILL; index++ )
        {
            int ownSkillId = fellow.GetOwnSkillId(index);
            Tab_SkillEx line = TableManager.GetSkillExByID(ownSkillId, 0);
            Tab_FellowSkill lineFellowSkill = TableManager.GetFellowSkillByID(ownSkillId, 0);
            if (line != null && lineFellowSkill != null)
            {
                int baseId = line.BaseId;
                Tab_SkillBase baseLine = TableManager.GetSkillBaseByID(baseId, 0);
                if (baseLine != null)
                {
                    m_OwnSkill[index].spriteName = baseLine.Icon;
                    m_OwnSkillBg[index].spriteName = FellowTool.GetFellowSkillQualityFrame(lineFellowSkill.Quality);
                    //m_OwnSkill[index].MakePixelPerfect();
                }
            }
            else
            {
				m_OwnSkill[index].spriteName = "";//fellowskill0
                m_OwnSkillBg[index].spriteName = "QualityGrey";
            }
        }

        m_Fellow = fellow;
    }

    private List<int> GetRealTotalList(List<int> total, List<int> active)
    {
        List<int> resultList = new List<int>();
        resultList.AddRange(total);
        for (int i = 0; i < active.Count; ++i)
        {
            Tab_FellowSkill line = TableManager.GetFellowSkillByID(active[i], 0);
            if (line != null)
            {
                int index = resultList.IndexOf(line.BaseSkill);
                if (index >= 0)
                {
                    resultList[index] = active[i];
                }
            }
        }
        return resultList;
    }

    private static List<int> GetUnActiveList(List<int> total, List<int> active)
    {
        List<int> resultList = new List<int>();
        resultList.AddRange(total);
        for (int i = 0; i < active.Count; ++i)
        {
            Tab_FellowSkill line = TableManager.GetFellowSkillByID(active[i], 0);
            if (line != null)
            {
                resultList.Remove(line.BaseSkill);
            }
        }
        return resultList;
    }

    private static List<int> GetSameClassList(List<int> total, Fellow fellow)
    {
        List<int> resultList = new List<int>();
        List<int> removeList = new List<int>();
        resultList.AddRange(total);
        for (int i = 0; i < resultList.Count; ++i)
        {
            Tab_FellowSkill line = TableManager.GetFellowSkillByID(resultList[i], 0);
            if (line != null && fellow != null)
            {
                if (line.SkillClass != fellow.GetClassId())
                {
                    removeList.Add(line.BaseSkill);
                }
            }
        }
        for (int index = 0; index < removeList.Count; index++)
        {
            resultList.Remove(removeList[index]);
        }
        return resultList;
    }

    static List<int> GetTotalSkillList()
    {
        List<int> totalSkillList = new List<int>();
        //遍历表格找出所有一级技能
        foreach (KeyValuePair<int, List<Tab_FellowSkill>> kvp in TableManager.GetFellowSkill())
        {
            Tab_FellowSkill line = kvp.Value[0];
            if (line != null)
            {
                if (line.Level == 1)
                {
                    totalSkillList.Add(line.Id);
                }
            }
        }
        return totalSkillList;
    }

    public void OnClickPartnerSkill(int skillId, bool isActive, bool bShowInfo)
    {
        if (m_SkillInfo.activeSelf == false && bShowInfo == false)
        {
            return;
        }
        //展现技能操作界面
        m_SkillInfo.gameObject.SetActive(true);

        bool isOwnSkill = false;
        bool isHighestSkill = false;
        int skillQuality = 0;

        m_LevelUpItemLabel.gameObject.SetActive(false);
        m_ActiveItemLabel.gameObject.SetActive(false);
        m_LevelUpItem.gameObject.SetActive(false);

        m_NextSkillEffect.gameObject.SetActive(false);
        m_CurSkillEffect.gameObject.SetActive(true);

        if (isActive)
        {
            m_NextSkillEffectLabel.text = "";
            Tab_FellowSkill lineFellowSkill = TableManager.GetFellowSkillByID(skillId, 0);
            if (lineFellowSkill != null)
            {
                int nextLevelId = lineFellowSkill.NextSkillId;
                if (nextLevelId == -1)
                {
                    //最高级技能
                    isHighestSkill = true;
                }
                else
                {
                    Tab_SkillEx lineNextSkillEx = TableManager.GetSkillExByID(nextLevelId, 0);
                    if (lineNextSkillEx != null)
                    {
                        //下一级效果
                        m_NextSkillEffectLabel.text = lineNextSkillEx.Desc;
                        m_NextSkillEffect.gameObject.SetActive(true);
                    }

                    m_LevelUpItemLabel.gameObject.SetActive(true);
                    int itemId = lineFellowSkill.LevelupConsumeSubType;
                    int itemNum = lineFellowSkill.LevelupConsumeNum;
                    m_LevelUpItem.gameObject.SetActive(true);
                    m_LevelUpItem.InitInfo(ItemSlotLogic.SLOT_TYPE.TYPE_ITEM, itemId, OnClickSkillItem, itemNum.ToString(), true);
                }
                skillQuality = lineFellowSkill.Quality;
            }
        }
        else
        {
            Tab_FellowSkill lineFellowSkill = TableManager.GetFellowSkillByID(skillId, 0);
            if (lineFellowSkill != null)
            {
                //只有1级技能才能激活获得
                if (lineFellowSkill.Level == 1)
                {
                    m_ActiveItemLabel.gameObject.SetActive(true);
                    int itemId = lineFellowSkill.ActiveConsumeSubType;
                    int itemNum = lineFellowSkill.ActiveConsumeNum;
                    m_LevelUpItem.gameObject.SetActive(true);
                    m_LevelUpItem.InitInfo(ItemSlotLogic.SLOT_TYPE.TYPE_ITEM, itemId, OnClickSkillItem, itemNum.ToString(), true);
                }
                skillQuality = lineFellowSkill.Quality;
            }
        }

        Tab_SkillEx lineSkillEx = TableManager.GetSkillExByID(skillId, 0);
        if (lineSkillEx != null)
        {
            Tab_SkillBase _SkillBase = TableManager.GetSkillBaseByID(lineSkillEx.BaseId, 0);
            if (_SkillBase !=null)
            {
                //技能名
                m_SkillNameLabel.text = _SkillBase.Name;
                //技能效果
                m_SkillEffectLabel.text = lineSkillEx.Desc;
                //技能图标
                m_SkillIconSprite.spriteName = _SkillBase.Icon;
                //技能品质框
                m_SkillQualityFrame.spriteName = FellowTool.GetFellowSkillQualityFrame(skillQuality);
            }
        }
        
        for (int index = 0; index < Fellow.FELLOW_MAXOWNSKILL; index++)
        {
            int ownSkillId = m_Fellow.GetOwnSkillId(index);
            if (ownSkillId == skillId)
            {
                //已经装备的技能
                isOwnSkill = true;
                break;
            }
        }


        //按钮全部隐藏
        m_EquipButton.gameObject.SetActive(false);
        m_UnEquipButton.gameObject.SetActive(false);
        m_LevelUpButton.gameObject.SetActive(false);
        m_ActiveButton.gameObject.SetActive(false);
        if (isActive)
        {
            if (isHighestSkill == false)
            {
                m_LevelUpButton.gameObject.SetActive(true);
                //m_NextEffectBtn.gameObject.SetActive(true);
            }

            if (isOwnSkill)
            {
                //显示卸载按钮
                m_UnEquipButton.gameObject.SetActive(true);
            }
            else
            {
                //显示装备按钮
                m_EquipButton.gameObject.SetActive(true);
            }
        }
        else
        {
            //显示激活按钮
            m_ActiveButton.gameObject.SetActive(true);
        }

        m_SkillId = skillId;
        //m_SkillActive = isActive;
        m_SkillIndex = m_allSkillList.IndexOf(skillId);
    }

    public void OnClickEquipSKill()
    {
        if (m_SkillId <= 0)
        {
            //请先选择技能
            return;
        }
        CG_EQUIP_FELLOW_SKILL fellowPacket = (CG_EQUIP_FELLOW_SKILL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_EQUIP_FELLOW_SKILL);
        fellowPacket.SetFellowguid(m_Fellow.Guid);
        fellowPacket.SetSkillId(m_SkillId);
        fellowPacket.SendPacket();
    }

    public void OnClickUnEquipSkill()
    {
        if (m_SkillId <= 0)
        {
            //请先选择技能
            return;
        }
        CG_UNEQUIP_FELLOW_SKILL fellowPacket = (CG_UNEQUIP_FELLOW_SKILL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_UNEQUIP_FELLOW_SKILL);
        fellowPacket.SetFellowguid(m_Fellow.Guid);
        fellowPacket.SetSkillId(m_SkillId);
        fellowPacket.SendPacket();
    }

    public void OnClickLevelUpSkill()
    {
        if (m_SkillId <= 0)
        {
            //请先选择技能
            return;
        }

        //消耗物品是否足够
        Tab_FellowSkill line = TableManager.GetFellowSkillByID(m_SkillId, 0);
        if (line == null)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1016}");
            return;
        }
        GameItemContainer backpack = GameManager.gameManager.PlayerDataPool.BackPack;
        if (backpack.GetItemCountByDataId(line.LevelupConsumeSubType) < line.LevelupConsumeNum)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1016}");
            return;
        }

        if (null != GameManager.gameManager)
        {
            GameManager.gameManager.SoundManager.PlaySoundEffect(28);       //intensify
        }

        CG_LEVELUP_FELLOW_SKILL fellowPacket = (CG_LEVELUP_FELLOW_SKILL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_LEVELUP_FELLOW_SKILL);
        fellowPacket.SetSkillId(m_SkillId);
        fellowPacket.SendPacket();
        //m_SkillId = -1;
    }

    public void OnClickActiveSkill()
    {
        if (m_SkillId <= 0)
        {
            //请先选择技能
            return;
        }

        //消耗物品是否足够
        Tab_FellowSkill line = TableManager.GetFellowSkillByID(m_SkillId, 0);
        if (line == null)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1016}");
            return;
        }
        GameItemContainer backpack = GameManager.gameManager.PlayerDataPool.BackPack;
        if (backpack.GetItemCountByDataId(line.ActiveConsumeSubType) < line.ActiveConsumeNum)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1016}");
            return;
        }

        if (null != GameManager.gameManager)
        {
            GameManager.gameManager.SoundManager.PlaySoundEffect(28);      //intensify
        }

        CG_ACTIVE_FELLOW_SKILL fellowPacket = (CG_ACTIVE_FELLOW_SKILL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ACTIVE_FELLOW_SKILL);
        fellowPacket.SetSkillId(m_SkillId);
        fellowPacket.SendPacket();
    }


    public void OnClickSkillItem(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
    {
        if (nItemID >= 0)
        {
            GameItem item = new GameItem();
            item.DataID = nItemID;
            ItemTooltipsLogic.ShowItemTooltip(item, ItemTooltipsLogic.ShowType.Info);
        }
    }

    public void OnReturnSkillInfo()
    {
        m_SkillInfo.gameObject.SetActive(false);
    }

    public void ClickSkillItemById(int skillId)
    {
        if (skillId <= 0)
        {
            return;
        }

        PartnerSkillItemLogic[] item = m_PartnerSkillListGrid.gameObject.GetComponentsInChildren<PartnerSkillItemLogic>();
        for (int i = 0; i < item.Length; ++i)
        {
            if (null != item[i])
            {
                if (item[i].m_SkillId == skillId)
                {
                    item[i].OnClickPartnerSkill();
                }
            }
        }
    }

    public void OnClickEquipSkill(int index)
    {
        if (m_Fellow != null)
        {
            int skillId = m_Fellow.GetOwnSkillId(index);
            if (skillId > 0)
            {
                ClickSkillItemById(skillId);
            }
        }
    }

    public void OnClickEquipSkill1()
    {
        OnClickEquipSkill(0);
    }

    public void OnClickEquipSkill2()
    {
        OnClickEquipSkill(1);
    }

    public void OnClickEquipSkill3()
    {
        OnClickEquipSkill(2);
    }

    public void OnClickEquipSkill4()
    {
        OnClickEquipSkill(3);
    }

    public void OnClickEquipSkill5()
    {
        OnClickEquipSkill(4);
    }

    public void OnClickEquipSkill6()
    {
        OnClickEquipSkill(5);
    }
}