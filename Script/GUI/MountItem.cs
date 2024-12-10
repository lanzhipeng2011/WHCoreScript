//********************************************************************
// 文件名: MountItem.cs
// 全路径：	\Script\GUI\MountItem.cs
// 描述: 坐骑界面 坐骑选项
// 作者: hewenpeng
// 创建时间: 2013-01-07
//********************************************************************
using UnityEngine;
using System.Collections;

public class MountItem : MonoBehaviour {

    public GameObject m_ItemActive;
    public GameObject m_ItemNormal;
    public UILabel m_ItemName;
    public void SetName(string strName)
    {
        if (m_ItemName)
        {
            m_ItemName.text = strName;
        }
    }
    public GameObject m_DisCollectFlag;
    public void SetCollectFlag(bool bFlag)
    {
        m_DisCollectFlag.SetActive(!bFlag);
    }

    private int m_MountID = -1;
    public int MountID
    {
        get { return m_MountID; }
        set { m_MountID = value; }
    }

    private static MountItem m_Instance = null;
    public static MountItem Instance()
    {
        return m_Instance;
    }

    void Awake()
    {
        m_Instance = this;
        m_ItemActive.SetActive(false);
        m_ItemNormal.SetActive(true);
        m_DisCollectFlag.SetActive(true);
    }

	// Use this for initialization
	void Start () {
	}

    public void MountItemClick()
    {
		if (null == gameObject)
			return;

        m_ItemActive.SetActive(true);
        m_ItemNormal.SetActive(false);

        if (null != PartnerAndMountLogic.Instance() &&
            null != PartnerAndMountLogic.Instance().m_MountRoot)
        {
            PartnerAndMountLogic.Instance().m_MountRoot.MountItemClick(m_MountID);            
        }
        
	}

    public void DisableItemUI()
    {
        m_ItemActive.SetActive(false);
        m_ItemNormal.SetActive(true);
    }

    void AutoFlagButton(GameObject value)
    {
        if (null == gameObject)
            return;
        
        MarkAutoMountFlag(m_MountID);
    }

    // 通知服务器修改标记
    void MarkAutoMountFlag(int nMountID)
    {
        CG_MOUNT_MARK packet = (CG_MOUNT_MARK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_MOUNT_MARK);
        packet.SetMountID(nMountID);
        packet.SendPacket();
    }
}
