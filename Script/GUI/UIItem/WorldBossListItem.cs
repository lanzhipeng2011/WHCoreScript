using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;
using Games.GlobeDefine;

public class WorldBossListItem : MonoBehaviour
{
    public UILabel LabelName;
    public UILabel LabelCd;
    public UILabel LabelScore;
    private WorldBossWindow m_parent;
    private HuaShanPVPData.WorldBossTeamInfo m_data;
    public UISlider m_ProgressSlider;

    public GameObject m_fightButton;
    public int CountdownCD { set; get; }
    private float m_fFrameTimeDiff;

    // Use this for initialization
    void Start()
    {
		InvokeRepeating ("CountdownCDTime", 1.0f, 1.0f);
    }
	
    void CountdownCDTime()
    {
        if (CountdownCD > 0)
        {
            LabelCd.text = ""; //TT7973 StrDictionary.GetClientDictionaryString("#{1994}", CountdownCD);
	        CountdownCD--;
            
            if (CountdownCD == 0)
            {
                bool active = IsCanActiveFightButton();
                m_fightButton.SetActive(active);
                LabelCd.text = ""; //TT7973 Utils.GetDicByID(1993);
            }
        }
    }

    private bool IsCanActiveFightButton()
    {
        //TT7973
        return false;
//         if(GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID == GlobeVar.INVALID_ID)
//             return false;
// 
//         if (GameManager.gameManager.PlayerDataPool.TeamInfo.TeamID == m_data.id)
//             return false;
// 
//         if (Singleton<ObjManager>.GetInstance().MainPlayer.IsTeamLeader() == false)
//             return false;
// 
//         if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WORLDBOSS)
//         {
//             return false;
//         }
// 
//         return true;
    }

    public static WorldBossListItem CreateItem(GameObject grid, GameObject resItem, string name, WorldBossWindow parent, HuaShanPVPData.WorldBossTeamInfo data)
    {
        if( grid != null && resItem != null )
        {
            GameObject curItem = Utils.BindObjToParent(resItem, grid, name);
            if (null != curItem)
            {
                WorldBossListItem curItemComponent = curItem.GetComponent<WorldBossListItem>();
                if (null != curItemComponent)
                    curItemComponent.SetData(parent, data);

                return curItemComponent;
            }
        }
        
        return null;
    }

    public void SetData(WorldBossWindow parent, HuaShanPVPData.WorldBossTeamInfo data)
    {
        m_data = data;
        m_parent = parent;
        LabelName.text = StrDictionary.GetClientDictionaryString("#{2113}", data.name);
        CountdownCD = data.cd / 1000;
        LabelScore.text = data.score.ToString();
        m_ProgressSlider.value = data.per/10000.0f;

        if (CountdownCD == 0)
        {
            LabelCd.text = ""; //TT7973 Utils.GetDicByID(1993);
            m_fightButton.SetActive(IsCanActiveFightButton());
        }
        else
        {
            m_fightButton.SetActive(false);
            LabelCd.text = ""; //TT7973 StrDictionary.GetClientDictionaryString("#{1994}", CountdownCD);
        }
    }

    public void OnChallengeTeamButtonClick()
    {
        if (null != m_parent) m_parent.OnOpItemClick(this);
    }

    public HuaShanPVPData.WorldBossTeamInfo GetData()
    {
        return m_data;
    }

}
