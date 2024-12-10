/********************************************************************
	created:	2014/01/16
	created:	16:1:2014   14:09
	filename: 	BelleMatrixWindow.cs
	author:		王迪
	
	purpose:	美人阵窗口
*********************************************************************/

using UnityEngine;
using System.Collections;
using GCGame.Table;
using GCGame;
using System.Collections.Generic;
using Games.LogicObj;
using Module.Log;
using System;
public class BelleMatrixWindow : MonoBehaviour {

  
    //public GameObject[] MatrixButtons;
    public GameObject MatrixListGrid;
    public GameObject MatrixBand6;
    public GameObject MatrixBand5;
    public GameObject MatrixBand3;
    public GameObject MatrixInfo;
    public UIImageButton btnActive;
    public BelleMatrixDetailBand matrixDetailBand;
    public Vector2 matrixStartPos;
    public float matrixXDiff;
    public float matrixYDiff;
    public MyBelleWindow m_MyBelleWindow;
    public GameObject m_ObjBelleMatrixItem;

    private BelleMatrixItem m_curSelectMatrix;
    private string m_curBelleID;
    private bool m_bEnableSelectMode = false;        // 是否增在等待选择确定模式
    private int m_curShowMatrixID;
    private SelectFromType m_curSelectFromType;
    private GameObject m_curMatrixBand;
    private int m_curSelectMatrixIndex = 0;
    public enum SelectFromType
    {
        TYPE_MYBELLE,
        TYPE_BELLEDETAIL,
    }

    // 新手指引
    private int m_NewPlayerGuide_Step = -1;
    public int NewPlayerGuide_Step
    {
        get { return m_NewPlayerGuide_Step; }
        set { m_NewPlayerGuide_Step = value; }
    }

    void OnEnable()
    {
        BelleData.delRest += Ret_BelleRest;
        BelleData.delActiveMatrix += Ret_ActiveMatrix;
		BelleData.delUnActiveMatrix += Ret_ActiveMatrix;
        BelleData.delBattle += Ret_Battle;
        m_bEnableSelectMode = false;
        ShowMatrixByItem();
        UpdateMatrixItems();

        if (m_NewPlayerGuide_Step == 0)
        {
            NewPlayerGuidLogic.CloseWindow();
            if (BelleController.Instance())
            {
                BelleController.Instance().NewPlayerGuide_Step = -1;
            }

            NewPlayerGuide(0);
        }
    }

    void OnDisable()
    {
        BelleData.delRest -= Ret_BelleRest;
        BelleData.delActiveMatrix -= Ret_ActiveMatrix;
		BelleData.delUnActiveMatrix -= Ret_ActiveMatrix;
        BelleData.delBattle -= Ret_Battle;
    }
    void Start()
    {
        UpdateBelleMatrixItems();
    }

    void UpdateBelleMatrixItems()
    {
        // 加载阵型Item
        if (null == m_ObjBelleMatrixItem)
        {
            LogModule.ErrorLog("can not load belle matrix item prefab in m_ObjBelleMatrixItem:");
            return;
        }
        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
        if (null == mainPlayer)
        {
            return;
        }
        for (int i = 1; i <= TableManager.GetBelleMatrix().Count; i++)
        {
            Tab_BelleMatrix curBelleMatrix = TableManager.GetBelleMatrixByID(i, 0);
            if (null == curBelleMatrix)
            {
                continue;
            }

            GameObject curItem = Utils.BindObjToParent(m_ObjBelleMatrixItem, MatrixListGrid);
            if (null != curItem && null != curItem.GetComponent<BelleMatrixItem>())
            {
                curItem.GetComponent<BelleMatrixItem>().SetData(this, i, curBelleMatrix);
            }
        }
        MatrixListGrid.GetComponent<UIGrid>().repositionNow = true;

        // 显示第一个阵型信息
        Transform curMatrixItem = MatrixListGrid.transform.FindChild("1");
        if (null != curMatrixItem)
        {
            m_curSelectMatrix = curMatrixItem.GetComponent<BelleMatrixItem>();
            if (null != m_curSelectMatrix)
            {
                m_curSelectMatrix.EnableHightLight(true);
            }

            MatrixInfo.SetActive(true);
            ShowMatrixByItem();
        }
        else
        {
            MatrixInfo.SetActive(false);
        }
    }
   

