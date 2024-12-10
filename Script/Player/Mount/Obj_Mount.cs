using System.Collections.Generic;
using Games.GlobeDefine;
using UnityEngine;
using System.Collections;
using Games.LogicObj;
using Games.ObjAnimModule;
using GCGame.Table;
using Module.Log;

public class Obj_Mount : MonoBehaviour {

    private ObjAnimModel m_MountPlayer = null;
    public ObjAnimModel MountPlayer
    {
        get { return m_MountPlayer; }
        set { m_MountPlayer = value; }
    }

    private Obj_OtherPlayer m_PlayerObj = null;
    public Obj_OtherPlayer PlayerObj
    {
        get { return m_PlayerObj; }
        set { m_PlayerObj = value; }
    }

    private int m_MountID = -1;
    public int MountID
    {
        get { return m_MountID; }
        set { m_MountID = value; }
    }

	// Use this for initialization
	void Start ()
	{
	    
	}
    // 创建坐骑
    public void InitMount(Obj_OtherPlayer PlayerObj, int nMountID)
    {
        if (PlayerObj == null)
        {
            return;
        }

        Tab_MountBase MountBase = TableManager.GetMountBaseByID(nMountID, 0);
        if (null == MountBase)
        {
            LogModule.DebugLog("MountBase.txt has not Line ID=" + nMountID);
            return;
        }
        Tab_CharMount MountTable = TableManager.GetCharMountByID(MountBase.ModelID, 0);
        if (MountTable == null)
        {
            LogModule.DebugLog("CharMount.txt has not Line ID=" + MountBase.ModelID);
            return;
        }
        GameObject Obj = PlayerObj.gameObject;
        if (Obj == null)
        {
            return;
        }

        //PlayerObj.MountID = nMountID;
        m_PlayerObj = PlayerObj;
		//玩家骑乘时直接播放骑乘站立动作
		PlayerObj.AnimLogic.Play (1007);
        Singleton<ObjManager>.GetInstance().ReloadModel(Obj, MountTable.MountModel, OnAsycLoadMount, PlayerObj,MountTable.BindPoint);

    }
    void OnAsycLoadMount(string modelName, GameObject resObj, object param1, object param2, object param3)
    {
        if (param1 == null || param2 == null || param3 == null)
        {
            return;
        }       

        GameObject objRoot = (GameObject)param1;
        Obj_OtherPlayer PlayerObj = (Obj_OtherPlayer)param2;
        if (null == objRoot || null == PlayerObj)
        {
            return;
        }

        if (resObj == null || PlayerObj.MountID <= 0)
        {
            if (PlayerObj.MountObj != null)
            {
                GameObject.Destroy(PlayerObj.MountObj.gameObject);
            }            
            return;
        }

        string strBindPoint = (string)param3;

        // 保存角色模型 找Model 放在MountPlayer下
        Transform gMountPlayerTrans = objRoot.transform.FindChild("Model");
        if (gMountPlayerTrans == null || gMountPlayerTrans.gameObject == null)
        {
            return;
        }
        gMountPlayerTrans.parent = this.gameObject.transform;
        gMountPlayerTrans.localPosition = Vector3.zero;
        gMountPlayerTrans.localRotation = Quaternion.identity;
        // 初始化动作
        m_MountPlayer = this.gameObject.AddComponent<ObjAnimModel>() as ObjAnimModel;
        if (m_MountPlayer == null)
        {
            return;
        }
        m_MountPlayer.AnimationFilePath = PlayerObj.AnimationFilePath;
        m_MountPlayer.InitAnimation();

        // 添加马Model
        Singleton<ObjManager>.GetInstance().AsycReloadMountModelOver(modelName, resObj, objRoot, PlayerObj.AnimationFilePath, null);

        string str = "Model" + strBindPoint;
        Transform mountTransferm = objRoot.transform.FindChild(str);
        if (null == mountTransferm)
        {
            LogModule.ErrorLog("can not find the mount's bindpoint:" + str);
            return;
        }
        GameObject gBindPoint = mountTransferm.gameObject;
        if (gBindPoint)
        {
            m_MountPlayer.transform.parent = gBindPoint.transform;
            m_MountPlayer.transform.localPosition = Vector3.zero;
            m_MountPlayer.transform.rotation = objRoot.transform.rotation;
        }

        PlayerObj.OnSwithObjAnimState(PlayerObj.CurObjAnimState);
        //需要下马时 还骑着马 走下面流程(防止出现 上马立马下马的情况 下马失败)
        if (PlayerObj.IsNeedUnMount && PlayerObj.MountID !=-1)
        {
            UnMount(PlayerObj);
        }

        if (null != PlayerObj)
        {
            PlayerObj.OnReloadModle();
        }
    }

