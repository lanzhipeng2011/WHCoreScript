//********************************************************************
// 文件名: GameItem.cs
// 描述: 物品
// 作者: TangYi
// 创建时间: 2013-12-25
//
// 修改历史:
//********************************************************************

using UnityEngine;
using System.Collections.Generic;
using System;
using GCGame.Table;
using Games.GlobeDefine;

namespace Games.Item
{
    /// <summary>
    /// 物品总类（对应commonitem.txt中ClassID）
    /// </summary>
    public enum ItemClass
    {
        EQUIP = 1,              //装备
        MATERIAL,           //材料    
        STRENGTHEN,         //强化道具
        PRIZE,              //奖励物品
        MEDIC,              //药品
        MISSION,            //任务道具
        FASHION,            //时装
        MOUNT,              //坐骑
        FELLOW,             //伙伴令牌
        BELLE,              //美人
        SWORDSMAN,          //侠客

    }
    /// <summary>
    /// 装备子类（对应commonitem.txt中SubClassID）
    /// </summary>
    public enum EquipSubClass
    {
        WEAPON = 1,         //武器
        HEAD,               //头部
        ARMOR,              //上衣
        LEG_GUARD,          //下衣
        CUFF,               //护腕
        SHOES,              //鞋子
        CHARM1,             //饰品1(吊坠)
        CHARM2,             //饰品2(戒指)
        AMULET,             //护符
        BELT,               //腰带
    }
    /// <summary>
    /// 材料子类（对应commonitem.txt中SubClassID）
    /// </summary>
    public enum MaterialSubClass
    {
        BLUEPRINT = 1,          //打造图
        LIFE_MATERIAL,          //生活材料
        SEED,                   //庄园种子
    }
    /// <summary>
    /// 强化道具子类（对应commonitem.txt中SubClassID）
    /// </summary>
    public enum StrengthenSubClass
    {
        GEM = 1,            //宝石
        EQUIP_ENCHANCE,     //装备强化
        GEM_COMPOSE,        //宝石合成
        FELLOW_SKILL,       //伙伴技能
        FELLOW_REBORN,      //伙伴还童
        SKILL_STRENGTHEN,   //技能强化
        BEAUTY,             //美人材料
    }
    /// <summary>
    /// 奖励物品子类（对应commonitem.txt中SubClassID）
    /// </summary>
    public enum PrizeSubClass
    {
        BIND_YUANBAO = 1,   //绑定元宝
        GOLD,               //金币
        BAG,                //礼包
        FORCE_MEDIC,        //体力药
        EXP,                //经验丹
        DOUBLE,             //双倍丹
        SPEAKER,            //喇叭
        BOX,                //宝箱
    }
    /// <summary>
    /// 药品子类（对应commonitem.txt中SubClassID）
    /// </summary>
    public enum MedicSubClass
    {
        HP = 1,             //红药
        MP,                 //蓝药
        ATTR,               //属性
        HP_DY,              //动态红药
        MP_DY,              //动态蓝药
        HPMP_DY,            //动态红蓝药
    }
    /// <summary>
    /// 任务子类（对应commonitem.txt中SubClassID）
    /// </summary>
    public enum MissionSubClass
    {
        MISSION = 1,        //任务道具
    }
    /// <summary>
    /// 坐骑子类（对应commonitem.txt中SubClassID）
    /// </summary>
    public enum MountSubClass
    {
        MOUNT = 1,         //坐骑
    }
    /// <summary>
    /// 时装子类（对应commonitem.txt中SubClassID）
    /// </summary>
    public enum FashionSubClass
    {
        FASHION = 1,       //时装
    }
    /// <summary>
    /// 伙伴令牌子类（对应commonitem.txt中SubClassID）
    /// </summary>
    public enum FellowSubClass
    {
        FELLOW = 1,        //伙伴令牌
    }

    /// <summary>
    /// 美人子类（对应commonitem.txt中SubClassID）
    /// </summary>
    public enum BelleSubClass
    {
        BELLE = 1,        //美人令牌
    }

    /// <summary>
    /// 侠客子类（对应commonitem.txt中SubClassID）
    /// </summary>
    public enum SwordsManSubClass
    {
        SWORDSMAN = 1,      //侠客令牌
    }

    /// <summary>
    /// 物品品质
    /// </summary>
    public enum ItemQuality
    {
        QUALITY_INVALID = 0,
        QUALITY_WHITE,				//白
        QUALITY_GREEN,				//绿
        QUALITY_BLUE,				//蓝
        QUALITY_PURPLE,				//紫
        QUALITY_ORANGE,				//橙
    }

