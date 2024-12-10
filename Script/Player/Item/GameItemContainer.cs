//********************************************************************
// 文件名: GameItemContainer.cs
// 描述: 物品容器
// 作者: TangYi
// 创建时间: 2013-12-25
//
// 修改历史:
//********************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GCGame.Table;
using Games.GlobeDefine;

namespace Games.Item
{
	public class GameItemContainer
	{
        public enum Type
        {
            TYPE_INVALID = -1,
            TYPE_BACKPACK = 0,          // 背包
            TYPE_EQUIPPACK,             // 装备
            TYPE_BUYBACKPACK = 2,       // 回购
            TYPE_STORAGEPACK = 3,       // 仓库
            TYPE_FASHION = 4,           // 时装
            TYPE_GEM = 5,               // 宝石
            TYPE_GUILDSHOP = 6,         // 帮会仓库
        }

        // 背包默认容量
        public const int SIZE_BACKPACK = 60;
        // 背包最大容量
        public const int MAXSIZE_BACKPACK = 90;
        // 装备槽默认容量
        public const int SIZE_EQUIPPACK = 10;
        // 回购背包容量
        public const int MAXSIZE_BUYBACKPACK = 8;
        // 仓库最大容量
        public const int MAXSIZE_STORAGEPACK = 144;
        // 仓库默认容量
        public const int SIZE_STORAGEPACK = 32;

        // 帮会仓库默认长度
        public const int SIZE_GUILDPACK = 4;

        private List<GameItem> m_Items = new List<GameItem>();
        private int m_ContainerSize = 0;

        public GameItemContainer(int nSize, Type nType)
        {
            m_ContainerSize = nSize;
            m_ContainerType = nType;
            for (int i = 0; i < m_ContainerSize; ++i)
            {
                m_Items.Add(new GameItem());
            }
        }
        /// <summary>
        /// 容器大小
        /// </summary>
        public int ContainerSize
        {
            get { return m_ContainerSize; }
        }
        public void AddContainerSize(int nAdd)
        {
            m_ContainerSize += nAdd;
            for (int i = 0; i < nAdd; ++i)
            {
                m_Items.Add(new GameItem());
            }
        }
        /// <summary>
        /// 容器可用大小
        /// </summary>
        public int GetCanContainerSize()
        {
            int Num = 0;
            for (int i = 0; i < m_Items.Count; ++i)
            {
                if (m_Items[i].IsValid() == false)
                {
                    Num++;
                }
            }
            return Num;
        }


        /// <summary>
        /// 容器类型
        /// </summary>
        private Type m_ContainerType = Type.TYPE_INVALID;
        public GameItemContainer.Type ContainerType
        {
            get { return m_ContainerType; }
        }

        /// <summary>
        /// 取得物品
        /// </summary>
        public GameItem GetItem(int slot)
        {
            if (slot >= 0 && slot < m_Items.Count)
            {
                return m_Items[slot];
            }
            return null;
        }

        public void Clear() 
        {
            m_ContainerSize = 0;
            m_Items.Clear();
        }

