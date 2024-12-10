/********************************************************************
	created:	2014/01/15
	created:	15:1:2014   16:00
	filename: 	BelleSmallCardItem.cscs
	author:		王迪
	
	purpose:	我的美人对面列表对应条目
*********************************************************************/
using UnityEngine;
using System.Collections;
using GCGame.Table;
using Module.Log;
using System;
public class BelleSmallCardItem : MonoBehaviour {

    public UILabel labelName;
    public UILabel labelLevel;
    public UILabel labelMatrixName;
    public UILabel labelmatrixPos;
    public GameObject objEquipSpr;
    public UITexture texCard;
    //public GameObject btnOK;
	public UISprite m_SprPower;

    private MyBelleWindow parentWindow;
    private Belle m_belleData;
    private Tab_Belle m_belleTableData = null;

    private static Color[] m_ColorLevel = new Color[]
    {
        new Color(1, 1, 1, 0.3f),
       new Color(0, 1, 0, 0.3f),
        new Color(0, 0, 0, 0.3f),
        new Color(1, 0, 1, 0.3f),
        new Color(1, 142.0f/255.0f, 0, 0.3f),
    };

    public void SetData(MyBelleWindow myBelleWindow, int nKey, Tab_Belle curBelle, int level)
    {

        UnityEngine.Object belleTextureObj = ResourceManager.LoadResource(BelleData.GetBelleBigTextureName(curBelle), typeof(Texture)) as Texture; 

        if (null != belleTextureObj)
        {
            texCard.mainTexture = belleTextureObj as Texture;
            texCard.gameObject.SetActive(true);
        }
        else
        {
            texCard.gameObject.SetActive(false);
        }
        
        parentWindow = myBelleWindow;
        gameObject.name = nKey.ToString();
        m_belleTableData = curBelle;

		Tab_BelleCamp tbC = TableManager.GetBelleCampByID (curBelle.Camp,0);
		m_SprPower.spriteName = tbC.IconName;

        objEquipSpr.SetActive(false);
        if (BelleData.OwnedBelleMap.ContainsKey(nKey) )
        {
            int curBelleMatrix = BelleData.OwnedBelleMap[nKey].matrixID;
            if (BelleData.OwnedMatrixMap.ContainsKey(curBelleMatrix))
            {
                if (BelleData.OwnedMatrixMap[curBelleMatrix].isActive && BelleData.OwnedBelleMap[nKey].matrixIndex >= 0)
                {
                    objEquipSpr.SetActive(true);
                }
            }
        }
        UpdateInfo();

    }

    public void UpdateInfo()
    {
        labelMatrixName.gameObject.SetActive(false);
        labelmatrixPos.gameObject.SetActive(false);

        labelName.text = m_belleTableData.Name;
        //btnOK.SetActive(false);

        int nID = Int32.Parse(gameObject.name);
        if (BelleData.OwnedBelleMap.ContainsKey(nID))
        {
            m_belleData = BelleData.OwnedBelleMap[nID];
            //labelLevel.text = m_belleData.subLevel.ToString() + "阶";
            labelLevel.text = StrDictionary.GetClientDictionaryString("#{2785}", m_belleData.subLevel);
            labelName.color = BelleData.GetBelleColorByColorLevel(BelleData.OwnedBelleMap[nID].colorLevel);
            if (BelleData.OwnedMatrixMap.ContainsKey(m_belleData.matrixID))
            {
                Tab_BelleMatrix curTabMatrix = TableManager.GetBelleMatrixByID(m_belleData.matrixID, 0);
                if (null != curTabMatrix)
                {
                    labelMatrixName.text = curTabMatrix.Name;
                    //labelmatrixPos.text = "位置:" + (m_belleData.matrixIndex + 1).ToString();
                    labelmatrixPos.text = StrDictionary.GetClientDictionaryString("#{2786}", m_belleData.matrixIndex + 1);
                    labelMatrixName.gameObject.SetActive(true);
                    labelmatrixPos.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            LogModule.ErrorLog("can not read cur belleData");
            m_belleData = null;
            return;
        }

        if (m_belleData.colorLevel <= m_ColorLevel.Length && m_belleData.colorLevel > 0)
        {
//            labelName.color = m_ColorLevel[m_belleData.colorLevel - 1];
        }
        else
        {
            LogModule.ErrorLog("belle color level big than define: " + m_belleData.colorLevel);
        }
    }

    public Belle GetBelleData()
    {
        return m_belleData;
    }

    public Tab_Belle GetBelleTableData()
    {
        return m_belleTableData;
    }

    public void EnableConfirm(bool bEnable)
    {
        
    }

    void OnItemClick()
    {
        if (null != parentWindow)
        {
            parentWindow.SelectBelleItem(this);
        }
    }

    void OnDetailClick()
    {
        if (null != BelleController.Instance())
        {
            BelleController.Instance().OpenBelleDetailWindow(m_belleData.id);
        }
    }
}
