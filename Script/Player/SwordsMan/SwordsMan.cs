//********************************************************************
// 文件名: SwordsMan.cs
// 描述: 伙伴
// 作者: grx
// 创建时间: 2014-06-26
//********************************************************************
using UnityEngine;
using System.Collections.Generic;
using System;
using GCGame.Table;
using Games.GlobeDefine;

namespace Games.SwordsMan
{
    public class SwordsMan
    {

        public const int SWORDSMAN_LEVEL_MAX = 10;

        public enum SWORDSMANQUALITY
        {
            WHITE = 0,      //白色品质
            GREEN = 1,      //绿色品质
            BLUE = 2,       //蓝色品质
            PURPLE = 3,     //紫色品质
            ORANGE = 4,     //橙色品质
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <returns></returns>
        public SwordsMan()
        {
            CleanUp();
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void CleanUp()
        {
            m_nGuid = GlobeVar.INVALID_GUID;
            m_nDataId = -1;
            m_szName = "";
            m_nExp = 0;
            m_nLevel = -1;
            m_nQuality = -1;
            m_bLocked = false;
        }

        /// <summary>
        /// IsValid
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (m_nDataId < 0)
            {
                return false;
            }
            Tab_SwordsManAttr line = TableManager.GetSwordsManAttrByID(m_nDataId, 0);
            if (line != null)
            {
                return true;
            }            
            return false;
        }

        /// <summary>
        /// 侠客Guid
        /// </summary>
        /// <returns></returns>
        private UInt64 m_nGuid;
        public System.UInt64 Guid
        {
            get { return m_nGuid; }
            set { m_nGuid = value; }
        }

        /// <summary>
        /// 侠客ID
        /// </summary>
        /// <returns></returns>
        private int m_nDataId;
        public int DataId
        {
            get { return m_nDataId; }
            set { m_nDataId = value; }
        }

        /// <summary>
        /// 名称
        /// </summary>
        /// <returns></returns>
        private string m_szName;
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        /// <summary>
        /// 经验值
        /// </summary>
        /// <returns></returns>
        private int m_nExp;
        public int Exp
        {
            get { return m_nExp; }
            set { m_nExp = value; }
        }

        /// <summary>
        /// 经验值最大值
        /// </summary>
        /// <returns></returns>
        private int m_nMaxExp;
        public int MaxExp
        {
            get { return m_nMaxExp; }
            set { m_nMaxExp = value; }
        }

        /// <summary>
        /// 级别
        /// </summary>
        /// <returns></returns>
        private int m_nLevel;
        public int Level
        {
            get { return m_nLevel; }
            set { m_nLevel = value; }
        }

        /// <summary>
        /// 品质
        /// </summary>
        private int m_nQuality;
        public int Quality
        {
            get { return m_nQuality; }
            set { m_nQuality = value; }
        }

        /// <summary>
        /// 加锁
        /// </summary>
        private bool m_bLocked;
        public bool Locked
        {
            get { return m_bLocked; }
            set { m_bLocked = value; }
        }

        /// <summary>
        /// 战斗力 
        /// </summary>
        /// <returns></returns>
        public int GetCombatValue()
        {          
            float fTotalCombatValue = 0.0f;
            //血影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.MAXHP) * 0.2f);
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.MAXMP) * 0.1f);

            int nProfession = GameManager.gameManager.PlayerDataPool.Profession;
            if (nProfession == (int)CharacterDefine.PROFESSION.DALI ||
                nProfession == (int)CharacterDefine.PROFESSION.SHAOLIN ||
                nProfession == (int)CharacterDefine.PROFESSION.TIANSHAN)
            {
                //物理攻击影响
                fTotalCombatValue += (GetComBatAttrById(COMBATATTE.PYSATTACK) * 5.1f);
                //物理防御影响
                int nPDef = GetComBatAttrById(COMBATATTE.PYSDEF);
                int nMDef = GetComBatAttrById(COMBATATTE.MAGDEF);
                fTotalCombatValue += ((nPDef * 1.0f) + (nMDef * 1.0f)) * 3.0f;
            }
            else if (nProfession == (int)CharacterDefine.PROFESSION.XIAOYAO)
            {
                //法术攻击影响
                fTotalCombatValue += (GetComBatAttrById(COMBATATTE.MAGATTACK) * 5.1f);
                //法术防御影响
                int nPDef = GetComBatAttrById(COMBATATTE.PYSDEF);
                int nMDef = GetComBatAttrById(COMBATATTE.MAGDEF);
                fTotalCombatValue += ((nPDef * 1.0f) + (nMDef * 1.0f)) * 3.0f;
            }

