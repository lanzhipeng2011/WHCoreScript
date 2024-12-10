using UnityEngine;
using System.Collections;
using Games.Fellow;
using GCGame.Table;
using Module.Log;
using GCGame;

public class PartnerFrameLogic_Stren : MonoBehaviour {

    public enum CONSTVALUE
    {
        MATERIAL_NUM = 10,
        PRESS_COUNT = 2,
    }
    public UISprite m_PartnerHeadIcon;
    public UISprite m_PartnerQualityFrame;
    public UISprite[] m_MaterialHeadIcon;
    public UISprite[] m_DragHeadIcon;
    public UISprite[] m_MaterialQualityFrame;
    public UILabel m_ZizhiPointCount;
    public UIImageButton m_FastChooseBtn;
    public UILabel m_PartnerName;

    public enum ZIZHI_TYPE
    {
        ZIZHI_ATTACK = 0,   //攻击资质
        ZIZHI_HIT,          //命中资质
        ZIZHI_CRITICAL,     //暴击资质
        ZIZHI_ATTACKSPEED,  //攻速资质
        ZIZHI_BLESS,     //加持资质
    }
    public UISprite[] m_ZizhiSprite;
    public UILabel[] m_ZizhiLabel;
    public UISprite[] m_ZiZhiEffect;
    public GameObject m_ZizhiInfo;
    public GameObject m_ZizhiEnchance;

    private Fellow m_fellow = null;                       //主伙伴
    public Fellow[] m_MaterialFellow = new Fellow[(int)CONSTVALUE.MATERIAL_NUM];    //材料伙伴
    private int m_CurZzPoint = 0;               //剩余资质点
    private int m_ExpectZzPoint = 0;            //预计增加资质点
    private int m_AddAttack = 0;                //分配给攻击的未确认资质点
    private int m_AddHit = 0;                   //分配给命中的未确认资质点
    private int m_AddCritical = 0;              //分配给暴击的未确认资质点
    private int m_AddAttackSpeed = 0;           //分配给攻速的未确认资质点
    private int m_AddBless = 0;                 //分配给加持的未确认资质点
    private int m_CurDragMaterialIndex = -1;    //当前拖动哪个材料伙伴
    private bool m_bPressAdd1 = false;
    private bool m_bPressMinus1 = false;
    private bool m_bPressAdd2 = false;
    private bool m_bPressMinus2 = false;
    private bool m_bPressAdd3 = false;
    private bool m_bPressMinus3 = false;
    private bool m_bPressAdd4 = false;
    private bool m_bPressMinus4 = false;
    private bool m_bPressAdd5 = false;
    private bool m_bPressMinus5 = false;
    private int m_nPressCount = 0;
    private int m_nPressChangNum = 10;  //按住时每次增加减少点数
    private static PartnerFrameLogic_Stren m_Instance = null;
    public static PartnerFrameLogic_Stren Instance()
    {
        return m_Instance;
    }

    void OnEnable()
    {
        m_Instance = this;

        InvokeRepeating("Tick_Press", 0f, 0.1f);

        if (m_fellow == null)
        {
            UpdateEmpty_Stren();
        }
        else
        {
            ClearAddZzPoint();
        }
    }

    void OnDisable()
    {
        CancelInvoke("Tick_Press");
        m_Instance = null;
    }

