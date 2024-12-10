//修改日志
// 2014-5-28 Lijia: 客户端效率优化，把GemData从class改为struct
using UnityEngine;
using System.Collections;
using Games.Item;
using GCGame.Table;
using Games.GlobeDefine;

namespace Games.Item
{
    public struct GemData
    {

        public enum CONSTVALUE
        {
            SIZE = EquipPackSlot.Slot_NUM * GemSlot.OPEN_NUM,
        }

        public static int GetGemAttrValue(COMBATATTE attr, int gemid)
        {
            Tab_GemAttr line = TableManager.GetGemAttrByID(gemid, 0);
            if (line != null)
            {
                switch (attr)
                {
                    case COMBATATTE.MAXHP:
                        if (line.MaxHP > 0)
                        {
                            return line.MaxHP;
                        }
                        break;
                    case COMBATATTE.MAXMP:
                        if (line.MaxMP > 0)
                        {
                            return line.MaxMP;
                        }
                        break;
                    case COMBATATTE.MAXXP:
                        break;
                    case COMBATATTE.PYSATTACK:
                        if (line.PysAttack > 0)
                        {
                            return line.PysAttack;
                        }
                        break;
                    case COMBATATTE.MAGATTACK:
                        if (line.MagAttack > 0)
                        {
                            return line.MagAttack;
                        }
                        break;
                    case COMBATATTE.PYSDEF:
                        if (line.PysDef > 0)
                        {
                            return line.PysDef;
                        }
                        break;
                    case COMBATATTE.MAGDEF:
                        if (line.MagDef > 0)
                        {
                            return line.MagDef;
                        }
                        break;
                    case COMBATATTE.HIT:
                        if (line.Hit > 0)
                        {
                            return line.Hit;
                        }
                        break;
                    case COMBATATTE.DODGE:
                        if (line.Dodge > 0)
                        {
                            return line.Dodge;
                        }
                        break;
                    case COMBATATTE.CRITICAL:
                        if (line.Critical > 0)
                        {
                            return line.Critical;
                        }
                        break;
                    case COMBATATTE.DECRITICAL:
                        if (line.Decritical > 0)
                        {
                            return line.Decritical;
                        }
                        break;
                    case COMBATATTE.STRIKE:
                        if (line.Strike > 0)
                        {
                            return line.Strike;
                        }
                        break;
                    case COMBATATTE.DUCTICAL:
                        if (line.Ductical > 0)
                        {
                            return line.Ductical;
                        }
                        break;
                    case COMBATATTE.CRITIADD:
                        if (line.CritiAdd > 0)
                        {
                            return line.CritiAdd;
                        }
                        break;
                    case COMBATATTE.CRITIMIS:
                        if (line.CritiMis > 0)
                        {
                            return line.CritiMis;
                        }
                        break;
                    case COMBATATTE.MOVESPEED:
                        break;
                    case COMBATATTE.ATTACKSPEED:
                        break;
                    default:
                        break;
                }
            }
            return 0;
        }

        public static int GetGemCombatValue(int gemid)
        {
            float fTotalCombatValue = 0.0f;
            if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
                return (int)fTotalCombatValue;

            //血影响
            fTotalCombatValue += (GetGemAttrValue(COMBATATTE.MAXHP, gemid) * 0.2f);
            int profession = Singleton<ObjManager>.GetInstance().MainPlayer.Profession;
            if (profession == (int)CharacterDefine.PROFESSION.DALI ||
                profession == (int)CharacterDefine.PROFESSION.SHAOLIN ||
                profession == (int)CharacterDefine.PROFESSION.TIANSHAN)
            {
                //物理攻击影响
                fTotalCombatValue += (GetGemAttrValue(COMBATATTE.PYSATTACK, gemid) * 5.1f);
                //物理防御影响
                int nPDef = GetGemAttrValue(COMBATATTE.PYSDEF, gemid);
                int nMDef = GetGemAttrValue(COMBATATTE.MAGDEF, gemid);
                fTotalCombatValue += ((nPDef * 0.6f) + (nMDef * 0.4f)) * 3.0f;
            }
            else if (profession == (int)CharacterDefine.PROFESSION.XIAOYAO)
            {
                //法术攻击影响
                fTotalCombatValue += (GetGemAttrValue(COMBATATTE.MAGATTACK, gemid) * 5.1f);
                //法术防御影响
                int nPDef = GetGemAttrValue(COMBATATTE.PYSDEF, gemid);
                int nMDef = GetGemAttrValue(COMBATATTE.MAGDEF, gemid);
                fTotalCombatValue += ((nPDef * 0.4f) + (nMDef * 0.6f)) * 3.0f;
            }
            //命中影响
            fTotalCombatValue += (GetGemAttrValue(COMBATATTE.HIT, gemid) * 3.0f);
            //闪避影响
            fTotalCombatValue += (GetGemAttrValue(COMBATATTE.DODGE, gemid) * 4.2f);
            //暴击影响
            fTotalCombatValue += (GetGemAttrValue(COMBATATTE.CRITICAL, gemid) * 1.0f);
            //暴抗影响
            fTotalCombatValue += (GetGemAttrValue(COMBATATTE.DECRITICAL, gemid) * 0.8f);
            //穿透影响
            fTotalCombatValue += (GetGemAttrValue(COMBATATTE.STRIKE, gemid) * 8.3f);
            //韧性影响
            fTotalCombatValue += (GetGemAttrValue(COMBATATTE.DUCTICAL, gemid) * 8.3f);
            //暴击加成影响
            fTotalCombatValue += (GetGemAttrValue(COMBATATTE.CRITIADD, gemid) * 6.5f);
            //暴击减免影响
            fTotalCombatValue += (GetGemAttrValue(COMBATATTE.CRITIMIS, gemid) * 6.5f);

            return (int)fTotalCombatValue;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public void CleanUp()
        {
            GemId = new int[(int)CONSTVALUE.SIZE];
            for (int i = 0; i < (int)CONSTVALUE.SIZE; i++)
            {
                GemId[i] = -1;
            }
        }

        public int GetGemId(int EquipSlot, int Index)
        {
            if (EquipSlot < 0 || EquipSlot >= (int)EquipPackSlot.Slot_NUM)
            {
                return -1;
            }

            if (Index < 0 || Index >= (int)GemSlot.OPEN_NUM)
            {
                return -1;
            }

            int totalIndex = EquipSlot * (int)GemSlot.OPEN_NUM + Index;
            if (totalIndex >= 0 && totalIndex < (int)CONSTVALUE.SIZE)
            {
                return GemId[totalIndex];
            }

            return -1;
        }

        public bool SetGemId(int EquipSlot, int Index, int Id)
        {
            if (EquipSlot < 0 || EquipSlot >= (int)EquipPackSlot.Slot_NUM)
            {
                return false;
            }

            if (Index < 0 || Index >= (int)GemSlot.OPEN_NUM)
            {
                return false;
            }

            int totalIndex = EquipSlot * (int)GemSlot.OPEN_NUM + Index;
            if (totalIndex >= 0 && totalIndex < (int)CONSTVALUE.SIZE)
            {
                GemId[totalIndex] = Id;
                PlayerGemDataViewModel GemViewModel = GameViewModel.Get<PlayerGemDataViewModel>();
                GemViewModel.GemsId[totalIndex] = Id;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 记录宝石ID
        /// </summary>
        private int[] GemId;
        public int[] Gem
        {
            get { return GemId; }
            set { GemId = value; }
        }
    }
}
