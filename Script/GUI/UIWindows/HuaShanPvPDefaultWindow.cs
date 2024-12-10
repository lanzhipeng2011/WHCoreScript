using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using GCGame;
using GCGame.Table;

public class HuaShanPvPDefaultWindow : MonoBehaviour {

    public GameObject m_RegisterMemberButton;
    public GameObject m_RegisterButton;
    public GameObject m_PkInfoButton;

    public UILabel m_LabelPos;

	// Use this for initialization
	void Start () 
	{
        m_RegisterMemberButton.SetActive(false);
        m_RegisterButton.SetActive(false);
        m_PkInfoButton.SetActive(false);
        m_LabelPos.text = Utils.GetDicByID(2987);
	}

    void SetButtonActive(bool member, bool reg, bool bPkInfButton)
    {
        m_RegisterMemberButton.SetActive(member);
        m_RegisterButton.SetActive(reg);
        m_PkInfoButton.SetActive(bPkInfButton);
    }

    void ShowDefaultWindowMySelf()
    {
        //TODO:
        string strPos = Utils.GetDicByID(2565); 
        string s64Plus = "64+";
        switch ((GC_HUASHAN_PVP_STATE.HSPVPSTATE)HuaShanPVPData.HuaShanState)
        {
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.CLOSED:
                strPos = Utils.GetDicByID(2565); //未开启
                SetButtonActive(false, false,false);
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.WAITNEXTROUND:
                strPos = Utils.GetDicByID(2566);// "请等待本轮结束";
                SetButtonActive(false, false,true);
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.KICKED:
                //..增加通知
                strPos = StrDictionary.GetClientDictionaryString("#{1635}", s64Plus);
                SetButtonActive(true, false,true);
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.REGISTER:
                //..增加通知
                strPos = StrDictionary.GetClientDictionaryString("#{1635}", s64Plus);
                SetButtonActive(false, true,false);
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.REGOK:
                strPos = StrDictionary.GetClientDictionaryString("#{1635}", (HuaShanPVPData.HuaShanPosition).ToString());
                SetButtonActive(true, false,false);
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.REGISTERED:
                //..增加通知
                strPos = StrDictionary.GetClientDictionaryString("#{1635}", (HuaShanPVPData.HuaShanPosition).ToString());
                SetButtonActive(true, false,false);
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.MAKEEFF:
                //..增加通知
                strPos = Utils.GetDicByID(2567);
                SetButtonActive(true, false,true);
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.FINISH:
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.START:
                strPos = Utils.GetDicByID(2980); //华山论剑已开始
                SetButtonActive(false, false, true);
                break;
            case GC_HUASHAN_PVP_STATE.HSPVPSTATE.SEARCH:         
                SetButtonActive(false, false, true);
                break;
        }
        m_LabelPos.text = strPos;
    }


    void OnEnable()
    {
        HuaShanPVPData.delegateShowDefaultWindowMySelf += ShowDefaultWindowMySelf;
    }

    void OnDisable()
    {
        HuaShanPVPData.delegateShowDefaultWindowMySelf -= ShowDefaultWindowMySelf;
    }

    void OnRegisterButtonClick( )
    {
        if (Singleton<ObjManager>.Instance.MainPlayer.IsInJianYu())
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2216}");
            return;
        }

        //sendMsg
        CG_HUASHAN_PVP_REGISTER packet = (CG_HUASHAN_PVP_REGISTER)PacketDistributed.CreatePacket(MessageID.PACKET_CG_HUASHAN_PVP_REGISTER);
        packet.None = 0;
        packet.SendPacket();
    }

    void OnRegisterListButtonClick()
    {
        CG_HUASHAN_PVP_MEMBERLIST packet = (CG_HUASHAN_PVP_MEMBERLIST)PacketDistributed.CreatePacket(MessageID.PACKET_CG_HUASHAN_PVP_MEMBERLIST);
        packet.None = 0;
        packet.SendPacket();
    }

    void OnClickPkInfo()
    {
        CG_REQ_HUASHAN_PKINFO packet = (CG_REQ_HUASHAN_PKINFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_HUASHAN_PKINFO);
        packet.None = 0;
        packet.SendPacket();
    }

#region Useless

    void AskForJinYaoDaiRank( )
    {
        //GUIData.AddNotifyData("#{2129}");
        CG_ASK_RANK scoreRankPak = (CG_ASK_RANK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_RANK);
        scoreRankPak.NType = (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANJINYAODAI;
        scoreRankPak.NPage = 0;
        scoreRankPak.SendPacket();
       // UIManager.ShowUI(UIInfo.RankRoot, SendJinYaoDaiRank);
    }

    void SendJinYaoDaiRank(bool bSuccess, object param)
    {
//         if (RankWindow.Instance())
//             RankWindow.Instance().ChangeTabTableau("Tab3");
     
        //MessageBoxLogic.OpenWaitBox(1290, 3, 0); 

    }

    void AskForPosRank()
    {
        //GUIData.AddNotifyData("#{2129}");
        //UIManager.ShowUI(UIInfo.RankRoot, SendHuaShanPosRank);
        CG_ASK_RANK scoreRankPak = (CG_ASK_RANK)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_RANK);
        scoreRankPak.NType = (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANPOS;
        scoreRankPak.NPage = 0;
        scoreRankPak.SendPacket();
    }

    void SendHuaShanPosRank(bool bSuccess, object param)
    {
//         if (RankWindow.Instance())
//             RankWindow.Instance().ChangeTabTableau("Tab8");
     
        //MessageBoxLogic.OpenWaitBox(1290, 3, 0); 
    }
#endregion
}
