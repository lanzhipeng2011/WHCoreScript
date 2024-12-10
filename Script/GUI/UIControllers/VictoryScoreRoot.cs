using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Games.GlobeDefine;
using GCGame;
public class VictoryScoreRoot : UIControllerBase<VictoryScoreRoot>
{

    public struct ItemInfo
    {
        public int type;
        public int itemId;
        public int count;
    }

    public UISprite[] m_SpriteReward;
    public UILabel[] m_LabelReward;

    public UISprite[] m_SpriteDraw;
    public UISprite[] m_DisableSpriteDraw;
    public UILabel[] m_LabelDraw;
	public UILabel[] m_LabelDrawName;
    public UISprite[] m_SignSprite;
    public UISprite[] m_QualitySprite;
	private bool[] m_LotteryStatus;

	public UILabel[] m_ConstLabel;
	public UISprite[] m_ConstSprite;

    public UILabel m_Lianji;
    public UILabel m_TongGuanTime;
    public UILabel m_Exp;
    public UILabel m_Money;
    public UILabel m_JiFen;
	public UILabel m_Nandu;
	public UILabel m_Jishaxiaoguai;

    public GameObject cover;

    public GameObject ScoreWindow;
    public GameObject LotteryWindow;

    public GameObject GameWinWindow;
    public GameObject GameFailWindow;        //失败励UI
    public GameObject GameStaticWinWindow;   //无奖励显示胜利励UI

    public GameObject GameWinCartoonWindow;

    public GameObject m_OpenCopyButton;  

    public static List<ItemInfo> m_DrawList;  //奖赏物品
    public static List<ItemInfo> m_RewardList;  //奖赏物品

    public static List<ulong> m_UserInCopyScene;
    
    public static int m_nResult = 0;
    public static int m_nSceneId = 0;
    public static int m_nSolo = 0;
    public static int m_Difficult = 0;
    public static int m_nStar = 0;
    public static int m_nScore = 0;
    public static int m_nCarom = 0;
    public static int m_nTime = 0;
    public static int m_nExp = 0;
    public static int m_nMoney = 0;
    public static int m_nDrawIndex = 0;
	public static int m_nKillMon = 0;

    private float m_fDrawTime = Time.realtimeSinceStartup;
    private int m_nDrawIndexEx = -1;
    private bool m_bSend = false;
    private float m_fEndTime = 5.0f;

    public UIImageButton ContinueGame;
    public UIImageButton AutoAddFriend;
    public GameObject[] ButtonDraw;
    public UILabel m_FailText;
    public UILabel m_StaticWinText;
	// Use this for initialization

    // 结算背景移动进入动画相关
    public TweenPosition m_WinBGLeftTween;
    public TweenPosition m_WinBGRigthTween;
    public GameObject m_ScoreInfo;
    public GameObject m_LotteryInfo;

	public UISprite m_PingJiSprite;


    // 令牌闪烁动画相关
    private bool m_BeginFanPaiShine = false;
    private float m_FanPaiShineStartTime = 0;
    private const float PerFanPaiShine = 0.03f;
    private int CurFanPaiShineFrame = 1;

    // 令牌翻转动画相关
    private bool m_BeginFanPaiAni = false;
    private float m_FanPaiAniStartTime = 0;
    private const float PerFanPaiAni = 0.03f;
    private int CurFanPaiAniFrame = 1;
    private int m_PlayerClickIndex = 0;

    // 其他令牌翻转动画相关
    private bool m_BeginFanPaiOther = false;
    private float m_FanPaiOtherStartTime = 0;
    private const float PerFanPaiOther = 0.03f;
    private int CurFanPaiOtherFrame = 1;