    public void EnableSelectMode(bool bEnable, string belleID, SelectFromType type)
    {
        m_bEnableSelectMode = bEnable;
        m_curBelleID = belleID;
        m_curSelectFromType = type;
    }

    public void SelectRoleToMatrix(string strBelleID)
    {
        m_MyBelleWindow.Hide();
        //BelleData.delBattle = Ret_MatrixSelectBelle;
        int belleID;
        if (!int.TryParse(strBelleID, out belleID))
        {
            LogModule.ErrorLog("id is invalid " + strBelleID);
            return;
        }

        CG_BELLE_BATTLE battleRequest = (CG_BELLE_BATTLE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_BELLE_BATTLE);
        battleRequest.SetBelleID(belleID);
        battleRequest.SetMatrixID(m_curShowMatrixID);
        battleRequest.SetMatrixIndex(m_curSelectMatrixIndex);
        battleRequest.SendPacket();
    }
    // 根据ID显示当前矩阵
    void ShowMatrixByItem()
    {
        if (null == m_curSelectMatrix)
        {
            return;
        }
        int matrixID = 1;
        if (!int.TryParse(m_curSelectMatrix.gameObject.name, out matrixID))
        {
            return;
        }

        Tab_BelleMatrix curTabMatrix = TableManager.GetBelleMatrixByID(matrixID, 0);
        if (null == curTabMatrix)
        {
            LogModule.ErrorLog("can not find cur matrix id :" + matrixID.ToString());
            return;
        }

        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;

		int totalOpenMatrixMapNumber = 0;
		for(int i=1; i <= BelleData.OwnedMatrixMap.Count; i++)
		{
			if(BelleData.OwnedMatrixMap[i].isActive)
			{
				totalOpenMatrixMapNumber++;
			}
		}


        if (!BelleData.OwnedMatrixMap.ContainsKey(matrixID) || null == mainPlayer || mainPlayer.BaseAttr.Level < curTabMatrix.OpenLevel)
        {
            btnActive.isEnabled = false;
        }
        else
        {
//            btnActive.isEnabled = !BelleData.OwnedMatrixMap[matrixID].isActive;
			btnActive.isEnabled = true;
			if(BelleData.OwnedMatrixMap[matrixID].isActive)
			{
				btnActive.GetComponentInChildren<UILabel>().text = "[fafafa]取消激活";
			}else{
				if(totalOpenMatrixMapNumber >= 2)
				{
					btnActive.isEnabled = false;
				}
				btnActive.GetComponentInChildren<UILabel>().text = "激活";
			}

        }

        
        matrixDetailBand.SetMatrix(matrixID);

        if (null != m_curMatrixBand)
        {
            m_curMatrixBand.SetActive(false);
        }

        if (curTabMatrix.Layout == 1)
        {
            m_curMatrixBand = MatrixBand5;
        }
        else if(curTabMatrix.Layout == 2)
        {
            m_curMatrixBand = MatrixBand6;
        }
        else if (curTabMatrix.Layout == 3)
        {
            m_curMatrixBand = MatrixBand3;
        }
        m_curShowMatrixID = matrixID;
        m_curMatrixBand.SetActive(true);
        m_curMatrixBand.GetComponent<BelleMatrixBand>().SetMatrixInfo(matrixID);
    }

    void UpdateMatrixItems()
    {
        BelleMatrixItem[] curItem = MatrixListGrid.GetComponentsInChildren<BelleMatrixItem>();
        if (null == curItem)
            return;

        for (int i = 0; i < curItem.Length; ++i)
        {
            if (null != curItem[i])
            {
                curItem[i].UpdateData();
            }
        }
    }

    void OnCloseBelleListPopWindowClick()
    {
        m_MyBelleWindow.Hide();
    }

