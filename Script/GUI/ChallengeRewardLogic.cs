using UnityEngine;
using System.Collections;
using Games.LogicObj;
using Games.GlobeDefine;
using GCGame.Table;
using GCGame;
public class ChallengeRewardLogic : MonoBehaviour {

    private static ChallengeRewardLogic m_Instance = null;
    public static int RewardType{ set; get; }


    public static ChallengeRewardLogic Instance()
    {
        return m_Instance;
    }


    public UISprite SprWinOrLose;
	public UILabel m_WinOrLoseLabel;
    public UILabel LableResult;


    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start ()
	{
        ShowRewardUIByType( );
	}

    void OnDestroy()
    {
        m_Instance = null;
    }
 
	public static void  ShowRewardUI( int type )
	{
        ChallengeRewardLogic.RewardType = type;
        UIManager.ShowUI(UIInfo.ChallengeRewardRoot);
	}

    public void ShowRewardUIByType( )
    {
        switch( ChallengeRewardLogic.RewardType )
        {
            case 0: 
                ShowChallengeResult();
                break;
            case 1:
                ShowHuaShanPvPResult();
                break;
            case 2:
                ShowDuelResultUI();
                break;
            case 3:
                ShowWorldBossChallResultUI();
                break;

        };
    }

    public void ShowChallengeResult( )
    {
        if (PVPData.ChallengeIsLose == 0)
        {
            LableResult.text = StrDictionary.GetClientDictionaryString("#{1230}", PVPData.ChallengeSpirit, PVPData.ChallengeReputation);
            SprWinOrLose.spriteName = "Win";
			m_WinOrLoseLabel.text = "挑战成功";
        }
        else
        {
            LableResult.text = StrDictionary.GetClientDictionaryString("#{1231}", PVPData.ChallengeSpirit, PVPData.ChallengeReputation);
			SprWinOrLose.spriteName = "Lose";
			m_WinOrLoseLabel.text = "挑战失败";
        }
    }

     public void ShowHuaShanPvPResult( )
    {
        string tips = HuaShanPVPData.HSRoundTipPrefix();
        if (HuaShanPVPData.Resultwin == 1)
        {
           
            if( HuaShanPVPData.Rounder == 0)//====1
                LableResult.text = StrDictionary.GetClientDictionaryString("#{2346}"); 
            else
                LableResult.text = StrDictionary.GetClientDictionaryString("#{1664}", tips);
          
            SprWinOrLose.spriteName = "Win";
			m_WinOrLoseLabel.text = "挑战成功";
          
        }
        else
        {
           
            LableResult.text = StrDictionary.GetClientDictionaryString("#{1665}", tips);
			SprWinOrLose.spriteName = "Lose";
			m_WinOrLoseLabel.text = "挑战失败";
        }
    }

    public void ShowDuelResultUI( )
    {
        if (HuaShanPVPData.DuelResult == 1)
         {
             LableResult.text = StrDictionary.GetClientDictionaryString("#{1661}"); ;
			SprWinOrLose.spriteName = "Win";
			m_WinOrLoseLabel.text = "挑战成功";
         }
         else
         {
             LableResult.text = StrDictionary.GetClientDictionaryString("#{1662}"); ;
			SprWinOrLose.spriteName = "Lose";
			m_WinOrLoseLabel.text = "挑战失败";
         }
     
    }

    public void ShowWorldBossChallResultUI()
    {
        if (HuaShanPVPData.DuelResult == 1)
        {
            LableResult.text = StrDictionary.GetClientDictionaryString("#{3024}"); ;
			SprWinOrLose.spriteName = "Win";
			m_WinOrLoseLabel.text = "挑战成功";
        }
        else
        {
            LableResult.text = StrDictionary.GetClientDictionaryString("#{3025}"); ;
			SprWinOrLose.spriteName = "Lose";
			m_WinOrLoseLabel.text = "挑战失败";
        }

    }

    void ChallengeOkCallBack()
    {
        /*
        switch (ChallengeRewardLogic.RewardType)
        {
            case 0:
                if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUDAOZHIDIAN)
                {// 副本场景直接发包返回副本前场景 不查表
                    CG_LEAVE_COPYSCENE packet = (CG_LEAVE_COPYSCENE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_LEAVE_COPYSCENE);
                    packet.NoParam = -1;
                    packet.SendPacket();
                }
                break;
            case 1:
                if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HUASHANLUNJIAN)
                {// 副本场景直接发包返回副本前场景 不查表
                    CG_LEAVE_COPYSCENE packet = (CG_LEAVE_COPYSCENE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_LEAVE_COPYSCENE);
                    packet.NoParam = -1;
                    packet.SendPacket();
                }
                break;
            case 2:
                if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_RICHANGJUEDOU)
                {// 副本场景直接发包返回副本前场景 不查表
                    CG_LEAVE_COPYSCENE packet = (CG_LEAVE_COPYSCENE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_LEAVE_COPYSCENE);
                    packet.NoParam = -1;
                    packet.SendPacket();
                }
                break;
            default:
                break;
        };
        */
        CG_LEAVE_COPYSCENE packet = (CG_LEAVE_COPYSCENE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_LEAVE_COPYSCENE);
        packet.NoParam = -1;
        packet.SendPacket();
        UIManager.CloseUI(UIInfo.ChallengeRewardRoot);
    }
}