    void Awake()
    {
        SetInstance(this);

    }
	void OnEnable()
	{
		InvokeRepeating ("AutoPt", 5.0f, 1.0f);
	}
	void AutoPt()
	{
		if(GameManager.gameManager.RunningScene==20)
		{
			if(ObjManager.GetInstance().MainPlayer.AutoComabat)
			{
				OnTierClick();
			}
		}
	}
	void Start () {

		//=====临时使用逻辑TODO
//		if (m_nResult == 1)
//		{
//			BG.SetActive(false);
//			GameWinWindow.SetActive(false);
//			GameWinCartoonWindow.SetActive(true);
//			GameFailWindow.SetActive(false);
//			GameStaticWinWindow.SetActive(false); 
//			//m_FailText.text = StrDictionary.GetClientDictionaryString("#{2219}", m_nStar);
//		}else{
//			GameWinWindow.SetActive(false);
//			GameWinCartoonWindow.SetActive(false);
//			GameFailWindow.SetActive(true);
//			GameStaticWinWindow.SetActive(false); 
//			//m_FailText.text = StrDictionary.GetClientDictionaryString("#{2784}");
//		}
//		StartCoroutine (DelayLiveCopyScene(3f));
//		return;
		//=======end
	

        if (m_DrawList == null)
        {
            m_DrawList = new List<ItemInfo>();
        }
        if (m_RewardList == null)
        {
            m_RewardList = new List<ItemInfo>();
        }
        m_nDrawIndexEx = -1;
//         for (int i = 0; i < m_Star.Length; i++)
//         {
//             m_Star[i].spriteName = "weijihuo";//.gameObject.SetActive(false);
//         }
          
        if (m_nResult == 1)
        {
            if (m_nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA)
            {
                GameWinWindow.SetActive(false);
                GameWinCartoonWindow.SetActive(false);
                GameFailWindow.SetActive(false);
                GameStaticWinWindow.SetActive(true);
                if (m_nStar >= 100)
                {
                    m_OpenCopyButton.SetActive(false);
                    m_StaticWinText.text = StrDictionary.GetClientDictionaryString("#{2658}");
                }
                else
                {
                    m_OpenCopyButton.SetActive(true);
                    m_StaticWinText.text = StrDictionary.GetClientDictionaryString("#{2220}", m_nStar);
                }
            }
            else
            {
                if (m_nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_QIXINGXUANZHEN ||
                   m_nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HULAOGUAN ||
                   m_nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HUSONGMEIREN ||
                   m_nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YEXIDAYING ||
                   m_nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_GUOGUANZHANJIANG ||
                   m_nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_FENGHUOLIANTIAN)
                {
                    GameWinWindow.SetActive(false);
                    GameFailWindow.SetActive(false);
                    GameWinCartoonWindow.SetActive(false);
                    m_fEndTime = Time.realtimeSinceStartup + 3.0f;
                    //m_StaticWinText.text = "副本成功";
                    m_StaticWinText.text = StrDictionary.GetClientDictionaryString("#{2783}");
                }
                else
                {
                    GameWinWindow.SetActive(false);
                    GameFailWindow.SetActive(false);
                    GameWinCartoonWindow.SetActive(true);
                    m_fEndTime = Time.realtimeSinceStartup + 2.0f;
                    //m_StaticWinText.text = "副本成功";
                    m_StaticWinText.text = StrDictionary.GetClientDictionaryString("#{2783}");
                }
            }

        }
        else
        {            
            if (m_nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA)
            {
                GameWinWindow.SetActive(false);
                GameWinCartoonWindow.SetActive(false);
                GameFailWindow.SetActive(true);
                GameStaticWinWindow.SetActive(false); 
                m_FailText.text = StrDictionary.GetClientDictionaryString("#{2219}", m_nStar);
            }
            else
            {
                GameWinWindow.SetActive(false);
                GameFailWindow.SetActive(true);
                //m_FailText.text = "副本失败";
                m_FailText.text = StrDictionary.GetClientDictionaryString("#{2784}");
            }
        }        
	    m_Lianji.text = m_nCarom.ToString();

		//==========
		string str1 = "";

		switch(m_Difficult)
		{
		case 1:
			str1 = StrDictionary.GetClientDictionaryString ("#{1311}");
			break;
		case 2:
			str1 = StrDictionary.GetClientDictionaryString ("#{1312}");
			break;
		case 3:
			str1 = StrDictionary.GetClientDictionaryString ("#{1313}");
			break;
		default:
			break;
		}
		m_Nandu.text = str1;
		m_Jishaxiaoguai.text = m_nKillMon.ToString ();
		//===========

        Tab_SceneClass pSceneClass = null;
        Tab_CopyScene pCopyScene = null;
        Tab_CopySceneRule pCopySceneRule = null;
        if (m_nSceneId == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA)
        {

            pCopySceneRule = TableManager.GetCopySceneRuleByID(m_nStar, 0);
            if (pCopySceneRule == null)
            {
                return;
            }
        }
        else
        {
            pSceneClass = TableManager.GetSceneClassByID(m_nSceneId, 0);
            if (pSceneClass == null)
            {
                return;
            }

            pCopyScene = TableManager.GetCopySceneByID(pSceneClass.CopySceneID, 0);
            if (pCopyScene == null)
            {
                return;
            }
            if (m_nSolo == 1)
            {
                pCopySceneRule = TableManager.GetCopySceneRuleByID(pCopyScene.GetRulebyIndex(m_Difficult - 1), 0);
            }
            else
            {
                pCopySceneRule = TableManager.GetCopySceneRuleByID(pCopyScene.GetRuleTeambyIndex(m_Difficult - 1), 0);
            }
            if (pCopySceneRule == null)
            {
                return;
            }
        }
        
        string TongGuanSec = (m_nTime % 60).ToString();
        if (m_nTime % 60 < 10 )
        {
            TongGuanSec = "0" + (m_nTime % 60).ToString();
        }
        string RuleSec = (pCopySceneRule.ExistTime % 60).ToString();
        if (pCopySceneRule.ExistTime % 60 < 10)
        {
            RuleSec = "0" + (pCopySceneRule.ExistTime % 60).ToString();
        }

        m_TongGuanTime.text = (m_nTime / 60).ToString() + ":" + TongGuanSec + "/" + (pCopySceneRule.ExistTime / 60).ToString() + ":" + RuleSec;
        m_Exp.text = m_nExp.ToString();
        m_Money.text = m_nMoney.ToString();
        m_JiFen.text = m_nScore.ToString();

		for (int i = 0; i < m_DrawList.Count; ++i )
        {
            m_SpriteDraw[i].spriteName = "";
            m_LabelDraw[i].text = "0";
			m_LabelDrawName[i].text = "";
			m_QualitySprite[i].spriteName = "";
        }
        for (int i = 0; i < 10; ++i)
        {
            m_SpriteReward[i].spriteName = "";
            m_LabelReward[i].text = "0";
        }
		if (m_DrawList.Count > 0) 
		{
			m_LotteryStatus = new bool[4]{true, true, true, true};
		}
        for (int i = 0; i < m_DrawList.Count; ++i)
        {

            if (m_DrawList[i].type== 1)  //物品
            {
                Tab_CommonItem curItem = TableManager.GetCommonItemByID(m_DrawList[i].itemId, 0);
                if (curItem != null)
                {
                    m_SpriteDraw[i].spriteName = curItem.Icon;
                    m_LabelDraw[i].text = m_DrawList[i].count.ToString();
					m_LabelDrawName[i].text = curItem.Name;
					m_QualitySprite[i].spriteName =  GlobeVar.QualityColorGrid[curItem.Quality - 1];
                }
            }
            else if (m_DrawList[i].type == 2)  //金钱
            {
                m_SpriteDraw[i].spriteName = "jinbi";
                m_LabelDraw[i].text = m_DrawList[i].count.ToString();
				m_LabelDrawName[i].text = "";
            }
            else if (m_DrawList[i].type == 3)  //元宝
            {
                m_SpriteDraw[i].spriteName = "yuanbao";
                m_LabelDraw[i].text = m_DrawList[i].count.ToString();
            }
            else if (m_DrawList[i].type == 4)  //经验
            {
                m_SpriteDraw[i].spriteName = "jingyan";
                m_LabelDraw[i].text = m_DrawList[i].count.ToString();
				m_LabelDrawName[i].text = "";
            }
              
        }
        for (int i = 0; i < m_RewardList.Count && i < 10; ++i)
        {
            if (m_RewardList[i].type == 1)  //物品
            {
                Tab_CommonItem curItem = TableManager.GetCommonItemByID(m_RewardList[i].itemId, 0);
                if (curItem != null)
                {
                    m_SpriteReward[i].spriteName = curItem.Icon;
                    m_LabelReward[i].text = m_RewardList[i].count.ToString();
                }
            }
            else if (m_RewardList[i].type == 2)  //金钱
            {
                m_SpriteReward[i].spriteName = "jinbi";
                m_LabelReward[i].text = m_RewardList[i].count.ToString();
            }
            else if (m_RewardList[i].type == 3)  //元宝
            {
                m_SpriteReward[i].spriteName = "yuanbao";
                m_LabelReward[i].text = m_RewardList[i].count.ToString();
            }
            else if (m_RewardList[i].type == 4)  //经验
            {
                m_SpriteReward[i].spriteName = "jingyan";
                m_LabelReward[i].text = m_RewardList[i].count.ToString();
            }

        }
        UpdateButtonState(false);
		UpdateConst (0);
    }

