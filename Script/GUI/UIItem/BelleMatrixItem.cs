/********************************************************************
	created:	2014/01/16
	created:	16:1:2014   15:40
	filename: 	BelleMatrixItem.cs
	author:		王迪
	
	purpose:	美人阵列表
*********************************************************************/
using UnityEngine;
using System.Collections;
using GCGame.Table;
using System;
using Games.LogicObj;

public class BelleMatrixItem : MonoBehaviour {

    public UISprite sprBack;
    public GameObject activeFlag;
    public GameObject sprHighlight;
    public UILabel labelName;
    

    private BelleMatrixWindow parentWindow;

    public void SetData(BelleMatrixWindow belleMatrixWindow, int id, Tab_BelleMatrix tabMatrix)
    {
        if (null == tabMatrix)
            return;
        /*
        if (BelleData.OwnedMatrixMap.ContainsKey(id))
        {
            sprBack.spriteName = "BelleMatrixBtnNormal";
        }
        else
        {
            sprBack.spriteName = "BelleMatrixBtnEmpty";
        }
        */
        parentWindow = belleMatrixWindow;
        gameObject.name = id.ToString();

        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
        if (null != mainPlayer && tabMatrix.OpenLevel <= mainPlayer.BaseAttr.Level)
        {
            labelName.text = tabMatrix.Name;
        }
        else
        {
            labelName.text = tabMatrix.OpenLevel + "级开放";
        }
        
        //ownedFlag.SetActive(BelleData.OwnedMatrixMap.ContainsKey(id));
        EnableHightLight(false);
        UpdateData();
    }

    public void UpdateData()
    {
        int nID = Int32.Parse(gameObject.name);
        activeFlag.SetActive(BelleData.OwnedMatrixMap.ContainsKey(nID) && BelleData.OwnedMatrixMap[nID].isActive);
    }

    void OnItemClick()
    {
        if (null != parentWindow)
        {
            parentWindow.OnMatrixListItemClick(gameObject);
        }
    }

    public void EnableHightLight(bool bEnable)
    {
        if (null != sprHighlight)
        {
            sprHighlight.SetActive(bEnable);
            sprBack.gameObject.SetActive(!bEnable);
        }
    }
}