    public void UpdateEmpty_Stren()
    {
        m_PartnerName.text = "";
        m_PartnerHeadIcon.gameObject.SetActive(false);
        m_PartnerQualityFrame.gameObject.SetActive(false);
        m_ZizhiPointCount.text = "0";
        for (int i = 0; i < (int)CONSTVALUE.MATERIAL_NUM; i++)
        {
            m_MaterialHeadIcon[i].gameObject.SetActive(false);
            m_MaterialQualityFrame[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < (int)CONSTVALUE.MATERIAL_NUM; i++)
        {
            m_DragHeadIcon[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 5; i++)
        {
            m_ZizhiSprite[i].fillAmount = 0;
            m_ZizhiLabel[i].text = "0/0";
            m_ZiZhiEffect[i].gameObject.SetActive(false);
        }
    }

    public void UpdateFellow_Stren(Fellow fellow)
    {
        m_fellow = fellow;
		ClearMaterialFellow();          //清空材料
        UpdatePartnerChoose(fellow);    //选中伙伴信息
        UpdateZiZhiInfo(fellow);        //更新资质信息
        
    }

    void UpdatePartnerChoose(Fellow fellow)
    {
        //头像
        m_PartnerName.text = fellow.Name;
        m_PartnerHeadIcon.spriteName = fellow.GetIcon();
        m_PartnerHeadIcon.gameObject.SetActive(true);
        m_PartnerQualityFrame.spriteName = FellowTool.GetFellowSkillQualityFrame(fellow.Quality);
        m_PartnerQualityFrame.gameObject.SetActive(true);
        ClearAddZzPoint();
        UpdateZzPoint();
    }

    void UpdateZzPoint()
    {
        //资质点
        m_ZizhiPointCount.text = m_CurZzPoint.ToString();
        if (m_ExpectZzPoint > 0)
        {
            m_ZizhiPointCount.text += "[33CCFF]+" + m_ExpectZzPoint.ToString();
        }
    }

    void ClearAddZzPoint()
    {
        if (m_fellow != null)
        {
            m_CurZzPoint = m_fellow.ZzPoint;
        }
        m_AddAttack = 0;
        m_AddHit = 0;
        m_AddCritical = 0;
        m_AddAttackSpeed = 0;
        m_AddBless = 0;
    }

    void ClearMaterialFellow()
    {
        for (int i = 0; i < (int)CONSTVALUE.MATERIAL_NUM; i++)
        {
            m_MaterialFellow[i] = null;
            m_MaterialHeadIcon[i].gameObject.SetActive(false);
            m_MaterialQualityFrame[i].gameObject.SetActive(false);
            m_DragHeadIcon[i].gameObject.SetActive(false);
        }
        m_ExpectZzPoint = 0;
    }

    public void UpdateMaterialFellow()
    {
        for (int i = 0; i < (int)CONSTVALUE.MATERIAL_NUM; i++)
        {
            if (m_MaterialFellow[i] != null)
            {
                m_MaterialHeadIcon[i].spriteName = m_MaterialFellow[i].GetIcon();
                m_MaterialHeadIcon[i].gameObject.SetActive(true);
                m_DragHeadIcon[i].spriteName = m_MaterialFellow[i].GetIcon();
                m_DragHeadIcon[i].gameObject.SetActive(true);
                m_MaterialQualityFrame[i].spriteName = FellowTool.GetFellowSkillQualityFrame(m_MaterialFellow[i].Quality);
                m_MaterialQualityFrame[i].gameObject.SetActive(true);
            }
            else
            {
                m_MaterialHeadIcon[i].gameObject.SetActive(false);
                m_DragHeadIcon[i].gameObject.SetActive(false);
                m_MaterialQualityFrame[i].gameObject.SetActive(false);
            }
        }
        UpdateExpectZzPoint();
    }

    void UpdateExpectZzPoint()
    {
        m_ExpectZzPoint = 0;
        for (int i = 0; i < (int)CONSTVALUE.MATERIAL_NUM; i++)
        {
            if (m_MaterialFellow[i] != null)
            {
                //伙伴本身的潜力点
                m_ExpectZzPoint += m_MaterialFellow[i].GetFullZzPoint();
            }
        }
        UpdateZzPoint();
    }

    void UpdateZiZhiInfo(Fellow fellow)
    {
        if (m_AddAttack > 0)
        {
            m_ZizhiSprite[(int)ZIZHI_TYPE.ZIZHI_ATTACK].fillAmount = (fellow.Zizhi_Attack + m_AddAttack) / fellow.GetCurZzAttackMax();
            m_ZizhiLabel[(int)ZIZHI_TYPE.ZIZHI_ATTACK].text = string.Format("{0}+{1}/{2}", (int)fellow.Zizhi_Attack, m_AddAttack, (int)fellow.GetCurZzAttackMax());
            m_ZiZhiEffect[(int)ZIZHI_TYPE.ZIZHI_ATTACK].gameObject.SetActive(true);
			float px = 405f * ((float)(fellow.Zizhi_Attack + m_AddAttack) / (float)fellow.GetCurZzAttackMax());
            m_ZiZhiEffect[(int)ZIZHI_TYPE.ZIZHI_ATTACK].gameObject.transform.localPosition = new Vector3(px, 0, 0);
        }
        else
        {
            m_ZizhiSprite[(int)ZIZHI_TYPE.ZIZHI_ATTACK].fillAmount = fellow.Zizhi_Attack / fellow.GetCurZzAttackMax();
            m_ZizhiLabel[(int)ZIZHI_TYPE.ZIZHI_ATTACK].text = string.Format("{0}/{1}", (int)fellow.Zizhi_Attack, (int)fellow.GetCurZzAttackMax());
            m_ZiZhiEffect[(int)ZIZHI_TYPE.ZIZHI_ATTACK].gameObject.SetActive(false);
        }

        if (m_AddHit > 0)
        {
            m_ZizhiSprite[(int)ZIZHI_TYPE.ZIZHI_HIT].fillAmount = (fellow.Zizhi_Hit + m_AddHit) / fellow.GetCurZzHitMax();
            m_ZizhiLabel[(int)ZIZHI_TYPE.ZIZHI_HIT].text = string.Format("{0}+{1}/{2}", (int)fellow.Zizhi_Hit, m_AddHit, (int)fellow.GetCurZzHitMax());
            m_ZiZhiEffect[(int)ZIZHI_TYPE.ZIZHI_HIT].gameObject.SetActive(true);
			float px = 405f * ((float)(fellow.Zizhi_Hit + m_AddHit) / (float)fellow.GetCurZzHitMax());
			m_ZiZhiEffect[(int)ZIZHI_TYPE.ZIZHI_HIT].gameObject.transform.localPosition = new Vector3(px, 0, 0);
        }
        else
        {
            m_ZizhiSprite[(int)ZIZHI_TYPE.ZIZHI_HIT].fillAmount = fellow.Zizhi_Hit / fellow.GetCurZzHitMax();
            m_ZizhiLabel[(int)ZIZHI_TYPE.ZIZHI_HIT].text = string.Format("{0}/{1}", (int)fellow.Zizhi_Hit, (int)fellow.GetCurZzHitMax());
            m_ZiZhiEffect[(int)ZIZHI_TYPE.ZIZHI_HIT].gameObject.SetActive(false);
        }

        if (m_AddCritical > 0)
        {
            m_ZizhiSprite[(int)ZIZHI_TYPE.ZIZHI_CRITICAL].fillAmount = (fellow.Zizhi_Critical + m_AddCritical) / fellow.GetCurZzCriticalMax();
            m_ZizhiLabel[(int)ZIZHI_TYPE.ZIZHI_CRITICAL].text = string.Format("{0}+{1}/{2}", (int)fellow.Zizhi_Critical, m_AddCritical, (int)fellow.GetCurZzCriticalMax());
            m_ZiZhiEffect[(int)ZIZHI_TYPE.ZIZHI_CRITICAL].gameObject.SetActive(true);
			float px = 405f * ((float)(fellow.Zizhi_Critical + m_AddCritical) / (float)fellow.GetCurZzCriticalMax());
			m_ZiZhiEffect[(int)ZIZHI_TYPE.ZIZHI_CRITICAL].gameObject.transform.localPosition = new Vector3(px, 0, 0);
        }
        else
        {
            m_ZizhiSprite[(int)ZIZHI_TYPE.ZIZHI_CRITICAL].fillAmount = fellow.Zizhi_Critical / fellow.GetCurZzCriticalMax();
            m_ZizhiLabel[(int)ZIZHI_TYPE.ZIZHI_CRITICAL].text = string.Format("{0}/{1}", (int)fellow.Zizhi_Critical, (int)fellow.GetCurZzCriticalMax());
            m_ZiZhiEffect[(int)ZIZHI_TYPE.ZIZHI_CRITICAL].gameObject.SetActive(false);
        }

        if (m_AddAttackSpeed > 0)
        {
            m_ZizhiSprite[(int)ZIZHI_TYPE.ZIZHI_ATTACKSPEED].fillAmount = (fellow.Zizhi_Guard + m_AddAttackSpeed) / fellow.GetCurZzGuardMax();
            m_ZizhiLabel[(int)ZIZHI_TYPE.ZIZHI_ATTACKSPEED].text = string.Format("{0}+{1}/{2}", (int)fellow.Zizhi_Guard, m_AddAttackSpeed, (int)fellow.GetCurZzGuardMax());
            m_ZiZhiEffect[(int)ZIZHI_TYPE.ZIZHI_ATTACKSPEED].gameObject.SetActive(true);
			float px = 405f * ((float)(fellow.Zizhi_Guard + m_AddAttackSpeed) / (float)fellow.GetCurZzGuardMax());
			m_ZiZhiEffect[(int)ZIZHI_TYPE.ZIZHI_ATTACKSPEED].gameObject.transform.localPosition = new Vector3(px, 0, 0);
        }
        else
        {
            m_ZizhiSprite[(int)ZIZHI_TYPE.ZIZHI_ATTACKSPEED].fillAmount = fellow.Zizhi_Guard / fellow.GetCurZzGuardMax();
            m_ZizhiLabel[(int)ZIZHI_TYPE.ZIZHI_ATTACKSPEED].text = string.Format("{0}/{1}", (int)fellow.Zizhi_Guard, (int)fellow.GetCurZzGuardMax());
            m_ZiZhiEffect[(int)ZIZHI_TYPE.ZIZHI_ATTACKSPEED].gameObject.SetActive(false);
        }

        if (m_AddBless > 0)
        {
            m_ZizhiSprite[(int)ZIZHI_TYPE.ZIZHI_BLESS].fillAmount = (fellow.Zizhi_Bless + m_AddBless) / fellow.GetCurZzBlessMax();
            m_ZizhiLabel[(int)ZIZHI_TYPE.ZIZHI_BLESS].text = string.Format("{0}+{1}/{2}", (int)fellow.Zizhi_Bless, m_AddBless, (int)fellow.GetCurZzBlessMax());
            m_ZiZhiEffect[(int)ZIZHI_TYPE.ZIZHI_BLESS].gameObject.SetActive(true);
			float px = 405f * ((float)(fellow.Zizhi_Bless + m_AddBless) / (float)fellow.GetCurZzBlessMax());
			m_ZiZhiEffect[(int)ZIZHI_TYPE.ZIZHI_BLESS].gameObject.transform.localPosition = new Vector3(px, 0, 0);
        }
        else
        {
            m_ZizhiSprite[(int)ZIZHI_TYPE.ZIZHI_BLESS].fillAmount = fellow.Zizhi_Bless / fellow.GetCurZzBlessMax();
            m_ZizhiLabel[(int)ZIZHI_TYPE.ZIZHI_BLESS].text = string.Format("{0}/{1}", (int)fellow.Zizhi_Bless, (int)fellow.GetCurZzBlessMax());
            m_ZiZhiEffect[(int)ZIZHI_TYPE.ZIZHI_BLESS].gameObject.SetActive(false);
        }
    }
    
    public void OnClickAbsorb()
    {
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return;
        }

        int count = 0;
		int purpleNum = 0;
		Fellow topFellow = null;
        for (int i = 0; i < (int)CONSTVALUE.MATERIAL_NUM; i++)
        {
            if (m_MaterialFellow[i] != null)
            {
                count++;

				if(m_MaterialFellow[i].Quality >= (int)FELLOWQUALITY.PURPLE)
				{
					purpleNum++;
					if(topFellow == null)
						topFellow = m_MaterialFellow[i];
					if(topFellow != null && topFellow.Quality < m_MaterialFellow[i].Quality)
						topFellow = m_MaterialFellow[i];
					
				}
            }
        }
        if (count <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1838}");
            return;
        }

		//========判断紫色或以上
		if(purpleNum>0)
		{
			string str =  Utils.GetFellowNameColor(topFellow.Quality);
			str += topFellow.Name + "[-]";
			if(purpleNum==1)
			{
				MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{4658}",str), "", AbsorbOK, null);
			}else{

				MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{4659}",str,purpleNum.ToString()), "", AbsorbOK, null);
			}
			return;
		}

        if (null != GameManager.gameManager)
        {
            GameManager.gameManager.SoundManager.PlaySoundEffect(135);       //yes
        }

        CG_FELLOW_ENCHANCE fellowPacket = (CG_FELLOW_ENCHANCE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_FELLOW_ENCHANCE);
        fellowPacket.SetType(1);
        fellowPacket.SetMainguid(m_fellow.Guid);
        for (int i = 0; i < (int)CONSTVALUE.MATERIAL_NUM; i++)
        {
            if (m_MaterialFellow[i] != null)
            {
                fellowPacket.AddFellowguid(m_MaterialFellow[i].Guid);
            }
        }
        fellowPacket.SendPacket();
    }