	void UpdateConst(int IndexNum)
	{
		//=====不再更新花费
		string str = "";
		switch(IndexNum)
		{
		case 0:
			str = StrDictionary.GetClientDictionaryString ("#{3458}");
			break;
		case 1:
			if(checkConstNumber())
			{
				str = "";
			}else{
				str = StrDictionary.GetClientDictionaryString ("#{6007}");
			}
			break;
		default:
			break;
		}

		for (int i = 0; i<4; i++ )
		{
			m_ConstLabel[i].text = str;
			m_ConstSprite[i].spriteName = "";
		}
		return;
		//=====不再更新花费end


		/*
		string str = "";
		string spriteName = "qian5";
		switch(IndexNum)
		{
		case 0:
			str = StrDictionary.GetClientDictionaryString ("#{3458}");
			spriteName = "qian5";
			break;
		case 1:
			str = "2000";
			spriteName = "qian5";
			break;
		case 2:
			str = "20";
			spriteName = "yuanbao1";
			break;
		case 3:
			str = "40";
			spriteName = "yuanbao1";
			break;
		}

		for (int i = 0; i<4; i++ )
		{
			m_ConstLabel[i].text = str;
			m_ConstSprite[i].spriteName = spriteName;
		}
		*/


	}
	
	//====暂时逻辑TODO
//	IEnumerator DelayLiveCopyScene(float delayTime) 
//	{
//		yield return new WaitForSeconds(delayTime);
//		OnQuitCopyScene ();
//	}
	
