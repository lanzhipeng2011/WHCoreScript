using UnityEngine;
using System.Collections;
using GCGame.Table;
using Module.Log;
using GCGame;

public enum RewardTargetType
{
    TARGETTYPE_SHARE_USER,
    TARGETTYPE_JOIN_USER,
}

public enum ShareType
{
    ShareType_Invalid   = -1,
    ShareType_SNS       = 0,        //SNS 
    ShareType_NanGua    = 1,        //南瓜
    ShareType_Num,       
}

public enum OpenType
{
    OpenType_Invalid = -1,
    OpenType_Share,
    OpenType_ActiviteCode,
}

public class ShareRootWindow : MonoBehaviour
{
   
    public ShareRewardItem[] m_JoinRewardItem;
    public ShareRewardItem[] m_ShareRewardItem;
    public UILabel m_labelTimes;
    public UILabel m_inputCode;
    public UILabel m_labelDesc;

    public GameObject m_ActiviteGameObject;
    public GameObject m_ShareGameObject;

    private ShareType m_nShareType = ShareType.ShareType_Invalid;
    private OpenType m_nOpenType = OpenType.OpenType_Invalid;

    const int REWARD_ITEMCOUNT_MAX = 5;
    

    private static ShareRootWindow m_Instance = null;
    public static ShareRootWindow Instance()
    {
        return m_Instance;
    }

    
    private static ShareType m_sShareType;
    private static OpenType m_sOpenType; 
    public static void ShowShareWindow(ShareType nShareType, OpenType nOpenType)
    {
        m_sShareType = nShareType;
        m_sOpenType = nOpenType;
        UIManager.ShowUI(UIInfo.ShareRoot, OnLoadShareWindow);
    }

    private static void OnLoadShareWindow(bool bSuccess, object param)
    {
        if (!bSuccess)
        {
            LogModule.ErrorLog("load ShareRoot Window error");
            return;
        }
        ShareRootWindow.Instance().SetShareType(m_sShareType, m_sOpenType);
    }

    void Awake()
    {
        m_Instance = this;
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    void ClearUp()
    {
        m_nShareType = ShareType.ShareType_Invalid;
        m_nOpenType = OpenType.OpenType_Invalid;

        for (int i = 0; i < m_JoinRewardItem.Length; i++)
        {
            m_JoinRewardItem[i].ClearInfo();
        }
        for (int i = 0; i < m_ShareRewardItem.Length; i++)
        {
            m_ShareRewardItem[i].ClearInfo();
        }
        m_labelDesc.text = "";
        m_ActiviteGameObject.SetActive(false);
        m_ShareGameObject.SetActive(false);
    }

    void SetShareType(ShareType nShareType, OpenType nOpenType)
    {
        if (PlayerFrameLogic.Instance() != null)
        {
            PlayerFrameLogic.Instance().PlayerFrameHeadOnClick();
        }
        ClearUp();
        m_nShareType = nShareType;
        m_nOpenType = nOpenType;

        int JoinSet = 0, shareSet = 0;
        for (int i = 0; i < REWARD_ITEMCOUNT_MAX; i++ )
        {
            Tab_ShareReward reward = TableManager.GetShareRewardByID((int)m_nShareType, 0);
            if (null == reward)
            {
                continue;
            }
            int nTargetType = reward.GetTargetTypebyIndex(i);
            int nItemID = reward.GetItemDataIdbyIndex(i);
            if ((int)RewardTargetType.TARGETTYPE_JOIN_USER == nTargetType)
            {
                if (JoinSet < m_JoinRewardItem.Length)
                {
                    m_JoinRewardItem[JoinSet].InitItem(nItemID);
                    JoinSet++;
                }
            }
            else if ((int)RewardTargetType.TARGETTYPE_SHARE_USER == nTargetType)
            {
                if (shareSet < m_ShareRewardItem.Length)
                {
                    m_ShareRewardItem[shareSet].InitItem(nItemID);
                    shareSet++;
                }
            }
        }

        if (OpenType.OpenType_ActiviteCode == m_nOpenType)
        {
            m_ActiviteGameObject.SetActive(true);
        }
        else if (OpenType.OpenType_Share == m_nOpenType)
        {
            m_ShareGameObject.SetActive(true);
        }
        else
        {
            LogModule.ErrorLog("ShareWindow OpenType Invalid");
        }
        if (ShareType.ShareType_NanGua == m_nShareType)
        {
            m_labelDesc.text = StrDictionary.GetClientDictionaryString("#{3102}");
            UpdateRewardCount();
        }
    }

    public void UpdateRewardCount()
    {
        if (m_labelTimes != null)
        {
            int rewardTimes = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_SHARE_NANGUA_CODE_REWARD_COUNT);
            int left = REWARD_ITEMCOUNT_MAX - rewardTimes;
            if (left <= 0) left = 0;
            m_labelTimes.text = left.ToString() + "/" + REWARD_ITEMCOUNT_MAX.ToString();
        }
    }

    public void OnClickShare()
    {
        if (false == GameManager.gameManager.PlayerDataPool.IsServerFlagOpen(SERVER_FLAGS_ENUM.FLAG_SNS))
        {
            return;
        }

        Tab_ShareReward reward = TableManager.GetShareRewardByID((int)m_nShareType, 0);
        if (null == reward)
        {
            LogModule.ErrorLog("OnClickShare::ShareType Invalid");
            return;
        }
        CG_SNS_SHARE packet = (CG_SNS_SHARE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SNS_SHARE);
        packet.Sharetype = (int)ShareType.ShareType_NanGua;
        packet.SendPacket();

        string str = StrDictionary.GetClientDictionaryString("#{3245}", "",
            Utils.GenServerNameWithSelfGuid(), Utils.GenCodeWithSelfGuid(ShareType.ShareType_NanGua));
        PlatformHelper.ShowSocialShareCenter(str);

        ClearUp();
        UIManager.CloseUI(UIInfo.ShareRoot);
    }

    public void OnCloseClick()
    {
        ClearUp();
        UIManager.CloseUI(UIInfo.ShareRoot);
    }

    public void OnOkClick()
    {
        if (m_inputCode.text.Length > 0)
        {
            CG_SNS_INVITE_CODE packet = (CG_SNS_INVITE_CODE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SNS_INVITE_CODE);
            packet.Code = m_inputCode.text;
            packet.SendPacket();
        }
    }
}