    // 选择一个阵型
    public void OnMatrixListItemClick(GameObject objItem)
    {

        if (null != m_curSelectMatrix)
        {
            m_curSelectMatrix.EnableHightLight(false);
        }

        m_curSelectMatrix = objItem.GetComponent<BelleMatrixItem>();
        if (null != m_curSelectMatrix)
            m_curSelectMatrix.EnableHightLight(true);

        ShowMatrixByItem();
    }

    // 阵型按钮点击
    void OnMatrixButtonClick(GameObject btn)
    {
        if (BelleController.Instance() == null)
        {
            LogModule.ErrorLog("what happened?");
            return;
        }

        if (null == m_curSelectMatrix)
        {
            return;
        }

       
        if (!m_bEnableSelectMode)
        {
            int curMatrixID = Int32.Parse(m_curSelectMatrix.gameObject.name);
            string strCurMatrixIndex = btn.name;
            int curMatrixIndex = 1;
            if (int.TryParse(strCurMatrixIndex, out curMatrixIndex))
            {
                if (!BelleData.OwnedMatrixMap.ContainsKey(curMatrixID))
                {
                    LogModule.ErrorLog("can not find matrixid in data" + curMatrixID);
                    return;
                }

                Tab_BelleMatrix curTabMatrix = TableManager.GetBelleMatrixByID(curMatrixID, 0);
                if (null == curTabMatrix)
                {
                    LogModule.ErrorLog("can not find cur matrix id :" + curMatrixID.ToString());
                    return;
                }

                Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;

                if (!BelleData.OwnedMatrixMap.ContainsKey(curMatrixID) || null == mainPlayer || mainPlayer.BaseAttr.Level < curTabMatrix.OpenLevel)
                {
                    return;
                }


                m_curSelectMatrixIndex = curMatrixIndex;
                m_MyBelleWindow.Show();
            }
            else
            {
                LogModule.ErrorLog("can not parse matrixindex" + curMatrixID);
            }
        }
        else
        {
            BelleController.Instance().FinishSelectMatrix(m_curBelleID, m_curSelectMatrix.gameObject.name, btn.name, m_curSelectFromType);
        }

    }

    // 阵型激活信息
    void OnActiveMatrixClick()
    {
        // 新手指引直接去掉
        if (m_NewPlayerGuide_Step == 0)
        {
            NewPlayerGuidLogic.CloseWindow();
            m_NewPlayerGuide_Step = -1;
        }

        if (null == m_curSelectMatrix)
        {
            return;
        }

        int matrixID;
        if (!int.TryParse(m_curSelectMatrix.gameObject.name, out matrixID))
        {
            LogModule.ErrorLog("can not parse cur matrixID:" + matrixID);
            return;
        }

		if(BelleData.OwnedMatrixMap[matrixID].isActive)
		{
			CG_BELLE_CANCELMATRIX activeMatrixRequest = (CG_BELLE_CANCELMATRIX)PacketDistributed.CreatePacket(MessageID.PACKET_CG_BELLE_CANCELMATRIX);
			activeMatrixRequest.SetMatrixID((uint)matrixID);
			activeMatrixRequest.SendPacket();
		}else{
			CG_BELLE_ACTIVEMATRIX activeMatrixRequest = (CG_BELLE_ACTIVEMATRIX)PacketDistributed.CreatePacket(MessageID.PACKET_CG_BELLE_ACTIVEMATRIX);
			activeMatrixRequest.SetMatrixID(matrixID);
			activeMatrixRequest.SendPacket();
		}
       
    }



    void Ret_ActiveMatrix()
    {
        ShowMatrixByItem();
        UpdateMatrixItems();
    }

    void Ret_BelleRest()
    {
        ShowMatrixByItem();
        UpdateMatrixItems();
    }

    void Ret_Battle()
    {
        ShowMatrixByItem();
        UpdateMatrixItems();
    }

    public void NewPlayerGuide(int nIndex)
    {
        switch (nIndex)
        {
            case 0:
                if (btnActive)
                {
                    NewPlayerGuidLogic.OpenWindow(btnActive.gameObject, 190, 80, "", "top", 2, true, true);
                }
                break;
        }
        m_NewPlayerGuide_Step = nIndex;
    }
}
