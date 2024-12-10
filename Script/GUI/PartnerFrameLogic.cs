using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.Fellow;
using GCGame;
using Module.Log;
using GCGame.Table;
using Games.UserCommonData;
using Games.GlobeDefine;

public class PartnerFrameLogic : MonoBehaviour
{

    public enum CONTENT_TYPE
    {
        CONTENT_TYPE_INVALID        = -1,
        CONTENT_TYPE_INFO           = 0,        // 我的伙伴
        CONTENT_TYPE_GAMBLE         = 1,        // 伙伴获得
        CONTENT_TYPE_STREN,                     // 资质强化
        CONTENT_TYPE_SKILL,                     // 伙伴技能
        CONTENT_TYPE_STAR ,                     // 伙伴升星
    }

    private enum GAMBLE_CONTENT
    {
        GAMBLE_CONTENT_EXCHAGNE     = 0,
        GAMBLE_CONTENT_MAIN         = 1,
        GAMBLE_CONTENT_PREVIEW      = 2,
    }

    public List<UIImageButton> m_TabButtonList;
    public List<GameObject> m_ContentList; 
    public GameObject m_GambleMain;
    public GameObject m_PartnerListGrid;
    public UILabel m_PartnerNumLabel;
    public GameObject m_PartnerList;
    public UILabel m_PartnerGainRemain;
    public UILabel m_PartnerSkillRemain;

    public CONTENT_TYPE m_CurContent;           //当前Content
    public Fellow m_CurFellow;                  //当前选中伙伴

    private static PartnerFrameLogic m_Instance = null;
    public static PartnerFrameLogic Instance()
    {
        return m_Instance;
    }

    private int m_NewPlayerGuide_Step = -1;
    public int NewPlayerGuide_Step
    {
        get { return m_NewPlayerGuide_Step; }
        set { m_NewPlayerGuide_Step = value; }
    }
    public GameObject m_CoinGambleButton;

    public delegate void StartAfterCallDelegate();
    public StartAfterCallDelegate startDelegate;

    public void SetStartDelegate(int type)
    {
        switch (type)
        {
            case 1:

                startDelegate = ShowPartnerGamble;
                break;
            case 2:
                startDelegate = ShowZiZhiStren;
                break;
            case 3:
                startDelegate = ShowRaiseStar;
                break;
            case 4:
                startDelegate = ShowPartnerSkill;
                break;
        }
        if (startDelegate != null)
        {
            startDelegate();
            startDelegate = null;
        }
    }

    private bool m_UpdateUI = false;
    public void SetUpdateUI()
    {
        m_UpdateUI = true;
    }

    void Start()
    {
        if (startDelegate != null)
        {
            startDelegate();
            startDelegate = null;
        }
        Check_NewPlayerGuide();
    }

	void OnEnable () {
        m_Instance = this;

        ShowPartnerInfo();
        InitPartnerList();

        Check_ShowContentTab();

        UpdateGainRemain();
        UpdateSkillRemain();

        InvokeRepeating("Tick_Update", 0f, 0.2f);

        Check_NewPlayerGuide();
	}

    void OnDisable()
    {
        m_Instance = null;
        m_CurFellow = null;
        CancelInvoke("Tick_Update");
    }

    void Tick_Update()
    {
        if (m_UpdateUI)
        {
            UpdatePartnerFrame();
            m_UpdateUI = false;
        }
    }

    bool IsAllContentValid()
    {
        for (int i = 0; i < m_ContentList.Count; i++)
        {
            if (m_ContentList[i] == null)
            {
                return false;
            }
        }
        if (m_GambleMain == null)
        {
            return false;
        }
        return true;
    }

