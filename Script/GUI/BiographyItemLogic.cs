/********************************************************************
	created:	2014/09/12
	created:	21:1:2014   16:55
	filename: 	BiographyItemLogic.cs
	author:		于凤磊
	
	purpose:	秘籍界面条目
*********************************************************************/
using UnityEngine;
using System.Collections;
using Games.LogicObj;
using Module.Log;
using GCGame.Table;

public class BiographyItemLogic : MonoBehaviour {

    public UILabel m_LevelRangeName = null;
    public UISprite m_isAlreadyReach = null;
    //public UISprite m_UnrechMask = null;
    public UILabel m_LevelRangeContent = null;
    public UISprite m_ContentBackSp = null;
    public UISprite m_ContentIconSp = null;
	// Use this for initialization

    private int rangeStart = 0;
    private int rangeEnd = 0;
    public void SetData(int index)
    {
        gameObject.name = index.ToString();

        rangeStart = 10*index + 1;
        rangeEnd = 10*index + 10;

        Tab_BiographyItem itemTab = TableManager.GetBiographyItemByID(index,0);

        int contentLbHeight = 0;

        if (itemTab != null)
        {
            m_LevelRangeName.text = StrDictionary.GetClientString_WithNameSex(itemTab.Name);
            m_LevelRangeContent.text = StrDictionary.GetClientString_WithNameSex(itemTab.Content);

            // contentBkSp

            m_LevelRangeContent.MakePixelPerfect();

            contentLbHeight = m_ContentBackSp.height + itemTab.AddBackHeigh;

            m_ContentBackSp.height = contentLbHeight;

            // icon position

            m_ContentIconSp.spriteName = itemTab.SpriteIconName;

            m_ContentIconSp.MakePixelPerfect();

            float IconPosition_y = m_ContentBackSp.gameObject.transform.localPosition.y + itemTab.AddBackHeigh + 100;

            m_ContentIconSp.gameObject.transform.localPosition = new Vector3(m_ContentIconSp.gameObject.transform.localPosition.x, IconPosition_y, 0f);


            SetAlreadyReachSp(itemTab.BeginLevel,itemTab.EndLevel);
        }

        //if (m_LevelRangeName != null)
            //m_LevelRangeName.text = rangeStart.ToString() + " 级到 " +rangeEnd.ToString() + " 级";
    }

    void SetAlreadyReachSp(int beginLevel,int endLevel)
    {
        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (null == mainPlayer)
        {
            LogModule.ErrorLog("BiographyController SetAlreadyReachSp mainPlayer empty");
            return;
        }

        if (/*(mainPlayer.BaseAttr.Level >= endLevel) ||*/ (mainPlayer.BaseAttr.Level >= beginLevel && mainPlayer.BaseAttr.Level <= endLevel))
        {
            if (m_isAlreadyReach != null)
                NGUITools.SetActive(m_isAlreadyReach.gameObject,true);
        }
        else 
        {
            if (m_isAlreadyReach != null)
                NGUITools.SetActive(m_isAlreadyReach.gameObject, false);
        }
    }
}
