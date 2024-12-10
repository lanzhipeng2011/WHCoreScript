using UnityEngine;
using System.Collections;
using Games.LogicObj;

public class MissionBoard : MonoBehaviour {

    public enum MissionBoardState
    {
        MISSION_NONE,
        MISSION_CANACCEPTED,
        MISSION_ACCEPTED,
        MISSION_CANCOMPLETED,
        MISSION_COMPLETED
    }

    private Obj_NPC m_MissionNpc;
    private float m_MissionBoadTime;    // 任务提醒版刷新时间
    private string m_MissionCanAcceptedSprit = "TanHaoYellow";
    //private string m_MissionCanNotAcceptedSprit = "TanHaoGrey";
    private string m_MissionCanCompletedSprit = "WenHaoGrey";
    private string m_MissionCompletedSprit = "WenHaoYellow";

	private BillBoard m_curBillBoard; 
	// Use this for initialization
	void Start () {
        init();
	}


    void FixedUpdate()
    {
        UpdateMissionBoard();
    }

    void init()
    {
        m_MissionBoadTime = 0;
        this.gameObject.GetComponent<UISprite>().spriteName = "";

		m_curBillBoard = this.gameObject.transform.parent.gameObject.GetComponent<BillBoard>();
		if (m_curBillBoard && m_curBillBoard.BindObj != null)
        {
            m_MissionNpc = m_curBillBoard.BindObj.GetComponent<Obj_NPC>();
        }
    }

    void UpdateMissionBoard()
    {
        m_MissionBoadTime += Time.deltaTime;
        if (m_MissionBoadTime < 1)  // 一秒刷新一次
        {
            return;
        }
        m_MissionBoadTime = 0;


        // 无NPC
        if (m_MissionNpc == null)
        {
            if (null != gameObject.transform.parent)
            {
                BillBoard billBaoard = gameObject.transform.parent.gameObject.GetComponent<BillBoard>();
                if (billBaoard)
                {
                    if (billBaoard.BindObj != null)
                    {
                        m_MissionNpc = billBaoard.BindObj.GetComponent<Obj_NPC>();
                    }
                }
            }

            if (m_MissionNpc == null)
            {
                return;
            }
        }

        // 已有接受的任务
        MissionBoardState boardState = GameManager.gameManager.MissionManager.GetMissionBoardState(m_MissionNpc);
        bool bRet = false;
        switch (boardState)
        {
            case MissionBoardState.MISSION_ACCEPTED:
                {
                    //this.gameObject.GetComponent<UISprite>().spriteName = m_MissionAcceptedSprit;
                    //bRet = true;
                 }
                 break;
            case MissionBoardState.MISSION_CANCOMPLETED:
                {
                    this.gameObject.GetComponent<UISprite>().spriteName = m_MissionCanCompletedSprit;
                    bRet = true;
                } 
                 break;
            case MissionBoardState.MISSION_COMPLETED:
                {
                    this.gameObject.GetComponent<UISprite>().spriteName = m_MissionCompletedSprit;
                    bRet = true;
                }
                 break;
            default:
                break;
        }
        if (bRet == true)
        {
            gameObject.SetActive(true);
           return;
        }

        // 有任务可接
        if (GameManager.gameManager.MissionManager.IsHaveMissionAccepted(m_MissionNpc))
        {
            this.gameObject.GetComponent<UISprite>().spriteName = m_MissionCanAcceptedSprit;
            gameObject.SetActive(true);
            return;
        }

        // 没有就清空
        this.gameObject.GetComponent<UISprite>().spriteName = "";
    }
}
