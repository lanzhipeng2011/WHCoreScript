/********************************************************************************
 *	文件名：	ObjManager.cs
 *	全路径：	\Script\Obj\ObjManager.cs
 *	创建人：	李嘉
 *	创建时间：2013-10-25
 *
 *	功能说明：游戏逻辑Obj管理器，对游戏中所有Obj提供创建，移除和管理
 *	修改记录：
 *	//2013-11-18 修改list结构为Dictionary结构，
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Games.GlobeDefine;
using Games.LogicObj;
using GCGame.Table;
using Games.AI_Logic;
using GCGame;
using System;
using Games.Animation_Modle;
using Module.Log;
using Games.FakeObject;
using Games.ObjAnimModule;
using Games.Item;
using Games.Scene;
//初始化一个Obj所需数据
public class Obj_Init_Data
{
	public float    m_fX;           //坐标X
	public float    m_fY;           //坐标Y
	public float    m_fZ;           //坐标Z
	public float    m_fDir;           //朝向
	public int      m_RoleBaseID;   //在RoleBase表中的ID
	public int      m_CharModelID;  //在CharModel表中ID（NPC可以从RoleBase表读取，但变换外形后会导致和RoleBase中的CharModelID不一致） 
	public int      m_ClientInitID; //在ClientInitTable中的ID
	public int      m_ServerID;     //服务器ID
	public UInt64   m_Guid;         //Obj在服务器的GUID
	public int      m_Force;        //势力
	public string   m_StrName;      //Obj的名字
	public int      m_nProfession;  //职业 （创建玩家用的）
	public string   m_strTitleName; //称号名
	public int      m_CurTitleID;   //称号ID
	public bool     m_isInMainPlayerPKList;//是否在主角反击列表中 只对非主角玩家有用
	public bool     m_IsDie;        //是否死亡
	public int      m_PkModel;        //PK模式
	public int      m_OwnerObjId;   //主人objid
	public int      m_MountID;  // 坐骑ID
	public float    m_MoveSpeed;  // 移动速度
	public int      m_WeaponDataID;     // 当前武器
	public int      m_ModelVisualID;    // 当前模型外观ID
	public int      m_WeaponEffectGem;  // 武器特效宝石
	public int      m_StealthLev;//隐身级别
	public bool     m_bNpcBornCreate;//是否是刚刚刷出来的NPC
	public int		m_nOtherVipCost;//vip info
	public UInt64   m_GuildGuid;         //帮会GUID
	public int      m_FellowQuality;     //伙伴品质（创建伙伴用）
	public int      m_nOtherCombatValue;//战力
	public int      m_BindParent;//绑定父节点
	public List<int> m_BindChildren;//绑定子节点
	public bool m_bIsWildEnemyForMainPlayer; //是否与主角敌对
	
	public int m_nGuildBusinessState;       //帮会跑商状态
	public Obj_Init_Data()
	{
		m_BindChildren = new List<int>();
	}
	public void CleanUp()
	{
		m_fX = 0.0f;
		m_fY = 0.0f;
		m_fZ = 0.0f;
		m_RoleBaseID = GlobeVar.INVALID_ID;
		m_CharModelID = GlobeVar.INVALID_ID;
		m_ServerID = GlobeVar.INVALID_ID;
		m_Guid = GlobeVar.INVALID_GUID;
		m_Force = GlobeVar.INVALID_ID;
		m_ClientInitID = GlobeVar.INVALID_ID;
		m_StrName = "";
		m_nProfession =-1; //职业 （创建玩家用的）
		m_isInMainPlayerPKList = false;
		m_IsDie = false;
		m_PkModel = -1;
		m_fDir = 0.0f;
		m_OwnerObjId = -1;
		m_MountID = -1;
		m_MoveSpeed = 0.0f;
		m_ModelVisualID = GlobeVar.INVALID_ID;
		m_WeaponDataID = GlobeVar.INVALID_ID;
		m_WeaponEffectGem = GlobeVar.INVALID_ID;
		m_strTitleName = "";
		m_CurTitleID = GlobeVar.INVALID_ID;
		m_StealthLev = 0;
		m_bNpcBornCreate = false;
		m_nOtherVipCost = -1;
		m_GuildGuid = GlobeVar.INVALID_GUID;
		m_FellowQuality = 0;
		m_nOtherCombatValue = 0;
		m_BindParent = -1;
		m_BindChildren.Clear();
		m_bIsWildEnemyForMainPlayer = false;
		
		m_nGuildBusinessState = -1;
	}
}
public class ObjSnare_Init_Data
{
	public float m_fX;           //坐标X
	public float m_fY;           //坐标Y
	public float m_fZ;           //坐标Z
	public int   m_SnareID;       //在RoleBase表中的ID
	public int   m_ServerID;     //服务器ID
	public int m_OwnerObjId;   //主人objid
	public UInt64 m_OwerGuid;//主人GUID
	
	public void CleanUp()
	{
		m_fX = 0.0f;
		m_fY = 0.0f;
		m_fZ = 0.0f;
		m_SnareID = GlobeVar.INVALID_ID;
		m_ServerID = GlobeVar.INVALID_ID;
		m_OwnerObjId = -1;
		m_OwerGuid = GlobeVar.INVALID_GUID;
	}
}
public class ObjJuqingItem_Init_Data
{

	public int   m_ID;       //在RoleBase表中的ID
	public int   m_EffectID;     //服务器ID

	public void CleanUp()
	{

		m_ID = GlobeVar.INVALID_ID;
		m_EffectID = GlobeVar.INVALID_ID;

	}
}

public class ObjYanHua_Init_Data
{
	public float m_fX;           //坐标X
	public float m_fY;           //坐标Y
	public float m_fZ;           //坐标Z
	public int m_nYanHuaID;       //在RoleBase表中的ID
	public int m_ServerID;     //服务器ID
	public int m_OwnerObjId;   //主人objid
	public UInt64 m_OwerGuid;//主人GUID
	
	public void CleanUp()
	{
		m_fX = 0.0f;
		m_fY = 0.0f;
		m_fZ = 0.0f;
		m_nYanHuaID = GlobeVar.INVALID_ID;
		m_ServerID = GlobeVar.INVALID_ID;
		m_OwnerObjId = -1;
		m_OwerGuid = GlobeVar.INVALID_GUID;
	}
}
//初始化一个DropItemObj所需数据
public class Obj_DroopItemData
{
	public float m_fX;           //坐标X
	public float m_fY;           //坐标Y
	public float m_fZ;           //坐标Z
	public int m_nType;          //掉落类型
	public int m_nItemId;        //物品Id
	public int m_nItemCount;     //物品数量
	public int m_nServerID;      //服务器ID
	public UInt64 m_OwnerGuid;   //归属者Guild
	public void CleanUp()
	{
		m_fX = 0.0f;
		m_fY = 0.0f;
		m_fZ = 0.0f;
		m_nItemId = GlobeVar.INVALID_ID;
		m_nItemCount = 0;
		m_nServerID = GlobeVar.INVALID_ID;
		m_OwnerGuid = GlobeVar.INVALID_GUID;
	}
}

public class Obj_HidePlayerData : IComparable
{
	public Obj_HidePlayerData(string serverID, int value)
	{
		m_serverID = serverID;
		m_value = value;
	}
	
	public int CompareTo(object obj)
	{
		Obj_HidePlayerData info = obj as Obj_HidePlayerData;
		return (this.m_value < info.m_value ? -1 : 1);
	}
	public string m_serverID;
	public int m_value;
	
	//#if UNITY_ANDROID
	public string ResPath;
	public Obj_Init_Data InitData;
	
	//#endif
}

public struct NPCGameManager
{
	public GameObject NpcoGameObject;
	public float deleteTime;
}


public class ObjManager : Singleton<ObjManager>
{
	public delegate void DelAsycModelOver(object param1, object param2);
	//当前客户端所有非玩家Obj池，使用Obj（场景中）名字（唯一）索引，对应Obj的数据
	private Dictionary<string, Obj> m_ObjPools;
	public Dictionary<string, Obj> ObjPools
	{
		get { return m_ObjPools; }
		set { m_ObjPools = value; }
	}
	
	private List<Obj_HidePlayerData> m_ObjOtherPlayerHideList { set; get; }
	private List<Obj_HidePlayerData> m_ObjOtherPlayerShowList { set; get; }
	
	private Obj_MainPlayer m_MainPlayer;                //当前客户端主角色
	public Obj_MainPlayer MainPlayer { get { return m_MainPlayer; } }
	
	//当前客户端所有非Obj的GameObj池，使用 自定义 名字（唯一）索引，对应Obj的数据
	private Dictionary<string, GameObject> m_OtherGameObjPools;
	public Dictionary<string, GameObject> OtherGameObjPools
	{
		get { return m_OtherGameObjPools; }
		set { OtherGameObjPools = value; }
	}
	
	//优化Android下多人同时在线问题 
	private Dictionary<string, GameObject> m_NPCGameObjectList;
	private Dictionary<string, float> m_DeleteNPCList;
	private bool m_IsUseAndroid = true;
	public Dictionary<string,Obj_Character>  m_MonsterList;
	public ObjManager()
	{
		m_ObjPools = new Dictionary<string, Obj>();  //动态的，注意使用上限
		m_ObjPools.Clear();
		
		m_ObjOtherPlayerShowList = new List<Obj_HidePlayerData>();
		m_ObjOtherPlayerHideList = new List<Obj_HidePlayerData>();
		
		m_MainPlayer = null;
		
		m_OtherGameObjPools = new Dictionary<string, GameObject>();  //动态的，注意使用上限
		m_OtherGameObjPools.Clear();
		m_MonsterList=new Dictionary<string,Obj_Character >();
		m_MonsterList.Clear ();
	}
	
	private void AddPoolObj(string name, Obj obj)
	{
		try
		{
			
			//暂时的，服务器传过来的item id有重复的。解决这个问题  by dsy;
			if(m_ObjPools.ContainsKey(name)==false)
				m_ObjPools.Add(name, obj);
		}
		catch (System.Exception ex)
		{
			LogModule.ErrorLog("ObjManager AddPoolObj(" + name + "," + obj.ServerID.ToString() + " Error: " + ex.Message);
		}
	}
	
	//初始化Obj管理器
	public bool Init()
	{
		return true;
	}
	
	public void OnEnterScene()
	{
		#if UNITY_ANDROID
		m_IsUseAndroid = true;
		m_ShowingNPCList.Clear();
		#endif
		
		DeleteNPCPool();
		m_ObjPools.Clear();
		m_OtherGameObjPools.Clear();
		m_ObjOtherPlayerHideList.Clear();
		m_ObjOtherPlayerShowList.Clear();
	}
	
	public void CleanSceneObj()
	{
		foreach (KeyValuePair<string, GameObject> otherGameObj in m_OtherGameObjPools)
		{
			ResourceManager.DestroyResource(otherGameObj.Value);
		}
		
		m_OtherGameObjPools.Clear();
		
		List<GameObject> removeObjList = new List<GameObject>();
		foreach(KeyValuePair<string, Obj> gameObj in m_ObjPools)
		{
			removeObjList.Add(gameObj.Value.gameObject);
		}
		
		for (int i = 0; i < removeObjList.Count; ++i)
		{
			ReomoveObjInScene(removeObjList[i]);
		}
		
		m_MainPlayer = null;
		m_ObjPools.Clear();
		m_ObjOtherPlayerHideList.Clear();
		m_ObjOtherPlayerShowList.Clear();
		
	}
	
	public GameObject CreateGameObjectByResource(Obj_Init_Data initData)
	{
		if (null == initData)
		{
			return null;
		}
		
		//根据RoleBase的ID获得路径
		Tab_CharModel charModel = TableManager.GetCharModelByID(initData.m_RoleBaseID, 0);
		if (null == charModel)
		{
			return null;
		}
		
		return ResourceManager.InstantiateResource(charModel.ResPath, initData.m_ServerID.ToString()) as GameObject;
	}
	
	// 建立剧情obj
	public void CreateModelStoryObj(int nCharModelID, string strName, DelAsycModelOver delAsycStroyModel)
	{
		Tab_CharModel charModel = TableManager.GetCharModelByID(nCharModelID, 0);
		if (null == charModel)
		{
			LogModule.WarningLog("can not find char model id in table :" + nCharModelID);
			return;
		}
		
		GameObject charObj = ResourceManager.InstantiateResource("Prefab/Model/PlayerRoot", strName) as GameObject;
		if (null == charObj)
		{
			LogModule.WarningLog("can not load PlayerRoot :" + nCharModelID);
			return;
		}
		
		ReloadModel(charObj, charModel.ResPath, AsycCreateModelStoryOver, charModel, delAsycStroyModel);
	}
	
	private void AsycCreateModelStoryOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		if (null == param1 || null == param2 || null == param3)
		{
			return;
		}
		
		GameObject charObj = (GameObject)param1;
		if (null == charObj)
		{
			return;
		}
		Tab_CharModel charModelTab = (Tab_CharModel)param2;
		DelAsycModelOver delOverFunction = (DelAsycModelOver)param3;
		
		GameObject charModel = (GameObject)GameObject.Instantiate(resObj);
		if (null == charModel)
		{
			return;
		}
		else
		{
			charModel.name = "Model";
		}
		
		if (false == ReloadModel(charObj, charModel))
		{
			return;
		}
		
		Obj_Client curCharacter = charObj.GetComponent<Obj_Client>();
		if (null == curCharacter)
		{
			curCharacter = charObj.AddComponent<Obj_Client>();
		}
		curCharacter.BaseAttr.RoleName = charModelTab.Name;
		
		curCharacter.gameObject.transform.localScale = new Vector3(charModelTab.Scale, charModelTab.Scale, charModelTab.Scale);
		curCharacter.AnimLogic = charObj.AddComponent<AnimationLogic>();
		curCharacter.ObjEffectLogic = charObj.AddComponent<EffectLogic>();
		curCharacter.AnimationFilePath = charModelTab.AnimPath;
		curCharacter.InitAnimation();
		curCharacter.InitEffect();
		
		if (null != delOverFunction)
		{
			delOverFunction(curCharacter, null);
		}
	}
	
	public void DeleteNPCPool()
	{
		if (m_NPCGameObjectList != null)
		{
			m_NPCGameObjectList.Clear();
			m_NPCGameObjectList = null;
		}
		
		if (m_DeleteNPCList != null)
		{
			m_DeleteNPCList.Clear();
			m_DeleteNPCList = null;
		}
	}
	
	/// <summary>
	/// 返回当前NPC数量
	/// </summary>
	/// <returns></returns>
	public int GetNPCNum()
	{
		if (m_NPCGameObjectList != null)
		{
			return m_NPCGameObjectList.Count;
		}
		return 0;
	}
    /// <summary>
    /// 返回真正显示的npc个数 
    /// </summary>
    /// <returns></returns>
    public int GetShowNPCNum()
    {
        if (this.m_IsUseAndroid)
        {
            if (m_NPCGameObjectList != null)
            {
                int count = m_NPCGameObjectList.Count;

                foreach (KeyValuePair<string, GameObject> npc in m_NPCGameObjectList)
                { 
                    Obj_Character character = npc.Value.GetComponent<Obj_Character>();
					if (npc.Value.activeSelf == false || character.IsDie()||character.ObjType==GameDefine_Globe.OBJ_TYPE.OBJ_SNARE)
					{
						count--;
                    }
                }
                return count;
            }
        }
        else
        {

            if (m_ObjPools == null) return 0;
            return m_ObjPools.Count - 1;
        }
        return 0;
    }
	/// <summary>
	/// 获取当前其他玩家数量
	/// </summary>
	/// <returns></returns>
	public int GetOtherPlayerNum()
	{
		int iTag = 0;
		if (m_ObjPools != null)
		{
			foreach (KeyValuePair<string, Obj> objPair in m_ObjPools)
			{
				if (objPair.Value.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
				{
					iTag++;
				}
			}
		}
		
		return iTag;
	}
	
	//创建NPC
	public void CreateNPC(Obj_Init_Data initData)
	{
		if (null == initData)
			return;
		
		if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
		{
			if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HULAOGUAN)
			{
				Singleton<ObjManager>.GetInstance().MainPlayer.RemoveAllSpicalClient();
			}
		}
		
		if (m_IsUseAndroid)
		{
			if (m_NPCGameObjectList == null)
			{
				m_NPCGameObjectList = new Dictionary<string, GameObject>();
			}
			
			if (null != m_NPCGameObjectList && m_NPCGameObjectList.ContainsKey(initData.m_ServerID.ToString()))
			{
				if (m_DeleteNPCList != null)
				{
					if (m_DeleteNPCList.ContainsKey(initData.m_ServerID.ToString()))
					{
						m_DeleteNPCList.Remove(initData.m_ServerID.ToString());
					}
				}
				
				GameObject npcGameObject = m_NPCGameObjectList[initData.m_ServerID.ToString()];
				
				Obj_NPC objNpc = npcGameObject.GetComponent<Obj_NPC>();
				
				if (objNpc != null)
				{
					objNpc.PlayUnDissolve();
					objNpc.CanLogic = true;
					objNpc.Init(initData);
					objNpc = null;
				}
				//	objNpc.enabled=false;
				npcGameObject.SetActive(true);

                if (CreateCharacterCallBack != null)
                {
                    CreateCharacterCallBack(initData);
                }
				return;
			}
		}
		
		//根据RoleBase的ID获得路径
		Tab_RoleBaseAttr roleBaseAttr = TableManager.GetRoleBaseAttrByID(initData.m_RoleBaseID, 0);
		if (null == roleBaseAttr)
		{
			return;
		}
		
		//NPC创建的时候统一用RoleBase中的ID，而不用Obj_Init_Data结构中的CharModelID
		
		Tab_CharModel charModel = TableManager.GetCharModelByID(roleBaseAttr.CharModelID, 0);
		//Tab_CharModel charModel = TableManager.GetCharModelByID(6, 0);
		if (null == charModel)
		{
			return;
		}
		
		GameObject npc = ResourceManager.InstantiateResource("Prefab/Model/NPCRoot", initData.m_ServerID.ToString()) as GameObject;
		//GameObject npc = CreateGameObjectByResource(initData);
		if (null != npc)
		{
			//加载逻辑体，同时异步加载渲染体
			Obj_NPC objNPC = (Obj_NPC)npc.GetComponent<Obj_NPC>();
			if (!objNPC)
			{
				objNPC = (Obj_NPC)npc.AddComponent<Obj_NPC>();
			}
			
			if (objNPC && objNPC.Init(initData))
			{
				objNPC.CanLogic = true;
				AddPoolObj(objNPC.ServerID.ToString(), objNPC);
				CharacterDefine.REPUTATION_TYPE nType = Reputation.GetObjReputionType(objNPC);
				if (nType == CharacterDefine.REPUTATION_TYPE.REPUTATION_HOSTILE ||
				    nType == CharacterDefine.REPUTATION_TYPE.REPUTATION_NEUTRAL)
				{
					if(!m_MonsterList.ContainsKey(objNPC.ServerID.ToString()))
						m_MonsterList.Add(objNPC.ServerID.ToString(),objNPC);
				}
				
			}
			ReloadModel(npc, charModel.ResPath, AsycCreateNPCOver, initData);
			
			if (m_IsUseAndroid)
			{
				if (null != m_NPCGameObjectList)
				{
					if (!m_NPCGameObjectList.ContainsKey(npc.name) && objNPC.IsNeedBecameVisible())
					{
						m_NPCGameObjectList.Add(npc.name, npc);
					}
					
					//缓存数量多时，主动调用
					if (m_NPCGameObjectList.Count > 50)
					{
						DeleteNPCGameObject();
					}
				}
			}
		}
        
	}
	
	private void AsycCreateNPCOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
       
		if (null == param1 || null == param2)
		{
			return;
		}
		
		GameObject npc = (GameObject)param1;
		if (null == npc)
		{
			return;
		}
		Obj_Init_Data initData = (Obj_Init_Data)param2;
		GameObject NPCModel = (GameObject)GameObject.Instantiate(resObj);
		if (null == NPCModel)
		{
			return;
		}
		else
		{
			NPCModel.name = "Model";
		}
		//  DeltaHeight = charModel.HeadInfoHeight;
		
		if (false == ReloadModel(npc, NPCModel))
		{
			return;
		}
		
		//由于InitAnimation中用到了Model，所以放在模型加载结束后初始化
		Obj_NPC objNPC = (Obj_NPC)npc.GetComponent<Obj_NPC>();
		if (null != objNPC)
		{
			
			//开启阴影
			//if (GameManager.gameManager.IsLowAndroid)
			//{
			
			//                Transform trans = objNPC.transform.FindChild("shadow");
			//                if (trans != null)
			//                {
			//                    trans.gameObject.SetActive(false);
			//                    trans = null;
			//                }
			//}
			//AddOutLineMaterial(NPCModel);
			
			if ((objNPC.BaseAttr.RoleBaseID==20201||objNPC.BaseAttr.RoleBaseID==40205||objNPC.BaseAttr.RoleBaseID==20206 ||objNPC.BaseAttr.RoleBaseID==30201||objNPC.BaseAttr.RoleBaseID==40206||objNPC.BaseAttr.RoleBaseID==100204||objNPC.BaseAttr.RoleBaseID==100205))
			{
				//transform.localScale = new Vector3(charModel.Scale*1.6f, charModel.Scale*1.6f, charModel.Scale*1.6f);
				NPCModel.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
				
				
			}
			Tab_RoleBaseAttr roleBaseAttr = TableManager.GetRoleBaseAttrByID(initData.m_RoleBaseID, 0);
			if (null == roleBaseAttr)
			{
				return;
			}
			if(roleBaseAttr.IsZA==1)
			{
				NavMeshAgent age=npc.GetComponent<NavMeshAgent>();
				if(age!=null)
				age.enabled=false;
				float x=npc.transform.position.x;
				float z=npc.transform.position.z;
				if(roleBaseAttr.Id==190209)
			    	npc.transform.position=new Vector3(x,9.97f,z);
				if(roleBaseAttr.Id==190210)
					npc.transform.position=new Vector3(x,12.47f,z);
				objNPC.m_ObjType=GameDefine_Globe.OBJ_TYPE.OBJ_LUZHANG;
				objNPC.gameObject.AddComponent<Obj_LuZhang>();
//				NavMeshObstacle NavMeshObst = npc.AddComponent<NavMeshObstacle>();
//				if (NavMeshObst)
//				{
//					NavMeshObst.height=0.9f;
//					NavMeshObst.radius=3.0f;
//					NavMeshObst.carving=true;
//					//				ModelObj.transform.localPosition = Vector3.zero;
//					//				ModelObj.transform.localRotation = Quaternion.identity;
//					//				ModelObj.transform.localScale = new Vector3(table.ScaleX, table.ScaleY, table.ScaleZ);
//				}
			}

			objNPC.InitAnimation();
            if (CreateCharacterCallBack != null)
            {
                CreateCharacterCallBack(initData);
            }
		}
	}
	
	//创建NPC
	public void CreateSnareObj(ObjSnare_Init_Data initData)
	{
		if (null == initData)
			return;
		
		//根据RoleBase的ID获得路径
		Tab_SnareObjInfo SnareObjInfo = TableManager.GetSnareObjInfoByID(initData.m_SnareID, 0);
		if (null == SnareObjInfo)
		{
			return;
		}
		
		Tab_CharModel charModel = TableManager.GetCharModelByID(SnareObjInfo.CharModelId, 0);
		if (null == charModel)
		{
			return;
		}
		
		GameObject SnareObj = ResourceManager.InstantiateResource("Prefab/Model/SnareRoot", initData.m_ServerID.ToString()) as GameObject;
		if (null != SnareObj)
		{
			//加载逻辑体，同时异步加载渲染体
			Obj_Snare objSnare = (Obj_Snare)SnareObj.GetComponent<Obj_Snare>();
			if (!objSnare)
			{
				objSnare = (Obj_Snare)SnareObj.AddComponent<Obj_Snare>();
			}
			
			if (objSnare && objSnare.Init(initData))
			{
				objSnare.CanLogic = true;
				AddPoolObj(objSnare.ServerID.ToString(), objSnare);
			}
			ReloadModel(SnareObj, charModel.ResPath, AsycSnareObjOver, initData);
		}
	}
	
	private void AsycSnareObjOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		//动态参数1为模型父节点
		//动态参数2为initData结构
		if (null == param1 || null == param2 || null == resObj)
		{
			return;
		}
		
		GameObject SnareRoot = (GameObject)param1;
		
		if (null == SnareRoot)
		{
			return;
		}
		//ObjSnare_Init_Data initData = (ObjSnare_Init_Data)param2;
		GameObject SnareModel = (GameObject)GameObject.Instantiate(resObj);
		if (null == SnareModel)
		{
			return;
		}
		else
		{
			SnareModel.name = "Model";
		}
		
		if (false == ReloadModel(SnareRoot, SnareModel))
		{
			return;
		}
	}
	
	//创建NPC
	public void CreateYanHuaObj(ObjYanHua_Init_Data initData)
	{
		if (null == initData)
			return;
		
		//根据RoleBase的ID获得路径
		Tab_YanHua YanHuaInfo = TableManager.GetYanHuaByID(initData.m_nYanHuaID, 0);
		if (null == YanHuaInfo)
		{
			return;
		}
		
		Tab_CharModel charModel = TableManager.GetCharModelByID(YanHuaInfo.CharModelId, 0);
		if (null == charModel)
		{
			return;
		}
		
		GameObject SnareObj = ResourceManager.InstantiateResource("Prefab/Model/SnareRoot", initData.m_ServerID.ToString()) as GameObject;
		if (null != SnareObj)
		{
			//加载逻辑体，同时异步加载渲染体
			Obj_YanHua objSnare = (Obj_YanHua)SnareObj.GetComponent<Obj_YanHua>();
			if (!objSnare)
			{
				objSnare = (Obj_YanHua)SnareObj.AddComponent<Obj_YanHua>();
			}
			
			if (objSnare && objSnare.Init(initData))
			{
				objSnare.CanLogic = true;
				AddPoolObj(objSnare.ServerID.ToString(), objSnare);
			}
			ReloadModel(SnareObj, charModel.ResPath, AsycSnareObjOver, initData);
		}
	}
	
	//创建其他玩家
	public void CreateOtherPlayer(Obj_Init_Data initData)
	{
		if (null == initData)
			return;
		
		//所有的其他玩家都创建自固定的OtherPlayerRoot的Prefab，具体模型在OtherPlayerRoot下建立Model节点
		GameObject otherPlayer = ResourceManager.InstantiateResource("Prefab/Model/OtherPlayerRoot", initData.m_ServerID.ToString()) as GameObject;
		if (null != otherPlayer)
		{
			//加载逻辑体，同时异步加载渲染体
			Obj_OtherPlayer objOtherPlayer = (Obj_OtherPlayer)otherPlayer.GetComponent<Obj_OtherPlayer>();
			if (null == objOtherPlayer)
			{
				objOtherPlayer = (Obj_OtherPlayer)otherPlayer.AddComponent<Obj_OtherPlayer>();
			}
			
			if (null != objOtherPlayer && objOtherPlayer.Init(initData))
			{
				objOtherPlayer.CanLogic = true;
			}
			
			Tab_ItemVisual tabItemVisual = TableManager.GetItemVisualByID(objOtherPlayer.ModelVisualID, 0);
			if (tabItemVisual == null)
			{
				tabItemVisual = TableManager.GetItemVisualByID(GlobeVar.DEFAULT_VISUAL_ID, 0);
				if (tabItemVisual == null)
				{
					return;
				}
			}
			int nCharmodelID = objOtherPlayer.GetCharModelID(tabItemVisual, objOtherPlayer.Profession);
			Tab_CharModel charModel = TableManager.GetCharModelByID(nCharmodelID, 0);
			if (charModel == null)
			{
				return;
			}
			
			AddPoolObj(objOtherPlayer.ServerID.ToString(), objOtherPlayer);
			
			//#if UNITY_ANDROID
			//添加判断，当前显示人数是否已经达到玩家要求的最大人数
			//如果>= 当前显示人数时，只显示名字版等，不显示玩家模型
			if (m_ObjOtherPlayerShowList != null)
			{
				if (m_ObjOtherPlayerShowList.Count >= PlayerPreferenceData.SystemShowOtherPlayerCount)
				{
					Obj_HidePlayerData hidePlayerData = new Obj_HidePlayerData(objOtherPlayer.ServerID.ToString(),
					                                                           objOtherPlayer.GetVisibleValue());
					hidePlayerData.ResPath = charModel.ResPath;
					hidePlayerData.InitData = initData;
					
					if (m_ObjOtherPlayerHideList != null)
					{
						m_ObjOtherPlayerHideList.Add(hidePlayerData);
					}
					
					objOtherPlayer.SetVisible(false);
					
					return;
				}
			}
			//#endif
			
			TestOtherPlayerVisible(objOtherPlayer);
			ReloadModel(otherPlayer, charModel.ResPath, AsycCreateOtherPlayerOver, initData);
			
		}
	}
	
	private void AsycCreateOtherPlayerOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		//动态参数1为模型父节点
		//动态参数2为initData
		if (null == param1 || null == param2 || null == resObj)
		{
			return;
		}
		
		GameObject otherPlayer = (GameObject)param1;
		if (null == otherPlayer)
		{
			return;
		}
		Obj_Init_Data initData = (Obj_Init_Data)param2;
		if (null == initData)
			return;
		
		GameObject OtherPlayerModel = (GameObject)GameObject.Instantiate(resObj);
		if (null == OtherPlayerModel)
		{
			return;
		}
		else
		{
			OtherPlayerModel.name = "Model";
		}
		
		if (false == ReloadModel(otherPlayer, OtherPlayerModel))
		{
			return;
		}
		
		Obj_OtherPlayer objOtherPlayer = (Obj_OtherPlayer)otherPlayer.GetComponent<Obj_OtherPlayer>();
		if (null == objOtherPlayer)
		{
			return;
		}
		
		// 改为直接加载玩家当前外观 不是先加载默认 再重载
		//         if (objOtherPlayer.ModelVisualID != GlobeVar.INVALID_ID)
		//         {
		//             objOtherPlayer.ReloadPlayerModelVisual();
		//         }
		//         if (objOtherPlayer.CurWeaponDataID != GlobeVar.INVALID_ID)
		//         {
		objOtherPlayer.RealoadPlayerWeaponVisual();
		//         }
		//         else
		//         {
		//             if (initData.m_MountID > 0)
		//             {
		//                 objOtherPlayer.RideOrUnMount(initData.m_MountID);
		//             }
		//             if (objOtherPlayer.WeaponEffectGem != GlobeVar.INVALID_ID)
		//             {
		//                 objOtherPlayer.ReloadWeaponEffectGem();
		//             }
		//         }
		
		//由于InitAnimation中用到了Model，所以放在模型加载结束后初始化
		objOtherPlayer.InitAnimation();
		objOtherPlayer.UpdateCombatValue();
		//  初始给个站立动画状态，背箭正常
		objOtherPlayer.CurObjAnimState=GameDefine_Globe.OBJ_ANIMSTATE.STATE_NORMOR;
		
		
		//#if UNITY_ANDROID
		objOtherPlayer.RideOrUnMount(objOtherPlayer.MountID);
		//#endif
	}
	
	//创建其他玩家
	public void CreateZombieUser(Obj_Init_Data initData)
	{
		if (null == initData)
			return;
		
		//mwh 根据RoleBase的ID获得路径
		Tab_RoleBaseAttr roleBaseAttr = TableManager.GetRoleBaseAttrByID(initData.m_RoleBaseID, 0);
		if (null == roleBaseAttr)
		{
		//	return ;
		}
		//initData.m_CharModelID = roleBaseAttr.CharModelID;
		//所有的其他玩家都创建自固定的OtherPlayerRoot的Prefab，具体模型在OtherPlayerRoot下建立Model节点
		GameObject otherPlayer = ResourceManager.InstantiateResource("Prefab/Model/OtherPlayerRoot", initData.m_ServerID.ToString()) as GameObject;
		if (null != otherPlayer)
		{            
			Obj_ZombieUser objZombiePlayer = (Obj_ZombieUser)otherPlayer.GetComponent<Obj_ZombieUser>();
			if (null == objZombiePlayer)
			{
				objZombiePlayer = (Obj_ZombieUser)otherPlayer.AddComponent<Obj_ZombieUser>();
			}
			
			if (null != objZombiePlayer && objZombiePlayer.Init(initData))
			{
				objZombiePlayer.CanLogic = true;
			}
			
			Tab_ItemVisual tabItemVisual = TableManager.GetItemVisualByID(objZombiePlayer.ModelVisualID, 0);
			if (tabItemVisual == null)
			{
				tabItemVisual = TableManager.GetItemVisualByID(GlobeVar.DEFAULT_VISUAL_ID, 0);
				if (tabItemVisual == null)
				{
					return;
				}
			}
			int nCharmodelID = objZombiePlayer.GetCharModelID(tabItemVisual, objZombiePlayer.Profession);
			Tab_CharModel charModel = TableManager.GetCharModelByID(nCharmodelID, 0);
			if (charModel == null)
			{
				return;
			}
			initData.m_CharModelID=nCharmodelID;
			//加载逻辑体，同时异步加载渲染体
			//otherPlayer.transform.localScale = new Vector3(charModel.Scale, charModel.Scale, charModel.Scale);
			otherPlayer.transform.localRotation = Utils.DirServerToClient(initData.m_fDir);
			
			AddPoolObj(objZombiePlayer.ServerID.ToString(), objZombiePlayer);
			
			ReloadModel(otherPlayer, charModel.ResPath, AsycCreateZombiePlayerOver, initData);
		}
	}
	
	private void AsycCreateZombiePlayerOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		//动态参数1为模型父节点
		//动态参数2为initData结构
		if (null == param1 || null == param2 || null == resObj)
		{
			return;
		}
		
		GameObject zombiePlayer = (GameObject)param1;
		if (null == zombiePlayer)
		{
			return;
		}
		//Obj_Init_Data initData = (Obj_Init_Data)param2;
		GameObject zombiePlayerModel = (GameObject)GameObject.Instantiate(resObj);
		if (null == zombiePlayerModel)
		{
			return;
		}
		else
		{
			zombiePlayerModel.name = "Model";
		}
		
		if (false == ReloadModel(zombiePlayer, zombiePlayerModel))
		{
			return;
		}
		
		Obj_ZombieUser objZombiePlayer = (Obj_ZombieUser)zombiePlayer.GetComponent<Obj_ZombieUser>();
		if (null == objZombiePlayer)
		{
			return;
		}
		
		objZombiePlayer.RealoadPlayerWeaponVisual();
	}
	
	//创建伙伴
	public void CreateFellow(Obj_Init_Data initData)
	{
		if (null == initData)
			return;
		
		//Fellow的CharModelID配置在FellowAttr
		Tab_FellowAttr fellowAttr = TableManager.GetFellowAttrByID(initData.m_RoleBaseID, 0);
		if (null == fellowAttr)
		{
			return;
		}
		//根据ModelId找到CharModel资源
		Tab_CharModel charModel = TableManager.GetCharModelByID(fellowAttr.ModelId, 0);
		if (null == charModel)
		{
			return;
		}
		
		GameObject fellow = ResourceManager.InstantiateResource("Prefab/Model/FellowRoot", initData.m_ServerID.ToString()) as GameObject;
		if (null != fellow)
		{
			//加载逻辑体，同时异步加载渲染体
			Obj_Fellow objFellow = (Obj_Fellow)fellow.GetComponent<Obj_Fellow>();
			if (!objFellow)
			{
				objFellow = (Obj_Fellow)fellow.AddComponent<Obj_Fellow>();
			}
			
			if (objFellow && objFellow.Init(initData))
			{
				objFellow.CanLogic = true;
				AddPoolObj(objFellow.ServerID.ToString(), objFellow);
			}
			
			ReloadModel(fellow, charModel.ResPath, AsycCreateFellowOver, initData);
		}
	}
	
	private void AsycCreateFellowOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		//动态参数1为模型父节点
		//动态参数2为initData结构
		if (null == param1 || null == param2 || null == resObj)
		{
			return;
		}
		
		GameObject fellow = (GameObject)param1;
		
		if (null == fellow)
		{
			return;
		}
		//Obj_Init_Data initData = (Obj_Init_Data)param2;
		GameObject FellowModel = (GameObject)GameObject.Instantiate(resObj);
		if (null == FellowModel)
		{
			return;
		}
		else
		{
			FellowModel.name = "Model";
		}
		
		if (false == ReloadModel(fellow, FellowModel))
		{
			return;
		}
		
		//由于InitAnimation中用到了Model，所以放在模型加载结束后初始化
		Obj_Fellow objFellow = (Obj_Fellow)fellow.GetComponent<Obj_Fellow>();
		if (null != objFellow)
		{
			objFellow.InitAnimation();
			objFellow.SetVisible(objFellow.IsVisibleChar());
		}
	}
	
	//创建主角
	private bool m_bBeginAsycCreateMainPlayer = false;      //是否开始创建主角标记位，由于主角会在Update中创建，所以改为异步之后需要判断此标记位
	public void CreateMainPlayer()
	{
		
		if (true == m_bBeginAsycCreateMainPlayer && null != m_MainPlayer)
		{
			return;
		}
		if (null == m_MainPlayer)
		{
			//所有的玩家都创建自固定的PlayerRoot的Prefab，具体模型在PlayerRoot下建立Model节点
			GameObject mainPlayer = ResourceManager.InstantiateResource("Prefab/Model/PlayerRoot", "MainPlayer") as GameObject;
			if (null != mainPlayer)
			{
				//先加载逻辑
				m_MainPlayer = mainPlayer.GetComponent<Obj_MainPlayer>();
				if (!m_MainPlayer)
				{
					m_MainPlayer = mainPlayer.AddComponent<Obj_MainPlayer>();
				}
				
				if (m_MainPlayer)
				{
					//赋值玩家guid和职业
					m_MainPlayer.Profession = GameManager.gameManager.PlayerDataPool.EnterSceneCache.Profession;
					m_MainPlayer.GUID = GameManager.gameManager.PlayerDataPool.EnterSceneCache.Guid;
					
					//                    if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YANMENGUANWAI)
					//                    {
					//                        m_MainPlayer.InitYanMenGuanWaiVisual();
					//                    }
					//                    else
					{
						m_MainPlayer.ModelVisualID = GameManager.gameManager.PlayerDataPool.EnterSceneCache.ModelVisualID;
						m_MainPlayer.CurWeaponDataID = GameManager.gameManager.PlayerDataPool.EnterSceneCache.WeaponDataID;
						m_MainPlayer.WeaponEffectGem = GameManager.gameManager.PlayerDataPool.EnterSceneCache.WeaponEffectGem;
					}
					
					Tab_ItemVisual tabItemVisual = TableManager.GetItemVisualByID(m_MainPlayer.ModelVisualID, 0);
					if (tabItemVisual == null)
					{
						tabItemVisual = TableManager.GetItemVisualByID(GlobeVar.DEFAULT_VISUAL_ID, 0);
						if (tabItemVisual == null)
						{
							return;
						}
					}
					
					int nCharmodelID = m_MainPlayer.GetCharModelID(tabItemVisual, m_MainPlayer.Profession);
					Tab_CharModel charModel = TableManager.GetCharModelByID(nCharmodelID, 0);
					if (charModel == null)
					{
						return;
					}
					
					//初始化动作资源路径
					m_MainPlayer.AnimationFilePath = charModel.AnimPath;
					m_MainPlayer.ModelID = nCharmodelID;
					//进行进场经处理
					m_MainPlayer.OnPlayerEnterScene();
					m_MainPlayer.ChangeHeadPic();
					
					AddPoolObj(m_MainPlayer.ServerID.ToString(), m_MainPlayer);
					
					//ObjMainPlayer初始化完成
					m_MainPlayer.CanLogic = true;
					
					// m_MainPlayer.transform.localScale = new Vector3(charModel.Scale, charModel.Scale, charModel.Scale);
					//		GameObject  Model=m_MainPlayer.transform.FindChild("Model").gameObject;
					//	if(Model!=null)
					//	Model.transform.localScale=new Vector3(charModel.Scale, charModel.Scale, charModel.Scale);
					//朝向屏幕
					m_MainPlayer.FaceToScreen();
					
					//向服务器发送EnterSceneOK消息包
					CG_ENTER_SCENE_OK packet = (CG_ENTER_SCENE_OK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ENTER_SCENE_OK);
					packet.IsOK = 1;
					packet.SendPacket();

                    try
                    {

                        PlatformHelper.EnterSceneOK();
                    }
                    catch (System.Exception e)
                    {
                        NGUILogHelpler.Log(e.Message, "PlatformHelper");
                    }


                    //m_MainPlayer.MountID = GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterSceneMountID;
                    if (Singleton<ObjManager>.GetInstance().MainPlayer.IsDie())
					{
						Singleton<ObjManager>.GetInstance().MainPlayer.OnCorpse();
					}
					
					m_MainPlayer.VipCost = GameManager.gameManager.PlayerDataPool.VipCost;
					m_MainPlayer.GuildBusinessState = GameManager.gameManager.PlayerDataPool.EnterSceneCache.GuildBusinessState;
					m_MainPlayer.OtherCombatValue = GameManager.gameManager.PlayerDataPool.PoolCombatValue;
					m_MainPlayer.BaseAttr.CombatValue = GameManager.gameManager.PlayerDataPool.PoolCombatValue;
					//如果主角已经创建，清理服务器数据，防止重复建立，并且准备下次切换场景使用
					GameManager.gameManager.PlayerDataPool.EnterSceneCache.ClearEnterSceneInfo();
					ReloadModel(mainPlayer, charModel.ResPath, AsycCreateMainPlayerOver, charModel);
					//GameManager.gameManager.SceneLogic.StartCoroutine(BundleManager.LoadModel(resPath, AsycCreateMainPlayerOver, mainPlayer, charModel));
					m_bBeginAsycCreateMainPlayer = true;
					

					//朝向屏幕
					m_MainPlayer.BindParent = GameManager.gameManager.PlayerDataPool.MainBindParent;
					m_MainPlayer.UpdateBindChildren(GameManager.gameManager.PlayerDataPool.MainBindChildren);
				}
			}
		}
	}
	

    public event System.Action MainPlayerOnLoad;
	//异步加载主角OK
	private void AsycCreateMainPlayerOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		
		
		//动态参数1为模型父节点
		//动态参数2为CharModel的表项
		if (null == param1 || null == param2 || null == resObj)
		{
			return;
		}
		
		GameObject mainPlayer = (GameObject)param1;
		
		if (null == mainPlayer)
		{
			LogModule.ErrorLog("create main player model error");
			return;
		}
		//Tab_CharModel charModel = (Tab_CharModel)param2;
		GameObject MainPlayerModel = (GameObject)GameObject.Instantiate(resObj);
		if (null == MainPlayerModel)
		{
			return;
		}
		else
		{
			MainPlayerModel.name = "Model";
		}
		
		if (false == ReloadModel(mainPlayer, MainPlayerModel))
		{
			return;
		}
		
		AddOutLineMaterial(MainPlayerModel);
		if (null != m_MainPlayer)
		{
			// 直接按玩家外形创建玩家 不是先创建默认的 再重载了
			//             if (m_MainPlayer.ModelVisualID != GlobeVar.INVALID_ID)
			//             {
			//                 m_MainPlayer.ReloadPlayerModelVisual();
			//             }
			//if (m_MainPlayer.CurWeaponDataID != GlobeVar.INVALID_ID)
			//{
			m_MainPlayer.RealoadPlayerWeaponVisual();
			//}
			//             else if (m_MainPlayer.WeaponEffectGem != GlobeVar.INVALID_ID)
			//             {
			//                 m_MainPlayer.ReloadWeaponEffectGem();
			//             }
			
			m_MainPlayer.InitAnimation();
			m_MainPlayer.UpdateCombatValue();
			m_MainPlayer.OptNameChange();
			Obj_MainPlayer main=m_MainPlayer.GetComponent<Obj_MainPlayer>();
			//  初始给个站立动画状态，背箭正常
			main.CurObjAnimState=GameDefine_Globe.OBJ_ANIMSTATE.STATE_NORMOR;
            if (GameManager.gameManager.ActiveScene != null)
            {
                GameManager.gameManager.ActiveScene.MainPlayerCreateOver();
            }
            if (MainPlayerOnLoad != null) 
            {
                MainPlayerOnLoad();
            }
		}
//		if (GameManager.gameManager.RunningScene == 21) 
//		{
//			if(GameManager.gameManager.PlayerDataPool.IsFirstYeXiDaYing==true)
//
//			{
//				CG_GUIDE_STEP pact = (CG_GUIDE_STEP)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUIDE_STEP);
//				pact.StepID = 0;
//				pact.SendPacket();
//			}
//		}
		m_bBeginAsycCreateMainPlayer = false;
	}
	
	//创建掉落包
	public void CreateDropItem(Obj_DroopItemData initData)
	{
		UIManager.LoadItem(UIInfo.DropItemHeadInfo, OnCreateDropItem, initData);
	}
	
	public void OnCreateDropItem(GameObject resObj, object param)
	{
		if (null == resObj)
		{
			LogModule.ErrorLog("create drop item error");
			return;
		}
		Obj_DroopItemData initData = param as Obj_DroopItemData;
		if (null == initData)
			return;
		
		
		Obj_DropItem objDropItem = null;
		GameObject dorpItem = GameObject.Instantiate(resObj) as GameObject;
		dorpItem.name = initData.m_nServerID.ToString();
		if (null != dorpItem)
		{
			objDropItem = (Obj_DropItem)dorpItem.GetComponent<Obj_DropItem>();
			if (!objDropItem)
			{
				objDropItem = (Obj_DropItem)dorpItem.AddComponent<Obj_DropItem>();
			}
			
			if (objDropItem && objDropItem.Init(initData))
			{
				objDropItem.CanLogic = true;
				AddPoolObj(objDropItem.ServerID.ToString(), objDropItem);
			}
		}
	}
	
	//创建采集点
	public void CreateCollectItem(Tab_CollectItem CollectItem, string szName, int nItemIndex)
	{
		if (null == CollectItem)
		{
			return;
		}
		
		Tab_CharModel pCharModel = TableManager.GetCharModelByID(CollectItem.CharModelID, 0);
		if (pCharModel == null)
		{
			LogModule.DebugLog("CollectItem Error CharModel is null");
			return;
		}
		
		GameObject CollectItemRoot = ResourceManager.InstantiateResource("Prefab/Model/CollectItemRoot", szName) as GameObject;
		if (null == CollectItemRoot)
		{
			LogModule.DebugLog("CollectItem Error CharModel is null");
			return;
		}
		Obj_OtherGameObj OtherGameObj = CollectItemRoot.AddComponent<Obj_OtherGameObj>();
		if (OtherGameObj == null)
		{
			OtherGameObj = CollectItemRoot.AddComponent<Obj_OtherGameObj>();
		}
		OtherGameObj.SetIntParam(0, CollectItem.CharModelID);
		OtherGameObj.SetIntParam(1, CollectItem.Index);
		OtherGameObj.SetIntParam(2, nItemIndex);
		
		ReloadModel(CollectItemRoot, pCharModel.ResPath, AsycCreateCollectItemOver, CollectItem,szName);
	}
	
	private void AsycCreateCollectItemOver(string modelName, GameObject resObj, object param1, object param2, object param3 = null)
	{
		if (null == param1 || null == param2 || resObj == null)
		{
			return;
		}
		
		GameObject CollectItemRoot = (GameObject)param1;
		Tab_CollectItem CollectItem = (Tab_CollectItem)param2;
		//string strName = (string)param3;
		
		if (null == CollectItemRoot || null == CollectItem)
		{
			return;
		}
		GameObject ItemObj = (GameObject)GameObject.Instantiate(resObj);
		if ( ItemObj == null)
		{
			return;
		}
		
		ItemObj.name = "Model";
		if (false == ReloadModel(CollectItemRoot, ItemObj))
		{
			return;
		}
		ItemObj.transform.localRotation = Quaternion.Euler(CollectItem.RotX, CollectItem.RotY, CollectItem.RotZ);
		Vector3 vPos = new Vector3(CollectItem.PosX, 0, CollectItem.PosZ);
		if (CollectItem.Radius > 0)
		{
			vPos.x += UnityEngine.Random.Range(-CollectItem.Radius, CollectItem.Radius);
			vPos.z += UnityEngine.Random.Range(-CollectItem.Radius, CollectItem.Radius);
		}
		
		//         if (GameManager.gameManager.ActiveScene.IsT4MScene())
		//         {
		//             vPos.y = GameManager.gameManager.ActiveScene.GetTerrainHeight(vPos);
		//         }
		//         else if (null != Terrain.activeTerrain)
		//         {
		//             vPos.y = Terrain.activeTerrain.SampleHeight(vPos);
		//         }
		vPos.y = CollectItem.PosY;
		vPos = ActiveScene.GetTerrainPosition (vPos);
		vPos.y=vPos.y+CollectItem.PosY;
		CollectItemRoot.transform.position = vPos;
		Tab_CharModel pCharModel = TableManager.GetCharModelByID(CollectItem.CharModelID, 0);
		if (pCharModel == null)
		{
			LogModule.DebugLog("CollectItem Error CharModel is null");
			return;
		}
		float scale = pCharModel.Scale;
		
		ItemObj.transform.localScale = new Vector3(scale,scale,scale);
		CollectItemRoot.tag = "CollectItem";
		
		AddPoolOtherGameObj(CollectItemRoot.name, CollectItemRoot);
	}
	//创建采集点
	public void CreateJuqingItem(Tab_JuqingItem JuqingItem, string szName, int nItemIndex)
	{
		if (null == JuqingItem)
		{
			return;
		}
		
		Tab_CharModel pCharModel = TableManager.GetCharModelByID(JuqingItem.CharModelID, 0);
		if (pCharModel == null)
		{
			LogModule.DebugLog("CollectItem Error CharModel is null");
			return;
		}
		
		GameObject JuqingItemRoot = ResourceManager.InstantiateResource("Prefab/Model/JuqingItemRoot", szName) as GameObject;
		if (null == JuqingItemRoot)
		{
			LogModule.DebugLog("JuqingItemRoot Error CharModel is null");
			return;
		}
		Obj_JuqingItem OtherGameObj = JuqingItemRoot.AddComponent<Obj_JuqingItem>();
		if (OtherGameObj == null)
		{
			OtherGameObj = JuqingItemRoot.AddComponent<Obj_JuqingItem>();
		}
//		OtherGameObj.SetIntParam(0, JuqingItem.CharModelID);
//		OtherGameObj.SetIntParam(1, JuqingItem.Index);
//		OtherGameObj.SetIntParam(2, nItemIndex);
		//		OtherGameObj

		ObjJuqingItem_Init_Data data = new ObjJuqingItem_Init_Data ();
		data.m_ID = JuqingItem.ID;
		OtherGameObj.Init (data);

		ReloadModel(JuqingItemRoot, pCharModel.ResPath, AsycCreateJuqingItemOver, JuqingItem,szName);
	}
	
	private void AsycCreateJuqingItemOver(string modelName, GameObject resObj, object param1, object param2, object param3 = null)
	{
		if (null == param1 || null == param2 || resObj == null)
		{
			return;
		}
		
		GameObject JuqingItemRoot = (GameObject)param1;
		Tab_JuqingItem JuqingItem = (Tab_JuqingItem)param2;
		//string strName = (string)param3;
		
		if (null == JuqingItemRoot || null == JuqingItem)
		{
			return;
		}
		GameObject ItemObj = (GameObject)GameObject.Instantiate(resObj);
		if ( ItemObj == null)
		{
			return;
		}
		
		ItemObj.name = "Model";
		if (false == ReloadModel(JuqingItemRoot, ItemObj))
		{
			return;
		}
		ItemObj.transform.localRotation = Quaternion.Euler(JuqingItem.RotX, JuqingItem.RotY, JuqingItem.RotZ);
		Vector3 vPos = new Vector3(JuqingItem.PosX, 0, JuqingItem.PosZ);
		if (JuqingItem.Radius > 0)
		{
			vPos.x += UnityEngine.Random.Range(-JuqingItem.Radius, JuqingItem.Radius);
			vPos.z += UnityEngine.Random.Range(-JuqingItem.Radius, JuqingItem.Radius);
		}
		
		//         if (GameManager.gameManager.ActiveScene.IsT4MScene())
		//         {
		//             vPos.y = GameManager.gameManager.ActiveScene.GetTerrainHeight(vPos);
		//         }
		//         else if (null != Terrain.activeTerrain)
		//         {
		//             vPos.y = Terrain.activeTerrain.SampleHeight(vPos);
		//         }
		vPos.y = JuqingItem.PosY;
		vPos = ActiveScene.GetTerrainPosition (vPos);
		vPos.y=vPos.y+JuqingItem.PosY;
		JuqingItemRoot.transform.position = vPos;
		Tab_CharModel pCharModel = TableManager.GetCharModelByID(JuqingItem.CharModelID, 0);
		if (pCharModel == null)
		{
			LogModule.DebugLog("CollectItem Error CharModel is null");
			return;
		}
		float scale = pCharModel.Scale;
		
		ItemObj.transform.localScale = new Vector3(scale,scale,scale);
		JuqingItemRoot.tag = "JuqingItem";
		
		AddPoolOtherGameObj(JuqingItemRoot.name, JuqingItemRoot);
	}

	public void CreateDyncObstacle(Tab_DynamicObstacle tab, string strName)
	{
		if (null == tab)
		{
			return;
		}
		
		GameObject DyncObstRoot = ResourceManager.InstantiateResource("Prefab/Model/DynamisObstacleRoot", strName) as GameObject;
		if (null == DyncObstRoot)
		{
			LogModule.DebugLog("DynamicObstacle Error DynamisObstacleRoot is null");
			return;
		}
		ReloadModel(DyncObstRoot, tab.ResPath, AsycCreateDyncObstacle, tab, strName);
		
	}
	
	void AsycCreateDyncObstacle(string modelName, GameObject resObj, object param1, object param2, object param3 = null)
	{
		if (null == param1 || null == param2 || resObj == null)
		{
			return;
		}
		
		GameObject DyncObstRoot = (GameObject)param1;
		if (null == DyncObstRoot)
		{
			return;
		}
		Tab_DynamicObstacle table = (Tab_DynamicObstacle)param2;
		string strName = (string)param2;
		GameObject ModelObj = (GameObject)GameObject.Instantiate(resObj);
		if (ModelObj == null)
		{
			return;
		}
		
		ModelObj.name = "Model";
		if (false == ReloadModel(DyncObstRoot, ModelObj))
		{
			return;
		}
		
		DyncObstRoot.transform.localPosition = new Vector3(table.PosX, table.PosY, table.PosZ);
		DyncObstRoot.transform.localRotation = Quaternion.Euler(table.RotX, table.RotY, table.RotZ);
		DyncObstRoot.transform.localScale = Vector3.one;
		
		NavMeshObstacle NavMeshObst = ModelObj.AddComponent<NavMeshObstacle>();
		if (NavMeshObst)
		{
			ModelObj.transform.localPosition = Vector3.zero;
			ModelObj.transform.localRotation = Quaternion.identity;
			ModelObj.transform.localScale = new Vector3(table.ScaleX, table.ScaleY, table.ScaleZ);
			
			AddPoolOtherGameObj(strName, DyncObstRoot);
		}
	}
	
	//为一个PlayerRoot加载一个Model节点，节点内容为模型的骨骼和mesh
	//此做法的优点在于便于随时更换模型而无需重新挂接脚本
	public bool ReloadModel(GameObject objRoot, string modelPath, BundleManager.LoadSingleFinish delFinish, object param2, object param3 = null, string animationPath = null)
	{
		//判断参数合法性
		if (null == objRoot || modelPath.Length <= 0)
		{
			return false;
		}
		
		string loadPath = "Prefab/Model/" + modelPath;
		
		//创建新的Model节点并挂到objRoot
		GameObject objModel = ResourceManager.InstantiateResource(loadPath, "Model") as GameObject;
		if (objModel)
		{
			return ReloadModel(objRoot, objModel, animationPath);
		}
		else
		{
			//try asyc load model
			BundleManager.LoadModelInQueue(modelPath, delFinish, objRoot, param2, param3);
			//GameManager.gameManager.SceneLogic.StartCoroutine(BundleManager.LoadModel(modelPath, delFinish, objRoot, param2, param3));
		}
		
		return false;
	}
	
	public void AsycReloadModelOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		if (null == param1 || null == param2 || null == resObj)
		{
			return;
		}
		
		GameObject objRoot = (GameObject)param1;
		if (null == objRoot)
		{
			return;
		}
		string animationPath = (string)param2;
		Obj_Mount MountObj = (Obj_Mount)param3;
		
		GameObject model = (GameObject)GameObject.Instantiate(resObj);
		if (null == model)
		{
			return;
		}
		else
		{
			model.name = "Model";
		}
		
		ReloadModel(objRoot, model, animationPath, MountObj);
		
		Obj_Character curChar = null;
		// 加载武器 有可能是脱武器 所以INVALID_ID也一样传进去
		Obj_MainPlayer mainPlayer = objRoot.GetComponent<Obj_MainPlayer>();
		if (null != mainPlayer)
		{
			mainPlayer.ChangeHeadPic();
			
			mainPlayer.RealoadPlayerWeaponVisual();
			
			AddOutLineMaterial(model);
			
			mainPlayer.UpdateCombatValue();
			
			curChar = mainPlayer;
		}
		else
		{
			Obj_OtherPlayer otherPlayer = objRoot.GetComponent<Obj_OtherPlayer>();
			if (otherPlayer != null)
			{
				otherPlayer.RealoadPlayerWeaponVisual();
				otherPlayer.UpdateCombatValue();
				curChar = otherPlayer;
			}
			Obj_NPC npc = objRoot.GetComponent<Obj_NPC>();
			if (npc != null)
			{
				npc.RealoadPlayerWeaponVisual(npc.WeaponDataID,npc.Profession,npc.WeaponEffectGem);
				curChar = npc;
			}
			
			
		}
		
		if (null != curChar)
		{
			curChar.OnReloadModle();
		}
	}
	
	public void AsycReloadMountModelOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		if (null == param1 || null == param2)
		{
			return;
		}
		
		GameObject objRoot = (GameObject)param1;
		
		if (null == objRoot)
		{
			return;
		}
		string animationPath = (string)param2;
		
		GameObject model = (GameObject)GameObject.Instantiate(resObj);
		if (null == model)
		{
			return;
		}
		else
		{
			model.name = "Model";
		}
		
		//判断参数合法性
		if (null == objRoot)
		{
			return;
		}
		
		//如果发现已经有Model节点，则删除之
		Transform modelTransform = objRoot.transform.FindChild("Model");
		if (null != modelTransform)
		{
			// 立即删除 替换马模型 后续需要寻找节点
			GameObject.DestroyImmediate(modelTransform.gameObject);
		}
		
		//创建新的Model节点并挂到objRoot
		if (model)
		{
			model.transform.parent = objRoot.transform;
			model.transform.localPosition = Vector3.zero;
			model.transform.localRotation = Quaternion.identity;
			model.transform.localScale = Vector3.one;
			
			//设置模型layer的和root一致
			Transform[] trans = model.GetComponentsInChildren<Transform>();
			for (int i = 0; i < trans.Length; ++i)
			{
				trans[i].gameObject.layer = objRoot.layer;
			}
			
			Obj_Character curChar = objRoot.GetComponent<Obj_Character>();
			if (null != curChar)
			{
				if (null != animationPath)
				{
					curChar.AnimationFilePath = animationPath;
				}
				
				curChar.InitAnimation();
				
			}
			
			if (null != objRoot.GetComponent<Obj_MainPlayer>())
			{
				AddOutLineMaterial(model);
			}
		}
	}
	
	public void AsycLoadFakeObjOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		if (null == param1 || null == param2 || null == param3)
		{
			return;
		}
		
		GameObject Obj = (GameObject)param1;
		if (null == Obj)
		{
			return;
		}
		Tab_FakeObject FakeObjTable = (Tab_FakeObject)param2;
		FakeObject FakeObj = (FakeObject)param3;
		
		if (null == FakeObj)
		{
			return;
		}
		
		GameObject FakeModel = (GameObject)GameObject.Instantiate(resObj);
		if (null == FakeModel)
		{
			return;
		}
		else
		{
			FakeModel.name = "Model";
		}
		
		if (false == ReloadModel(Obj, FakeModel))
		{
			return;
		}
		
		// 创建成功 设置节点
		GameObject gParentObj = FakeObj.FakeObjNode;
		if (gParentObj == null)
		{
			return;
		}
		Obj.transform.parent = gParentObj.transform;
		Obj.transform.localPosition = new Vector3(FakeObjTable.XOffSet, FakeObjTable.YOffSet, FakeObjTable.ZOffset);
		Obj.transform.localScale = Vector3.one * FakeObjTable.Scale;
		Obj.transform.localRotation = Quaternion.identity;
		
		// 方向
		Transform CameraTransform = gParentObj.transform.parent;
		if (null == CameraTransform)
		{
			LogModule.DebugLog("error: MountAndFellowLogic::MountItemClick() can't find obj from /Camera-FakeObject");
		}
		//Vector3 lookRot = CameraTransform.position - Obj.transform.position;
		Obj.transform.localRotation = Quaternion.Euler(FakeObjTable.XRotation, FakeObjTable.YRotation, FakeObjTable.ZRotation);
		
		FakeObj.ObjAnim = Obj.GetComponent<ObjAnimModel>() as ObjAnimModel;
		if (FakeObj.ObjAnim == null)
		{
			FakeObj.ObjAnim = Obj.AddComponent<ObjAnimModel>() as ObjAnimModel;
		}
		
		if (FakeObj.ObjAnim)
		{
			FakeObj.ObjAnim.AnimationFilePath = FakeObjTable.AnimPath;
			FakeObj.ObjAnim.InitAnimation();
			
			if (null != FakeObj.ObjAnim.AnimLogic)
				FakeObj.ObjAnim.AnimLogic.Play(0);
		}
		
		if (FakeObjTable.IsPlayer == 1)
		{
			FakeObj.InitFakeObjWeapon(Obj, Singleton<ObjManager>.Instance.MainPlayer.CurWeaponDataID, FakeObjTable);
		}        
	}
	
	//查看他人属性专用使用，将WeaponID 作为上边的函数参数实在改动太大，所以加一个新的
	public void OtherView_AsycLoadFakeObjOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		if (null == param1 || null == param2 || null == param3)
		{
			return;
		}
		
		GameObject Obj = (GameObject)param1;
		Tab_FakeObject FakeObjTable = (Tab_FakeObject)param2;
		OtherFakeObject FakeObj = (OtherFakeObject)param3;
		if(null == Obj || null == FakeObj)
		{
			return;
		}
		
		GameObject FakeModel = (GameObject)GameObject.Instantiate(resObj);
		if (null == FakeModel)
		{
			return;
		}
		else
		{
			FakeModel.name = "Model";
		}
		
		if (false == ReloadModel(Obj, FakeModel))
		{
			return;
		}
		
		// 创建成功 设置节点
		GameObject gParentObj = FakeObj.FakeObjNode;
		Obj.transform.parent = gParentObj.transform;
		Obj.transform.localPosition = new Vector3(FakeObjTable.XOffSet, FakeObjTable.YOffSet, FakeObjTable.ZOffset);
		Obj.transform.localScale = Vector3.one;
		Obj.transform.localRotation = Quaternion.identity;
		
		// 方向
		Transform CameraTransform = gParentObj.transform.parent;
		if (null == CameraTransform)
		{
			LogModule.DebugLog("error: MountAndFellowLogic::MountItemClick() can't find obj from /Camera-FakeObject");
		}
		//Vector3 lookRot = CameraTransform.position - Obj.transform.position;
		Obj.transform.localRotation = Quaternion.Euler(FakeObjTable.XRotation, FakeObjTable.YRotation, FakeObjTable.ZRotation);
		
		FakeObj.ObjAnim = Obj.AddComponent<ObjAnimModel>() as ObjAnimModel;
		
		if (FakeObj.ObjAnim)
		{
			FakeObj.ObjAnim.AnimationFilePath = FakeObjTable.AnimPath;
			FakeObj.ObjAnim.InitAnimation();
			
			if (null != FakeObj.ObjAnim.AnimLogic)
				FakeObj.ObjAnim.AnimLogic.Play(0);
		}
		
		if (FakeObjTable.IsPlayer == 1)
		{
			FakeObj.InitFakeObjWeapon(Obj, GameManager.gameManager.OtherPlayerData.CurWeaponDataID, FakeObjTable);
		}
	}
	
	
	public void AsycLoadFitOnObjOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		if (null == param1 || null == param2)
		{
			return;
		}
		
		GameObject Obj = (GameObject)param1;
		List<object> param = (List<object>)param2;
		if (null == Obj || null == param || param.Count < 3)
		{
			return;
		}
		
		Tab_FakeObject FakeObjTable = (Tab_FakeObject)param[0];
		FakeObject FakeObj = (FakeObject)param[1];
		YuanBaoShopLogic.FitOnVisual playerFitOnVisual = (YuanBaoShopLogic.FitOnVisual)param[2];
		
		if (FakeObj == null)
		{
			return;
		}
		
		GameObject FakeModel = (GameObject)GameObject.Instantiate(resObj);
		if (null == FakeModel)
		{
			return;
		}
		else
		{
			FakeModel.name = "Model";
		}
		
		if (false == ReloadModel(Obj, FakeModel))
		{
			return;
		}
		
		// 创建成功 设置节点
		GameObject gParentObj = FakeObj.FakeObjNode;
		Obj.transform.parent = gParentObj.transform;
		Obj.transform.localPosition = new Vector3(FakeObjTable.XOffSet, FakeObjTable.YOffSet, FakeObjTable.ZOffset);
		Obj.transform.localScale = Vector3.one;
		Obj.transform.localRotation = Quaternion.identity;
		
		// 方向
		Transform CameraTransform = gParentObj.transform.parent;
		if (null == CameraTransform)
		{
			LogModule.DebugLog("error: MountAndFellowLogic::MountItemClick() can't find obj from /Camera-FakeObject");
		}
		
		Obj.transform.localRotation = Quaternion.Euler(FakeObjTable.XRotation, FakeObjTable.YRotation, FakeObjTable.ZRotation);
		
		FakeObj.ObjAnim = Obj.GetComponent<ObjAnimModel>() as ObjAnimModel;
		if (FakeObj.ObjAnim == null)
		{
			FakeObj.ObjAnim = Obj.AddComponent<ObjAnimModel>() as ObjAnimModel;
		}
		
		if (FakeObj.ObjAnim)
		{
			FakeObj.ObjAnim.InitAnimation();
			
			if (null != FakeObj.ObjAnim.AnimLogic)
				FakeObj.ObjAnim.AnimLogic.Play(0);
		}
		
		if (playerFitOnVisual.WeaponID == GlobeVar.INVALID_ID)
		{
			FakeObj.InitFakeObjWeapon(Obj, Singleton<ObjManager>.Instance.MainPlayer.CurWeaponDataID, FakeObjTable);
		}
		else
		{
			FakeObj.InitFakeObjWeapon(Obj, playerFitOnVisual.WeaponID, FakeObjTable);
		}
	}
	
	public void AsycLoadRoleViewFitOnObjOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		if (null == param1 || null == param2)
		{
			return;
		}
		
		GameObject Obj = (GameObject)param1;
		if (null == Obj)
		{
			return;
		}
		List<object> param = (List<object>)param2;
		if (null == param || param.Count < 2)
		{
			return;
		}
		
		Tab_FakeObject FakeObjTable = (Tab_FakeObject)param[0];
		FakeObject FakeObj = (FakeObject)param[1];
		
		if (null == FakeObj)
		{
			return;
		}
		GameObject FakeModel = (GameObject)GameObject.Instantiate(resObj);
		if (null == FakeModel)
		{
			return;
		}
		else
		{
			FakeModel.name = "Model";
		}
		
		if (false == ReloadModel(Obj, FakeModel))
		{
			return;
		}
		
		// 创建成功 设置节点
		GameObject gParentObj = FakeObj.FakeObjNode;
		Obj.transform.parent = gParentObj.transform;
		Obj.transform.localPosition = new Vector3(FakeObjTable.XOffSet, FakeObjTable.YOffSet, FakeObjTable.ZOffset);
		Obj.transform.localScale = Vector3.one;
		Obj.transform.localRotation = Quaternion.identity;
		
		// 方向
		Transform CameraTransform = gParentObj.transform.parent;
		if (null == CameraTransform)
		{
			LogModule.DebugLog("error: MountAndFellowLogic::MountItemClick() can't find obj from /Camera-FakeObject");
		}
		
		Obj.transform.localRotation = Quaternion.Euler(FakeObjTable.XRotation, FakeObjTable.YRotation, FakeObjTable.ZRotation);
		
		FakeObj.ObjAnim = Obj.GetComponent<ObjAnimModel>() as ObjAnimModel;
		if (FakeObj.ObjAnim == null)
		{
			FakeObj.ObjAnim = Obj.AddComponent<ObjAnimModel>() as ObjAnimModel;
		}
		
		if (FakeObj.ObjAnim)
		{
			FakeObj.ObjAnim.InitAnimation();
			
			if (null != FakeObj.ObjAnim.AnimLogic)
				FakeObj.ObjAnim.AnimLogic.Play(0);
		}
		
		if (FakeObjTable.IsPlayer == 1)
		{
			FakeObj.InitFakeObjWeapon(Obj, Singleton<ObjManager>.Instance.MainPlayer.CurWeaponDataID, FakeObjTable);
		}
	}
	
	public bool ReloadWeapon(GameObject objRoot, string modelPath, BundleManager.LoadSingleFinish delFinish, object param2, object param3 = null)
	{
		if (objRoot == null || modelPath.Length <= 0)
		{
			return false;
		}
		
		BundleManager.LoadModelInQueue(modelPath, delFinish, objRoot, param2, param3);
		//GameManager.gameManager.SceneLogic.StartCoroutine(BundleManager.LoadModel(modelPath, delFinish, objRoot, param2, param3));
		return true;
	}
	
	public void AsycReloadWeaponOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		if (param1 == null || param2 == null)
		{
			return;
		}
		GameObject objRoot = (GameObject)param1;
		List<object> Params = (List<object>)param2;
		
		if (null == objRoot || null == Params || Params.Count < 4)
		{
			return;
		}
		string strWeaponPoint = (string)Params[0];
		int nWeaponEffectGem = (int)Params[1];
		Obj_Mount MountObj = (Obj_Mount)Params[2];
		int nProfession = (int)Params[3];
		
		GameObject weaponModel = (GameObject)GameObject.Instantiate(resObj);
		if (null == weaponModel)
		{
			return;
		}
		
		GameObject playerGameObject = null;
		// 根据是否在坐骑上 确定玩家模型所在位置
		if (MountObj == null)
		{
			playerGameObject = objRoot;
		}
		else
		{
			if (MountObj.MountPlayer == null)
			{
				return;
			}
			playerGameObject = MountObj.MountPlayer.gameObject;
		}
		
		if (playerGameObject == null)
		{
			return;
		}
		
		Transform modelTrans = playerGameObject.transform.FindChild("Model");
		if (modelTrans == null)
		{
			return;
		}
		
		//  Transform modelallTrans = modelTrans.FindChild("all");
		// if (modelallTrans == null)
		//  {
		//      return;
		//  }
		
		Transform weaponParent = modelTrans.FindChild(strWeaponPoint);
		if (weaponParent == null)
		{
			return;
		}
		
		foreach(Transform tr in weaponParent)
		{
			GameObject.Destroy(tr.gameObject);
		}
		
		Transform effectParent;
		//if (nProfession == (int)CharacterDefine.PROFESSION.XIAOYAO)
		// {
		//     effectParent = weaponParent.FindChild("Weapon_R");
		//    if (effectParent == null)
		//    {
		//        return;
		//    }
		// }
		// else
		// {
		effectParent = weaponParent;
		// }
		
		// 删除武器 逍遥职业特殊 武器节点和特效节点不是同一个
		for (int i = 0; i < weaponParent.childCount; ++i)
		{
			Transform child = weaponParent.GetChild(i);
			if (null != child && null != child.gameObject && child.gameObject.name != "EffectPoint" && child.gameObject.name != effectParent.name)
			{
				GameObject.Destroy(child.gameObject);
			}
		}
		
		Quaternion weaponRotation = weaponModel.transform.localRotation;
		weaponModel.transform.parent = weaponParent;
		weaponModel.transform.localPosition = Vector3.zero;
		
		//Tab_WeaponModel weapModelForScale = TableManager.GetWeaponModelByID (m_MainPlayer.CurWeaponDataID,0);
		//Vector3 scaleV3 = weapModelForScale == null ? Vector3.one :  new Vector3(weapModelForScale.Scale,weapModelForScale.Scale,weapModelForScale.Scale);
		
		weaponModel.transform.localScale =Vector3.one; //scaleV3; //Vector3.one;
		weaponModel.transform.localRotation = weaponRotation;
		
		Transform[] trans = weaponModel.GetComponentsInChildren<Transform>();
		for (int i = 0; i < trans.Length; ++i)
		{
			trans[i].gameObject.layer = playerGameObject.layer;
		}
		
		// 加载武器宝石特效
		EffectLogic effectLogic = effectParent.gameObject.GetComponent<EffectLogic>();
		if (null == effectLogic)
		{
			effectLogic = effectParent.gameObject.AddComponent<EffectLogic>();
			effectLogic.InitEffect(effectParent.gameObject);
		}
		if (null != effectLogic)
		{
			Tab_GemAttr tabGemAttr = TableManager.GetGemAttrByID(nWeaponEffectGem, 0);
			if (tabGemAttr != null)
			{
				Tab_Effect tabEffect = TableManager.GetEffectByID(tabGemAttr.EffectID, 0);
				if (tabEffect != null)
				{
					effectLogic.PlayEffect(tabGemAttr.EffectID,null,weaponModel);
				}
			}
		}
		
		// 加载坐骑 按时装->武器->坐骑加载玩家模型
		if (MountObj == null)
		{
			Obj_OtherPlayer otherPlayer = objRoot.GetComponent<Obj_OtherPlayer>();
			if (otherPlayer != null)
			{
				int nMountID = -1;
				if (otherPlayer.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER
				    && otherPlayer.MountID > 0)
				{
					nMountID = otherPlayer.MountID;
				}
				else if (otherPlayer.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER)
				{
					Obj_MainPlayer mPlayer = otherPlayer as Obj_MainPlayer;
					if (mPlayer && mPlayer.GetEquipMountID() > 0)
					{
						nMountID = mPlayer.GetEquipMountID();
					}
				}
				otherPlayer.RideOrUnMount(nMountID);
				Tab_CharModel  charmodle=TableManager.GetCharModelByID(otherPlayer.ModelID,0);
				//trans init bind to obj_charactor
				GameObject Model=otherPlayer.transform.FindChild("Model").gameObject;
				if(nMountID>0)
				{
				GameObject  rid=otherPlayer.transform.GetComponentInChildren<Obj_Mount>().gameObject;
				if(rid!=null)
				{
			
					Model=rid.transform.FindChild("Model").gameObject;

				}
				}
				//缩放mainplayer中毒model，不能直接缩放mainplayer。这样会影响特效
				if(Model!=null)
					Model.transform.localScale=new Vector3(charmodle.Scale,charmodle.Scale,charmodle.Scale);
			}
		}
		//武器重新加载之后 初始化武器的材质信息
		Obj _obj = objRoot.GetComponent<Obj>();
		if (_obj!=null)
		{
			_obj.InitWeaponMaterialInfo();
		}
		
	}
	
	
	public void OtherView_AsycReloadWeaponOver(string modelName, GameObject resObj, object param1, object param2, object param3)
	{
		if (param1 == null || param2 == null)
		{
			return;
		}
		GameObject objRoot = (GameObject)param1;
		List<object> Params = (List<object>)param2;
		
		if (null == objRoot || null == Params || Params.Count < 3)
		{
			return;
		}
		
		string strWeaponPoint = (string)Params[0];
		int nWeaponEffectGem = (int)Params[1];
		Obj_Mount MountObj = (Obj_Mount)Params[2];
		
		GameObject weaponModel = (GameObject)GameObject.Instantiate(resObj);
		if (null == weaponModel)
		{
			return;
		}
		
		GameObject playerGameObject = null;
		// 根据是否在坐骑上 确定玩家模型所在位置
		if (MountObj == null)
		{
			playerGameObject = objRoot;
		}
		else
		{
			if (MountObj.MountPlayer == null)
			{
				return;
			}
			playerGameObject = MountObj.MountPlayer.gameObject;
		}
		
		Transform modelTrans = playerGameObject.transform.FindChild("Model");
		if (modelTrans == null)
		{
			return;
		}
		
		//Transform modelallTrans = modelTrans.FindChild("all");
		// if (modelallTrans == null)
		//{
		//    return;
		//}
		
		Transform weaponParent = modelTrans.FindChild(strWeaponPoint);//modelallTrans
		if (weaponParent == null)
		{
			return;
		}
		
		foreach(Transform tr in weaponParent)
		{
			GameObject.Destroy(tr.gameObject);
		}
		
		for (int i = 0; i < weaponParent.childCount; ++i)
		{
			Transform child = weaponParent.GetChild(i);
			if (null != child && null != child.gameObject && child.gameObject.name != "EffectPoint")
			{
				GameObject.Destroy(child.gameObject);
			}
		}
		
		Quaternion weaponRotation = weaponModel.transform.localRotation;
		weaponModel.transform.parent = weaponParent;
		weaponModel.transform.localPosition = Vector3.zero;
		//Tab_WeaponModel weapModelForScale = TableManager.GetWeaponModelByID (m_MainPlayer.CurWeaponDataID,0);
		//Vector3 scaleV3 = weapModelForScale == null ? Vector3.one :  new Vector3(weapModelForScale.Scale,weapModelForScale.Scale,weapModelForScale.Scale);
		
		//weaponModel.transform.localScale = scaleV3;
		weaponModel.transform.localScale = Vector3.one;
		weaponModel.transform.localRotation = weaponRotation;
		
		Transform[] trans = weaponModel.GetComponentsInChildren<Transform>();
		for (int i = 0; i < trans.Length; ++i)
		{
			trans[i].gameObject.layer = playerGameObject.layer;
		}
		
		// 加载武器宝石特效
		EffectLogic effectLogic = weaponParent.gameObject.GetComponent<EffectLogic>();
		if (null == effectLogic)
		{
			effectLogic = weaponParent.gameObject.AddComponent<EffectLogic>();
			effectLogic.InitEffect(weaponParent.gameObject);
		}
		if (null != effectLogic)
		{
			Tab_GemAttr tabGemAttr = TableManager.GetGemAttrByID(nWeaponEffectGem, 0);
			if (tabGemAttr != null)
			{
				Tab_Effect tabEffect = TableManager.GetEffectByID(tabGemAttr.EffectID, 0);
				if (tabEffect != null)
				{
					effectLogic.PlayEffect(tabGemAttr.EffectID,null,weaponModel);
				}
			}
		}
	}
	
	//将一个加载好的Model加入节点
	public bool ReloadModel(GameObject objRoot, GameObject objModel, string animationPath = null, Obj_Mount MountObj = null)
	{
		//判断参数合法性
		if (null == objRoot)
		{
			return false;
		}
		
		GameObject playerGameObject = null;
		// 根据是否在坐骑上 确定玩家模型所在位置
		if (MountObj == null)
		{
			playerGameObject = objRoot;
		}
		else
		{
			if (null != MountObj.MountPlayer)
			{
				playerGameObject = MountObj.MountPlayer.gameObject;
			}
			else
			{
				return false;
			}
		}
		
		//如果发现已经有Model节点，则删除之
		Transform modelTransform = playerGameObject.transform.FindChild("Model");
		if (null != modelTransform)
		{
			GameObject.DestroyImmediate(modelTransform.gameObject);
		}
		
		//创建新的Model节点并挂到objRoot
		if (objModel)
		{
			objModel.transform.parent = playerGameObject.transform;
			objModel.transform.localPosition = Vector3.zero;
			objModel.transform.localRotation = Quaternion.identity;
			objModel.transform.localScale = Vector3.one;
			
			//设置模型layer的和root一致
			Transform[] trans = objModel.GetComponentsInChildren<Transform>();
			for (int i = 0; i < trans.Length; ++i)
			{
				trans[i].gameObject.layer = playerGameObject.layer;
			}
			
			Obj_Character curChar = objRoot.GetComponent<Obj_Character>();
			if (null != curChar)
			{
				if (MountObj == null)
				{
					if (null != animationPath)
					{
						curChar.AnimationFilePath = animationPath;
					}
					
					curChar.InitAnimation();

					//初始化缓存的材质信息 坐骑加载不需要重置
					Tab_RoleBaseAttr tab = TableManager.GetRoleBaseAttrByID (curChar.BaseAttr.RoleBaseID, 0);
					if ((tab!=null)&&(tab.IsZA == 1)) 
					{
					}
					else
					{
						curChar.InitMaterialInfo();
					}
				}
				else
				{
					if (MountObj.MountPlayer)
					{
						MountObj.MountPlayer.AnimationFilePath = animationPath;
						MountObj.MountPlayer.InitAnimation();
					}
				}
				
				curChar.OnSwithObjAnimState(curChar.CurObjAnimState);
				curChar.OnReloadModle();
				
				curChar.ModelNode = objModel;
			}           
		}
		
		return true;
	}
	
	#if UNITY_ANDROID
	
	private Dictionary<string, Obj_Init_Data> m_ShowingNPCList = new Dictionary<string, Obj_Init_Data>();
	private void ShowingNPCList(Obj_Init_Data initData)
	{
		if (m_ShowingNPCList == null)
		{
			m_ShowingNPCList = new Dictionary<string, Obj_Init_Data>();
		}
		
		if (!m_ShowingNPCList.ContainsKey(initData.m_ServerID.ToString()))
		{
			m_ShowingNPCList.Add(initData.m_ServerID.ToString(), initData);
		}
	}
	
	public void ShowNPC()
	{
		if (m_ShowingNPCList == null) return;
		if (m_ShowingNPCList.Count <= 0) return;
		
		int iShowTag = 1;
		int iTag = 0;
		string[] strNPCName = new string[iShowTag];
		foreach (KeyValuePair<string, Obj_Init_Data> keyValuePair in m_ShowingNPCList)
		{
			if (keyValuePair.Value != null)
			{
				CreateNPC(keyValuePair.Value);
			}
			strNPCName.SetValue(keyValuePair.Key, iTag);
			
			iTag++;
			if (iTag >= iShowTag) break;
		}
		
		for (int i = 0; i < strNPCName.Length; i++)
		{
			if (string.IsNullOrEmpty(strNPCName[i])) continue;
			
			if (m_ShowingNPCList.ContainsKey(strNPCName[i]))
			{
				m_ShowingNPCList[strNPCName[i]] = null;
				m_ShowingNPCList.Remove(strNPCName[i]);
			}
		}
		
		strNPCName = null;
	}
	
	#endif
    public event System.Action<Obj_Init_Data> CreateCharacterCallBack;
	//根据类型创建非主角玩家
	public void NewCharacterObj(GameDefine_Globe.OBJ_TYPE type, Obj_Init_Data initData)
	{
		switch (type)
		{
		case GameDefine_Globe.OBJ_TYPE.OBJ_NPC:
		{
			#if UNITY_ANDROID
			ShowingNPCList(initData);
			#else
			CreateNPC(initData);
			#endif
		}
			break;
		case GameDefine_Globe.OBJ_TYPE.OBJ_FELLOW:
		{
			CreateFellow(initData);
		}
			break;
		case GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER:
		{
			CreateOtherPlayer(initData);
		}
			break;
		case GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER:
		{
			CreateZombieUser(initData);
		}
			break;;
		default:
			break;
		}
	}
	
	
	public void NewDropObj(GameDefine_Globe.OBJ_TYPE type, Obj_DroopItemData initData)
	{
		switch (type)
		{
		case GameDefine_Globe.OBJ_TYPE.OBJ_DROP_ITEM:
		{
			CreateDropItem(initData);
		}
			break;
		default:
			break;
		}
		
	}
	//根据ServerID来查找Obj
	public Obj FindObjInScene(int nServerID)
	{
		if (m_ObjPools.ContainsKey(nServerID.ToString()))
		{
			return m_ObjPools[nServerID.ToString()];
		}
		
		return null;
	}
	
	//根据ServerID来查找Obj_Character
	public Obj_Character FindObjCharacterInScene(int nServerID)
	{
		if (nServerID < 0)
		{
			return null;
		}
		
		if (m_ObjPools.ContainsKey(nServerID.ToString()))
		{
			
			return m_ObjPools[nServerID.ToString()] as Obj_Character;
		}
		
		return null;
	}
    public List<Obj_Character> FindCharacterByName(string name)  
    {
        List<Obj_Character> lstchar = new List<Obj_Character>();
        foreach (KeyValuePair<string, Obj> objitem in m_ObjPools) 
        {
            if (objitem.Value is Obj_Character) 
            {
                Obj_Character character = objitem.Value as Obj_Character;
                if (character.BaseAttr.RoleName == name)
                {
                    lstchar.Add(character);
                }
            }
        }
        return lstchar;
    }
	//根据Obj_Character的BaseAttr中的名字来查找NPC
	//遍历，不推荐反复使用
	public Obj_Character FindObjCharacterInSceneByName(string szBaseAttrName, bool bIsAlive = true)
	{
		Obj_Character objTarget = null;
		float minDistance = 8f;
		
		foreach (KeyValuePair<string, Obj> objPair in m_ObjPools)
		{
			Obj_Character objChar = objPair.Value as Obj_Character;
			if (objChar && objChar.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC && objChar.BaseAttr.RoleName == szBaseAttrName)
			{
				//是否要寻找非死亡目标
				if (bIsAlive && objChar.IsDie())
				{
					continue;
				}
				
				CharacterDefine.REPUTATION_TYPE nType = Reputation.GetObjReputionType(objChar);
				if (nType == CharacterDefine.REPUTATION_TYPE.REPUTATION_INVALID
				    || nType == CharacterDefine.REPUTATION_TYPE.REPUTATION_FRIEND)
				{
					return objChar;
				}
				else
				{
					Vector3 UserPos = Singleton<ObjManager>.GetInstance().MainPlayer.Position;
					float distance = Mathf.Abs(Vector3.Distance(UserPos, objChar.Position));
					if (distance - minDistance <= 0)
					{
						minDistance = distance;
						objTarget = objChar;
					}
				}
			}
		}
		
		return objTarget;
	}
	
	//查找某个玩家
	public Obj_OtherPlayer FindOtherPlayerInScene(UInt64 guid)
	{
		if (guid == GlobeVar.INVALID_GUID)
		{
			return null;
		}
		
		foreach (KeyValuePair<string, Obj> objPair in m_ObjPools)
		{
			if (objPair.Value.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
			{
				Obj_OtherPlayer objPlayer = objPair.Value as Obj_OtherPlayer;
				if (null != objPlayer && objPlayer.GUID == guid)
				{
					return objPlayer;
				}
			}
		}
		
		return null;
	}
	
	//判断场景中某个Obj是否存在
	public bool IsObjExist(int nServerID)
	{
		return m_ObjPools.ContainsKey(nServerID.ToString());
	}
	
	int [] GuildBossId = new int[3]{50001, 50002, 50003};
	public bool IsGuildBoss(int nDataID)
	{
		for (int i=0; i < GuildBossId.Length; i++)
		{
			if(GuildBossId[i] == nDataID)
				return true;
		}
		return false;
	}
	
	//根据ServerID删除场景中的Obj
	public bool RemoveObj(int nServerID)
	{
		Obj obj = FindObjInScene(nServerID);
		if (null != obj)
		{
			ReomoveObjInScene(obj.gameObject);
			return true;
		}
		
		return false;
	}
	
	
	/// <summary>
	/// 删除NPC缓存中对象
	/// </summary>
	/// <param name="isInitiative">是否主动调用删除</param>
	public void DeleteNPCGameObject()
	{
		if (!m_IsUseAndroid) return;
		
		if (m_DeleteNPCList == null) return;
		
		List<string> temp = new List<string>();
		int deleteTag = 0;
		foreach (KeyValuePair<string, float> deleteNpc in m_DeleteNPCList)
		{
			if (deleteTag >= 8) break;
			
			if (null != m_NPCGameObjectList && m_NPCGameObjectList.ContainsKey(deleteNpc.Key))
			{
				ReomoveObjInScene(m_NPCGameObjectList[deleteNpc.Key], true);
				m_NPCGameObjectList[deleteNpc.Key] = null;
				m_NPCGameObjectList.Remove(deleteNpc.Key);
			}
			
			temp.Add(deleteNpc.Key);
			deleteTag++;
			
		}
		
		for (int i = 0; i < temp.Count; i++)
		{
			m_DeleteNPCList.Remove(temp[i]);
		}
		
		temp.Clear();
		temp = null;
	}
	
	//删除场景中的Obj
	public bool ReomoveObjInScene(GameObject removeObject, bool isDelete = false)
	{
		if (null == removeObject)
		{
			return false;
		}
		
		Obj tempObj = removeObject.GetComponent<Obj>();
		if (tempObj)
		{
			if (m_IsUseAndroid && !isDelete)
			{
				Obj_NPC npc = removeObject.GetComponent<Obj_NPC>();
				if (npc != null)
				{
					if (null != m_NPCGameObjectList && m_NPCGameObjectList.ContainsKey(removeObject.name))
					{
						m_NPCGameObjectList[removeObject.name].SetActive(false);
						
						if (m_DeleteNPCList == null)
						{
							m_DeleteNPCList = new Dictionary<string, float>();
						}
						
						if (!m_DeleteNPCList.ContainsKey(removeObject.name))
						{
							m_DeleteNPCList.Add(removeObject.name, Time.time);
						}
						
						npc.StopNPCEffect();
						
						npc = null;
						removeObject = null;
						return true;
					}
				}
			}
			
			#if UNITY_ANDROID
			//删除延迟缓存中的NPC列表信息
			if (m_ShowingNPCList != null)
			{
				if (m_ShowingNPCList.ContainsKey(tempObj.ServerID.ToString()))
				{
					m_ShowingNPCList.Remove(tempObj.ServerID.ToString());
				}
			}
			#endif
			
			m_ObjPools.Remove(tempObj.ServerID.ToString());
			if(m_MonsterList.ContainsKey(tempObj.ServerID.ToString()))
				m_MonsterList.Remove(tempObj.ServerID.ToString());
			for (int i = 0; i < m_ObjOtherPlayerHideList.Count; ++i)
			{
				if (m_ObjOtherPlayerHideList[i].m_serverID == tempObj.ServerID.ToString())
				{
					m_ObjOtherPlayerHideList.Remove(m_ObjOtherPlayerHideList[i]);
					break;
				}
			}
			
			for (int i = 0; i < m_ObjOtherPlayerShowList.Count; ++i)
			{
				if (m_ObjOtherPlayerShowList[i].m_serverID == tempObj.ServerID.ToString())
				{
					m_ObjOtherPlayerShowList.Remove(m_ObjOtherPlayerShowList[i]);
					break;
				}
			}
			
			UpdateHidePlayers();
			
			//删除名字版
			Obj_Character tempObjCharacter = tempObj as Obj_Character;
			if (tempObjCharacter)
			{
				ResourceManager.UnLoadHeadInfoPrefab(tempObjCharacter.HeadInfoBoard);
			}
			ResourceManager.DestroyResource(ref removeObject);
			return true;
		}
		
		return false;
	}
	
	//同步场景中的Obj位置
	public void SyncObjectPosition(int nServerId, int nPosX, int nPosZ)
	{
		//如果是自己则不进行同步
		if (Singleton<ObjManager>.GetInstance().MainPlayer.ServerID == nServerId)
		{
			return;
		}
		float fPosX = ((float)nPosX) / 100;
		float fPosZ = ((float)nPosZ) / 100;
		Obj_Character obj = FindObjCharacterInScene(nServerId);
		if (null != obj)
		{
			Vector3 pos = new Vector3(fPosX, obj.gameObject.transform.position.y, fPosZ);
			//            if (GameManager.gameManager.ActiveScene.IsT4MScene())
			//            {
			//				pos.y = GameManager.gameManager.ActiveScene.GetTerrainHeight(pos);
			//            }
			//            else if (null != Terrain.activeTerrain)
			//            {
			//				pos.y = Terrain.activeTerrain.SampleHeight(pos);
			//            }
			
			//校验，如果发现距离相差太远，则直接拉过去
			if (Vector3.Distance(pos, obj.Position) > 5.0f || obj.ObjType !=GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
			{
				obj.Position = pos;
			}
			else
			{
				obj.MoveTo(pos, null, 0.5f);
			}
		}
	}
	
	
	private void AddPoolOtherGameObj(string name, GameObject gObj)
	{
		try
		{
			m_OtherGameObjPools.Add(name, gObj);
		}
		catch (System.Exception ex)
		{
			LogModule.ErrorLog("ObjManager AddPoolOtherGameObj(" + name + "," + "Error: " + ex.Message);
		}
	}
	
	//根据Gameobject的Name在非Obj池查找
	public GameObject FindOtherGameObj(string strName)
	{
		if (m_OtherGameObjPools.ContainsKey(strName))
		{
			return m_OtherGameObjPools[strName];
		}
		
		return null;
	}
	
	//删除场景中非obj池 中的GameObject
	public bool RemoveOtherGameObj(string strName)
	{
		GameObject removeObject = FindOtherGameObj(strName);
		if (null == removeObject)
		{
			return false;
		}
		
		if (m_OtherGameObjPools.Remove(removeObject.name))
		{
			ResourceManager.DestroyResource(ref removeObject);
			return true;
		}
		return false;
	}
	
	// 根据屏蔽规则判断隐藏的玩家是否可以显示
	public void UpdateHidePlayers()
	{
		if (m_ObjOtherPlayerShowList.Count > PlayerPreferenceData.SystemShowOtherPlayerCount)
		{
			// 当前显示的已经超过预期，将多余的隐藏
			int canHideCount =  m_ObjOtherPlayerShowList.Count - PlayerPreferenceData.SystemShowOtherPlayerCount;
			
			canHideCount = canHideCount > m_ObjOtherPlayerShowList.Count ? m_ObjOtherPlayerShowList.Count : canHideCount;
			
			for (int i = 0; i < canHideCount; i++)
			{
				// 排在显示队列末尾的拥有比较高的显示优先级
				Obj_HidePlayerData curObjData = m_ObjOtherPlayerShowList[m_ObjOtherPlayerShowList.Count - 1];
				m_ObjOtherPlayerShowList.RemoveAt(m_ObjOtherPlayerShowList.Count - 1);
				
				if (m_ObjPools.ContainsKey(curObjData.m_serverID))
				{
					Obj_OtherPlayer curChar = m_ObjPools[curObjData.m_serverID] as Obj_OtherPlayer;
					m_ObjOtherPlayerHideList.Add(curObjData);
					if (null != curChar)
					{
						curChar.SetVisible(false);
					}
					
				}
			}
		}
		else
		{
			// 当前显示不足预期，放开显示
			int canShowCount = PlayerPreferenceData.SystemShowOtherPlayerCount - m_ObjOtherPlayerShowList.Count;
			
			canShowCount = canShowCount > m_ObjOtherPlayerHideList.Count ? m_ObjOtherPlayerHideList.Count : canShowCount;
			for (int i = 0; i < canShowCount; i++)
			{
				Obj_HidePlayerData curObjData = m_ObjOtherPlayerHideList[0];
				m_ObjOtherPlayerHideList.RemoveAt(0);
				
				if (m_ObjPools.ContainsKey(curObjData.m_serverID))
				{
					Obj_OtherPlayer curChar = m_ObjPools[curObjData.m_serverID] as Obj_OtherPlayer;
					m_ObjOtherPlayerShowList.Add(curObjData);
					if (null != curChar)
					{
						curChar.SetVisible(true);
						//#if UNITY_ANDROID
						//判断是否已经下载模型。如果没加载模型，加载模型
						Transform childTrans = curChar.transform.FindChild("Model");
						if (childTrans == null)
						{
							ReloadModel(curChar.gameObject, curObjData.ResPath, AsycCreateOtherPlayerOver, curObjData.InitData);
						}
						else
						{
							curChar.RideOrUnMount(curChar.MountID);
						}
						childTrans = null;
						//#endif
					}
					
				}
			}
			
		}
	}
	
	public void TestOtherPlayerVisible(Obj_OtherPlayer curPlayer)
	{
		m_ObjOtherPlayerShowList.Add(new Obj_HidePlayerData(curPlayer.ServerID.ToString(), curPlayer.GetVisibleValue()));
		UpdateHidePlayers();
	}
	
	
	public static void AddOutLineMaterial(GameObject parentObj)
	{
		if (PlayerPreferenceData.SystemWallVisionEnable == false)
		{
			return;
		}
		foreach (SkinnedMeshRenderer curMeshRender in parentObj.GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			Material[] newMaterialArray = new Material[curMeshRender.materials.Length+1];
			for (int i = 0; i < curMeshRender.materials.Length; i++)
			{
				if (curMeshRender.materials[i].name.Contains("PlayerXplay"))
				{
					return;
				}
				else
				{
					newMaterialArray[i] = curMeshRender.materials[i];
				}
			}
			
			UnityEngine.Object playerXPlayObj = Resources.Load("Material/PlayerXplay");
			if (null != playerXPlayObj)
			{
				newMaterialArray[curMeshRender.materials.Length] = GameObject.Instantiate(playerXPlayObj) as Material;
			}
			//			UnityEngine.Object OutLineObj = Resources.Load("Material/OutLine");
			//			if (null != playerXPlayObj)
			//			{
			//				newMaterialArray[curMeshRender.materials.Length+1] = GameObject.Instantiate(OutLineObj) as Material;
			//				newMaterialArray[curMeshRender.materials.Length+1].mainTexture=curMeshRender.materials[curMeshRender.materials.Length-1].mainTexture;
			//			}
			
			curMeshRender.materials = newMaterialArray;
			
		}
	}
	
	public static void RemoveOutLineMaerial(GameObject parentObj)
	{
		foreach (SkinnedMeshRenderer curMeshRender in parentObj.GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			int newMaterialArrayCount = 0;
			for (int i = 0; i < curMeshRender.materials.Length; i++)
			{
				if (curMeshRender.materials[i].name.Contains("PlayerXplay"))
				{
					newMaterialArrayCount++;
				}
			}
			
			if (newMaterialArrayCount > 0)
			{
				Material[] newMaterialArray = new Material[newMaterialArrayCount];
				int curMaterialIndex = 0;
				for (int i = 0; i < curMeshRender.materials.Length; i++)
				{
					if (curMaterialIndex >= newMaterialArrayCount)
					{
						break;
					}
					if (!curMeshRender.materials[i].name.Contains("PlayerXplay"))
					{
						newMaterialArray[curMaterialIndex] = curMeshRender.materials[i];
						curMaterialIndex++;
					}
				}
				
				curMeshRender.materials = newMaterialArray;
			}
		}
	}
}
