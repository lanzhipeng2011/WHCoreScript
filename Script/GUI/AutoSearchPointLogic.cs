using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;

public class AutoSearchPointLogic : MonoBehaviour 
{
    public UILabel m_LabelLeft;
    public UILabel m_LabelRight;

    //该自动寻路点的基础信息
    private int m_StartSceneId;
    public int StartSceneId
    {
        get { return m_StartSceneId; }
    }

    private int m_EndSceneId;
    public int EndSceneId
    {
        get { return m_EndSceneId; }
        set { m_EndSceneId = value; }
    }

    private float m_fPosX;
    public float PosX
    {
        get { return m_fPosX; }
    }
    private float m_fPosZ;
    public float PosZ
    {
        get { return m_fPosZ; }
    }


    public void CreateAutoSearchPointInfo(Tab_AutoSearch autoSearchInfo)
    {
        SetLabelText(autoSearchInfo.TargetJob, autoSearchInfo.TargetName, autoSearchInfo.Color);
        m_StartSceneId = autoSearchInfo.Id;
        m_EndSceneId = autoSearchInfo.DstSceneID;
        m_fPosX = autoSearchInfo.X;
        m_fPosZ = autoSearchInfo.Z;
    }
    
	// Use this for initialization
	void Start () 
    {	
	}
	

    public void SetLabelText(string labelLeft, string labelRight, string color)
    {
        m_LabelLeft.color = Utils.GetColorByString(color);
        m_LabelLeft.text = labelLeft;
        m_LabelRight.color = Utils.GetColorByString(color);
        m_LabelRight.text = labelRight;
    }

    void TransmitPointOnClicked(GameObject value)
    {
        //根据记录的点进行寻路
        AutoSearchPoint point = new AutoSearchPoint(m_EndSceneId, m_fPosX, m_fPosZ);
        if (GameManager.gameManager && GameManager.gameManager.AutoSearch)
        {
            GameManager.gameManager.AutoSearch.BuildPath(point);
            //如果成功，则设置目标名字等信息
            if (null != GameManager.gameManager.AutoSearch &&
                true == GameManager.gameManager.AutoSearch.IsAutoSearching)
            {
                GameManager.gameManager.AutoSearch.Path.AutoSearchTargetName = m_LabelRight.text;

                if (null != SceneMapLogic.Instance())
                    SceneMapLogic.Instance().CloseWindow();
            }            
        }
    }
}
