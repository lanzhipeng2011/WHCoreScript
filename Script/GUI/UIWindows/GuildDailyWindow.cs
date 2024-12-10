using UnityEngine;
using System.Collections;
using Games.UserCommonData;
using GCGame.Table;

public class GuildDailyWindow : MonoBehaviour {

    public GameObject m_ActivityInfo;
    public UILabel m_ActivityDec;

    public GameObject m_Help;

    private static GuildDailyWindow m_Instance = null;
    public static GuildDailyWindow Instance()
    {
        return m_Instance;
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        m_Instance = this;
        UpdateActivityInfo();
    }

    void OnDisable()
    {
        m_Instance = null;
    }

    public void UpdateActivityInfo()
    {
        bool bFlag = GameManager.gameManager.PlayerDataPool.CommonData.GetCommondFlag((int)USER_COMMONFLAG.CF_GUILDACTIVITY_FLAG);
        if (bFlag == true)
        {
            //m_Help.SetActive(false);
            m_ActivityInfo.SetActive(true);
//            GuildActivityBossData BossData = GameManager.gameManager.PlayerDataPool.BossData;
            
//             int nSceneClassID = BossData.SceneClassID;
//             Tab_SceneClass SceneTab = TableManager.GetSceneClassByID(nSceneClassID,0);
//             if (SceneTab == null)
//            {
//                 return;
//            }
// 
//             int nSceneInstenceID = BossData.SceneInstanceID;
//             int nPosX = (int)BossData.PosX;
//             int nPosZ = (int)BossData.PosZ;
//             m_ActivityDec.text = StrDictionary.GetClientDictionaryString("#{3217}", SceneTab.Name, nSceneInstenceID, nPosX, nPosZ);
        }
        else
        {
            m_ActivityDec.text = "";
            m_ActivityInfo.SetActive(false);
            //m_Help.SetActive(true);
        }
    }

    void OnButtonGoClick()
    {
        UIManager.CloseUI(UIInfo.Activity);

        GuildActivityBossData BossData = GameManager.gameManager.PlayerDataPool.BossData;
        int nSceneClassID = BossData.SceneClassID;
        int nSceneInstenceID = BossData.SceneInstanceID;
        float fPosX = BossData.PosX;
        float fPosZ = BossData.PosZ;

        SceneData.RequestChangeScene((int)CG_REQ_CHANGE_SCENE.CHANGETYPE.POINT, 0, nSceneClassID, nSceneInstenceID, (int)(fPosX*100), (int)(fPosZ*100));
    }
}