        /// <summary>
        /// 根据GUID取得物品
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public GameItem GetItemByGuid(UInt64 guid)
        {
            if (guid == GlobeVar.INVALID_GUID)
            {
                return null;
            }

            for (int i = 0; i < m_Items.Count; ++i )
            {
                if (m_Items[i].Guid == guid)
                {
                    return m_Items[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 更新物品
        /// </summary>
        /// <param name="slot">槽位</param>
        /// <param name="item">物品</param>
        public bool UpdateItem(int slot, GameItem item)
        {
            bool bRet = false;
            if (slot >= 0 && slot < m_Items.Count)
            {
                m_Items[slot] = item;
                bRet = true;
            }
            return bRet;
        }

        /// <summary>
        /// 有效物品数量
        /// </summary>
        /// <returns></returns>
        public int GetItemCount()
        {
            int count = 0;
            for (int i = 0; i < m_Items.Count; ++i)
            {
                GameItem item = m_Items[i];
                if (item.IsValid())
                {
                    ++count;
                }
            }
            return count;
        }

        /// <summary>
        /// 取得指定ID物品数量
        /// </summary>
        /// <returns></returns>
        public int GetItemCountByDataId(int dataid)
        {
            int count = 0;
            if (dataid >= 0)
            {
                for (int i = 0; i < m_Items.Count; ++i)
                {
                    if (m_Items[i].DataID == dataid)
                    {
                        count = count + m_Items[i].StackCount;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// 取得指定ID绑定物品数量
        /// </summary>
        /// <returns></returns>
        public int GetBindItemCountByDataId(int dataid)
        {
            int count = 0;
            if (dataid >= 0)
            {
                for (int i = 0; i < m_Items.Count; ++i)
                {
                    if (m_Items[i].DataID == dataid && m_Items[i].BindFlag)
                    {
                        count = count + m_Items[i].StackCount;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// 取得指定套装的装备数量
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        public int GetEquipCountBySetId(int setId)
        {
            int count = 0;
            if (setId >= 0)
            {
                for (int i = 0; i < m_Items.Count; ++i)
                {
                    if (m_Items[i].IsValid() && m_Items[i].IsEquipMent())
                    {
                        if (m_Items[i].GetEquipSetId() == setId)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }


        /// <summary>
        /// 取得物品列表
        /// </summary>
        /// <returns></returns>
        public List<GameItem> GetList()
        {
            return m_Items;
        }
	}

    /// <summary>
    /// 装备槽位类型
    /// </summary>
    public enum EquipPackSlot
    {
        Slot_WEAPON = 0,         //武器
        Slot_HEAD,               //头部
        Slot_ARMOR,              //上衣
        Slot_LEG_GUARD,          //下衣
        Slot_CUFF,               //护腕
        Slot_SHOES,              //鞋子
        Slot_NECK,               //吊坠
        Slot_RING,               //戒指
        Slot_AMULET,             //护符
        Slot_BELT,               //腰带

        Slot_NUM,                //数量 
    };

    public enum GemSlot
    {
        OPEN_NUM = 4,           //宝石槽位数量
    }

    class ItemTool
    {
        /// <summary>
        /// 过滤出需要类型的物品
        /// </summary>
        /// <param name="Container"></param>
        /// <param name="nClass"></param>
        /// <param name="nSubClass"></param>
        /// <returns></returns>
        static public List<GameItem> ItemFilter(GameItemContainer Container, int nClass, int nSubClass = 0,bool isQianKunDai=false)
        {
            List<GameItem> resultlist = new List<GameItem>();
            for (int nIndex = 0; nIndex < Container.ContainerSize; ++nIndex)
            {
                GameItem item = Container.GetItem(nIndex);
                if (item.IsValid())
                {
                    Tab_CommonItem tabItem = TableManager.GetCommonItemByID(item.DataID, 0);
                    if (null != tabItem)
                    {
                        int itemclass = tabItem.ClassID;
                        int itemsubclass = tabItem.SubClassID;
						if ((((itemclass == nClass) || (nClass == 0)) && ((itemsubclass == nSubClass) || (nSubClass == 0))) && (((tabItem.IsCanQianKunDai==1) && isQianKunDai) ||!isQianKunDai))
						{

       
                            resultlist.Add(item);
                        }
                    }
                }

            }
            return ItemSort(resultlist);
        }

        /// <summary>
        /// 筛选出高于品质的物品
        /// </summary>
        /// <param name="ItemList"></param>
        /// <param name="nQuality"></param>
        /// <returns></returns>
		static  public List<GameItem> ItemFilter(List<GameItem> ItemList, int nQuality)
        {
            List<GameItem> resultlist = new List<GameItem>();
            for (int nIndex = 0; nIndex < ItemList.Count; ++nIndex)
            {
                GameItem item = ItemList[nIndex];
                if (item.IsValid())
                {
                    Tab_CommonItem tabItem = TableManager.GetCommonItemByID(item.DataID, 0);
                    if (null != tabItem)
                    {
                        int quality = tabItem.Quality;
                        if (quality >= nQuality)
                        {
                            resultlist.Add(item);
                        }
                    }
                }

            }
            return ItemSort(resultlist);
        }

        /// <summary>
        /// 背包显示排序
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        static public List<GameItem> ItemSort(List<GameItem> list)
        {
            for (int i = 0; i < list.Count - 1; ++i)
            {
                int flag = 0;
                for (int j = 0; j < list.Count - i - 1; ++j)
                {
                    //排序等级高的排前面
                    if (list[j].SortId() < list[j + 1].SortId())
                    {
                        GameItem temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                        flag = 1;
                    }
                    else if (list[j].SortId() == list[j + 1].SortId())
                    {
                        //装备
                        if (list[j].IsEquipMent() && list[j + 1].IsEquipMent())
                        {
                            //装备等级小的排后面
                            if (list[j].GetEquipLevel() < list[j + 1].GetEquipLevel())
	                        {
                                GameItem temp = list[j];
                                list[j] = list[j + 1];
                                list[j + 1] = temp;
                                flag = 1;
	                        }
                            else if (list[j].GetEquipLevel() == list[j + 1].GetEquipLevel())
                            {
                                //战斗力低的排后面
                                if (list[j].GetCombatValue() < list[j + 1].GetCombatValue())
                                {
                                    GameItem temp = list[j];
                                    list[j] = list[j + 1];
                                    list[j + 1] = temp;
                                    flag = 1;
                                }
                                else if (list[j].GetCombatValue() == list[j + 1].GetCombatValue())
                                {
                                    //Guid小的排后面
                                    if (list[j].Guid < list[j + 1].Guid)
                                    {
                                        GameItem temp = list[j];
                                        list[j] = list[j + 1];
                                        list[j + 1] = temp;
                                        flag = 1;
                                    }
                                }
                            }
                        }
                        //非装备
                        else
                        {
                            //Guid小的排后面
                            if (list[j].Guid < list[j + 1].Guid)
                            {
                                GameItem temp = list[j];
                                list[j] = list[j + 1];
                                list[j + 1] = temp;
                                flag = 1;
                            }
                        }
                    }
                }
                if (flag == 0)
                {
                    break;
                }
            }

            return list;
        }

        static public int GetEnchanceLevelUpExp(int level, ItemQuality itemquality)
        {
            int levelupexp = 0;
            Tab_EquipEnchance line = TableManager.GetEquipEnchanceByID(level+1, 0);
            if (line != null)
            {
                switch (itemquality)
                {
                    case ItemQuality.QUALITY_WHITE:
                        {
                            levelupexp = line.WhiteExp;
                        }
                        break;
                    case ItemQuality.QUALITY_GREEN:
                        {
                            levelupexp = line.GreenExp;
                        }
                        break;
                    case ItemQuality.QUALITY_BLUE:
                        {
                            levelupexp = line.BlueExp;
                        }
                        break;
                    case ItemQuality.QUALITY_PURPLE:
                        {
                            levelupexp = line.PurpleExp;
                        }
                        break;
                    case ItemQuality.QUALITY_ORANGE:
                        {
                            levelupexp = line.OrangeExp;
                        }
                        break;
                    default:
                        break;
                }
            }
            return levelupexp;
        }

        static public string GetStarColourSprite(int value)
        {
            switch (value)
            {
                case 0:
                    return "ui_pub_048";
                case 1:
                    return "ui_pub_049";
                case 2:
                    return "ui_pub_050";
                case 3:
                    return "ui_pub_051";
                case 4:
                    return "ui_pub_052";
                default:
                    break;
            }
            return "";
        }

        static public string GetStarColor(int starLevel)
        {
            string strColor = "[FFFFFF]";
            if (starLevel >= 0 && starLevel <= 12)
            {
                strColor = "[FFFFFF]";
            }
            else if (starLevel >= 13 && starLevel <= 24)
            {
                strColor = "[33CC66]";
            }
            else if (starLevel >= 25 && starLevel <= 36)
            {
                strColor = "[33CCFF]";
            }
            else if (starLevel >= 37 && starLevel <= 48)
            {
                strColor = "[CC66FF]";
            }
            else if (starLevel >= 49 && starLevel <= 60)
            {
                strColor = "[FF9933]";
            }
            return strColor;
        }

        static public string GetGemAttr(int GemId)
        {
            Tab_GemAttr lineGemAttr = TableManager.GetGemAttrByID(GemId, 0);
            if (lineGemAttr != null)
            {
                if (lineGemAttr.MaxHP > 0)
                {
                    return string.Format("{0} + {1}", ItemTool.ConvertAttrToString(COMBATATTE.MAXHP), lineGemAttr.MaxHP); 
                }
                if (lineGemAttr.MaxMP > 0)
                {
                    return string.Format("{0} + {1}", ItemTool.ConvertAttrToString(COMBATATTE.MAXMP), lineGemAttr.MaxMP);
                }
                if (lineGemAttr.PysAttack > 0)
                {
                    return string.Format("{0} + {1}", ItemTool.ConvertAttrToString(COMBATATTE.PYSATTACK), lineGemAttr.PysAttack);
                }
                if (lineGemAttr.MagAttack > 0)
                {
                    return string.Format("{0} + {1}", ItemTool.ConvertAttrToString(COMBATATTE.MAGATTACK), lineGemAttr.MagAttack);
                }
                if (lineGemAttr.PysDef > 0)
                {
                    return string.Format("{0} + {1}", ItemTool.ConvertAttrToString(COMBATATTE.PYSDEF), lineGemAttr.PysDef);
                }
                if (lineGemAttr.MagDef > 0)
                {
                    return string.Format("{0} + {1}", ItemTool.ConvertAttrToString(COMBATATTE.MAGDEF), lineGemAttr.MagDef);
                }
                if (lineGemAttr.Hit > 0)
                {
                    return string.Format("{0} + {1}", ItemTool.ConvertAttrToString(COMBATATTE.HIT), lineGemAttr.Hit);
                }
                if (lineGemAttr.Dodge > 0)
                {
                    return string.Format("{0} + {1}", ItemTool.ConvertAttrToString(COMBATATTE.DODGE), lineGemAttr.Dodge);
                }
                if (lineGemAttr.Critical > 0)
                {
                    return string.Format("{0} + {1}", ItemTool.ConvertAttrToString(COMBATATTE.CRITICAL), lineGemAttr.Critical);
                }
                if (lineGemAttr.Decritical > 0)
                {
                    return string.Format("{0} + {1}", ItemTool.ConvertAttrToString(COMBATATTE.DECRITICAL), lineGemAttr.Decritical);
                }
                if (lineGemAttr.Strike > 0)
                {
                    return string.Format("{0} + {1}", ItemTool.ConvertAttrToString(COMBATATTE.STRIKE), lineGemAttr.Strike);
                }
                if (lineGemAttr.Ductical > 0)
                {
                    return string.Format("{0} + {1}", ItemTool.ConvertAttrToString(COMBATATTE.DUCTICAL), lineGemAttr.Ductical);
                }
                if (lineGemAttr.CritiAdd > 0)
                {
                    return string.Format("{0} + {1}", ItemTool.ConvertAttrToString(COMBATATTE.CRITIADD), lineGemAttr.CritiAdd);
                }
                if (lineGemAttr.CritiMis > 0)
                {
                    return string.Format("{0} + {1}", ItemTool.ConvertAttrToString(COMBATATTE.CRITIMIS), lineGemAttr.CritiMis);
                }
            }
            return "";
        }

        static public string ConvertAttrToString(COMBATATTE attr)
        {
            switch (attr)
            {
                case COMBATATTE.MAXHP:
                    return "生命";
                case COMBATATTE.MAXMP:
                    return "法力";
                case COMBATATTE.MAXXP:
                    return "战意";
                case COMBATATTE.PYSATTACK:
                    return "武力攻击";
                case COMBATATTE.MAGATTACK:
                    return "法力攻击";
                case COMBATATTE.PYSDEF:
                    return "武力防御";
                case COMBATATTE.MAGDEF:
                    return "法力防御";
                case COMBATATTE.HIT:
                    return "命中";
                case COMBATATTE.DODGE:
                    return "闪避";
                case COMBATATTE.CRITICAL:
                    return "暴击";
                case COMBATATTE.DECRITICAL:
                    return "暴击抗性";
                case COMBATATTE.STRIKE:
                    return "穿透";
                case COMBATATTE.DUCTICAL:
                    return "韧性";
                case COMBATATTE.CRITIADD:
                    return "暴击加成";
                case COMBATATTE.CRITIMIS:
                    return "暴击减免";
                case COMBATATTE.MOVESPEED:
                    return "移动速度";
                case COMBATATTE.ATTACKSPEED:
                    return "攻击速度";
                default:
                    break;
            }
            switch ((int)attr)
            {
                case 1000:
                    return "全攻击";
                case 1001:
                    return "全防御";
                default:
                    break;
            }
            return "";
        }

        /// <summary>
        /// 取得装备槽位类型 1攻击 2防御
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        static public int GetEquipSlotType(int slot)
        {
            switch (slot)
            {
                case (int)EquipPackSlot.Slot_WEAPON:
                    return 1;
                case (int)EquipPackSlot.Slot_HEAD:
                    return 2;
                case (int)EquipPackSlot.Slot_ARMOR:
                    return 2;
                case (int)EquipPackSlot.Slot_LEG_GUARD:
                    return 2;
                case (int)EquipPackSlot.Slot_SHOES:
                    return 2;
                case (int)EquipPackSlot.Slot_CUFF:
                    return 2;
                case (int)EquipPackSlot.Slot_NECK:
                    return 1;
                case (int)EquipPackSlot.Slot_RING:
                    return 1;
                case (int)EquipPackSlot.Slot_AMULET:
                    return 1;
                case (int)EquipPackSlot.Slot_BELT:
                    return 1;
                default:
                    break;
            }
            return 0;
        }

        /// <summary>
        /// 装备背包 UI显示顺序 转换为 数据容器顺序
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        static public int GetEquipSlotByUIIndex(int index)
        {
            switch (index)
            {
                case 0: return (int)EquipPackSlot.Slot_WEAPON;
                case 1: return (int)EquipPackSlot.Slot_HEAD;
                case 2: return (int)EquipPackSlot.Slot_RING;
                case 3: return (int)EquipPackSlot.Slot_ARMOR;
                case 4: return (int)EquipPackSlot.Slot_NECK;
                case 5: return (int)EquipPackSlot.Slot_CUFF;
                case 6: return (int)EquipPackSlot.Slot_AMULET;
                case 7: return (int)EquipPackSlot.Slot_LEG_GUARD;
                case 8: return (int)EquipPackSlot.Slot_BELT;
                case 9: return (int)EquipPackSlot.Slot_SHOES;
                default:
                    break;
            }
            return -1;
        }

        /// <summary>
        /// 装备背包 数据容器顺序 转换为 UI显示顺序
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        static public int GetUIIndexByEquipSlot(int slot)
        {
            switch (slot)
            {
                case (int)EquipPackSlot.Slot_WEAPON: return 0;
                case (int)EquipPackSlot.Slot_HEAD: return 1;
                case (int)EquipPackSlot.Slot_RING: return 2;
                case (int)EquipPackSlot.Slot_ARMOR: return 3;
                case (int)EquipPackSlot.Slot_NECK: return 4;
                case (int)EquipPackSlot.Slot_CUFF: return 5;
                case (int)EquipPackSlot.Slot_AMULET: return 6;
                case (int)EquipPackSlot.Slot_LEG_GUARD: return 7;
                case (int)EquipPackSlot.Slot_BELT: return 8;
                case (int)EquipPackSlot.Slot_SHOES: return 9;
                default:
                    break;
            }
            return 0;
        }

        /// <summary>
        /// 筛选装备强化材料
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        static public List<GameItem> EnchanceMaterialFilter(List<GameItem> list, bool valuable)
        {
            List<GameItem> resultlist = new List<GameItem>();
            resultlist.AddRange(list);
            //筛选规则1 材料需要是装备或者强化石
            resultlist.RemoveAll(delegate(GameItem item)
            {
                if (item.GetClass() == (int)ItemClass.EQUIP)
                {
                    return false;
                }
                if (item.GetClass() == (int)ItemClass.STRENGTHEN &&
                    item.GetSubClass() == (int)StrengthenSubClass.EQUIP_ENCHANCE)
                {
                    return false;
                }
                return true;
            });
            //筛选规则2 腰带不能强化
            resultlist.RemoveAll(delegate(GameItem item)
            {
                if (item.IsEquipMent())
                {
                    if (item.GetEquipSlotIndex() == (int)EquipPackSlot.Slot_BELT)
                    {
                        return true;
                    }
                }
                return false;
            });
            //筛选规则3 装备等级高于玩家20级以上 不能作为材料
            int PlayerLevel = GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr.Level;
            resultlist.RemoveAll(delegate(GameItem item)
            {
                if (item.IsEquipMent())
                {
                    if (item.GetEquipLevel() > (PlayerLevel + 20))
                    {
                        return true;
                    }
                }
                return false;
            });

            if (valuable == false)
            {
                //筛选规则4 紫色橙色 && 需求等级与玩家等级相差在正负20级以内 && 不绑定可交易 不能作为材料
                resultlist.RemoveAll(delegate(GameItem item)
                {
                    if (item.IsEquipMent())
                    {
                        if (item.GetQuality() == ItemQuality.QUALITY_PURPLE ||
                            item.GetQuality() == ItemQuality.QUALITY_ORANGE)
                        {
                            if (item.GetEquipLevel() <= (PlayerLevel + 20) ||
                                item.GetEquipLevel() >= (PlayerLevel - 20))
                            {
                                if (ConsignSaleBag.isCanConsignSale(item))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    return false;
                });
                //筛选规则5 紫色橙色 && 符合玩家职业 && 基础属性大于当前装备位 不能作为材料
                resultlist.RemoveAll(delegate(GameItem item)
                {
                    if (item.IsEquipMent())
                    {
                        if (item.GetQuality() == ItemQuality.QUALITY_PURPLE ||
                            item.GetQuality() == ItemQuality.QUALITY_ORANGE)
                        {
                            int nPlayerProfession = Singleton<ObjManager>.Instance.MainPlayer.Profession;
                            if (item.GetProfessionRequire() == nPlayerProfession || item.GetProfessionRequire() == -1)
                            {
                                int slotindex = item.GetEquipSlotIndex();
                                GameItem compareEquip = GameManager.gameManager.PlayerDataPool.EquipPack.GetItem(slotindex);
                                if (compareEquip != null)
                                {
                                    if (compareEquip.IsValid() == false || item.GetCombatValue_NoStarEnchance() > compareEquip.GetCombatValue_NoStarEnchance())
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    return false;
                });
                //筛选规则6 护符和戒指不能强化
                resultlist.RemoveAll(delegate(GameItem item)
                {
                    if (item.IsEquipMent())
                    {
                        if (item.GetEquipSlotIndex() == (int)EquipPackSlot.Slot_AMULET ||
                            item.GetEquipSlotIndex() == (int)EquipPackSlot.Slot_RING)
                        {
                            return true;
                        }
                    }
                    return false;
                });
            }
            return ItemSort(resultlist);
        }

    };
}