	private void AbsorbOK()
	{
		if (null != GameManager.gameManager)
		{
			GameManager.gameManager.SoundManager.PlaySoundEffect(135);       //yes
		}
		
		CG_FELLOW_ENCHANCE fellowPacket = (CG_FELLOW_ENCHANCE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_FELLOW_ENCHANCE);
		fellowPacket.SetType(1);
		fellowPacket.SetMainguid(m_fellow.Guid);
		for (int i = 0; i < (int)CONSTVALUE.MATERIAL_NUM; i++)
		{
			if (m_MaterialFellow[i] != null)
			{
				fellowPacket.AddFellowguid(m_MaterialFellow[i].Guid);
			}
		}
		fellowPacket.SendPacket();
	}

    bool PushMaterialFellow(Fellow fellow)
    {
        //品质好于主伙伴 不填充
        if (fellow.Quality > m_fellow.Quality)
        {
            return false;
        }
        //已经锁定的伙伴 不填充
        if (fellow.Locked)
        {
            return false;
        }
        //当前出战的伙伴 不填充
        if (fellow.Called)
        {
            return false;
        }
        //优先填满空格
        for (int index = 0; index < (int)CONSTVALUE.MATERIAL_NUM; index++)
        {
            if (m_MaterialFellow[index] == null)
            {
                m_MaterialFellow[index] = fellow;
                return true;
            }
        }
        for (int index = 0; index < (int)CONSTVALUE.MATERIAL_NUM; index++)
        {
            if (m_MaterialFellow[index] != null)
            {
                //品质比现有材料更低 替换
                if (m_MaterialFellow[index].Quality > fellow.Quality)
                {
                    m_MaterialFellow[index] = fellow;
                    return true;
                }
                if (m_MaterialFellow[index].Quality == fellow.Quality)
                {
                    //星级比现在有材料更低 替换
                    if (m_MaterialFellow[index].StarLevel > fellow.StarLevel)
                    {
                        m_MaterialFellow[index] = fellow;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void FastChoose(int limitQuality)
    {
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return;
        }
        //先清空
        ClearMaterialFellow();
        FellowContainer container = GameManager.gameManager.PlayerDataPool.FellowContainer;
        bool bRet = false;
        //遍历伙伴容器
        for (int i = 0; i < container.ContainerSize; ++i)
        {
            Fellow fellow = container.GetFellowByIndex(i);
            if (fellow.IsValid())
            {
                //不是自己
                if (fellow.Guid != m_fellow.Guid && fellow.Quality <= limitQuality)
                {
                    bRet |= PushMaterialFellow(fellow);
                }
            }
        }
        //是否填充伙伴
        if (bRet == false)
        {
            //无可用伙伴填充
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1839}");
        }
        UpdateMaterialFellow();
    }

    public void OnChangeFastChoose()
    {
        if (m_FastChooseBtn != null)
        {
            UIPopupList list = m_FastChooseBtn.GetComponent<UIPopupList>();
            if (list.value == StrDictionary.GetClientDictionaryString("#{2060}"))
            {
                //绿色以下
                FastChoose((int)FELLOWQUALITY.WHITE);
            }
            else if (list.value == StrDictionary.GetClientDictionaryString("#{2061}"))
            {
                //蓝色以下
                FastChoose((int)FELLOWQUALITY.GREEN);
            }
            else if (list.value == StrDictionary.GetClientDictionaryString("#{2062}"))
            {
                //紫色以下
                FastChoose((int)FELLOWQUALITY.BLUE);
            }
            else if (list.value == StrDictionary.GetClientDictionaryString("#{2063}"))
            {
                //橙色以下
                FastChoose((int)FELLOWQUALITY.PURPLE);
            }
            else if (list.value == StrDictionary.GetClientDictionaryString("#{2064}"))
            {
                //全部伙伴
                FastChoose((int)FELLOWQUALITY.ORANGE);
            }
            else
            {
                //清空选择
                ClearMaterialFellow();
                UpdateZzPoint();
            }
        }
    }

    public void OnClickSave()
    {
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return;
        }

        //if (m_fellow.Called)
        //{
        //    //出战伙伴无法加点
        //    Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2164}");
        //    return;
        //}

        if (m_AddAttack <= 0 &&
            m_AddHit <= 0 &&
            m_AddCritical <= 0 &&
            m_AddAttackSpeed <= 0 &&
            m_AddBless <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1840}");
            return;
        }

        CG_FELLOW_APPLY_POINT fellowPacket = (CG_FELLOW_APPLY_POINT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_FELLOW_APPLY_POINT);
        fellowPacket.SetAttackadd(m_AddAttack);
		fellowPacket.SetHitadd((uint)m_AddHit);
		fellowPacket.SetCriticaladd((uint)m_AddCritical);
        fellowPacket.SetGuardadd((uint)m_AddAttackSpeed);
        fellowPacket.SetBlessadd((uint)m_AddBless);
        fellowPacket.SetFellowguid(m_fellow.Guid);
        fellowPacket.SendPacket();

        ClearAddZzPoint();
        UpdateZzPoint();
        UpdateZiZhiInfo(m_fellow);
    }

    public void OnClickReset()
    {
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return;
        }

        if (m_fellow.Called)
        {
            //出战伙伴无法重洗
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2412}");
            return;
        }

