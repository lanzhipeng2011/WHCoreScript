using System.Runtime.Serialization.Formatters;
using Games.LogicObj;
using GCGame.Table;
using UnityEngine;
using System.Collections;
/*
       
 */
public class SkillRootBarItemLogic : MonoBehaviour
{
    public UISprite m_IconSprite;
    public GameObject m_button;
    private int m_nSkillIndex;//表示主角身上的索引
    public int SkillIndex
    {
        get { return m_nSkillIndex; }
        set { m_nSkillIndex = value; }
    }

    public void UpdateSkillBarInfo(int nSkillIndex)
    {
        m_nSkillIndex = nSkillIndex;
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            return;
        }
        if (m_nSkillIndex > 0 && m_nSkillIndex < _mainPlayer.OwnSkillInfo.Length)
        {
            int nSkillId = _mainPlayer.OwnSkillInfo[m_nSkillIndex].SkillId;
            Tab_SkillEx _skillEx = TableManager.GetSkillExByID(nSkillId, 0);
            if (_skillEx != null)
            {
                Tab_SkillBase _skillBase = TableManager.GetSkillBaseByID(_skillEx.BaseId, 0);
                if (_skillBase != null)
                {
                    m_IconSprite.spriteName = _skillBase.Icon;
                    m_IconSprite.MakePixelPerfect();
                    m_IconSprite.gameObject.SetActive(true);
                }
            }
        }
    }
    
}
    