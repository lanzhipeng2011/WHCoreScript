//********************************************************************
// 文件名: MountAndFellowLogic.cs
// 全路径：	\Script\GUI\MountAndFellowLogic.cs
// 描述: 坐骑界面
// 作者: hewenpeng
// 创建时间: 2013-01-06
//********************************************************************
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.Animation_Modle;
using GCGame.Table;
using Games.MountModule;
using Games.FakeObject;
using GCGame;

using Module.Log;
public class MountAndFellowLogic : MonoBehaviour
{
    private static MountAndFellowLogic m_Instance = null;
    public static MountAndFellowLogic Instance()
    {
        return m_Instance;
    }
    public MountItem m_MountItemObj;

    public GameObject m_MountRoot;
    public GameObject m_MountRoot_UIGrid;
    public UILabel m_MountDec;   // 文字描述
    public UILabel m_SpeedDec;   // 文字描述
    public UILabel m_RidingButtonDec;   // 坐骑按钮描述
    public UIImageButton m_RideButton;   // 骑乘按钮
    public UILabel m_NoMountDec;   // 文字描述
    public UILabel m_MountLeftTime;   // 文字描述
	public UILabel m_AccessWayLabel;

    private int m_CurMountID = -1;
    private int m_nUpdateLeftTimeInterval;
    //private int m_LastAutoMountID = -1;

    private FakeObject m_MountFakeObj;
    public FakeObject MountFakeObj
    {
        get { return m_MountFakeObj; }
        set { m_MountFakeObj = value; }
    }

    // 保存MountID
    private List<int> m_MountItem = new List<int>();
	//===保存所有坐骑Obj
	private List<GameObject> MountObjList = new List<GameObject> ();

    public GameObject m_FakeObjTopLeft;
    public GameObject m_FakeObjBottomRight;
    public ModelDragLogic m_ModelDrag;

     void Start () {
         m_nUpdateLeftTimeInterval = 60;
         InvokeRepeating("RefreshMountLeftTime", 0.5f, 1.0f);
	}   

    void Awake()
    {
        m_Instance = this;
    }
    
    void OnDestroy()
    {
        CleanUp();
        m_Instance = null;
        CancelInvoke("RefreshMountLeftTime");
        GameManager.gameManager.ActiveScene.HideFakeObj();
    }

    void OnEnable()
    {
        m_Instance = this;
        GameManager.gameManager.ActiveScene.InitFakeObjRoot(m_FakeObjTopLeft, m_FakeObjBottomRight);
        GameManager.gameManager.ActiveScene.ShowFakeObj();
    }

    void OnDisable()
    {
        CleanUp();
        GameManager.gameManager.ActiveScene.HideFakeObj();
        m_Instance = null;
    }

    void RefreshMountLeftTime()
    {
        if (m_nUpdateLeftTimeInterval > 0)
        {
            m_nUpdateLeftTimeInterval--;
        }
        else
        {
            UpdateMountLeftTime();
            m_nUpdateLeftTimeInterval = 60;
        }
    }

    void UpdateMountLeftTime()
    {
        if (m_MountLeftTime == null)
        {
            LogModule.ErrorLog("UpdateMountLeftTime:m_MountLeftTime is null");
            return;
        }
        if (m_CurMountID < 0 || m_CurMountID >= GameManager.gameManager.PlayerDataPool.m_objMountParam.MountCollect.Length)
        {
            m_MountLeftTime.text = "";
            return;
        }
        if (0 == GameManager.gameManager.PlayerDataPool.m_objMountParam.MountCollect[m_CurMountID])
        {
            m_MountLeftTime.text = "";
            return;
        }
        int nLeftTime = GameManager.gameManager.PlayerDataPool.m_objMountParam.GetMoountLeftTime(m_CurMountID);
        if (nLeftTime < 0)
        {
            m_MountLeftTime.text = StrDictionary.GetClientDictionaryString("#{2779}");
            return;
        }
        int nDurationSeconds = nLeftTime;
        int nDurationTotalMinutes = nDurationSeconds / 60;
        int nDurationMinutes = nDurationTotalMinutes % 60;
        int nDurationHours = (nDurationTotalMinutes / 60) % 24;
        int nDurationDays = nDurationTotalMinutes / 1440;
        string strLeftTime = StrDictionary.GetClientDictionaryString("#{2761}", nDurationDays,nDurationHours,nDurationMinutes);
        m_MountLeftTime.text = strLeftTime;
    }