        CG_FELLOW_RESET_POINT fellowPacket = (CG_FELLOW_RESET_POINT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_FELLOW_RESET_POINT);
        fellowPacket.SetFellowguid(m_fellow.Guid);
        fellowPacket.SendPacket();
    }

    public bool OnClickAdd_Attack()
    {
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_CurZzPoint <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1841}");
            return false;
        }
        if ( (m_AddAttack + m_fellow.Zizhi_Attack) >= m_fellow.GetCurZzAttackMax())
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1842}");
            return false;
        }
        m_AddAttack++;
        m_CurZzPoint--;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnClickMinus_Attack()
    {
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_AddAttack <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1840}");
            return false;
        }
        m_AddAttack--;
        m_CurZzPoint++;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnClickAdd_Hit()
    {
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_CurZzPoint <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1841}");
            return false;
        }
        if ((m_AddHit + m_fellow.Zizhi_Hit) >= m_fellow.GetCurZzHitMax())
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1842}");
            return false;
        }
        m_AddHit++;
        m_CurZzPoint--;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnClickMinus_Hit()
    {
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_AddHit <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1840}");
            return false;
        }
        m_AddHit--;
        m_CurZzPoint++;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnClickAdd_Critical()
    {
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_CurZzPoint <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1841}");
            return false;
        }
        if ((m_AddCritical + m_fellow.Zizhi_Critical) >= m_fellow.GetCurZzCriticalMax())
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1842}");
            return false;
        }
        m_AddCritical++;
        m_CurZzPoint--;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnClickMinus_Critical()
    {
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_AddCritical <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1840}");
            return false;
        }
        m_AddCritical--;
        m_CurZzPoint++;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnClickAdd_AttackSpeed()
    {
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_CurZzPoint <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1841}");
            return false;
        }
        if ((m_AddAttackSpeed + m_fellow.Zizhi_Guard) >= m_fellow.GetCurZzGuardMax())
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1842}");
            return false;
        }
        m_AddAttackSpeed++;
        m_CurZzPoint--;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnClickMinus_AttackSpeed()
    {
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_AddAttackSpeed <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1840}");
            return false;
        }
        m_AddAttackSpeed--;
        m_CurZzPoint++;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnClickAdd_Bless()
    {
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_CurZzPoint <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1841}");
            return false;
        }
        if ((m_AddBless + m_fellow.Zizhi_Bless) >= m_fellow.GetCurZzBlessMax())
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1842}");
            return false;
        }
        m_AddBless++;
        m_CurZzPoint--;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnClickMinus_Bless()
    {
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_AddBless <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1840}");
            return false;
        }
        m_AddBless--;
        m_CurZzPoint++;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnPressAdd_Attack(int nChangeNum)
    {
        int nOpNum = nChangeNum;
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_CurZzPoint <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1841}");
            return false;
        }
        if (m_CurZzPoint < nOpNum)
        {
            nOpNum = m_CurZzPoint;
        }
        if ((m_AddAttack + m_fellow.Zizhi_Attack) >= m_fellow.GetCurZzAttackMax())
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1842}");
            return false;
        }
        int nLess = (int)m_fellow.GetCurZzAttackMax() - m_AddAttack - (int)m_fellow.Zizhi_Attack;
        if (nLess < nOpNum)
        {
            nOpNum = nLess;
        }
        m_AddAttack += nOpNum;
        m_CurZzPoint -= nOpNum;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnPressMinus_Attack(int nChangeNum)
    {
        int nOpNum = nChangeNum;
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_AddAttack <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1840}");
            return false;
        }
        if (m_AddAttack < nOpNum)
        {
            nOpNum = m_AddAttack;
        }
        m_AddAttack -= nOpNum;
        m_CurZzPoint += nOpNum;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnPressAdd_Hit(int nChangeNum)
    {
        int nOpNum = nChangeNum;
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_CurZzPoint <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1841}");
            return false;
        }
        if (m_CurZzPoint < nOpNum)
        {
            nOpNum = m_CurZzPoint;
        }
        if ((m_AddHit + m_fellow.Zizhi_Hit) >= m_fellow.GetCurZzHitMax())
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1842}");
            return false;
        }
        int nLess = (int)m_fellow.GetCurZzHitMax() - m_AddHit - (int)m_fellow.Zizhi_Hit;
        if (nLess < nOpNum)
        {
            nOpNum = nLess;
        }
        m_AddHit += nOpNum;
        m_CurZzPoint -= nOpNum;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnPressMinus_Hit(int nChangeNum)
    {
        int nOpNum = nChangeNum;
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_AddHit <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1840}");
            return false;
        }
        if (m_AddHit < nOpNum)
        {
            nOpNum = m_AddHit;
        }
        m_AddHit -= nOpNum;
        m_CurZzPoint += nOpNum;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnPressAdd_Critical(int nChangeNum)
    {
        int nOpNum = nChangeNum;
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_CurZzPoint <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1841}");
            return false;
        }
        if (m_CurZzPoint < nOpNum)
        {
            nOpNum = m_CurZzPoint;
        }
        if ((m_AddCritical + m_fellow.Zizhi_Critical) >= m_fellow.GetCurZzCriticalMax())
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1842}");
            return false;
        }
        int nLess = (int)m_fellow.GetCurZzCriticalMax() - m_AddCritical - (int)m_fellow.Zizhi_Critical;
        if (nLess < nOpNum)
        {
            nOpNum = nLess;
        }
        m_AddCritical += nOpNum;
        m_CurZzPoint -= nOpNum;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnPressMinus_Critical(int nChangeNum)
    {
        int nOpNum = nChangeNum;
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_AddCritical <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1840}");
            return false;
        }
        if (m_AddCritical < nOpNum)
        {
            nOpNum = m_AddCritical;
        }
        m_AddCritical -= nOpNum;
        m_CurZzPoint += nOpNum;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnPressAdd_AttackSpeed(int nChangeNum)
    {
        int nOpNum = nChangeNum;
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_CurZzPoint <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1841}");
            return false;
        }
        if (m_CurZzPoint < nOpNum)
        {
            nOpNum = m_CurZzPoint;
        }
        if ((m_AddAttackSpeed + m_fellow.Zizhi_Guard) >= m_fellow.GetCurZzGuardMax())
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1842}");
            return false;
        }
        int nLess = (int)m_fellow.GetCurZzGuardMax() - m_AddAttackSpeed - (int)m_fellow.Zizhi_Guard;
        if (nLess < nOpNum)
        {
            nOpNum = nLess;
        }
        m_AddAttackSpeed += nOpNum;
        m_CurZzPoint -= nOpNum;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnPressMinus_AttackSpeed(int nChangeNum)
    {
        int nOpNum = nChangeNum;
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_AddAttackSpeed <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1840}");
            return false;
        }
        if (m_AddAttackSpeed < nOpNum)
        {
            nOpNum = m_AddAttackSpeed;
        }
        m_AddAttackSpeed -= nOpNum;
        m_CurZzPoint += nOpNum;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnPressAdd_Bless(int nChangeNum)
    {
        int nOpNum = nChangeNum;
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_CurZzPoint <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1841}");
            return false;
        }
        if (m_CurZzPoint < nOpNum)
        {
            nOpNum = m_CurZzPoint;
        }
        if ((m_AddBless + m_fellow.Zizhi_Bless) >= m_fellow.GetCurZzBlessMax())
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1842}");
            return false;
        }
        int nLess = (int)m_fellow.GetCurZzBlessMax() - m_AddBless - (int)m_fellow.Zizhi_Bless;
        if (nLess < nOpNum)
        {
            nOpNum = nLess;
        }
        m_AddBless += nOpNum;
        m_CurZzPoint -= nOpNum;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public bool OnPressMinus_Bless(int nChangeNum)
    {
        int nOpNum = nChangeNum;
        if (m_fellow == null)
        {
            //请先选择伙伴
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return false;
        }
        if (m_AddBless <= 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1840}");
            return false;
        }
        if (m_AddBless < nOpNum)
        {
            nOpNum = m_AddBless;
        }
        m_AddBless -= nOpNum;
        m_CurZzPoint += nOpNum;
        UpdateZiZhiInfo(m_fellow);
        UpdateZzPoint();
        return true;
    }

    public void OnPress_Material0()
    {
        OnPress_Material(0);
    }

    public void OnPress_Material1()
    {
        OnPress_Material(1);
    }

    public void OnPress_Material2()
    {
        OnPress_Material(2);
    }

    public void OnPress_Material3()
    {
        OnPress_Material(3);
    }

    public void OnPress_Material4()
    {
        OnPress_Material(4);
    }

    public void OnPress_Material5()
    {
        OnPress_Material(5);
    }

    public void OnPress_Material6()
    {
        OnPress_Material(6);
    }

    public void OnPress_Material7()
    {
        OnPress_Material(7);
    }

    public void OnPress_Material8()
    {
        OnPress_Material(8);
    }

    public void OnPress_Material9()
    {
        OnPress_Material(9);
    }

    public void OnPress_Material(int MaterialIndex)
    {
        if (m_CurDragMaterialIndex >= 0 && m_CurDragMaterialIndex < (int)CONSTVALUE.MATERIAL_NUM)
        {
            OnRelease_Material();
        }
        m_CurDragMaterialIndex = MaterialIndex;
        if (m_CurDragMaterialIndex >= 0 && m_CurDragMaterialIndex < (int)CONSTVALUE.MATERIAL_NUM)
        {
            m_DragHeadIcon[m_CurDragMaterialIndex].depth = 1000;
        }
    }

    public void OnRelease_Material()
    {
        if (m_CurDragMaterialIndex >= 0 && m_CurDragMaterialIndex < (int)CONSTVALUE.MATERIAL_NUM)
        {
            //松开时的距离
            Vector3 ReleasePos = m_DragHeadIcon[m_CurDragMaterialIndex].transform.localPosition;
            Vector3 PressPos = m_MaterialHeadIcon[m_CurDragMaterialIndex].transform.localPosition;
            float fDis = Vector3.Distance(PressPos, ReleasePos);
            if (fDis > 50)
            {
                //拖动距离较大时 清空这个格子
                m_DragHeadIcon[m_CurDragMaterialIndex].transform.localPosition = m_MaterialHeadIcon[m_CurDragMaterialIndex].transform.localPosition;
                m_DragHeadIcon[m_CurDragMaterialIndex].depth = 54;
                m_MaterialFellow[m_CurDragMaterialIndex] = null;
                UpdateMaterialFellow();
                m_CurDragMaterialIndex = -1;
            }
            else
            {
                //拖动距离较小时 放回原来的格子
                m_DragHeadIcon[m_CurDragMaterialIndex].transform.localPosition = m_MaterialHeadIcon[m_CurDragMaterialIndex].transform.localPosition;
                m_DragHeadIcon[m_CurDragMaterialIndex].depth = 54;
                m_CurDragMaterialIndex = -1;
            }
        }
    }

    void OnClickMaterial(int index)
    {
        if (index >= 0 && index < (int)CONSTVALUE.MATERIAL_NUM)
        {
            m_MaterialFellow[index] = null;
            UpdateMaterialFellow();
        }
    }

    void OnClickMaterial1()
    {
        OnClickMaterial(0);
    }

    void OnClickMaterial2()
    {
        OnClickMaterial(1);
    }

    void OnClickMaterial3()
    {
        OnClickMaterial(2);
    }

    void OnClickMaterial4()
    {
        OnClickMaterial(3);
    }

    void OnClickMaterial5()
    {
        OnClickMaterial(4);
    }

    void OnClickMaterial6()
    {
        OnClickMaterial(5);
    }

    void OnClickMaterial7()
    {
        OnClickMaterial(6);
    }

    void OnClickMaterial8()
    {
        OnClickMaterial(7);
    }

    void OnClickMaterial9()
    {
        OnClickMaterial(8);
    }

    void OnClickMaterial10()
    {
        OnClickMaterial(9);
    }

    void OpenPartnerListDrag()
    {
        PartnerFrameItemLogic[] item = PartnerFrameLogic.Instance().m_PartnerListGrid.GetComponentsInChildren<PartnerFrameItemLogic>();
        for (int i=0; i<item.Length; ++i)
        {
            if (null != item[i])
            {
                item[i].OpenDrag();
            }
        }
    }

    void ClosePartnerListDrag()
    {
        PartnerFrameItemLogic[] item = PartnerFrameLogic.Instance().m_PartnerListGrid.GetComponentsInChildren<PartnerFrameItemLogic>();
        for (int i = 0; i < item.Length; ++i)
        {
            if (null != item[i])
            {
                item[i].CloseDrag();
            }
        }
    }

    public void OnPressAdd1()
    {
        m_bPressAdd1 = true;
    }

    public void OnReleaseAdd1()
    {
        m_bPressAdd1 = false;
        m_nPressCount = 0;
    }

    public void OnPressMinus1()
    {
        m_bPressMinus1 = true;
    }

    public void OnReleaseMinus1()
    {
        m_bPressMinus1 = false;
        m_nPressCount = 0;
    }

    public void OnPressAdd2()
    {
        m_bPressAdd2 = true;
    }

    public void OnReleaseAdd2()
    {
        m_bPressAdd2 = false;
        m_nPressCount = 0;
    }

    public void OnPressMinus2()
    {
        m_bPressMinus2 = true;
    }

    public void OnReleaseMinus2()
    {
        m_bPressMinus2 = false;
        m_nPressCount = 0;
    }

    public void OnPressAdd3()
    {
        m_bPressAdd3 = true;
    }

    public void OnReleaseAdd3()
    {
        m_bPressAdd3 = false;
        m_nPressCount = 0;
    }

    public void OnPressMinus3()
    {
        m_bPressMinus3 = true;
    }

    public void OnReleaseMinus3()
    {
        m_bPressMinus3 = false;
        m_nPressCount = 0;
    }

    public void OnPressAdd4()
    {
        m_bPressAdd4 = true;
    }

    public void OnReleaseAdd4()
    {
        m_bPressAdd4 = false;
        m_nPressCount = 0;
    }

    public void OnPressMinus4()
    {
        m_bPressMinus4 = true;
    }

    public void OnReleaseMinus4()
    {
        m_bPressMinus4 = false;
        m_nPressCount = 0;
    }

    public void OnPressAdd5()
    {
        m_bPressAdd5 = true;
    }

    public void OnReleaseAdd5()
    {
        m_bPressAdd5 = false;
        m_nPressCount = 0;
    }

    public void OnPressMinus5()
    {
        m_bPressMinus5 = true;
    }

    public void OnReleaseMinus5()
    {
        m_bPressMinus5 = false;
        m_nPressCount = 0;
    }

    void Tick_Press()
    {
        if (m_bPressAdd1)
        {
            if (m_nPressCount <= (int)CONSTVALUE.PRESS_COUNT)
            {
                m_nPressCount++;
            }
            else
            {
                if (OnPressAdd_Attack(m_nPressChangNum) == false)
                {
                    m_bPressAdd1 = false;
                }
            }
        }
        if (m_bPressAdd2)
        {
            if (m_nPressCount <= (int)CONSTVALUE.PRESS_COUNT)
            {
                m_nPressCount++;
            }
            else
            {
                if (OnPressAdd_Hit(m_nPressChangNum) == false)
                {
                    m_bPressAdd2 = false;
                }
            }
        }
        if (m_bPressAdd3)
        {
            if (m_nPressCount <= (int)CONSTVALUE.PRESS_COUNT)
            {
                m_nPressCount++;
            }
            else
            {
                if (OnPressAdd_Critical(m_nPressChangNum) == false)
                {
                    m_bPressAdd3 = false;
                }
            }
        }
        if (m_bPressAdd4)
        {
            if (m_nPressCount <= (int)CONSTVALUE.PRESS_COUNT)
            {
                m_nPressCount++;
            }
            else
            {
                if (OnPressAdd_AttackSpeed(m_nPressChangNum) == false)
                {
                    m_bPressAdd4 = false;
                }
            }
        }
        if (m_bPressAdd5)
        {
            if (m_nPressCount <= (int)CONSTVALUE.PRESS_COUNT)
            {
                m_nPressCount++;
            }
            else
            {
                if (OnPressAdd_Bless(m_nPressChangNum) == false)
                {
                    m_bPressAdd5 = false;
                }
            }
        }
        if (m_bPressMinus1)
        {
            if (m_nPressCount <= (int)CONSTVALUE.PRESS_COUNT)
            {
                m_nPressCount++;
            }
            else
            {
                if (OnPressMinus_Attack(m_nPressChangNum) == false)
                {
                    m_bPressMinus1 = false;
                }
            }
        }
        if (m_bPressMinus2)
        {
            if (m_nPressCount <= (int)CONSTVALUE.PRESS_COUNT)
            {
                m_nPressCount++;
            }
            else
            {
                if (OnPressMinus_Hit(m_nPressChangNum) == false)
                {
                    m_bPressMinus2 = false;
                }
            }
        }
        if (m_bPressMinus3)
        {
            if (m_nPressCount <= (int)CONSTVALUE.PRESS_COUNT)
            {
                m_nPressCount++;
            }
            else
            {
                if (OnPressMinus_Critical(m_nPressChangNum) == false)
                {
                    m_bPressMinus3 = false;
                }
            }
        }
        if (m_bPressMinus4)
        {
            if (m_nPressCount <= (int)CONSTVALUE.PRESS_COUNT)
            {
                m_nPressCount++;
            }
            else
            {
                if (OnPressMinus_AttackSpeed(m_nPressChangNum) == false)
                {
                    m_bPressMinus4 = false;
                }
            }
        }
        if (m_bPressMinus5)
        {
            if (m_nPressCount <= (int)CONSTVALUE.PRESS_COUNT)
            {
                m_nPressCount++;
            }
            else
            {
                if (OnPressMinus_Bless(m_nPressChangNum) == false)
                {
                    m_bPressMinus5 = false;
                }
            }
        }
    }

    void OpenZizhiInfo()
    {
        m_ZizhiInfo.gameObject.SetActive(true);
        m_ZizhiEnchance.gameObject.SetActive(false);
        ClearMaterialFellow();
        UpdateZzPoint();
    }

    void OpenZizhiEnchance()
    {
        m_ZizhiInfo.gameObject.SetActive(false);
        m_ZizhiEnchance.gameObject.SetActive(true);
    }
}