    void ShowPartnerContent(CONTENT_TYPE eContent)
    {
		NewPlayerGuidLogic.CloseWindow();

        if (IsAllContentValid())
        {
            m_TabButtonList[(int)eContent].normalSprite = "ui_pub_010";
			m_TabButtonList[(int)eContent].hoverSprite = "ui_pub_010";
			m_TabButtonList[(int)eContent].pressedSprite = "ui_pub_010";
			m_TabButtonList[(int)eContent].disabledSprite = "ui_pub_010";
			m_TabButtonList[(int)eContent].target.spriteName = "ui_pub_010";

			m_TabButtonList[(int)eContent].GetComponentInChildren<UISprite>().depth = 62;


			if(eContent == CONTENT_TYPE.CONTENT_TYPE_INFO)
			{
				m_TabButtonList[(int)eContent].normalSprite = "ui_pub_008";
				m_TabButtonList[(int)eContent].hoverSprite = "ui_pub_008";
				m_TabButtonList[(int)eContent].pressedSprite = "ui_pub_008";
				m_TabButtonList[(int)eContent].disabledSprite = "ui_pub_008";
				m_TabButtonList[(int)eContent].target.spriteName = "ui_pub_008";
			}

//			UILabel[] labelArr = m_TabButtonList[(int)eContent].gameObject.GetComponentsInChildren<UILabel>();
//			foreach(UILabel label in labelArr)
//			{
//				if(label.name == "Label")
//				{
//					label.color = new Color(217f/255f,241f/255f,254f/255f);
//					label.effectColor = new Color(15f/255f,94f/255f,239f/255f,102f/255f);
//					label.effectDistance = new Vector2(2,2);
//				}
//			}

			m_TabButtonList[(int)eContent].target.MakePixelPerfect();

            m_ContentList[(int)eContent].SetActive(true);
            m_CurContent = eContent;
            for (int i = 0; i < m_ContentList.Count; i++)
            {
                if (i != (int)eContent)
                {
					m_TabButtonList[i].normalSprite = "ui_pub_009";
					m_TabButtonList[i].hoverSprite = "ui_pub_009";
					m_TabButtonList[i].pressedSprite = "ui_pub_009";
					m_TabButtonList[i].disabledSprite = "ui_pub_009";
					m_TabButtonList[i].target.spriteName = "ui_pub_009";

					m_TabButtonList[i].GetComponentInChildren<UISprite>().depth = 59 -  i;

					if(i == 0)
					{
						m_TabButtonList[i].normalSprite = "ui_pub_007";
						m_TabButtonList[i].hoverSprite = "ui_pub_007";
						m_TabButtonList[i].pressedSprite = "ui_pub_007";
						m_TabButtonList[i].disabledSprite = "ui_pub_007";
						m_TabButtonList[i].target.spriteName = "ui_pub_007";
					}
//					UILabel[] labelArr2  = m_TabButtonList[i].gameObject.GetComponentsInChildren<UILabel>();
//					foreach(UILabel label in labelArr2)
//					{
//						if(label.name == "Label")
//						{
//							label.color = new Color(25f/255f,105f/255f,180f/255f);
//							label.effectColor = new Color(0f/255f,15f/255f,73f/255f);
//							label.effectDistance = new Vector2(3,3);
//						}
//					}

					m_TabButtonList[i].target.MakePixelPerfect();

                    m_ContentList[(int)i].SetActive(false);
                }
            }
            if (eContent == CONTENT_TYPE.CONTENT_TYPE_GAMBLE)
            {
                ShowGambleContent(GAMBLE_CONTENT.GAMBLE_CONTENT_MAIN);
                m_PartnerList.SetActive(false);
            }
            else
            {
                m_PartnerList.SetActive(true);

                //选择当前伙伴
                if (m_CurFellow != null)
                {
                    OnPartnerClick(m_CurFellow);
                }
            }
        }                
    }

    void ShowGambleContent(GAMBLE_CONTENT eContent)
    {

        if (IsAllContentValid())
        {
            if (eContent == GAMBLE_CONTENT.GAMBLE_CONTENT_EXCHAGNE)
            {
                m_GambleMain.SetActive(false);
            }
            else if (eContent == GAMBLE_CONTENT.GAMBLE_CONTENT_MAIN)
            {
                m_GambleMain.SetActive(true); 
                m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_GAMBLE].gameObject.GetComponent<PartnerFrameLogic_Gamble>().UpdateMainInfo();
            }
            else if (eContent == GAMBLE_CONTENT.GAMBLE_CONTENT_PREVIEW)
            {
                m_GambleMain.SetActive(false);
            }
		}