    // 换马操作，和创建马分开
    public void ChangeMount(Obj_OtherPlayer PlayerObj, int nMountID)
    {
        if (PlayerObj == null)
        {
            return;
        }

        Tab_MountBase MountBase = TableManager.GetMountBaseByID(nMountID, 0);
        if (null == MountBase)
        {
            LogModule.DebugLog("MountBase.txt has not Line ID=" + nMountID);
            return;
        }
        Tab_CharMount MountTable = TableManager.GetCharMountByID(MountBase.ModelID, 0);
        if (MountTable == null)
        {
            LogModule.DebugLog("CharMount.txt has not Line ID=" + MountBase.ModelID);
            return;
        }
        GameObject Obj = PlayerObj.gameObject;
        if (Obj == null)
        {
            return;
        }

        Singleton<ObjManager>.GetInstance().ReloadModel(Obj, MountTable.MountModel, OnAsycLoadChangeMount, PlayerObj, MountTable.BindPoint);
    }
    void OnAsycLoadChangeMount(string modelName, GameObject resObj, object param1, object param2, object param3)
    {
        if (param1 == null || param2 == null || param3 == null)
        {
            return;
        }

        GameObject objRoot = (GameObject)param1;
        if (null == objRoot)
        {
            return;
        }

        Obj_OtherPlayer PlayerObj = (Obj_OtherPlayer)param2;
        if (null == PlayerObj || null == PlayerObj.gameObject)
        {
            return;
        }

        if (resObj == null || PlayerObj.MountID <= 0)
        {
            if (PlayerObj.MountObj != null)
            {
                GameObject.Destroy(PlayerObj.MountObj.gameObject);
            }
            return;
        }

        string strBindPoint = (string)param3;

        // 保存角色模型 
		Vector3 localS = this.transform.localScale;
        this.transform.parent = PlayerObj.gameObject.transform;


        // 添加马Model
        Singleton<ObjManager>.GetInstance().AsycReloadModelOver(modelName, resObj, objRoot, PlayerObj.AnimationFilePath, null);

        string str = "Model" + strBindPoint;
        Transform mountTransferm = objRoot.transform.FindChild(str);
        if (mountTransferm == null)
        {
            LogModule.ErrorLog("can not find the mount's bindpoint:" + str);
            return;
        }
        GameObject gBindPoint = mountTransferm.gameObject;
        if (gBindPoint)
        {
            this.transform.parent = gBindPoint.transform;
            this.transform.localPosition = Vector3.zero;
            this.transform.rotation = objRoot.transform.rotation;
			this.transform.localScale = localS;
        }

        PlayerObj.OnSwithObjAnimState(PlayerObj.CurObjAnimState);

        //需要下马时 还骑着马 走下面流程(防止出现 上马立马下马的情况 下马失败)
        if (PlayerObj.IsNeedUnMount && PlayerObj.MountID != -1)
        {
            UnMount(PlayerObj);
        }

        PlayerObj.OnReloadModle();
    }