    /// <summary>
    /// 物品出售类型
    /// </summary>
    public enum ItemSellMoneyType
    {
        TYPE_INVALID = -1,
        TYPE_COIN = 0,
        TYPE_YUANBAO,
        TYPE_BIND_YUANBAO,
    }

    public class GameItem
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public GameItem()
        {
            m_nDataID = -1;
            m_nGuid = GlobeVar.INVALID_GUID;
            m_bBindFlag = false;
            m_nStackCount = 0;
            m_nEnchanceLevel = 0;
            m_bEnchanceTotalExp = 0;
            m_nStarLevel = 0;
            m_oDynamicData = new int[DYNAMIC_NUM] { 0, 0, 0, 0, 0, 0, 0, 0 };
        }
        private const int DYNAMIC_NUM = 8;

        /// <summary>
        /// IsValid
        /// </summary>
        /// <returns></returns>
        public void CleanUp()
        {
            m_nDataID = -1;
            m_nGuid = GlobeVar.INVALID_GUID;
            m_bBindFlag = false;
            m_nStackCount = 0;
            m_nEnchanceLevel = 0;
            m_bEnchanceTotalExp = 0;
            m_nStarLevel = 0;
            for (int i = 0; i < DynamicData.Length; i++)
            {
                DynamicData[i] = 0;
            }
        }

        /// <summary>
        /// IsValid
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (m_nDataID >= 0)
            {
                Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
                if (line != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 取得排序ID
        /// </summary>
        /// <returns></returns>
        public int SortId()
        {
            if (m_nDataID >= 0)
            {
                Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
                if (line != null)
                {
                    return line.SortId;
                }
            }
            return -1;
        }

        /// <summary>
        /// 是否装备
        /// </summary>
        /// <returns></returns>
        public bool IsEquipMent()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                if (line.ClassID == (int)ItemClass.EQUIP)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 装备等级
        /// </summary>
        /// <returns></returns>
        public int GetEquipLevel()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                return line.MinLevelRequire;
            }
            return 0;
        }

        /// <summary>
        /// 战斗力 TODO
        /// </summary>
        /// <returns></returns>
        public int GetCombatValue()
        {
            float fTotalCombatValue = 0.0f;
            //血影响
            fTotalCombatValue += (GetComBatAttrById((int)COMBATATTE.MAXHP) * 0.2f);
            //蓝影响
            fTotalCombatValue += (GetComBatAttrById((int)COMBATATTE.MAXMP) * 0.1f);
            //全攻击影响
            fTotalCombatValue += (GetComBatAttrById((int)MIXBATATTR.MIXATTR_ALLATTACK) * 5.1f);
            //全防御影响
            fTotalCombatValue += (GetComBatAttrById((int)MIXBATATTR.MIXATTR_ALLDEF) * 3.0f);
            if (GetProfessionRequire() == (int)CharacterDefine.PROFESSION.DALI ||
                GetProfessionRequire() == (int)CharacterDefine.PROFESSION.SHAOLIN ||
                GetProfessionRequire() == (int)CharacterDefine.PROFESSION.TIANSHAN)
            {
                //物理攻击影响
                fTotalCombatValue += (GetComBatAttrById((int)COMBATATTE.PYSATTACK) * 5.1f);
                //物理防御影响
                int nPDef = GetComBatAttrById((int)COMBATATTE.PYSDEF);
                int nMDef = GetComBatAttrById((int)COMBATATTE.MAGDEF);
				fTotalCombatValue += ((nPDef * 1.0f) + (nMDef * 1.0f) )* 1.5f;
            }
            else if (GetProfessionRequire() == (int)CharacterDefine.PROFESSION.XIAOYAO)
            {
                //法术攻击影响
                fTotalCombatValue += (GetComBatAttrById((int)COMBATATTE.MAGATTACK) * 5.1f);
                //法术防御影响
                int nPDef = GetComBatAttrById((int)COMBATATTE.PYSDEF);
                int nMDef = GetComBatAttrById((int)COMBATATTE.MAGDEF);
				fTotalCombatValue += ((nPDef * 1.0f) + (nMDef * 1.0f) )* 1.5f;
            }
            //命中影响
            fTotalCombatValue += (GetComBatAttrById((int)COMBATATTE.HIT) * 3.0f);
            //闪避影响
            fTotalCombatValue += (GetComBatAttrById((int)COMBATATTE.DODGE) * 4.2f);
            //暴击影响
            fTotalCombatValue += (GetComBatAttrById((int)COMBATATTE.CRITICAL) * 1.0f);
            //暴抗影响
            fTotalCombatValue += (GetComBatAttrById((int)COMBATATTE.DECRITICAL) * 0.8f);
            //穿透影响
            fTotalCombatValue += (GetComBatAttrById((int)COMBATATTE.STRIKE) * 8.3f);
            //韧性影响
            fTotalCombatValue += (GetComBatAttrById((int)COMBATATTE.DUCTICAL) * 8.3f);
            //暴击加成影响
            fTotalCombatValue += (GetComBatAttrById((int)COMBATATTE.CRITIADD) * 6.5f);
            //暴击减免影响
            fTotalCombatValue += (GetComBatAttrById((int)COMBATATTE.CRITIMIS) * 6.5f);

            return (int)fTotalCombatValue;
        }