	// Update is called once per frame
	void Update () {

		//暂时返回TODO
		//return;
	

        if (m_fDrawTime - Time.realtimeSinceStartup <= 0 && m_nDrawIndexEx == m_nDrawIndex && m_nDrawIndexEx != -1)
        {
            SendData(0);
            m_nDrawIndexEx = -1;
            ClearCover();
        }
        else if (m_nDrawIndexEx != -1)
        {
            m_nDrawIndexEx += 1;
            if (m_nDrawIndexEx >= 6 )
            {
                m_nDrawIndexEx = 0;
            }
            //m_nDrawIndexEx
            m_DisableSpriteDraw[0].gameObject.SetActive(false);
            m_DisableSpriteDraw[1].gameObject.SetActive(false);
            m_DisableSpriteDraw[2].gameObject.SetActive(false);
            m_DisableSpriteDraw[3].gameObject.SetActive(false);
            m_DisableSpriteDraw[4].gameObject.SetActive(false);
            m_DisableSpriteDraw[5].gameObject.SetActive(false);
            m_DisableSpriteDraw[m_nDrawIndexEx].gameObject.SetActive(true);
            //SendData(0);
            //ClearCover();
        }
        if (m_nResult == 1 && m_nSceneId != (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA)
        {
            if (m_fEndTime - Time.realtimeSinceStartup <= 0)
            {
                if (!GameWinWindow.activeSelf)
                {
                    m_ScoreInfo.SetActive(false);
                    m_LotteryInfo.SetActive(false);

                    GameWinWindow.SetActive(true);
                    GameWinCartoonWindow.SetActive(false);

                    m_WinBGLeftTween.Play();
                    m_WinBGRigthTween.Play();                    
                }
            }
        }
       
        //UpdateShowStarActive();
        UpdateFanPaiShine();
        UpdateFanPaiAni();
        //UpdateFanPaiOther();
	}

    public void WinBGTweenOver()
    {
        m_ScoreInfo.SetActive(true);
        m_LotteryInfo.SetActive(true);

		UpdatePingJi ();
		m_BeginFanPaiShine = true;
		m_FanPaiShineStartTime = Time.fixedTime;

		//====直接显示关闭按钮
		ContinueGame.gameObject.SetActive(true);
    }


	void UpdatePingJi()
	{
		switch(m_nStar)
		{
		case 0:
		case 1:
			m_PingJiSprite.spriteName = "ui_activity_48";
			break;
		case 2:
			m_PingJiSprite.spriteName = "ui_activity_49";
			break;
		case 3:
			m_PingJiSprite.spriteName = "ui_activity_50";
			break;
		case 4:
			m_PingJiSprite.spriteName = "ui_activity_51";
			break;
		case 5:
			m_PingJiSprite.spriteName = "ui_activity_52";
			break;
		}
	}

    // 播放令牌闪光帧动画
    void UpdateFanPaiShine()
    {
        if (m_BeginFanPaiShine)
        {
            if (Time.fixedTime - m_FanPaiShineStartTime >= PerFanPaiShine)
            {
                if (CurFanPaiShineFrame == 1)
                {
                    for (int i = 0; i < ButtonDraw.Length; i++)
                    {
                        ButtonDraw[i].SetActive(true);
                    }
                }

                for (int i = 0; i < m_SignSprite.Length; i++ )
                {
					m_SignSprite[i].spriteName = "ui_activity_45";//"lingpai_00" + CurFanPaiShineFrame.ToString();
                }
                m_FanPaiShineStartTime = Time.fixedTime;

                // 共6帧
                if (CurFanPaiShineFrame >= 6)
                {
                    m_BeginFanPaiShine = false;
                    m_FanPaiShineStartTime = 0;
                    CurFanPaiShineFrame = 1;
                    return;
                }
                else
                {
                    CurFanPaiShineFrame += 1;
                }     
            }
        }
    }

    void UpdateFanPaiAni()
    {
        if (m_BeginFanPaiAni)
        {
            if (Time.fixedTime - m_FanPaiAniStartTime >= PerFanPaiAni)
            {
				m_SignSprite[m_PlayerClickIndex].spriteName = "ui_activity_53";//"fanpai_00" + CurFanPaiAniFrame.ToString();
                m_FanPaiShineStartTime = Time.fixedTime;

                // 共4帧
                if (CurFanPaiAniFrame >= 4)
                {
                    m_BeginFanPaiAni = false;
                    m_FanPaiAniStartTime = 0;
                    CurFanPaiAniFrame = 1;
                    // 播放结束后消失
                    m_SignSprite[m_PlayerClickIndex].gameObject.SetActive(false);
                    // 其他令牌开始翻转
                   // m_BeginFanPaiOther = true;
                    //m_FanPaiOtherStartTime = Time.fixedTime + 0.3f;
                    return;
                }
                else
                {
                    CurFanPaiAniFrame += 1;
                }
            }
        }
    }

//    void UpdateFanPaiOther()
//    {
//        if (m_BeginFanPaiOther)
//        {
//            if (Time.fixedTime - m_FanPaiOtherStartTime >= PerFanPaiOther)
//            {
//                for (int i = 0; i < m_SignSprite.Length; i++ )
//                {
//                    if (i != m_PlayerClickIndex)
//                    {
//						m_SignSprite[i].spriteName = "BlueBg";//"fanpai_00" + CurFanPaiOtherFrame.ToString();
//                    }
//                }
//                m_FanPaiOtherStartTime = Time.fixedTime;
//
//                // 共4帧
//                if (CurFanPaiOtherFrame >= 4)
//                {
//                    m_BeginFanPaiOther = false;
//                    m_FanPaiOtherStartTime = 0;
//                    CurFanPaiOtherFrame = 1;
//                    // 播放结束后消失
//                    for (int i = 0; i < m_SignSprite.Length; i++)
//                    {
//                        if (i != m_PlayerClickIndex)
//                        {
//                            m_SignSprite[i].gameObject.SetActive(false);
//                        }
//                    }
//                    return;
//                }
//                else
//                {
//                    CurFanPaiOtherFrame += 1;
//                }
//            }
//        }
//    }

    public static void Clear()
    {
        if (m_DrawList == null)
        {
            m_DrawList = new List<ItemInfo>();
        }
        if (m_RewardList == null)
        {
            m_RewardList = new List<ItemInfo>();
        }

        if (m_UserInCopyScene == null)
        {
            m_UserInCopyScene = new List<ulong>();
        }

        m_DrawList.Clear();  //奖赏物品
        m_RewardList.Clear();  //奖赏物品
        m_UserInCopyScene.Clear();
        m_nStar = 0;
        m_nScore = 0;
        m_nCarom = 0;
        m_nTime = 0;
        m_nExp = 0;
        m_nMoney = 0;
        m_nDrawIndex = 0;
        m_nSceneId = -1;
        
    }

    public static void addUserInScene(ulong guid)
    {
        if (m_UserInCopyScene == null)
        {
            m_UserInCopyScene = new List<ulong>();
        }

        m_UserInCopyScene.Add(guid);
    }

    public void AutoAddFriendAll()
    {
        if( m_UserInCopyScene != null )
        {
            for (int i = 0; i < m_UserInCopyScene.Count; i++)
            {
                if (m_UserInCopyScene[i] != Games.GlobeDefine.GlobeVar.INVALID_GUID)
                {
                    if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
                    {
                        Singleton<ObjManager>.GetInstance().MainPlayer.ReqAddFriend(m_UserInCopyScene[i]);
                    }
                }
            }
        }
    }

    public static void addDrawList(int type, int ItemId, int count)
    {
        if (m_DrawList == null)
        {
            m_DrawList = new List<ItemInfo>();
        }
        ItemInfo info;
        info.type = type;
        info.itemId = ItemId;
        info.count = count;
        m_DrawList.Add(info);

    }
    public static void addRewardList(int type, int ItemId, int count)
    {
        if (m_RewardList == null)
        {
            m_RewardList = new List<ItemInfo>();
        }
        ItemInfo info;
        info.type = type;
        info.itemId = ItemId;
        info.count = count;
        m_RewardList.Add(info);
    }
    public void OnCloseClick()
    {
        UIManager.CloseUI(UIInfo.VictoryScoreRoot);
        if (Singleton<ObjManager>.Instance.MainPlayer)
        {
            Singleton<ObjManager>.Instance.MainPlayer.ExitTime = 20;
        }
        
    }
   
    public void OnQuitCopyScene()
    {
        CG_LEAVE_COPYSCENE packet = (CG_LEAVE_COPYSCENE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_LEAVE_COPYSCENE);
        packet.NoParam = 1;
        packet.SendPacket();
    }
    public void ClearSend()
    {
        m_bSend = false;
    }
    void SendData(int All)
    {
        if (m_bSend == false)
        {
            CG_ASK_COPYSCENE_REWARD packet = (CG_ASK_COPYSCENE_REWARD)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_COPYSCENE_REWARD);
            packet.SetSceneID(GameManager.gameManager.RunningScene);
            packet.SetType(All);
            packet.SendPacket();
            //m_bSend = true;
            //MessageBoxLogic.OpenWaitBox(1290, 5, 0); 
        }
        else
        {
            if (Singleton<ObjManager>.Instance.MainPlayer)
            {
                //Singleton<ObjManager>.Instance.MainPlayer.SendMessage("请耐心等待");   //稍后替换
                Singleton<ObjManager>.Instance.MainPlayer.SendMessage(StrDictionary.GetClientDictionaryString("#{2891}"));   //稍后替换
            }
        }
        
    }
    public void ClearCover()
    {
        cover.SetActive(false);
    }