    // 骑马
    static public void RideMount(Obj_OtherPlayer PlayerObj, int nMountID)
    {
        if (null == PlayerObj)
            return;

        Tab_MountBase MountBase = TableManager.GetMountBaseByID(nMountID, 0);
        if (null == MountBase)
        {
            LogModule.DebugLog("MountBase.txt has not Line ID=" + nMountID);
            return;
        }
        Tab_CharMount MountTable = TableManager.GetCharMountByID(MountBase.ModelID, 0);
        if (MountTable == null)
        {
            LogModule.DebugLog("CharMount.txt has not Line ID=" + MountBase.ModelID);
            return;
        }
        GameObject Obj = PlayerObj.gameObject;
        if (Obj != null)
        {
            if (PlayerObj.MountObj) // 换个马
            {
                if (PlayerObj.MountObj.MountID > 0)
                {
                    if (PlayerObj.MountObj.MountID == nMountID)
                    {
                        return;
                    }
                    PlayerObj.MountObj.ChangeMount(PlayerObj, nMountID);
                }
            }
            else //  创建个马
            {
                GameObject MountPlayerObj = ResourceManager.InstantiateResource("Prefab/Model/PlayerRoot", "MountPlayer") as GameObject;
                if (MountPlayerObj != null)
                {
                    PlayerObj.MountObj = MountPlayerObj.AddComponent<Obj_Mount>() as Obj_Mount;

                    if (PlayerObj.MountObj)
                    {
                        PlayerObj.MountObj.InitMount(PlayerObj, nMountID);
                    }
                }
            }
            PlayerObj.MountObj.MountID = nMountID;
			Transform gMountPlayerTrans = PlayerObj.gameObject.transform.FindChild("Model");

			Tab_CharModel  tmodle=TableManager.GetCharModelByID(PlayerObj.ModelID,0);
			if(nMountID>0)
			{
				GameObject  rid=PlayerObj.transform.GetComponentInChildren<Obj_Mount>().gameObject;
				if(rid!=null)
				{
					
					gMountPlayerTrans=rid.transform.FindChild("Model");
					
				}
			}
			//这里会影响其他玩家的大小，先注释掉
			if(tmodle!=null)
				gMountPlayerTrans.localScale=new Vector3(tmodle.Scale,tmodle.Scale,tmodle.Scale);

//			Transform gMountPlayerTrans2 = PlayerObj.MountObj.gameObject.transform.FindChild("Model");
//			if (gMountPlayerTrans2 == null || gMountPlayerTrans2.gameObject == null)
//			{
//				return;
//			}
//			gMountPlayerTrans2.localScale=Vector3.one;
            // 上坐骑时名字板高度修正
            if (PlayerObj.HeadInfoBoard != null)
            {
                BillBoard billboard = PlayerObj.HeadInfoBoard.GetComponent<BillBoard>();
                if (billboard != null)
                {
                    billboard.fDeltaHeight = PlayerObj.DeltaHeight + PlayerObj.GetMountNameBoardHeight();
                }
            }
        }
    }

    // 下马
    static public void UnMount(Obj_OtherPlayer PlayerObj)
    {
        if (PlayerObj == null || PlayerObj.MountObj == null)
        {
            return;
        }

        Transform gMountPlayerTrans = PlayerObj.MountObj.gameObject.transform.FindChild("Model");
        if (gMountPlayerTrans == null || gMountPlayerTrans.gameObject == null)
        {
            return;
        }

        GameObject Obj = PlayerObj.gameObject;
        if (Obj != null)
        {
            gMountPlayerTrans.parent = Obj.transform;

            if (false == Singleton<ObjManager>.GetInstance().ReloadModel(Obj, gMountPlayerTrans.gameObject, PlayerObj.AnimationFilePath))
            {
                return;
            }
		//	Tab_RoleBaseAttr  rolebase=TableManager.GetRoleBaseAttrByID(PlayerObj.BaseAttr.RoleBaseID,0);
			Tab_CharModel  tmodle=TableManager.GetCharModelByID(PlayerObj.ModelID,0);
			if(tmodle!=null)
			gMountPlayerTrans.localScale=new Vector3(tmodle.Scale,tmodle.Scale,tmodle.Scale);
            PlayerObj.MountObj = null;
            PlayerObj.MountID = -1;
            PlayerObj.IsNeedUnMount = false;
            PlayerObj.OnSwithObjAnimState(PlayerObj.CurObjAnimState);

            // 下坐骑时名字板高度恢复
            if (PlayerObj.HeadInfoBoard != null)
            {
                BillBoard billboard = PlayerObj.HeadInfoBoard.GetComponent<BillBoard>();
                if (billboard != null)
                {
                    billboard.RecoverHeight();
                }
            }

        }
    }

    public void PlayMountPlayerAnima(int nAnimaID)
    {
        if (m_MountPlayer && m_MountPlayer.AnimLogic)
        {
            m_MountPlayer.AnimLogic.Play(nAnimaID);
        }
    }
}