        /// <summary>
        /// 战斗力(排除打星强化的属性)
        /// </summary>
        /// <returns></returns>
        public int GetCombatValue_NoStarEnchance()
        {
            float fTotalCombatValue = 0.0f;
            //血影响
            fTotalCombatValue += (GetComBatAttrById_NoStarEnchance((int)COMBATATTE.MAXHP) * 0.2f);
            //蓝影响
            fTotalCombatValue += (GetComBatAttrById_NoStarEnchance((int)COMBATATTE.MAXMP) * 0.1f);
            //全攻击影响
            fTotalCombatValue += (GetComBatAttrById_NoStarEnchance((int)MIXBATATTR.MIXATTR_ALLATTACK) * 5.1f);
            //全防御影响
            fTotalCombatValue += (GetComBatAttrById_NoStarEnchance((int)MIXBATATTR.MIXATTR_ALLDEF) * 3.0f);
            if (GetProfessionRequire() == (int)CharacterDefine.PROFESSION.DALI ||
                GetProfessionRequire() == (int)CharacterDefine.PROFESSION.SHAOLIN ||
                GetProfessionRequire() == (int)CharacterDefine.PROFESSION.TIANSHAN)
            {
                //物理攻击影响
                fTotalCombatValue += (GetComBatAttrById_NoStarEnchance((int)COMBATATTE.PYSATTACK) * 5.1f);
                //物理防御影响
                int nPDef = GetComBatAttrById_NoStarEnchance((int)COMBATATTE.PYSDEF);
                int nMDef = GetComBatAttrById_NoStarEnchance((int)COMBATATTE.MAGDEF);
                fTotalCombatValue += ((nPDef * 1.0f) + (nMDef * 1.0f)) * 1.5f;
            }
            else if (GetProfessionRequire() == (int)CharacterDefine.PROFESSION.XIAOYAO)
            {
                //法术攻击影响
                fTotalCombatValue += (GetComBatAttrById_NoStarEnchance((int)COMBATATTE.MAGATTACK) * 5.1f);
                //法术防御影响
                int nPDef = GetComBatAttrById_NoStarEnchance((int)COMBATATTE.PYSDEF);
                int nMDef = GetComBatAttrById_NoStarEnchance((int)COMBATATTE.MAGDEF);
                fTotalCombatValue += ((nPDef * 1.0f) + (nMDef * 1.0f)) * 1.5f;
            }
            //命中影响
            fTotalCombatValue += (GetComBatAttrById_NoStarEnchance((int)COMBATATTE.HIT) * 3.0f);
            //闪避影响
            fTotalCombatValue += (GetComBatAttrById_NoStarEnchance((int)COMBATATTE.DODGE) * 4.2f);
            //暴击影响
            fTotalCombatValue += (GetComBatAttrById_NoStarEnchance((int)COMBATATTE.CRITICAL) * 1.0f);
            //暴抗影响
            fTotalCombatValue += (GetComBatAttrById_NoStarEnchance((int)COMBATATTE.DECRITICAL) * 0.8f);
            //穿透影响
            fTotalCombatValue += (GetComBatAttrById_NoStarEnchance((int)COMBATATTE.STRIKE) * 8.3f);
            //韧性影响
            fTotalCombatValue += (GetComBatAttrById_NoStarEnchance((int)COMBATATTE.DUCTICAL) * 8.3f);
            //暴击加成影响
            fTotalCombatValue += (GetComBatAttrById_NoStarEnchance((int)COMBATATTE.CRITIADD) * 6.5f);
            //暴击减免影响
            fTotalCombatValue += (GetComBatAttrById_NoStarEnchance((int)COMBATATTE.CRITIMIS) * 6.5f);

            return (int)fTotalCombatValue;
        }

