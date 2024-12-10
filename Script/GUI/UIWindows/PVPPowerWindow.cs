using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Games.LogicObj;
using Games.SkillModle;
using GCGame;
using GCGame.Table;
using UnityEngine;
using System.Collections;
using Module.Log;

public class PVPPowerWindow : MonoBehaviour {

	public GameObject m_PVPSkillListGrid;

	
	public GameObject m_ObjCurSkillInfo;

    public UILabel m_LabelEnergyCost;
    public UILabel m_LabelPowerAdd;

    public UILabel m_LabelCurSkillName;
    public UILabel m_LabelCurSkillLeve;

    public UILabel m_LableCurSkillInfo;
    public UILabel m_LableNextSkillInfo;

    public UILabel m_LableCombatValue;
    private  IDictionary<string,PVPSkillListItem> m_ItemInfo =new Dictionary<string, PVPSkillListItem>();
    private int m_nCurSkillId = -1;

    private string m_curItemIndexName = "";
	// Use this for initialization
	void Start () 
    {
        m_PVPSkillListGrid.GetComponent<UIGrid>().Reposition();
        m_PVPSkillListGrid.GetComponent<UITopGrid>().Recenter(true);
    }

    void OnEnable()
    {
        Singleton<ObjManager>.GetInstance().MainPlayer.AskCombatValue(false);
        UpdateSkillList();
    }
    private static PVPPowerWindow m_Instance = null;
    public static PVPPowerWindow Instance()
    {
        return m_Instance;
    }
    void Awake()
    {
        m_Instance = this;
    }
    public  void UpdateSkillList()
    {
        UIManager.LoadItem(UIInfo.PVPSkillListItem, OnLoadPVPSkillItem);
	}

    void OnLoadPVPSkillItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load pvp skill item fail");
            return;
        }

        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;

        Utils.CleanGrid(m_PVPSkillListGrid);
        m_ItemInfo.Clear();
        if (_mainPlayer)
        {
            m_ObjCurSkillInfo.SetActive(false);
            //普攻和XP不可升级
            int nIndex = 0;
            for (int i = 0; i < _mainPlayer.OwnSkillInfo.Length; i++)
            {
                int nSkillId = _mainPlayer.OwnSkillInfo[i].SkillId;
                Tab_SkillEx _skillEx = TableManager.GetSkillExByID(nSkillId, 0);
                if (_skillEx != null)
                {
                    Tab_SkillBase _skillBase = TableManager.GetSkillBaseByID(_skillEx.BaseId, 0);
                    if (_skillBase != null &&
                        (_skillBase.SkillClass & (int)SKILLCLASS.AUTOREPEAT) == 0 &&
                        (_skillBase.SkillClass & (int)SKILLCLASS.XP) == 0)
                    {
						PVPSkillListItem _skillItem = PVPSkillListItem.CreateItem(m_PVPSkillListGrid, resItem, nIndex.ToString(), this, _skillBase.Name, "等级:" + _skillEx.Level.ToString(), nSkillId);
                        if (_skillItem != null)
                        {
                            m_ItemInfo.Add(nIndex.ToString(), _skillItem);
                            nIndex++;
                        }
                    }
                }
            }
            m_PVPSkillListGrid.GetComponent<UIGrid>().Reposition();
            m_PVPSkillListGrid.GetComponent<UITopGrid>().Recenter(false);
            if (m_curItemIndexName == "" && m_ItemInfo.ContainsKey("0"))
            {
                ShowCurSkill(m_ItemInfo["0"]);
            }
            else if (m_ItemInfo.ContainsKey(m_curItemIndexName))
            {
                ShowCurSkill(m_ItemInfo[m_curItemIndexName]);
            }
        }    

    }
    public void UpdateCombatValue()
    {
        m_LableCombatValue.text = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.CombatValue.ToString();
    }
	public void ShowCurSkill(PVPSkillListItem item)
	{
		if(null == item)
		{
			return;
		}
        m_ObjCurSkillInfo.SetActive(true);
	    m_curItemIndexName = item.name;
        item.EnableHighlight(true);
        
	    m_LabelCurSkillName.text = item.m_LabelName.text;
	    m_LabelCurSkillLeve.text = item.m_LabelLev.text;
	    m_nCurSkillId = item.SkillId;
	    Tab_SkillEx _CurskillEx = TableManager.GetSkillExByID(item.SkillId,0);
	    int nCurSkillCombatValue = 0;
	    int nNextSkillCombatValue = 0;
	    if (_CurskillEx !=null)
	    {
	        m_LableCurSkillInfo.text = _CurskillEx.Desc;
	        nCurSkillCombatValue = _CurskillEx.CombatValue;
	    }
        Tab_SkillLevelUp _skillLevUp = TableManager.GetSkillLevelUpByID(m_nCurSkillId, 0);

 	    if (_skillLevUp!=null)
 	    {
          
 	        if (_skillLevUp.NextSkillId !=-1)
 	        {
                m_LabelEnergyCost.text = _skillLevUp.NeedConsume.ToString();
                Tab_SkillEx _NextskillEx = TableManager.GetSkillExByID(_skillLevUp.NextSkillId, 0);
                if (_NextskillEx !=null)
                {
                    m_LableNextSkillInfo.text = _NextskillEx.Desc;
                    nNextSkillCombatValue = _NextskillEx.CombatValue;
                }
 	            int nCombatValueAdd = nNextSkillCombatValue - nCurSkillCombatValue;
                m_LabelPowerAdd.text = nCombatValueAdd.ToString();
 	        }
 	        else
 	        {
                m_LabelEnergyCost.text = "0";
                m_LableNextSkillInfo.text = "";
 	            m_LabelPowerAdd.text = "0";
 	        }
 	    }
       
	}

    bool IsCanLevelUpSkill(int nSkillId)
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer ==null)
        {
            return false;
        }
        Tab_SkillLevelUp _skillLevUp = TableManager.GetSkillLevelUpByID(m_nCurSkillId, 0);
        if (_skillLevUp ==null)
        {
            return false;
        }
        if (_mainPlayer.Profession !=_skillLevUp.Profession)
        {
            return false;
        }
        if (_mainPlayer.BaseAttr.Level <_skillLevUp.Level)
        {
            return false;
        }
        if (_mainPlayer.IsHaveSkill(_skillLevUp.NeedSkillId) ==false)
        {
            return false;
        }
        //真气不足
        if (PVPData.Power <_skillLevUp.NeedConsume)
        {
            return false;
        }
        return true;
    }
	void OnLevelupClick()
	{
	    if (IsCanLevelUpSkill(m_nCurSkillId))
	    {
            //MessageBoxLogic.OpenOKCancelBox("是否升级"+m_LabelCurSkillName.text, "", DoLevelUp);
            MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2808}",  m_LabelCurSkillName.text), "", DoLevelUp);
	    }
	}

	void DoLevelUp()
	{
        //发包
        CG_ASK_LEVELUPSKILL packet = (CG_ASK_LEVELUPSKILL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_LEVELUPSKILL);
        packet.SetSkillId(m_nCurSkillId);
        packet.SendPacket();
	}

		                               
}
