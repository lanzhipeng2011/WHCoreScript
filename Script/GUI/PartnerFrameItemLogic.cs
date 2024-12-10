using UnityEngine;
using System.Collections;
using Games.Fellow;
using GCGame.Table;
using GCGame;
using System;

public class PartnerFrameItemLogic : MonoBehaviour {

    enum CONSTVALUE
    {
        MATERIAL_NUM = 6,
    }

    public UISprite m_IconSprite;
    public UILabel m_LevelLabel;
    public UILabel m_NameLabel;
    public UISprite m_DragSprite;
    public UISprite m_ChooseSprite;
    public UISprite m_LockSprite;
    public UISprite m_QualitySprite;

    private Fellow m_fellow;
    //private bool m_bCanDrag = false;
    private UISprite[] m_DestSprite = new UISprite[(int)CONSTVALUE.MATERIAL_NUM];
	
    void OnEnable()
    {
        //m_bCanDrag = false;
        if (m_DragSprite != null)
        {
            m_DragSprite.gameObject.SetActive(false);
        }
    }

    public void UpdateFellowInfo(Fellow fellow)
    {
        SetName(fellow);
        SetLevel(fellow);
        SetIcon(fellow);
        SetLock(fellow);
        SetQuality(fellow);

        m_fellow = fellow;
    }

    private void SetName(Fellow fellow)
    {
        m_NameLabel.text = Utils.GetFellowNameColor(fellow.Quality);
        m_NameLabel.text += fellow.Name;
    }

    private void SetLevel(Fellow fellow)
    {
        m_LevelLabel.text = "等级 "+ fellow.Level.ToString();
    }

    private void SetIcon(Fellow fellow)
    {
        Tab_FellowAttr line = TableManager.GetFellowAttrByID(fellow.DataId, 0);
        if (line != null)
        {
            m_IconSprite.spriteName = line.Icon;
            m_IconSprite.MakePixelPerfect();
            //m_IconSprite.gameObject.transform.localScale = new Vector3(0.64f, 0.64f, 1);
        }
    }

    private void SetLock(Fellow fellow)
    {
        if (fellow.Locked)
        {
            m_LockSprite.gameObject.SetActive(true);
        }
        else
        {
            m_LockSprite.gameObject.SetActive(false);
        }
    }

    private void SetQuality(Fellow fellow)
    {
        if (m_QualitySprite != null)
        {
            m_QualitySprite.spriteName = FellowTool.GetFellowSkillQualityFrame(fellow.Quality);
        }
    }

    public void OnFellowClick()
    {
        if (PartnerFrameLogic.Instance())
        {
            ClearAllChooseFlag();
            SetChooseFlag(true);
            int nName = Convert.ToInt32(transform.name);
            if (nName > 900 && nName < 999)
            {
                PartnerFrameLogic.Instance().OnPartnerClick(m_fellow, true);
            }
            else
            {
                PartnerFrameLogic.Instance().OnPartnerClick(m_fellow, false);
            }
        }
    }

    public void ClearAllChooseFlag()
    {
        GameObject ParentObject = transform.parent.gameObject;
        if (ParentObject != null)
        {
            PartnerFrameItemLogic[] item = ParentObject.GetComponentsInChildren<PartnerFrameItemLogic>();
            for (int i=0; i<item.Length; ++i)
            {
                item[i].SetChooseFlag(false);
            }
        }
    }

    public void OpenDrag()
    {
        if (m_fellow != null && PartnerFrameLogic_Stren.Instance() != null)
        {
            //m_bCanDrag = true;
            m_DragSprite.spriteName = m_fellow.GetIcon();
            m_DragSprite.gameObject.SetActive(true);

            for (int i = 0; i < (int)CONSTVALUE.MATERIAL_NUM; i++)
            {
                m_DestSprite[i] = PartnerFrameLogic_Stren.Instance().m_MaterialHeadIcon[i];
            }
        }
    }

    public void CloseDrag()
    {
        //m_bCanDrag = false;
        m_DragSprite.gameObject.SetActive(false);
    }

    public void OnFellowPress()
    {
        m_DragSprite.depth = 10000;
    }

    public void OnFellowRelease()
    {
        if (PartnerFrameLogic_Stren.Instance() != null && m_fellow != null)
        {
            //判断松开位置是否符合某个目标槽位
            for (int i = 0; i < (int)CONSTVALUE.MATERIAL_NUM; i++)
            {
                if (m_DestSprite[i] != null)
                {
                    Vector3 destPos = transform.parent.InverseTransformPoint(m_DestSprite[i].transform.position);
                    Vector3 releasePos = m_DragSprite.transform.localPosition;
                    float fDis = Vector3.Distance(destPos, releasePos);
                    if (fDis < 50)
                    {
                        //看下这个fellow是否已经被放到材料里面了
                        bool bFlag = false;
                        for (int index = 0; index < (int)CONSTVALUE.MATERIAL_NUM; index++)
                        {
                            if (PartnerFrameLogic_Stren.Instance().m_MaterialFellow[index] != null &&
                                PartnerFrameLogic_Stren.Instance().m_MaterialFellow[index] == m_fellow)
                            {
                                bFlag = true;
                            }
                        }
                        //未放入 则放入
                        if (bFlag == false)
                        {
                            PartnerFrameLogic_Stren.Instance().m_MaterialFellow[i] = m_fellow;
                            PartnerFrameLogic_Stren.Instance().UpdateMaterialFellow();
                        }
                    }
                }
            }
        }
        m_DragSprite.transform.localPosition = m_IconSprite.transform.localPosition;
        m_DragSprite.depth = 55;
    }

    public void SetChooseFlag(bool bChoose)
    {
        if (bChoose)
        {
            m_ChooseSprite.gameObject.SetActive(true);
        }
        else
        {
            m_ChooseSprite.gameObject.SetActive(false);
        }
    }
}