    public void CleanUp()
    {
        // 清理
        m_CurMountID = -1;
        //m_LastAutoMountID = -1;
        m_MountItem.Clear();
		MountObjList.Clear ();

        MountItem[] mountItem = m_MountRoot_UIGrid.GetComponentsInChildren<MountItem>();
        for (int i = 0; i < mountItem.Length; ++i)
        {
            Destroy(mountItem[i].gameObject);
        }

        DestroyMountFakeObj();

        m_MountDec.text = "";
        m_SpeedDec.text = "";
		m_AccessWayLabel.text = "";
        m_RidingButtonDec.text = "";
        if (m_MountLeftTime != null)
        {
            m_MountLeftTime.text = "";
        }
    }

    //public void CloseWindow()
    //{
    //    UIManager.CloseUI(UIInfo.MountAndFellowRoot);
    //}

    void InitMountList()
    {
        //UIManager.LoadItem(UIInfo.MountItem, OnLoadMountItem);
        OnLoadMountItem(m_MountItemObj.gameObject, null);
    }

    void OnLoadMountItem(GameObject resItem, object param)
    {
        if (null == resItem)
            return;


		//===============
		List<int> sortMountListA = new List<int> ();
		List<int> sortMountListB = new List<int> ();

		List<int> sortMountListC = new List<int> ();
		for (int i = 0; i < MountParam.Max_MountCollect_Count; i++)
		{

			Tab_MountBase MountBase = TableManager.GetMountBaseByID(i, 0);
			if (MountBase == null)
			{
				continue;
			}
			bool bCollectFlag1 = GameManager.gameManager.PlayerDataPool.m_objMountParam.GetMountCollectFlag(i);
			if(bCollectFlag1)
			{
				sortMountListA.Add(i);
			}else{
				sortMountListB.Add(i);
			}
		}
		for (int i = 0; i < sortMountListA.Count; i++)
		{
			sortMountListC.Add(sortMountListA[i]);
		}
		for (int i = 0; i < sortMountListB.Count; i++)
		{
			sortMountListC.Add(sortMountListB[i]);
		}

		//=============

//        for (int i = 0; i < MountParam.Max_MountCollect_Count; i++)
//        {
//            Tab_MountBase MountBase = TableManager.GetMountBaseByID(i, 0);
		for (int i = 0; i < sortMountListC.Count; i++)
		{
			Tab_MountBase MountBase = TableManager.GetMountBaseByID(sortMountListC[i], 0);
            if (MountBase == null)
            {
                continue;
            }

//            bool bCollectFlag = GameManager.gameManager.PlayerDataPool.m_objMountParam.GetMountCollectFlag(i);
			bool bCollectFlag = GameManager.gameManager.PlayerDataPool.m_objMountParam.GetMountCollectFlag(sortMountListC[i]);
            // 未获得并且不显示
            if (MountBase.IsShowInUI == 0 && bCollectFlag == false)
            {
                continue;
            }

            GameObject gParentObj = m_MountRoot_UIGrid;
            if (null == gParentObj)
                return;

			GameObject gMountItem  = Utils.BindObjToParent(resItem, gParentObj, "MountItem" + ((i+1)/10) + ((i+1)%10));
            
            if (null != gMountItem)
            {
                MountItem mountItemScript = gMountItem.GetComponent<MountItem>();
                if (null != mountItemScript)
                {
//                    mountItemScript.MountID = i;
					mountItemScript.MountID = sortMountListC[i];
                    mountItemScript.SetName(MountBase.Name);

                    // 是否有该坐骑
                    if (bCollectFlag)
                    {
                        mountItemScript.SetCollectFlag(true);
                    }

//                    m_MountItem.Add(i);
					m_MountItem.Add(sortMountListC[i]);
					MountObjList.Add(gMountItem);
                }
            }            
        }

        //AutoFlagClick(GameManager.gameManager.PlayerDataPool.m_objMountParam.AutoFlagMountID);
		StartCoroutine (WaitSeconds (0.001f));

        if (m_MountItem.Count <=0)
        {
            m_RideButton.gameObject.SetActive(false);
            m_NoMountDec.text = Utils.GetDicByID(2741);
            m_NoMountDec.gameObject.SetActive(true);
        }
        else
        {
            m_NoMountDec.gameObject.SetActive(false);
            m_RideButton.gameObject.SetActive(true);
        }
        initMountFakeObj();
    }