            //命中影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.HIT) * 3.0f);
            //闪避影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.DODGE) * 4.2f);
            //暴击影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.CRITICAL) * 1.0f);
            //暴抗影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.DECRITICAL) * 0.8f);
            //穿透影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.STRIKE) * 8.3f);
            //韧性影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.DUCTICAL) * 8.3f);
            //暴击加成影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.CRITIADD) * 6.5f);
            //暴击减免影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.CRITIMIS) * 6.5f);

            return (int)fTotalCombatValue;
        }

        /// <summary>
        /// 战斗力 
        /// </summary>
        /// <returns></returns>
        public int GetNextlevelCombatValue()
        {
            // todo_xiake
            float fTotalCombatValue = 0.0f;
            //血影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.MAXHP) * 0.2f);
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.MAXMP) * 0.1f);

            int nProfession = GameManager.gameManager.PlayerDataPool.Profession;
            if (nProfession == (int)CharacterDefine.PROFESSION.DALI ||
                nProfession == (int)CharacterDefine.PROFESSION.SHAOLIN ||
                nProfession == (int)CharacterDefine.PROFESSION.TIANSHAN)
            {
                //物理攻击影响
                fTotalCombatValue += (GetComBatAttrById(COMBATATTE.PYSATTACK) * 5.1f);
                //物理防御影响
                int nPDef = GetComBatAttrById(COMBATATTE.PYSDEF);
                int nMDef = GetComBatAttrById(COMBATATTE.MAGDEF);
                fTotalCombatValue += ((nPDef * 0.6f) + (nMDef * 0.4f)) * 3.0f;
            }
            else if (nProfession == (int)CharacterDefine.PROFESSION.XIAOYAO)
            {
                //法术攻击影响
                fTotalCombatValue += (GetComBatAttrById(COMBATATTE.MAGATTACK) * 5.1f);
                //法术防御影响
                int nPDef = GetComBatAttrById(COMBATATTE.PYSDEF);
                int nMDef = GetComBatAttrById(COMBATATTE.MAGDEF);
                fTotalCombatValue += ((nPDef * 0.4f) + (nMDef * 0.6f)) * 3.0f;
            }

            //血影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.MAXHP) * 0.2f);
            //命中影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.HIT) * 3.0f);
            //闪避影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.DODGE) * 4.2f);
            //暴击影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.CRITICAL) * 1.0f);
            //暴抗影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.DECRITICAL) * 0.8f);
            //穿透影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.STRIKE) * 8.3f);
            //韧性影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.DUCTICAL) * 8.3f);
            //暴击加成影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.CRITIADD) * 6.5f);
            //暴击减免影响
            fTotalCombatValue += (GetComBatAttrById(COMBATATTE.CRITIMIS) * 6.5f);

            return (int)fTotalCombatValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nAttrType"></param>
        /// <returns></returns>
        bool IsValidAttrType(int attrtype)
        {
            if ((int)attrtype >= (int)COMBATATTE.MAXHP && (int)attrtype < (int)COMBATATTE.COMBATATTE_MAXNUM)
            {
                return true;
            }
            if ((int)attrtype > (int)MIXBATATTR.MIXATTR_BEGIN && (int)attrtype <= (int)MIXBATATTR.MIXATTR_ALLDEF)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 侠客附加的战斗属性值
        /// </summary>
        /// <param name="attrtype"></param>
        /// <returns></returns>
        public int GetComBatAttrById(COMBATATTE attrtype)
        {
            int nBaseAttrValue = 0;
            if (IsValidAttrType((int)attrtype) == false)
            {
                return nBaseAttrValue;
            }
            Tab_SwordsManAttr SwordsMmanTable = TableManager.GetSwordsManAttrByID(m_nDataId, 0);
            if ( null == SwordsMmanTable)
            {
                return nBaseAttrValue;
            }
               
            //附加属性
            for (int attrIndex = 0; attrIndex < SwordsMmanTable.getAddAttrTypeCount(); ++attrIndex)
            {
                int addAttrType = SwordsMmanTable.GetAddAttrTypebyIndex(attrIndex);
                float addAttrValue = SwordsMmanTable.GetAddAttrValuebyIndex(attrIndex);
                if ((int)attrtype == addAttrType || MaxBatAttrUtil.IsContainType((MIXBATATTR)addAttrType, attrtype))
                {
                    nBaseAttrValue += (int)addAttrValue;
                }              
            }
            float fAddValue = (float)nBaseAttrValue + (float)(m_nLevel-1)*(float)0.4*(float)nBaseAttrValue;
            int nAttrValue = (int)fAddValue;
            return nAttrValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attrtype"></param>
        /// <returns></returns>
        public int GetNextLevelComBatAttrById(COMBATATTE attrtype)
        {
            int nBaseAttrValue = 0;
            if (IsValidAttrType((int)attrtype) == false)
            {
                return nBaseAttrValue;
            }
            Tab_SwordsManAttr SwordsMmanTable = TableManager.GetSwordsManAttrByID(m_nDataId, 0);
            if (null == SwordsMmanTable)
            {
                return nBaseAttrValue;
            }

            //附加属性
            for (int attrIndex = 0; attrIndex < SwordsMmanTable.getAddAttrTypeCount(); ++attrIndex)
            {
                int addAttrType = SwordsMmanTable.GetAddAttrTypebyIndex(attrIndex);
                float addAttrValue = SwordsMmanTable.GetAddAttrValuebyIndex(attrIndex);
                if ((int)attrtype == addAttrType || MaxBatAttrUtil.IsContainType((MIXBATATTR)addAttrType, attrtype))
                {
                    nBaseAttrValue += (int)addAttrValue;
                }
            }
            int nLevel = m_nLevel+1;
            if (nLevel > SWORDSMAN_LEVEL_MAX)
            {
                nLevel = SWORDSMAN_LEVEL_MAX;
            }
            float fAddValue = (float)nBaseAttrValue + (float)(nLevel - 1) * (float)0.4 * (float)nBaseAttrValue;
            int nAttrValue = (int)fAddValue;
            return nAttrValue;
        }

        /// <summary>
        /// 头像
        /// </summary>
        /// <returns></returns>
        public string GetIcon()
        {
            Tab_SwordsManAttr SwordsMmanTable = TableManager.GetSwordsManAttrByID(m_nDataId, 0);
            if (null == SwordsMmanTable)
            {
                return null;
            }
            //todo_xiake
            return SwordsMmanTable.Icon;
        }

        /// <summary>
        /// 描述信息
        /// </summary>
        /// <returns></returns>
        public string GetTips()
        {
            Tab_SwordsManAttr SwordsMmanTable = TableManager.GetSwordsManAttrByID(m_nDataId, 0);
            if (null == SwordsMmanTable)
            {
                return null;
            }
            //todo_xiake
            return SwordsMmanTable.Tips;
        }

        public static string GetQualitySpriteName(SWORDSMANQUALITY quality)
        {
            switch (quality)
            {
                case SWORDSMANQUALITY.WHITE:
                    return "ui_pub_012";//"QualityGrey";
                case SWORDSMANQUALITY.GREEN:
                    return "ui_pub_013";//"QualityGreen";
                case SWORDSMANQUALITY.BLUE:
					return "ui_pub_014";//"QualityBlue";
                case SWORDSMANQUALITY.PURPLE:
					return "ui_pub_015";// return "QualityPurple";
                case SWORDSMANQUALITY.ORANGE:
					return "ui_pub_016";// return "QualityYellow";
                default:
					return "ui_pub_012";// return "QualityGrey";
            }
        }

       public bool IsFullLevel()
        {
           if (m_nLevel >=SWORDSMAN_LEVEL_MAX)
           {
               return true;
           }
           return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetEatExp()
       {
           Tab_SwordsManAttr SwordsmanAttrTable = TableManager.GetSwordsManAttrByID(m_nDataId, 0);
           if (null == SwordsmanAttrTable)
           {
               return 0;
           }
           Tab_SwordsManLevelUp SwordsManLevelUpTable = TableManager.GetSwordsManLevelUpByID(m_nLevel, 0);
           if (null == SwordsManLevelUpTable)
           {
               return 0;
           }
           int nBaseExp = SwordsmanAttrTable.SwordsManExp;
           int nTotalEatExp = Exp;
           for (int i = 0; i < SwordsManLevelUpTable.getExpNeedLvCount() && i < m_nLevel; i++)
           {
               nTotalEatExp += SwordsManLevelUpTable.GetExpNeedLvbyIndex(i);
           }
           //float fTotalEatExp = (float)nTotalEatExp * (float)0.4;
           int nTotalExp = nBaseExp + nTotalEatExp;
           return nTotalExp;
       }

    }
}