    public void OnTierClick()
    {
//         CG_OPEN_COPYSCENE packet = (CG_OPEN_COPYSCENE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_OPEN_COPYSCENE);
//         packet.SceneID = (int)GameDefine_Globe.SCENE_DEFINE.SCENE_CANGJINGGE;
//         packet.Type = 1;
//         packet.Difficult = 1;
//         packet.SendPacket();
        if (GameManager.gameManager.PlayerDataPool.CopySceneChange) //正在传送中
        {
            return;
        }
        GameManager.gameManager.PlayerDataPool.CopySceneChange = true;
        //进入下个副本
        CG_OPEN_COPYSCENE packet = (CG_OPEN_COPYSCENE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_OPEN_COPYSCENE);
        packet.SceneID = (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA;
        packet.Type = 1;
        packet.Difficult = 1;
        packet.EnterType = 2;   //传送点进入
        packet.SendPacket();
//         if (Singleton<ObjManager>.GetInstance() == null)
//         {
//             return;
//         }
//         Singleton<ObjManager>.GetInstance().MainPlayer.SendOpenScene((int)GameDefine_Globe.SCENE_DEFINE.SCENE_CANGJINGGE, 1, 1);
    }

    public void UpdateButtonState(bool isDraw)
    {
         AutoAddFriend.isEnabled = false;
        if (isDraw)
        {
            if (m_UserInCopyScene != null && m_UserInCopyScene.Count > 0)
            {
                AutoAddFriend.isEnabled = true;
            }
        }
        
    }
    public void Lottery1()
    {

		if (m_FanPaiAniStartTime > 0)
			return;

		if (!m_LotteryStatus [0]) 
		{
			return;
		}
		if(checkConstNumber())
		{
			return;
		}
		if (!CheckCanFanPai ()) 
		{
			GUIData.AddNotifyData2Client(false,"#{1019}");
			return;		
		}
        m_PlayerClickIndex = 0;
        m_BeginFanPaiAni = true;
        m_FanPaiAniStartTime = Time.fixedTime;
		m_LotteryStatus [0] = false;
        LotteryReward(0);
    }
    public void Lottery2()
    {

		if (m_FanPaiAniStartTime > 0)
			return;

		if (!m_LotteryStatus [1]) 
		{
			return;
		}
		if(checkConstNumber())
		{
			return;
		}
		if (!CheckCanFanPai ()) 
		{
			GUIData.AddNotifyData2Client(false,"#{1017}");
			return;		
		}
        m_PlayerClickIndex = 1;
        m_BeginFanPaiAni = true;
        m_FanPaiAniStartTime = Time.fixedTime;
		m_LotteryStatus [1] = false;
        LotteryReward(1);
    }
    public void Lottery3()
    {

		if (m_FanPaiAniStartTime > 0)
			return;

		if (!m_LotteryStatus [2]) 
		{
			return;
		}
		if(checkConstNumber())
		{
			return;
		}
		if (!CheckCanFanPai ()) 
		{
			GUIData.AddNotifyData2Client(false,"#{1017}");
			return;		
		}
        m_PlayerClickIndex = 2;
        m_BeginFanPaiAni = true;
		m_FanPaiAniStartTime = Time.fixedTime;
		m_LotteryStatus [2] = false;
        LotteryReward(2);
    }
    public void Lottery4()
    {

		if (m_FanPaiAniStartTime > 0)
			return;

		if (!m_LotteryStatus [3]) 
		{
			return;
		}
		if(checkConstNumber())
		{
			return;
		}
		if (!CheckCanFanPai ()) 
		{
			GUIData.AddNotifyData2Client(false,"#{1017}");
			return;		
		}
        m_PlayerClickIndex = 3;
        m_BeginFanPaiAni = true;
		m_FanPaiAniStartTime = Time.fixedTime;
		m_LotteryStatus [3] = false;
        LotteryReward(3);
    }

	bool CheckCanFanPai()
	{
		//======不进行价格判断
		return true;


		switch (ConstNumber) 
		{
		case 1:
			return 2000 <= GameManager.gameManager.PlayerDataPool.Money.GetMoney_Coin();
		case 2:
			return 20 <= (GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind () 
			              + GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao ());
		case 3:
			return 40 <= (GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBaoBind () 
			              + GameManager.gameManager.PlayerDataPool.Money.GetMoney_YuanBao ());
		case 0:
			return true;
		default:
			return false;
		}

	}

	private int ConstNumber = 0;
	private List<int> ClickArr = new List<int>();

	private bool checkConstNumber()
	{
		//=======正常近允许翻牌一个，五星可翻两个
		if(ConstNumber == 1)
		{
			if(m_nStar == 5)
			{
				return false;
			}

		}else if(ConstNumber == 0){
			return false;
		}

		return true;
		//========end
	}


    public void LotteryReward(int nReward)
    {

		if(checkConstNumber())
		{
			return;
		}

		//====
		if(ClickArr.IndexOf(nReward) != -1)
		{
			return;
		}else{
			ClickArr.Add(nReward);
		}
		SendData(nReward+1);
		ConstNumber++;
		UpdateConst (ConstNumber);

        //m_nDrawIndex
        for (int i = 0; i < m_DrawList.Count && i < 4; ++i)
        {
            if (m_DrawList[i].type == 1)  //物品
            {
                Tab_CommonItem curItem = TableManager.GetCommonItemByID(m_DrawList[i].itemId, 0);
                if (curItem != null)
                {
                    m_SpriteDraw[i].spriteName = curItem.Icon;
                    m_LabelDraw[i].text = m_DrawList[i].count.ToString();
					m_LabelDrawName[i].text = curItem.Name;
					m_QualitySprite[i].spriteName =  GlobeVar.QualityColorGrid[curItem.Quality - 1];
                }
            }
            else if (m_DrawList[i].type == 2)  //金钱
            {
                m_SpriteDraw[i].spriteName = "jinbi";
                m_LabelDraw[i].text = m_DrawList[i].count.ToString();
				m_LabelDrawName[i].text = "";
            }
            else if (m_DrawList[i].type == 3)  //元宝
            {
                m_SpriteDraw[i].spriteName = "yuanbao";
                m_LabelDraw[i].text = m_DrawList[i].count.ToString();
				m_LabelDrawName[i].text = "";
            }
            else if (m_DrawList[i].type == 4)  //经验
            {
                m_SpriteDraw[i].spriteName = "jingyan";
                m_LabelDraw[i].text = m_DrawList[i].count.ToString();
				m_LabelDrawName[i].text = "";
            }
            
        }

        //交换点击的物品,但资源就四个,抽奖物品10个,做特殊处理
//        if (nReward != m_nDrawIndex && nReward < 4)
//        {
//            string spriteName = m_SpriteDraw[nReward].spriteName;
//            string text = m_LabelDraw[nReward].text;
//            if (m_DrawList[m_nDrawIndex].type == 1)  //物品
//            {
//                Tab_CommonItem curItem = TableManager.GetCommonItemByID(m_DrawList[m_nDrawIndex].itemId, 0);
//                if (curItem != null)
//                {
//                    m_SpriteDraw[nReward].spriteName = curItem.Icon;
//                    m_LabelDraw[nReward].text = m_DrawList[m_nDrawIndex].count.ToString();
//                }
//            }
//            else if (m_DrawList[m_nDrawIndex].type == 2)  //金钱
//            {
//                m_SpriteDraw[nReward].spriteName = "jinbi";
//                m_LabelDraw[nReward].text = m_DrawList[m_nDrawIndex].count.ToString();
//            }
//            else if (m_DrawList[m_nDrawIndex].type == 3)  //元宝
//            {
//                m_SpriteDraw[nReward].spriteName = "yuanbao";
//                m_LabelDraw[nReward].text = m_DrawList[m_nDrawIndex].count.ToString();
//            }
//            else if (m_DrawList[m_nDrawIndex].type == 4)  //经验
//            {
//                m_SpriteDraw[nReward].spriteName = "jingyan";
//                m_LabelDraw[nReward].text = m_DrawList[m_nDrawIndex].count.ToString();
//            }
//            if (m_nDrawIndex < 4)
//            {
//                m_SpriteDraw[m_nDrawIndex].spriteName = spriteName;
//                m_LabelDraw[m_nDrawIndex].text = text;
//            }
//           
//        }


//        for (int i = 0; i < m_DrawList.Count && i < 4; ++i)
//        {
//            //m_SignSprite[i].gameObject.SetActive(false);
//            m_SpriteDraw[i].gameObject.SetActive(true);
//            if (nReward == i)
//            {
//                m_DisableSpriteDraw[i].gameObject.SetActive(true);
//            }
//            else
//            {
//                m_QualitySprite[i].gameObject.SetActive(true);
//            }
//            m_LabelDraw[i].gameObject.SetActive(true);
//            ButtonDraw[i].GetComponent<BoxCollider>().enabled = false;
//        }
//        ContinueGame.gameObject.SetActive(true);
//        AutoAddFriend.gameObject.SetActive(true);
//        SendData(0);
    }
}


