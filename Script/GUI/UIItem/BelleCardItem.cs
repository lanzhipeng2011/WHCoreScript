/********************************************************************
	created:	2014/01/15
	created:	15:1:2014   15:59
	filename: 	BelleCardItem.cs
	author:		王迪
	
	purpose:	美人图鉴列表条目
*********************************************************************/
using UnityEngine;
using System.Collections;
using GCGame.Table;
using Module.Log;
using System;

public class BelleCardItem : MonoBehaviour {

    public UILabel labelName;
    public UILabel labelDetail;
    public UILabel labelWay;

    public UITexture texCard;
    public UISprite SprState;
    public UISprite SprUnactive;
    public GameObject objBelleCloseTip;
	public UISprite m_SprPower;

    private Tab_Belle m_curBelleTab;

    private int m_NewPlayerGuide_Step = -1;
    
	public void SetData(int nID, Tab_Belle curTabBelle, bool isOwned)
    {
        texCard.mainTexture = ResourceManager.LoadResource(BelleData.GetBelleBigTextureName(curTabBelle), typeof(Texture)) as Texture; 
        gameObject.name = nID.ToString();
        labelName.text = curTabBelle.Name;
        SprUnactive.gameObject.SetActive(false);
        if (BelleData.OwnedBelleMap.ContainsKey(nID))
        {
            SprUnactive.gameObject.SetActive(true);
            objBelleCloseTip.SetActive(BelleData.IsCanCloseFree());
            labelName.color = BelleData.GetBelleColorByColorLevel(BelleData.OwnedBelleMap[nID].colorLevel);

            int curMatrixID = BelleData.OwnedBelleMap[nID].matrixID;
            if (BelleData.OwnedMatrixMap.ContainsKey(curMatrixID))
            {
                if (BelleData.OwnedMatrixMap[curMatrixID].isActive)
                {
                    SprUnactive.gameObject.SetActive(false);
                }
            }
        }
        
        labelWay.text = curTabBelle.Way;
        labelDetail.text = curTabBelle.Introduction;
		Tab_BelleCamp tbC = TableManager.GetBelleCampByID (curTabBelle.Camp,0);
		m_SprPower.spriteName = tbC.IconName;

        m_curBelleTab = curTabBelle;
        SprState.gameObject.SetActive(!isOwned);

        

    }

    void OnItemClick()
    {
        if (m_NewPlayerGuide_Step == 1)
        {
            NewPlayerGuidLogic.CloseWindow();
        }

        if (null != m_curBelleTab && null != BelleController.Instance())
        {
            BelleController.Instance().OpenBelleDetailWindow(m_curBelleTab.Id);
            //BelleDetailController.OpenWindow(m_curBelleTab.Id, true);
        }

        if (m_NewPlayerGuide_Step == 1)
        {
            if (BelleController.Instance())
            {
                if (BelleController.Instance().m_BelleDetailWindow
                    && BelleController.Instance().m_BelleDetailWindow.belleTickBand)
                {
                    BelleController.Instance().m_BelleDetailWindow.belleTickBand.NewPlayerGuide(1);
                }
            }
            m_NewPlayerGuide_Step = -1;
        }
        
    }

    public void NewPlayerGuide(int nIndex)
    {
        m_NewPlayerGuide_Step = nIndex;
        switch (nIndex)
        {
            case 1:
				NewPlayerGuidLogic.OpenWindow(gameObject, 450, 700, "", "bottom", 2, true, true);
                if (BelleController.Instance())
                {
                    BelleController.Instance().NewPlayerGuide_Step = -1;
                }
                break;
        }
    }
}
