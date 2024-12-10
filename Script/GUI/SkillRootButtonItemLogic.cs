using System;
using Games.LogicObj;
using GCGame.Table;
using UnityEngine;
using System.Collections;
using Games.SkillModle;

public class SkillRootButtonItemLogic : MonoBehaviour
{
    public UILabel m_SkillNameLabel;
    public UILabel m_SkillLevLabel;
    public UISprite m_SkillIconSprite;
    public UISprite m_SkillBakSprite;
    private int m_nSkillIndex =-1 ;//表示玩家身上技能的索引下标
    public GameObject m_LevelTipsIcon; //升级提醒的角标
    public int SkillIndex
    {
        get { return m_nSkillIndex; }
        set { m_nSkillIndex = value; }
    }

    private int m_nSkillID =-1;
    public int SkillID
    {
        get { return m_nSkillID; }
        set { m_nSkillID = value; }
    }

    private int m_nSkillBaseId = -1;
    public int SkillBaseId
    {
        get { return m_nSkillBaseId; }
        set { m_nSkillBaseId = value; }
    }
	// Use this for initialization
	void Start ()
    {
	
	}
    public void UpdateNoStudyButtonInfo(int nSkillId)
    {
        Tab_SkillEx _skillEx = TableManager.GetSkillExByID(nSkillId, 0);
        if (_skillEx != null)
        {
            Tab_SkillBase _skillBase = TableManager.GetSkillBaseByID(_skillEx.BaseId, 0);
            if (_skillBase != null)
            {
                //师门技能显示自定义名字
                if ((_skillBase.SkillClass & (int) SKILLCLASS.MASTERSKILL) != 0)
                {
                    m_SkillNameLabel.text = String.Format("[d13f30]{0}", GameManager.gameManager.PlayerDataPool.GetMasterSkillName(nSkillId));
                    if (m_SkillNameLabel.text == "")
                    {
                        m_SkillNameLabel.text = String.Format("[d13f30]{0}", _skillBase.Name);
                    }
                    m_SkillLevLabel.text = "";
                }
                else
                {
                    m_SkillNameLabel.text = String.Format("[d13f30]{0}", _skillBase.Name);
                    //显示可以激活的等级
                    for (int i = 1; i <= TableManager.GetSkillActive().Count; i++)
                    {
                        Tab_SkillActive _skillActiveInfo = TableManager.GetSkillActiveByID(i, 0);
                        if (_skillActiveInfo !=null)
                        {
                            if (_skillActiveInfo.SkillId ==nSkillId)
                            {
                                m_SkillLevLabel.text = String.Format("[d13f30]{0}", StrDictionary.GetClientDictionaryString("#{2770}", _skillActiveInfo.Level));
                                break;
                            }
                        }
                    }
                }
                m_SkillIconSprite.spriteName = _skillBase.Icon;
                m_SkillIconSprite.MakePixelPerfect();
                m_nSkillIndex = -1;
                m_nSkillID = nSkillId;
                m_nSkillBaseId = _skillEx.BaseId;
                m_LevelTipsIcon.SetActive(false);
            }
        }
    }
    public void UpdateButtonInfo(int nSkillIndex)
    {
        m_nSkillIndex = nSkillIndex;
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer ==null)
        {
            return;
        }
        if (m_nSkillIndex >= 0 && m_nSkillIndex<_mainPlayer.OwnSkillInfo.Length)
        {
            int nSkillId = _mainPlayer.OwnSkillInfo[m_nSkillIndex].SkillId;
            Tab_SkillEx _skillEx = TableManager.GetSkillExByID(nSkillId, 0);
            if (_skillEx != null)
            {
                Tab_SkillBase _skillBase = TableManager.GetSkillBaseByID(_skillEx.BaseId, 0);
                if (_skillBase != null)
                {
                    //师门技能显示自定义名字
                    if ((_skillBase.SkillClass & (int) SKILLCLASS.MASTERSKILL) != 0)
                    {
                        m_SkillNameLabel.text = GameManager.gameManager.PlayerDataPool.GetMasterSkillName(nSkillId);
                        if (m_SkillNameLabel.text == "")
                        {
                            m_SkillNameLabel.text = _skillBase.Name;
                        }
                    }
                    else
                    {
                        m_SkillNameLabel.text = _skillBase.Name;
                    }
                    m_nSkillID = nSkillId;
                    m_nSkillBaseId = _skillEx.BaseId;
                    m_SkillIconSprite.spriteName = _skillBase.Icon;
                    m_SkillIconSprite.MakePixelPerfect();
                    m_SkillLevLabel.text = StrDictionary.GetClientDictionaryString("#{2162}", _skillEx.Level);
                    //是否要显示升级提醒的角标
                    if (SkillRootLogic.IsCanLevelUpSkill(nSkillId) ||
                        SkillRootLogic.IsCanLevelUpMasterSkill(nSkillId))
                    {
                        m_LevelTipsIcon.SetActive(true);
                    }
                    else
                    {
                        m_LevelTipsIcon.SetActive(false);
                    }

                }
            }
        }
    }
    public void ClickButton()
    {
        if (SkillRootLogic.Instance() != null)
        {
            SkillRootLogic.Instance().SelectSkillIndex = m_nSkillIndex;
            if (SkillRootLogic.Instance().CurClickBtItem != null)
            {
                SkillRootLogic.Instance().CurClickBtItem.m_SkillBakSprite.spriteName = "CommonBack6Nor";
                m_SkillBakSprite.color = new Color(0, 0, 0, 0);
                //SkillRootLogic.Instance().CurClickBtItem.m_SkillBakSprite.MakePixelPerfect();
            }
            m_SkillBakSprite.spriteName = "ui_pub_038";
            m_SkillBakSprite.color = new Color(1,1,1,1);
            //m_SkillBakSprite.MakePixelPerfect();
            SkillRootLogic.Instance().CurClickBtItem = this;
            if (m_nSkillIndex !=-1) //已经学习的技能
            {
                if (SkillRootLogic.Instance().NewPlayerGuide_Step == 1 && gameObject.name == "1002")
                {
                    NewPlayerGuidLogic.CloseWindow();
                    SkillRootLogic.Instance().NewPlayerGuide(2);
                }
                SkillRootLogic.Instance().ShowSkillInfo();  
            }
            else //未学习的技能
            {
                SkillRootLogic.Instance().ShowNoStudySkillInfo(m_nSkillID);
            }
        }
    }

    void PressButton()
    {
        if (m_SkillIconSprite.GetComponent<UIDragObject>().enabled ==false)
        {
            return;
        }
       
        if (SkillRootLogic.Instance() != null)
        {
            UISprite DragSprite = SkillRootLogic.Instance().m_SkillButtonDragSprite;
            DragSprite.transform.localPosition = DragSprite.transform.parent.InverseTransformPoint(m_SkillIconSprite.transform.position);
            DragSprite.spriteName = m_SkillIconSprite.spriteName;
            DragSprite.MakePixelPerfect();
            DragSprite.gameObject.SetActive(true);
        }
    }

    void ReleaseButton()
    {
        if (m_SkillIconSprite.GetComponent<UIDragObject>().enabled == false)
        {
            return;
        }
        if (SkillRootLogic.Instance() != null)
        {
            SkillRootLogic.Instance().ReleaseButtonDragSprite(m_nSkillIndex);
        }
    }
}