        /// <summary>
        /// 装备附加的战斗属性值
        /// </summary>
        /// <param name="attrtype"></param>
        /// <returns></returns>
        public int GetComBatAttrById(int attrtype)
        {
            int attrValue = 0;
            Tab_EquipAttr line = TableManager.GetEquipAttrByID(m_nDataID, 0);
            if (line != null)
            {
                Tab_EquipStar starline = null;
                if (m_nStarLevel > 0)
                {
                    starline = TableManager.GetEquipStarByID(m_nStarLevel, 0);
                }
                Tab_EquipEnchance enchanceline = null;
                if (m_nEnchanceLevel > 0)
                {
                    enchanceline = TableManager.GetEquipEnchanceByID(m_nEnchanceLevel, 0);
                }
                //固定属性
                switch (attrtype)
                {
                    case (int)COMBATATTE.MAXHP:
                        {
                            if ((int)line.HP > 0)
                            {
                                //初始
                                attrValue += (int)line.HP;
                                //打星
                                if (m_nStarLevel > 0 && starline != null)
                                {
                                    attrValue += ((int)(line.HP * starline.AttrRate) + 1);
                                }
                                //强化
                                if (m_nEnchanceLevel > 0 && enchanceline != null)
                                {
                                    float rate = enchanceline.EnchanceRate;
                                    attrValue += ((int)(line.HP * rate) + 1);
                                }
                            }
                        }
                        break;
                    case (int)COMBATATTE.MAXMP:
                        {
                            if ((int)line.MP > 0)
                            {
                                //初始
                                attrValue += (int)line.MP;
                                //打星
                                if (m_nStarLevel > 0 && starline != null)
                                {
                                    attrValue += ((int)(line.MP * starline.AttrRate) + 1);
                                }
                                //强化
                                if (m_nEnchanceLevel > 0 && enchanceline != null)
                                {
                                    float rate = enchanceline.EnchanceRate;
                                    attrValue += ((int)(line.MP * rate) + 1);
                                }
                            }
                        }
                        break;
                    case (int)COMBATATTE.MAXXP:
                        break;
                    case (int)COMBATATTE.PYSATTACK:
                        {
                            if ((int)line.PhysicsAttack > 0)
                            {
                                //初始
                                attrValue += (int)line.PhysicsAttack;
                                //打星
                                if (m_nStarLevel > 0 && starline != null)
                                {
                                    attrValue += ((int)(line.PhysicsAttack * starline.AttrRate) + 1);
                                }
                                //强化
                                if (m_nEnchanceLevel > 0 && enchanceline != null)
                                {
                                    float rate = enchanceline.EnchanceRate;
                                    attrValue += ((int)(line.PhysicsAttack * rate) + 1);
                                }
                            }
                            
                        }
                        break;
                    case (int)COMBATATTE.MAGATTACK:
                        {
                            if ((int)line.MagicAttack > 0)
                            {
                                //初始
                                attrValue += (int)line.MagicAttack;
                                //打星
                                if (m_nStarLevel > 0 && starline != null)
                                {
                                    attrValue += ((int)(line.MagicAttack * starline.AttrRate) + 1);
                                }
                                //强化
                                if (m_nEnchanceLevel > 0 && enchanceline != null)
                                {
                                    float rate = enchanceline.EnchanceRate;
                                    attrValue += ((int)(line.MagicAttack * rate) + 1);
                                }
                            }
                        }
                        break;
                    case (int)COMBATATTE.PYSDEF:
                        {
                            if ((int)line.PhysicsDefence > 0)
                            {
                                //初始
                                attrValue += (int)line.PhysicsDefence;
                                //打星
                                if (m_nStarLevel > 0 && starline != null)
                                {
                                    attrValue += ((int)(line.PhysicsDefence * starline.AttrRate) + 1);
                                }
                                //强化
                                if (m_nEnchanceLevel > 0 && enchanceline != null)
                                {
                                    float rate = enchanceline.EnchanceRate;
                                    attrValue += ((int)(line.PhysicsDefence * rate) + 1);
                                }
                            }
                        }
                        break;
                    case (int)COMBATATTE.MAGDEF:
                        {
                            if ((int)line.MagicDefence > 0)
                            {
                                //初始
                                attrValue += (int)line.MagicDefence;
                                //打星
                                if (m_nStarLevel > 0 && starline != null)
                                {
                                    attrValue += ((int)(line.MagicDefence * starline.AttrRate) + 1);
                                }
                                //强化
                                if (m_nEnchanceLevel > 0 && enchanceline != null)
                                {
                                    float rate = enchanceline.EnchanceRate;
                                    attrValue += ((int)(line.MagicDefence * rate) + 1);
                                }
                            }
                        }
                        break;
                    case (int)COMBATATTE.HIT:
                        break;
                    case (int)COMBATATTE.DODGE:
                        break;
                    case (int)COMBATATTE.CRITICAL:
                        break;
                    case (int)COMBATATTE.DECRITICAL:
                        break;
                    case (int)COMBATATTE.STRIKE:
                        break;
                    case (int)COMBATATTE.DUCTICAL:
                        break;
                    case (int)COMBATATTE.CRITIADD:
                        break;
                    case (int)COMBATATTE.CRITIMIS:
                        break;
                    case (int)COMBATATTE.MOVESPEED:
                        break;
                    case (int)COMBATATTE.ATTACKSPEED:
                        {
                            //初始
                            attrValue += (int)line.AttackSpeed;
                            //攻速不受打星强化影响
                        }
                        break;
                    case (int)MIXBATATTR.MIXATTR_ALLATTACK:
                        //全攻击
                        if ((int)line.AllAttack > 0)
                        {
                            //初始
                            attrValue += (int)line.AllAttack;
                            //打星
                            if (m_nStarLevel > 0 && starline != null)
                            {
                                attrValue += ((int)(line.AllAttack * starline.AttrRate) + 1);
                            }
                            //强化
                            if (m_nEnchanceLevel > 0 && enchanceline != null)
                            {
                                float rate = enchanceline.EnchanceRate;
                                attrValue += ((int)(line.AllAttack * rate) + 1);
                            }
                        }
                        break;
                    case (int)MIXBATATTR.MIXATTR_ALLDEF:
                        //全防御
                        if ((int)line.AllDefence > 0)
                        {
                            //初始
                            attrValue += (int)line.AllDefence;
                            //打星
                            if (m_nStarLevel > 0 && starline != null)
                            {
                                attrValue += ((int)(line.AllDefence * starline.AttrRate) + 1);
                            }
                            //强化
                            if (m_nEnchanceLevel > 0 && enchanceline != null)
                            {
                                float rate = enchanceline.EnchanceRate;
                                attrValue += ((int)(line.AllDefence * rate) + 1);
                            }
                        }
                        break;
                    default:
                        break;
                }
                //附加属性
                for (int attrIndex = 0; attrIndex < line.getAddAttrTypeCount(); ++attrIndex)
                {
                    int addAttrType = line.GetAddAttrTypebyIndex(attrIndex);
                    float addAttrValue = line.GetAddAttrValuebyIndex(attrIndex);
                    if (addAttrType >= (int)COMBATATTE.MAXHP && addAttrType < (int)COMBATATTE.COMBATATTE_MAXNUM && addAttrValue > 0)
                    {
                        if ((int)attrtype == addAttrType)
                        {
                            attrValue += (int)addAttrValue;
                            //强化
                            if (addAttrType != (int)COMBATATTE.ATTACKSPEED) //攻速不受强化和打星影响
                            {
                                if ((int)addAttrValue > 0 && m_nEnchanceLevel > 0 && enchanceline != null)
                                {
                                    float rate = enchanceline.EnchanceRate;
                                    attrValue += ((int)(addAttrValue * rate) + 1);
                                }
                            }
                        }
                    }
                }
            }

            return attrValue;
        }

