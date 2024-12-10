/********************************************************************************
 *	文件名：MissionLogItem.cs
 *	全路径：	\Script\GUI\MissionLogItem.cs
 *	创建人：	贺文鹏
 *	创建时间：2014-02-17
 *
 *	功能说明： 任务日志界面 任务项UI。
 *	       
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.Mission;

public class MissionLogItem : MonoBehaviour {

    public UILabel m_MissionName;   // 任务名称
    public GameObject m_Active; // 点击状态

    private const byte MaxStateCount = 4;
    public UITexture[] m_MissionState = new UITexture[MaxStateCount];// 任务状态图

    private int m_MissionID;
    public int MissionID
    {
        get { return m_MissionID; }
        set { 
            m_MissionID = value;
            SetMissionLogItem();
        }
    }

	// Use this for initialization
	void Start () {

	}

    void CleanUpInfo()
    {
        for (int i = 0; i < 3; i++)
        {
            if (m_MissionState[i])
            {
                m_MissionState[i].gameObject.SetActive(false);
            }
        }

        if (m_MissionName)
        {
            m_MissionName.text = "";
        }

        if (m_Active)
        {
            m_Active.SetActive(false);
        }
    }

    // 点击 修改任务描述、任务奖励
    void ItemButtonClick()
    {
        SetChooseState(true);
        if (MissionLogLogic.Instance())
        {
            MissionLogLogic.Instance().UpdateMissionInfo(m_MissionID, gameObject);
        }
    }

    public void SetChooseState(bool bFlag)
    {
        if (m_Active)
        {
            m_Active.SetActive(bFlag);
        }
    }

    // 设置Item名字、状态UI
    void SetMissionLogItem()
    {
        if (m_MissionID < 0)
        {
            return;
        }

        CleanUpInfo();

        Tab_MissionDictionary MisDictionary = TableManager.GetMissionDictionaryByID(m_MissionID, 0);
        if (MisDictionary != null)
        {
            if (m_MissionName)
            {
                m_MissionName.text = string.Format(MisDictionary.MissionName,"","");
            }
        }

        // 状态
        byte byMisState = GameManager.gameManager.MissionManager.GetMissionState(m_MissionID);
        if (byMisState < MaxStateCount)
        {
            for (byte i = 0; i < MaxStateCount; i++ )
            {
                if (byMisState == i)
                {
                    m_MissionState[i].gameObject.SetActive(true);
                }
                else
                    m_MissionState[i].gameObject.SetActive(false);
            }
        }
    }
}