		if (m_NewPlayerGuide_Step == (int)GameDefine_Globe.NEWOLAYERGUIDE.PET_GAIN || m_NewPlayerGuide_Step == 7)
		{
			NewPlayerGuide(7);
		}
    }

    private CONTENT_TYPE m_Content_Type = CONTENT_TYPE.CONTENT_TYPE_INVALID;
    public PartnerFrameLogic.CONTENT_TYPE Content_Type
    {
        get { return m_Content_Type; }
        set { 
            m_Content_Type = value;
            Check_ShowContentTab();
        }
    }

    void Check_ShowContentTab()
    {
        if (m_Content_Type != CONTENT_TYPE.CONTENT_TYPE_INVALID)
        {
            ShowPartnerContent(m_Content_Type);
        }
    }
    public void ShowPartnerInfo()
    {
        ShowPartnerContent(CONTENT_TYPE.CONTENT_TYPE_INFO);
        Content_Type = CONTENT_TYPE.CONTENT_TYPE_INFO;
    }

    public void ShowRaiseStar()
    {
        ShowPartnerContent(CONTENT_TYPE.CONTENT_TYPE_STAR);
        Content_Type = CONTENT_TYPE.CONTENT_TYPE_STAR;
    }

    public void ShowZiZhiStren()
    {
        ShowPartnerContent(CONTENT_TYPE.CONTENT_TYPE_STREN);
        Content_Type = CONTENT_TYPE.CONTENT_TYPE_STREN;
    }

    public void ShowPartnerSkill()
    {
        ShowPartnerContent(CONTENT_TYPE.CONTENT_TYPE_SKILL);
        Content_Type = CONTENT_TYPE.CONTENT_TYPE_SKILL;
    }

    public void ShowPartnerGamble()
    {
        ShowPartnerContent(CONTENT_TYPE.CONTENT_TYPE_GAMBLE);
        Content_Type = CONTENT_TYPE.CONTENT_TYPE_GAMBLE;
    }

    void ShowGambleExchange()
    {
        ShowGambleContent(GAMBLE_CONTENT.GAMBLE_CONTENT_EXCHAGNE);
    }

    void PreviewGoBack()
    {
        ShowGambleContent(GAMBLE_CONTENT.GAMBLE_CONTENT_MAIN);
    }

    void ExchangeGoBack()
    {
        ShowGambleContent(GAMBLE_CONTENT.GAMBLE_CONTENT_MAIN);
    }

    public void InitPartnerList()
    {
        UIManager.LoadItem(UIInfo.PartnerFrameItem, OnLoadPartnerFrameItem);
    }

    void OnLoadPartnerFrameItem(GameObject resItem, object param)
    {
        if (null == resItem)
        {
            LogModule.ErrorLog("load partner frame error");
            return;
        }

        if (m_PartnerListGrid != null)
        {
            //先清空
            Utils.CleanGrid(m_PartnerListGrid);

            FellowContainer container = GameManager.gameManager.PlayerDataPool.FellowContainer;
            if (container == null)
            {
                return;
            }
            //遍历伙伴容器
            bool bFirst = true;
            List<Fellow> fellowList = FellowTool.FellowSort(container);
            for (int i = 0; i < fellowList.Count; ++i)
            {
                //有效槽位
                Fellow fellow = fellowList[i];
                if (fellow.IsValid())
                {
                    string objectName = "";
                    if (i < 10)
                    {
                        objectName += "0";
                    }
                    objectName += i.ToString();
                    GameObject fellowobject = Utils.BindObjToParent(resItem, m_PartnerListGrid, objectName);
                    if (fellowobject != null && fellowobject.GetComponent<PartnerFrameItemLogic>() != null)
                    {
                        fellowobject.GetComponent<PartnerFrameItemLogic>().UpdateFellowInfo(fellow);
                        if (bFirst)
                        {
                            //如果有上次选择 则显示上次选择
                            //如果没有上次选择 显示第一个
                            if (m_CurFellow != null)
                            {
                                if (m_CurFellow.Guid == fellow.Guid)
                                {
                                    fellowobject.GetComponent<PartnerFrameItemLogic>().OnFellowClick();
                                    bFirst = false;
                                }
                            }
                            else
                            {
                                fellowobject.GetComponent<PartnerFrameItemLogic>().OnFellowClick();
                                bFirst = false;
                            }
                        }
                    }

                }
            }
            m_PartnerListGrid.GetComponent<UIGrid>().repositionNow = true;
            m_PartnerListGrid.GetComponent<UITopGrid>().Recenter(true);
            //伙伴数量
            m_PartnerNumLabel.text = string.Format("{0}/{1}", container.GetFellowCount(), container.ContainerSize);
        }        

    }

    public void OnPartnerClick(Fellow fellow, bool bGamble=false)
    {
        m_CurFellow = fellow;
        switch (m_CurContent)
        {
            case CONTENT_TYPE.CONTENT_TYPE_INVALID:
                break;
            case CONTENT_TYPE.CONTENT_TYPE_INFO:
                if (m_CurFellow != null &&
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_INFO].GetComponent<PartnerFrameLogic_Info>() != null)
                {
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_INFO].GetComponent<PartnerFrameLogic_Info>().UpdateFellow_Info(m_CurFellow);
                }
                break;
            case CONTENT_TYPE.CONTENT_TYPE_STAR:
                if (m_CurFellow != null &&
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_STAR].GetComponent<PartnerFrameLogic_Star>() != null)
                {
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_STAR].GetComponent<PartnerFrameLogic_Star>().UpdateFellow_Star(m_CurFellow);
                }
                break;
            case CONTENT_TYPE.CONTENT_TYPE_STREN:
                if (m_CurFellow != null &&
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_STREN].GetComponent<PartnerFrameLogic_Stren>() != null)
                {
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_STREN].GetComponent<PartnerFrameLogic_Stren>().UpdateFellow_Stren(m_CurFellow);
                }
                break;
            case CONTENT_TYPE.CONTENT_TYPE_SKILL:
                if (m_CurFellow != null &&
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_SKILL].GetComponent<PartnerFrameLogic_Skill>() != null)
                {
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_SKILL].GetComponent<PartnerFrameLogic_Skill>().UpdateFellow_Skill(m_CurFellow);
                }
                break;
            case CONTENT_TYPE.CONTENT_TYPE_GAMBLE:
                {
                    if (bGamble)
                    {
                        ShowPartnerInfo();
                        m_CurFellow = fellow;
                        InitPartnerList();
                    }
                }
                break;
            default:
                break;
        }
    }

    public void UpdatePartnerFrame()
    {
        InitPartnerList();
        switch (m_CurContent)
        {
            case CONTENT_TYPE.CONTENT_TYPE_INFO:
                if (m_CurFellow != null &&
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_INFO].GetComponent<PartnerFrameLogic_Info>() != null)
                {
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_INFO].GetComponent<PartnerFrameLogic_Info>().UpdateFellow_Info(m_CurFellow);
                }
                break;
            case CONTENT_TYPE.CONTENT_TYPE_STAR:
                if (m_CurFellow != null &&
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_STAR].GetComponent<PartnerFrameLogic_Star>() != null)
                {
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_STAR].GetComponent<PartnerFrameLogic_Star>().UpdateFellow_Star(m_CurFellow);
                }
                break;
            case CONTENT_TYPE.CONTENT_TYPE_STREN:
                if (m_CurFellow != null &&
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_STREN].GetComponent<PartnerFrameLogic_Stren>() != null)
                {
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_STREN].GetComponent<PartnerFrameLogic_Stren>().UpdateFellow_Stren(m_CurFellow);
                }
                break;
            case CONTENT_TYPE.CONTENT_TYPE_SKILL:
                if (m_CurFellow != null &&
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_SKILL].GetComponent<PartnerFrameLogic_Skill>() != null)
                {
                    m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_SKILL].GetComponent<PartnerFrameLogic_Skill>().UpdateFellow_Skill(m_CurFellow);
                }
                break;
            case CONTENT_TYPE.CONTENT_TYPE_GAMBLE:
                break;
            default:
                break;
        }
    }

    public void UpdateGainRemain()
    {
        int mainPlayerLevel = GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr.Level;
        int remainCount = 10 - GameManager.gameManager.PlayerDataPool.FellowGainCount_Free;
        bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_FELLOWFUNCTION_OPENFLAG);
        if (remainCount > 0 && bRet && mainPlayerLevel >= GlobeVar.PARTNER_GAIN_TIPS_LEVEL)
        {
            m_PartnerGainRemain.gameObject.SetActive(true);
            m_PartnerGainRemain.text = remainCount.ToString();
        }
        else
        {
            m_PartnerGainRemain.gameObject.SetActive(false);
        }
    }

    public void UpdateSkillRemain()
    {
        int remainCount = PartnerFrameLogic_Skill.GetPartnerSkillCanActiveNum();
        bool bRet = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_FELLOWFUNCTION_OPENFLAG);
        if (remainCount > 0 && bRet)
        {
            m_PartnerSkillRemain.gameObject.SetActive(true);
            m_PartnerSkillRemain.text = remainCount.ToString();
        }
        else
        {
            m_PartnerSkillRemain.gameObject.SetActive(false);
        }
    }

    public static int GetPartnerRemainCount()
    {
        int remainCount = 0;
        int mainPlayerLevel = GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr.Level;
        if (mainPlayerLevel >= GlobeVar.PARTNER_GAIN_TIPS_LEVEL)
        {
            remainCount += 10 - GameManager.gameManager.PlayerDataPool.FellowGainCount_Free;
        }
        remainCount += PartnerFrameLogic_Skill.GetPartnerSkillCanActiveNum();
        return remainCount;
    }

	void Check_NewPlayerGuide()
	{
		if (PartnerAndMountLogic.Instance())
		{
			int nIndex = PartnerAndMountLogic.Instance().NewPlayerGuideFlag_Step;
			if (nIndex == (int)GameDefine_Globe.NEWOLAYERGUIDE.PET_GAIN || nIndex == (int)GameDefine_Globe.NEWOLAYERGUIDE.PET_FIGHT)
			{
				NewPlayerGuide(nIndex);
				PartnerAndMountLogic.Instance().NewPlayerGuideFlag_Step = -1;
			}
		}
	}
	
	public void NewPlayerGuide(int index)
	{
		if (index < 0)
		{
			return;
		}
		
		NewPlayerGuidLogic.CloseWindow();

		m_NewPlayerGuide_Step = index;

		switch (m_NewPlayerGuide_Step)
		{
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.PET_FIGHT:
			{
				m_ContentList[(int)CONTENT_TYPE.CONTENT_TYPE_INFO].GetComponent<PartnerFrameLogic_Info>().NewPlayerGuide(1);
			}
			break;
		case (int)GameDefine_Globe.NEWOLAYERGUIDE.PET_GAIN:
			if ((int)CONTENT_TYPE.CONTENT_TYPE_GAMBLE >=0
			    && (int)CONTENT_TYPE.CONTENT_TYPE_GAMBLE < m_TabButtonList.Count
			    &&m_TabButtonList[(int)CONTENT_TYPE.CONTENT_TYPE_GAMBLE])
			{
				GameObject gObj = m_TabButtonList[(int)CONTENT_TYPE.CONTENT_TYPE_GAMBLE].gameObject;
				NewPlayerGuidLogic.OpenWindow(gObj, 154, 64, "", "bottom", 2, true, true);
			}
			break;
		case 7:
			if (m_CoinGambleButton)
			{
				NewPlayerGuidLogic.OpenWindow(m_CoinGambleButton, 202, 64, "", "bottom", 2, true, true);
			}
			break;
		case 8:
			if (PartnerAndMountLogic.Instance())
			{
				PartnerAndMountLogic.Instance().NewPlayerGuide((int)GameDefine_Globe.NEWOLAYERGUIDE.UI_NOGUIDE);
			}
			m_NewPlayerGuide_Step = -1;
			break;
		default:
			break;
		}
		
	}
}