        /// <summary>
        /// 装备附加的战斗属性值(排除打星强化的属性)
        /// </summary>
        /// <param name="attrtype"></param>
        /// <returns></returns>
        public int GetComBatAttrById_NoStarEnchance(int attrtype)
        {
            int attrValue = 0;
            Tab_EquipAttr line = TableManager.GetEquipAttrByID(m_nDataID, 0);
            if (line != null)
            {
                //固定属性
                switch (attrtype)
                {
                    case (int)COMBATATTE.MAXHP:
                        {
                            if ((int)line.HP > 0)
                            {
                                //初始
                                attrValue += (int)line.HP;
                            }
                        }
                        break;
                    case (int)COMBATATTE.MAXMP:
                        {
                            if ((int)line.MP > 0)
                            {
                                //初始
                                attrValue += (int)line.MP;
                            }
                        }
                        break;
                    case (int)COMBATATTE.MAXXP:
                        break;
                    case (int)COMBATATTE.PYSATTACK:
                        {
                            if ((int)line.PhysicsAttack > 0)
                            {
                                //初始
                                attrValue += (int)line.PhysicsAttack;
                            }
                        }
                        break;
                    case (int)COMBATATTE.MAGATTACK:
                        {
                            if ((int)line.MagicAttack > 0)
                            {
                                //初始
                                attrValue += (int)line.MagicAttack;
                            }
                        }
                        break;
                    case (int)COMBATATTE.PYSDEF:
                        {
                            if ((int)line.PhysicsDefence > 0)
                            {
                                //初始
                                attrValue += (int)line.PhysicsDefence;
                            }
                        }
                        break;
                    case (int)COMBATATTE.MAGDEF:
                        {
                            if ((int)line.MagicDefence > 0)
                            {
                                //初始
                                attrValue += (int)line.MagicDefence;
                            }
                        }
                        break;
                    case (int)COMBATATTE.HIT:
                        break;
                    case (int)COMBATATTE.DODGE:
                        break;
                    case (int)COMBATATTE.CRITICAL:
                        break;
                    case (int)COMBATATTE.DECRITICAL:
                        break;
                    case (int)COMBATATTE.STRIKE:
                        break;
                    case (int)COMBATATTE.DUCTICAL:
                        break;
                    case (int)COMBATATTE.CRITIADD:
                        break;
                    case (int)COMBATATTE.CRITIMIS:
                        break;
                    case (int)COMBATATTE.MOVESPEED:
                        break;
                    case (int)COMBATATTE.ATTACKSPEED:
                        {
                            //初始
                            attrValue += (int)line.AttackSpeed;
                            //攻速不受打星强化影响
                        }
                        break;
                    case (int)MIXBATATTR.MIXATTR_ALLATTACK:
                        //全攻击
                        if ((int)line.AllAttack > 0)
                        {
                            //初始
                            attrValue += (int)line.AllAttack;
                        }
                        break;
                    case (int)MIXBATATTR.MIXATTR_ALLDEF:
                        //全防御
                        if ((int)line.AllDefence > 0)
                        {
                            //初始
                            attrValue += (int)line.AllDefence;
                        }
                        break;
                    default:
                        break;
                }
                //附加属性
                for (int attrIndex = 0; attrIndex < line.getAddAttrTypeCount(); ++attrIndex)
                {
                    int addAttrType = line.GetAddAttrTypebyIndex(attrIndex);
                    float addAttrValue = line.GetAddAttrValuebyIndex(attrIndex);
                    if (addAttrType >= (int)COMBATATTE.MAXHP && addAttrType < (int)COMBATATTE.COMBATATTE_MAXNUM && addAttrValue > 0)
                    {
                        if ((int)attrtype == addAttrType)
                        {
                            attrValue += (int)addAttrValue;
                        }
                    }
                }
            }

            return attrValue;
        }