	private IEnumerator WaitSeconds(float times)
	{
		yield return new WaitForSeconds (times);

		m_MountRoot_UIGrid.GetComponent<UIGrid>().sorted = true;
		m_MountRoot_UIGrid.GetComponent<UIGrid>().repositionNow = true;
	}

    // 坐骑界面按钮
    public void MountButton()
    {
        m_MountRoot.SetActive(true);
        CleanUp();

        InitMountList();
    }

    void initMountFakeObj()
    {
        if (m_MountItem.Count > 0)
        {
            int nMountID = 0;
            if (Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                nMountID = Singleton<ObjManager>.GetInstance().MainPlayer.GetEquipMountID();
            }
            if (nMountID <= 0)
            {
                nMountID = m_MountItem[0];
            }
           // string strName = "MountItem" + nMountID/10+nMountID%10;
            // Transform gItemObjTransform = m_MountRoot_UIGrid.transform.FindChild(strName);
            // if (gItemObjTransform != null && gItemObjTransform.gameObject)
            //{
             //   MountItem MountItemObj = gItemObjTransform.gameObject.GetComponent<MountItem>();
			//	if (MountItemObj && MountItemObj.MountID == nMountID)
            //    {
            //        MountItemObj.MountItemClick();
                    //MountItemClick(MountItemObj.MountID);
              //  }

           // }

			for (int i = 0; i < MountObjList.Count ; i++)
			{
				MountItem MountItemObj = MountObjList[i].gameObject.GetComponent<MountItem>();
				if (MountItemObj && MountItemObj.MountID == nMountID)
				{
					MountItemObj.MountItemClick();
				}
			}


        }
    }

    public void AutoFlagClick(int nMountID)
    {   
        // 去除上次点击效果
        //string strName = "MountItem" + m_LastAutoMountID;
        //Transform gLastItemTrans = m_MountRoot_UIGrid.transform.FindChild(strName);
        //if (gLastItemTrans != null)
        //{
        //    gLastItemTrans.gameObject.GetComponent<MountItem>().DisableAutoFlagUI();
        //}

        //if (m_LastAutoMountID != nMountID)
        //{
        //    m_LastAutoMountID = nMountID;

        //    string strCurName = "MountItem" + nMountID;
        //    Transform gCurItemTrans = m_MountRoot_UIGrid.transform.FindChild(strCurName);
        //    if (gCurItemTrans != null)
        //    {
        //        gCurItemTrans.gameObject.GetComponent<MountItem>().ActiveAutoFlagUI();
        //    }
        //}
    }