        /// <summary>
        /// 取得装备的全部强化经验
        /// （强化等级也兑换成经验值）
        /// </summary>
        /// <returns></returns>
        public int GetFullEnchanceExp()
        {
            int fullexp = 0;
            
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                if (IsEquipMent() && !IsBelt())
                {
                    fullexp += (int)m_bEnchanceTotalExp;
                }
                //加上物品表配置的经验
                fullexp += line.Exp;
            }
            return fullexp;
        }

        /// <summary>
        /// 取得物品品质
        /// </summary>
        /// <returns></returns>
        public ItemQuality GetQuality()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                return (ItemQuality)(line.Quality);
            }
            return ItemQuality.QUALITY_INVALID;
        }

        /// <summary>
        /// 取得装备套装ID
        /// </summary>
        /// <returns></returns>
        public int GetEquipSetId()
        {
            Tab_EquipAttr line = TableManager.GetEquipAttrByID(m_nDataID, 0);
            if (line != null)
            {
                return line.SetId;
            }
            return -1;
        }

        /// <summary>
        /// 取得装备在装备槽位的索引
        /// </summary>
        /// <returns></returns>
        public int GetEquipSlotIndex()
        {
            if (IsEquipMent())
            {
                int subclassid = TableManager.GetCommonItemByID(m_nDataID, 0).SubClassID;
                EquipSubClass ClassID = (EquipSubClass)subclassid;
                switch (ClassID)
                {
                    case EquipSubClass.WEAPON: return (int)EquipPackSlot.Slot_WEAPON;
                    case EquipSubClass.HEAD: return (int)EquipPackSlot.Slot_HEAD;
                    case EquipSubClass.ARMOR: return (int)EquipPackSlot.Slot_ARMOR;
                    case EquipSubClass.LEG_GUARD: return (int)EquipPackSlot.Slot_LEG_GUARD;
                    case EquipSubClass.CUFF: return (int)EquipPackSlot.Slot_CUFF;
                    case EquipSubClass.SHOES: return (int)EquipPackSlot.Slot_SHOES;
                    case EquipSubClass.CHARM1: return (int)EquipPackSlot.Slot_NECK;
                    case EquipSubClass.CHARM2: return (int)EquipPackSlot.Slot_RING;
                    case EquipSubClass.AMULET: return (int)EquipPackSlot.Slot_AMULET;
                    case EquipSubClass.BELT: return (int)EquipPackSlot.Slot_BELT;
                }
            }
            return -1;
        }

        public int GetProfessionRequire()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                return line.ProfessionRequire;
            }
            return -1;
        }

        //0无1拾取绑定2装备绑定
        public int GetBindType()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                return line.BindType;
            }
            return 0;
        }

        public int GetMinLevelRequire()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                return line.MinLevelRequire;
            }
            return -1;
        }

        public string GetIcon()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                return line.Icon;
            }
            return "";
        }
        public string GetName()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                return line.Name;
            }
            return "";
        }

        public int GetClass()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                return line.ClassID;
            }
            return -1;
        }

        public int GetSubClass()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                return line.SubClassID;
            }
            return -1;
        }

        public int GetEnchanceExpMax()
        {
            Tab_EquipEnchance tabEnchance = TableManager.GetEquipEnchanceByID(m_nEnchanceLevel + 1, 0);
            if (tabEnchance != null)
            {
                switch (GetQuality())
                {
                    case ItemQuality.QUALITY_WHITE:
                        return tabEnchance.WhiteExp;
                    case ItemQuality.QUALITY_GREEN:
                        return tabEnchance.GreenExp;
                    case ItemQuality.QUALITY_BLUE:
                        return tabEnchance.BlueExp;
                    case ItemQuality.QUALITY_PURPLE:
                        return tabEnchance.PurpleExp;
                    case ItemQuality.QUALITY_ORANGE:
                        return tabEnchance.OrangeExp;
                    default:
                        return -1;
                }
            }
            return -1;
        }

        /// <summary>
        /// 是否能售出
        /// </summary>
        /// <returns></returns>
        public bool CanSell()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                if (line.CanSell == 1)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否能使用
        /// </summary>
        /// <returns></returns>
        public bool CanUse()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                if (line.CanUse == 1)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否能丢弃
        /// </summary>
        /// <returns></returns>
        public bool CanThrow()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                if (line.CanThrow == 1)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是不是腰带
        /// </summary>
        /// <returns></returns>
        public bool IsBelt()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                if (line.ClassID == (int)ItemClass.EQUIP && line.SubClassID == (int)EquipSubClass.BELT)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是不是饰品
        /// </summary>
        /// <returns></returns>
        public bool IsCharm() 
        {
         Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                if (line.ClassID == (int)ItemClass.EQUIP && (line.SubClassID == (int)EquipSubClass.CHARM1||line.SubClassID==(int)EquipSubClass.CHARM2))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsGem()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (line != null)
            {
                if (line.ClassID == (int)ItemClass.STRENGTHEN && line.SubClassID == (int)StrengthenSubClass.GEM)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 取得物品的品质边框
        /// </summary>
        /// <returns></returns>
        public string GetQualityFrame()
        {
            switch (GetQuality())
            {
                case ItemQuality.QUALITY_WHITE:
                    return "ui_pub_012";
                case ItemQuality.QUALITY_GREEN:
                    return "ui_pub_013";
                case ItemQuality.QUALITY_BLUE:
                    return "ui_pub_014";
                case ItemQuality.QUALITY_PURPLE:
                    return "ui_pub_015";
                case ItemQuality.QUALITY_ORANGE:
                    return "ui_pub_016";
                default:
                    return "QualityGrey";
            }
        }

        /// <summary>
        /// 取得星级上限
        /// </summary>
        /// <returns></returns>
        public int GetMaxStarLevel()
        {
            int quality = (int)GetQuality();
            Tab_EquipStarLimit line = TableManager.GetEquipStarLimitByID(quality, 0);
            if (line != null)
            {
                return line.LimitLevel;
            }
            return 0;
        }

        public bool IsFitForPlayer()
        {
            Tab_CommonItem tabItem = TableManager.GetCommonItemByID(DataID, 0);
            if (tabItem == null)
            {
                return false;
            }

            if (Singleton<ObjManager>.GetInstance().MainPlayer == null)
            {
                return false;
            }

            int nPlayerLevel = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level;
            if (nPlayerLevel < tabItem.MinLevelRequire)
            {
                return false;
            }

            int nPlayerProfession = Singleton<ObjManager>.Instance.MainPlayer.Profession;
            if (nPlayerProfession != tabItem.ProfessionRequire && tabItem.ProfessionRequire != -1)
            {
                return false;
            }

            return true;
        }

        public bool IsEnchanceExpItem()
        {
            Tab_CommonItem tabItem = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (tabItem != null)
            {
                if (tabItem.ClassID == (int)ItemClass.STRENGTHEN && tabItem.SubClassID == (int)StrengthenSubClass.EQUIP_ENCHANCE)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsStarStone()
        {
            Tab_CommonItem tabItem = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (tabItem != null)
            {
                if (tabItem.ClassID == (int)ItemClass.STRENGTHEN && tabItem.SubClassID == (int)StrengthenSubClass.GEM_COMPOSE)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsLivingSkillDrawing()
        {
            Tab_CommonItem tabItem = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (tabItem != null)
            {
                if (tabItem.ClassID == (int)ItemClass.MATERIAL && tabItem.SubClassID == (int)MaterialSubClass.BLUEPRINT)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsFellowSkillBook()
        {
            Tab_CommonItem tabItem = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (tabItem != null)
            {
                if (tabItem.ClassID == (int)ItemClass.STRENGTHEN && tabItem.SubClassID == (int)StrengthenSubClass.FELLOW_SKILL)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsMountItem()
        {
            Tab_CommonItem tabItem = TableManager.GetCommonItemByID(m_nDataID, 0);
            if (tabItem != null)
            {
                if (tabItem.ClassID == (int)ItemClass.MOUNT)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetMountId()
        {
            int nMountID = -1;
            Tab_UsableItem tabItem = TableManager.GetUsableItemByID(m_nDataID, 0);
            if (null == tabItem)
            {
                return nMountID;
            }
            return tabItem.UseParamA;
        }

        /// <summary>
        /// 物品ID
        /// </summary>
        private int m_nDataID;
	    public int DataID
	    {
	        get { return m_nDataID; }
	        set { m_nDataID = value; }
	    }

        /// <summary>
        /// 物品GUID
        /// </summary>
        private UInt64 m_nGuid;
        public UInt64 Guid
        {
            get { return m_nGuid; }
            set { m_nGuid = value; }
        }

        /// <summary>
        /// 绑定标记
        /// </summary>
        private bool m_bBindFlag;
        public bool BindFlag
        {
            get { return m_bBindFlag; }
            set { m_bBindFlag = value; }
        }

        /// <summary>
        /// 堆叠数量
        /// </summary>
        private int m_nStackCount;
        public int StackCount
        {
            get { return m_nStackCount; }
            set { m_nStackCount = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        private int m_nCreateTime;
        public int CreateTime
        {
            get { return m_nCreateTime; }
            set { m_nCreateTime = value; }
        }

        /// <summary>
        /// 强化等级
        /// </summary>
        private int m_nEnchanceLevel;
        public int EnchanceLevel
        {
            get { return m_nEnchanceLevel; }
            set { m_nEnchanceLevel = value; }
        }
        
        /// <summary>
        /// 强化经验
        /// </summary>
        private int m_nEnchanceExp;
        public int EnchanceExp
        {
            get { return m_nEnchanceExp; }
            set { m_nEnchanceExp = value; }
        }

        /// <summary>
        /// 强化总经验
        /// </summary>
        private System.Int64 m_bEnchanceTotalExp;
        public System.Int64 EnchanceTotalExp
        {
            get { return m_bEnchanceTotalExp; }
            set { m_bEnchanceTotalExp = value; }
        }

        /// <summary>
        /// 打星等级
        /// </summary>
        private int m_nStarLevel;
        public int StarLevel
        {
            get { return m_nStarLevel; }
            set { m_nStarLevel = value; }
        }

        /// <summary>
        /// 打星次数
        /// </summary>
        private int m_nStarTimes;
        public int StarTimes
        {
            get { return m_nStarTimes; }
            set { m_nStarTimes = value; }
        }

        /// <summary>
        /// 动态数据
        /// </summary>
        private int[] m_oDynamicData;
        public int[] DynamicData
        {
            get { return m_oDynamicData; }
            set { m_oDynamicData = value; }
        }

        public bool IsPlayerEquiped()
        {
            if (false == IsValid())
            {
                return false;
            }

            if (false == IsEquipMent())
            {
                return false;
            }

            GameItemContainer equipPack = GameManager.gameManager.PlayerDataPool.EquipPack;
            if (equipPack.GetItemByGuid(Guid) == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 装备打星 显示用概率
        /// </summary>
        /// <returns></returns>
        public float GetStarShowRate()
        {
            Tab_EquipStar line = TableManager.GetEquipStarByID(m_nStarLevel+1, 0);
            if (line != null)
            {
                float baseRate = line.ShowRate;
                float addRate = 0f;
                if (line.MaxSatrTimes > 0 && StarTimes > 0)
                {
                    addRate = (1 - baseRate) / line.MaxSatrTimes * StarTimes;
                }
                float total = baseRate + addRate;
                return (total > 1) ? 1 : total;
            }
            return 0f;
        }

        public bool IsTimeLimitItem()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(DataID, 0);
            if (line != null)
            {
                if (line.ExistTime != -1)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetLeftTime()
        {
            int nLeftTime = -1;
            Tab_CommonItem line = TableManager.GetCommonItemByID(DataID, 0);
            if (null == line)
            {
                return nLeftTime;
            }
            if (-1 == line.ExistTime)
            {
                return nLeftTime;
            }
            nLeftTime = (line.ExistTime * 60) - (GlobalData.ServerAnsiTime - CreateTime);
            if (nLeftTime < 0)
            {
                nLeftTime = 0;
            }
            return nLeftTime;
        }
        
	}
}