    public void MountItemClick(int nMountID)
    {
        
        if (m_CurMountID == nMountID)
        {
            UpdateMountLeftTime();
            return;
        }

        if (m_CurMountID > 0)
        {
            if (m_MountRoot_UIGrid != null)
            {
                // 去除上次点击效果
                string strName = "MountItem" + m_CurMountID/10 + m_CurMountID%10;
                Transform gLastItemTrans = m_MountRoot_UIGrid.transform.FindChild(strName);
                if (gLastItemTrans != null)
                {
                    gLastItemTrans.gameObject.GetComponent<MountItem>().DisableItemUI();
                }
            }
        }

        m_CurMountID = nMountID;
        UpdateMountLeftTime();

        if (m_CurMountID <= 0)
        {
            return;
        }

        // 销毁模型FakeObj
        if (m_MountFakeObj != null)
        {
            DestroyMountFakeObj();
        }

        if (false == CreateMountFakeObj(nMountID))
        {
            LogModule.DebugLog("[MountAndFellowLogic:CreateMountFakeObj()] return false!");
            return;
        }

        // 修改 描述性文字
        Tab_MountBase MountBase = TableManager.GetMountBaseByID(nMountID, 0);
        if (MountBase != null)
        {
            m_MountDec.text = MountBase.MountDesc;
            m_SpeedDec.text = StrDictionary.GetClientDictionaryString("#{1176}", MountBase.Speed);
			m_AccessWayLabel.text = MountBase.AccessWay;
        }
        else
        {
            m_MountDec.text = "";
            m_SpeedDec.text = "";
			m_AccessWayLabel.text = "";
        }

        if (m_CurMountID == GameManager.gameManager.PlayerDataPool.m_objMountParam.MountID)
        {
            m_RidingButtonDec.text = Utils.GetDicByID(1408);
        }
        else
        {
            m_RidingButtonDec.text = Utils.GetDicByID(1407); ;
        }
        
        m_RideButton.isEnabled = GameManager.gameManager.PlayerDataPool.m_objMountParam.GetMountCollectFlag(m_CurMountID);
    }

    // 骑乘button
    void RidingButton(GameObject value)
    {
        // 给服务器发消息 骑乘
        // 客户端模拟
        int nMountID = GameManager.gameManager.PlayerDataPool.m_objMountParam.MountID;
        if (nMountID != m_CurMountID)
        {
            // 没有的坐骑 弹提示

            if (false == GameManager.gameManager.PlayerDataPool.m_objMountParam.GetMountCollectFlag(m_CurMountID))
            {
                Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false,"#{3051}");
                return;
            }
            else
            {
                GameManager.gameManager.SoundManager.PlaySoundEffect(30);
                //Singleton<ObjManager>.GetInstance().MainPlayer.RideMount(m_CurMountID);
                CG_MOUNT_MOUNT packet = (CG_MOUNT_MOUNT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MOUNT_MOUNT);
                packet.SetMountID(m_CurMountID);
                packet.SendPacket();
            }
        }
        else // 下坐骑
        {
            //Singleton<ObjManager>.GetInstance().MainPlayer.UnMount();
            CG_MOUNT_UNMOUNT packet = (CG_MOUNT_UNMOUNT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MOUNT_UNMOUNT);
            packet.SetMountID(m_CurMountID);
            packet.SendPacket();
        }

        if (PartnerAndMountLogic.Instance())
        {
            PartnerAndMountLogic.Instance().CloseWindow();
        }
    }

    // 创建坐骑 fakeobj
    private bool CreateMountFakeObj(int nCurMountID)
    {
        //return false;
        if (null != m_MountFakeObj)
        {
            DestroyMountFakeObj();
        }
        Tab_MountBase MountBase = TableManager.GetMountBaseByID(nCurMountID, 0);
        if (MountBase == null)
        {
            return false;
        }

        Tab_CharMount MountTable = TableManager.GetCharMountByID(MountBase.ModelID, 0);
        if (MountTable == null)
        {
            return false;
        }

        m_MountFakeObj = new FakeObject();
        if (m_MountFakeObj == null)
        {
            return false;
        }

        if (false == m_MountFakeObj.initFakeObject(MountTable.FakeObjID, GameManager.gameManager.ActiveScene.FakeObjTrans))
        {
            return false;
        }
        m_ModelDrag.ModelTrans = m_MountFakeObj.ObjAnim.transform;
        return true;
    }
    private void DestroyMountFakeObj()
    {
        if (m_MountFakeObj != null)
        {
            m_MountFakeObj.Destroy();
            m_MountFakeObj = null;
        }
    }
}

