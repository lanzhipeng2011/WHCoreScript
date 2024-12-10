/********************************************************************
	filename:	SystemLogic.cs
	date:		2014-7-14  10-40
	author:		
	purpose:	系统设置界面
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Module.Log;
using Games.LogicObj;
using Games.GlobeDefine;
using Games.UserCommonData;
using GCGame;
using GCGame.Table;
using Games.SkillModle;
public class SystemLogic : MonoBehaviour {

    public GameObject SystemStupWindow;
    public GameObject QuestionWindow;
    public GameObject CustomerWindow;
    public GameObject QuitGameButton;       // 这是用户中心按钮，不要被名字误导

    public GameObject UserCenterButton;

    public TabController m_TabTableau;
    public UIToggle m_NameEdition;  //名字板
    public UIToggle m_Music;        //音乐
    public UIToggle m_SoundEffect;  //音效
    public UIToggle m_Floodlight;  //全屏泛光
    public UIToggle m_ScreenMove;  //屏幕移动
    public UIToggle m_NewPlayerGuide;  //新手指引开关
    public UIToggle m_DeathPushEnable; //打开死亡推送
    public UIToggle m_KillNpcExp; //NPCEXP
    public UIToggle m_SkillEffect;  //技能特效
    public UIToggle m_DamageBoard;  //伤害板
    public UIToggle m_WallVision;   //透明遮挡

    //子Window
    public TabController m_TabQuestion;
    public UIToggle m_Bug;          //bug
    public UIToggle m_Complain;     //投诉
    public UIToggle m_propose;      //建议

    private static int m_ShowPlayerNumMax = 30;
    private int m_curShowPlayerNum = 0;
    public UILabel m_LabelHideOtherPlayerNum;
    public UISlider m_SliderHideOtherPlayer;
    public UILabel m_SweepTime;
    private bool m_bSystemTableau = false;

	public GameObject m_SkillBtGird;
	private List<SkillRootButtonItemLogic> m_skillButtonItemLogicList = new List<SkillRootButtonItemLogic>();
//    Obj_MainPlayer m_User = null;
	// Use this for initialization
	void Start ()
    {
        if (PlatformHelper.IsCYSDK())
        {
            QuitGameButton.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (AutoFightLogic.Instance())
        {
            AutoFightLogic.Instance().AutoFightOK(); //切UI的时候存储挂机设置
        }
#if UNITY_ANDROID
        if (UserCenterButton)
        {
            UserCenterButton.SetActive(false);
        }
#endif
        m_bSystemTableau = false;
        if (PlayerPreferenceData.SystemTableau == 1)
        {
            m_TabTableau.ChangeTab("Tab_High");
        }
        else if (PlayerPreferenceData.SystemTableau == 2)
        {
            m_TabTableau.ChangeTab("Tab_Centre");
        }
        else if (PlayerPreferenceData.SystemTableau == 3)
        {
            m_TabTableau.ChangeTab("Tab_Low");
        }
        //        m_User = Singleton<ObjManager>.Instance.MainPlayer;
        
        m_TabTableau.delTabChanged = OnTabChangeTableau;   //Start时不要调用到delTabChanged
        m_Music.value = PlayerPreferenceData.SystemMusic == 1 ? true : false;
        m_SoundEffect.value = PlayerPreferenceData.SystemSoundEffect == 1 ? true : false;
        m_NameEdition.value = PlayerPreferenceData.SystemNameBoard == 1 ? true : false;
        m_ScreenMove.value = PlayerPreferenceData.SystemScreenMove == 1 ? true : false;
        m_NewPlayerGuide.value = !PlayerPreferenceData.NewPlayerGuideClose;
        m_DeathPushEnable.value = PlayerPreferenceData.DeathPushDisable;
        m_KillNpcExp.value = PlayerPreferenceData.KillNpcExp;
        m_SkillEffect.value = PlayerPreferenceData.SystemSkillEffectEnable;
        m_DamageBoard.value = PlayerPreferenceData.SystemDamageBoardEnable;
        m_WallVision.value = PlayerPreferenceData.SystemWallVisionEnable;
        if (PlayerPreferenceData.SystemShowOtherPlayerCount > m_ShowPlayerNumMax)
        {
            m_SliderHideOtherPlayer.value = 1;
        }
        else
        {
            m_SliderHideOtherPlayer.value = (float)PlayerPreferenceData.SystemShowOtherPlayerCount / (float)(m_ShowPlayerNumMax);
        }

        //m_HideOtherPlayer.value = PlayerPreferenceData.SystemHideOtherPlayer;

        if (PlayerPreferenceData.SystemFloodlight == 0)
        {
            m_Floodlight.value = false;
        }
        else
        {
            m_Floodlight.value = true;
        }

        SystemStupWindow.SetActive(true);
        QuestionWindow.SetActive(false);
        CustomerWindow.SetActive(false);

        if (PlayerPreferenceData.SystemTableau == 0)   //未初始化
        {
            m_TabTableau.ChangeTab("Tab_Centre");
            TabChangeTableauEx("Tab_Centre");
        }
        m_bSystemTableau = true;

    }



    void OnTabChangeTableau(TabButton button)
    {
        TabChangeTableauEx(button.name);
    }
    public void TabChangeTableauEx(string str)
    {
        if (m_bSystemTableau == false)
        {
            return;
        }
        if (str == "Tab_High")
        {
            PlayerPreferenceData.SystemTableau = 1;
            m_Music.value = true;               //打开 音乐
            m_SoundEffect.value = true;         //打开 音效
            m_NameEdition.value = true;         //打开 名字板
            m_ScreenMove.value = true;          //打开 屏幕移动
            m_NewPlayerGuide.value = true;      //打开 新手指引
            m_DeathPushEnable.value = true;     //打开 死亡推送
            m_KillNpcExp.value = true;          //打开 杀怪经验
            m_SkillEffect.value = true;         //打开 技能特效
            m_DamageBoard.value = true;         //打开 伤害板
            m_WallVision.value = true;          //打开 透明遮挡
            m_Floodlight.value = true;          //打开 全屏泛光
            m_SliderHideOtherPlayer.value = (float)30 / (float)m_ShowPlayerNumMax;  //玩家数量 30
            OnNameEdition();
            OnMusic();
            OnSoundEffect();
            OnFloodlight();
            OnScreenMove();
            OnNewPlayerGuideClose();
            OnDeathPushClick();
            OnIsKillNpcExp();
            OnSkillEffect();
            OnDamageBoard();
            OnWallVision();
        }
        else if (str == "Tab_Centre")
        {
            PlayerPreferenceData.SystemTableau = 2;
            m_Music.value = true;               //打开 音乐
            m_SoundEffect.value = true;         //打开 音效
#if UNITY_ANDROID && !UNITY_EDITOR	
			string ret = AndroidHelper.platformHelper("shouldShowWarnning");
			if(ret.EndsWith("1")||ret.EndsWith("2"))
				m_NameEdition.value = false;
			else
            	m_NameEdition.value = true;     //打开 名字板
#else
			m_NameEdition.value = true;     	//打开 名字板
#endif
            m_ScreenMove.value = true;          //打开 屏幕移动
            m_NewPlayerGuide.value = true;      //打开 新手指引
            m_DeathPushEnable.value = true;     //打开 死亡推送
            m_KillNpcExp.value = true;          //打开 杀怪经验
            m_SkillEffect.value = true;         //打开 技能特效
            m_DamageBoard.value = true;         //打开 伤害板
            m_WallVision.value = false;         //关闭 透明遮挡
            m_Floodlight.value = false;         //关闭 全屏泛光
            m_SliderHideOtherPlayer.value = (float)15 / (float)m_ShowPlayerNumMax;  //玩家数量 15
            OnNameEdition();
            OnMusic();
            OnSoundEffect();
            OnFloodlight();
            OnScreenMove();
            OnNewPlayerGuideClose();
            OnDeathPushClick();
            OnIsKillNpcExp();
            OnSkillEffect();
            OnDamageBoard();
            OnWallVision();
        }
        else if (str == "Tab_Low")
        {
            PlayerPreferenceData.SystemTableau = 3;
            m_Music.value = false;              //关闭 音乐
            m_SoundEffect.value = false;        //关闭 音效
            m_NameEdition.value = false;        //关闭 名字板
            m_ScreenMove.value = true;          //打开 屏幕移动
            m_NewPlayerGuide.value = true;      //打开 新手指引
			m_DeathPushEnable.value = true;    // 打开 死亡推送
            m_KillNpcExp.value = true;          //打开 杀怪经验
			m_SkillEffect.value = true;        //打开 技能特效
            m_DamageBoard.value = false;        //关闭 伤害板
            m_WallVision.value = false;         //关闭 透明遮挡
            m_Floodlight.value = false;         //关闭 全屏泛光
            m_SliderHideOtherPlayer.value = (float)4 / (float)m_ShowPlayerNumMax;  //玩家数量 1
            OnNameEdition();
            OnMusic();
            OnSoundEffect();
            OnFloodlight();
            OnScreenMove();
            OnNewPlayerGuideClose();
            OnDeathPushClick();
            OnIsKillNpcExp();
            OnSkillEffect();
            OnDamageBoard();
            OnWallVision();
        }        
    }
//     public UIToggle m_NameEdition;  //名字板
//     public UIToggle m_Music;        //音乐
//     public UIToggle m_SoundEffect;  //音效
    public void OnNameEdition()
    {
        PlayerPreferenceData.SystemNameBoard = m_NameEdition.value ? 1 : 0;
        UpdateNameBoard();
    }

    public void OnMusic()
    {
        GameManager.gameManager.SoundManager.EnableBGM = m_Music.value;
        PlayerPreferenceData.SystemMusic = m_Music.value ? 1 : 0;
    }

    public void OnSoundEffect()
    {
        GameManager.gameManager.SoundManager.EnableSFX = m_SoundEffect.value;
        PlayerPreferenceData.SystemSoundEffect = m_SoundEffect.value ? 1 : 0;

        if (GameManager.gameManager.ActiveScene != null)
        {
            GameManager.gameManager.ActiveScene.SetSceneSoundEffect(m_SoundEffect.value);
        }
    }

    public void OnFloodlight()
    {
        FastBloom curCompont = (FastBloom)Camera.main.gameObject.GetComponent("FastBloom");
        if (m_Floodlight.value == false)
        {
            PlayerPreferenceData.SystemFloodlight = 0;
            if (curCompont != null)
            {
                curCompont.enabled = false;
            }
        }
        else
        {
            PlayerPreferenceData.SystemFloodlight = 1;
            if (curCompont != null)
            {
                curCompont.enabled = true;
            }
        }
    }
    public void OnScreenMove()
    {
        PlayerPreferenceData.SystemScreenMove = m_ScreenMove.value ? 1 : 0;
     }

    public void OnNewPlayerGuideClose()
    {
        PlayerPreferenceData.NewPlayerGuideClose = !m_NewPlayerGuide.value;
    }

    public void OnDeathPushClick()
    {
        //
        PlayerPreferenceData.DeathPushDisable = m_DeathPushEnable.value;
        CG_SET_DEATH_PUSH packet = (CG_SET_DEATH_PUSH)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SET_DEATH_PUSH);
        packet.Enable = m_DeathPushEnable.value ? 1 : 0;        
        packet.SendPacket();
    }
    public void OnIsKillNpcExp()
    {
        PlayerPreferenceData.KillNpcExp = m_KillNpcExp.value;
        GameManager.gameManager.PlayerDataPool.CommonData.AskSetCommonFlag( (int)USER_COMMONFLAG.CF_ISOPENKILLNPCEXP, !m_KillNpcExp.value);
    }

    //技能特效
    public void OnSkillEffect()
    {
        PlayerPreferenceData.SystemSkillEffectEnable = m_SkillEffect.value;
    }

    //伤害板
    public void OnDamageBoard()
    {
        PlayerPreferenceData.SystemDamageBoardEnable = m_DamageBoard.value;
    }

    //遮挡半透
    public void OnWallVision()
    {
        PlayerPreferenceData.SystemWallVisionEnable = m_WallVision.value;
        if (Singleton<ObjManager>.Instance.MainPlayer != null)
        { 
            if (m_WallVision.value)
            {
                ObjManager.AddOutLineMaterial(Singleton<ObjManager>.Instance.MainPlayer.gameObject);
            }
            else
            {
                ObjManager.RemoveOutLineMaerial(Singleton<ObjManager>.Instance.MainPlayer.gameObject);
            }
        }
      
    }
     
    //关闭
    public void OnCloseClick()
    {
        PlayerPreferenceData.SystemShowOtherPlayerCount = m_curShowPlayerNum;
        Singleton<ObjManager>.Instance.UpdateHidePlayers();
        UIManager.CloseUI(UIInfo.SystemAndAutoFight);
    }
    //切换账号
    public void OnAccountClick()
    {
        //Application.LoadLevel((int)GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN);
        //string dicStr = "确定切换账号";
        string dicStr = StrDictionary.GetClientDictionaryString("#{2897}");
        MessageBoxLogic.OpenOKCancelBox(dicStr, "", OnAccountOK, OnMessageBoxNO);
    }
    public void OnAccountOK()
    {
        NetManager.SendUserLogout();
        PlatformHelper.UserLogout();
        UIManager.CloseUI(UIInfo.SystemAndAutoFight);
    }
    public void OnMessageBoxNO()
    {

    }
    //切换角色
    public void OnPlayerClick()
    {
        //string dicStr = "确定切换角色";
        string dicStr = StrDictionary.GetClientDictionaryString("#{2898}");
        MessageBoxLogic.OpenOKCancelBox(dicStr, "", OnPlayerOK, OnMessageBoxNO);
    }
    public void OnPlayerOK()
    {
        CG_ASK_QUIT_GAME packet = (CG_ASK_QUIT_GAME)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_QUIT_GAME);
        packet.Type = (int)CG_ASK_QUIT_GAME.GameSelecTType.GAMESELECTTYPE_ROLE;
        packet.SendPacket();
        UIManager.CloseUI(UIInfo.SystemAndAutoFight);
    }
    //卡死处理
    public void OnBlindClick()
    {
        //string dicStr = "确定回到场景入口";
        string dicStr = StrDictionary.GetClientDictionaryString("#{2899}");
        MessageBoxLogic.OpenOKCancelBox(dicStr, "", OnBlindOK, OnMessageBoxNO);      
    }
    public void OnBlindOK()
    {
//        if (Singleton<ObjManager>.Instance.MainPlayer != null)
//        {
//            Singleton<ObjManager>.Instance.MainPlayer.StopMove();
//            if (null != Singleton<ObjManager>.Instance.MainPlayer.NavAgent)
//            {
//                Destroy(Singleton<ObjManager>.Instance.MainPlayer.NavAgent);
//            }
//        }       
//        GameManager.gameManager.AutoSearch.Stop();
       

        CG_CHANGE_MAJORCITY packet = (CG_CHANGE_MAJORCITY)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CHANGE_MAJORCITY);
        packet.SetType(2);
        packet.SendPacket();
        UIManager.CloseUI(UIInfo.SystemAndAutoFight);
    }
    //游戏帮助
    public void OnHelpClick()
    {
        GUIData.AddNotifyData("功能未开启");
        return;
        UIManager.ShowUI(UIInfo.CheckPowerRoot);
        CG_REQ_POWERUP packet = (CG_REQ_POWERUP)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_POWERUP);
        packet.Type = (int)BePowerData.BePowerType.BPTDEFINE_EQUIP;
        packet.Flag = 1;
        packet.SendPacket();
        UIManager.CloseUI(UIInfo.SystemAndAutoFight);

        //string dicStr = "目前没有实现";
       // MessageBoxLogic.OpenOKCancelBox(dicStr, "", OnAccountOK, OnMessageBoxNO);
    }
    //联系客服
    public void OnCustomServiceClick()
    {
        SystemStupWindow.SetActive(false);
        CustomerWindow.SetActive(true);
    }
    //问题反馈
    public void OnQuestionClick()
    {
        SystemStupWindow.SetActive(false);
        QuestionWindow.SetActive(true);
    }
    //退出游戏
    public void OnQuitClick()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (PlatformHelper.GetChannelType() != PlatformHelper.ChannelType.TEST)
        { 
            PlatformHelper.UserLogout();
        }
        else
        {
            CG_ASK_QUIT_GAME packet = (CG_ASK_QUIT_GAME)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_QUIT_GAME);
            packet.Type = (int)CG_ASK_QUIT_GAME.GameSelecTType.GAMESELECTTYPE_QUIT;
            packet.SendPacket();
            UIManager.CloseUI(UIInfo.SystemAndAutoFight);
            Application.Quit();
        }
#else
        CG_ASK_QUIT_GAME packet = (CG_ASK_QUIT_GAME)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_QUIT_GAME);
        packet.Type = (int)CG_ASK_QUIT_GAME.GameSelecTType.GAMESELECTTYPE_QUIT;
        packet.SendPacket();
        UIManager.CloseUI(UIInfo.SystemAndAutoFight);
        Application.Quit();
#endif
    }
    //弹出公告
    public void OnNoticeClick()
    {
        GUIData.AddNotifyData("功能未开启");
        //UIManager.ShowUI(UIInfo.Notice);
        //UIManager.CloseUI(UIInfo.SystemAndAutoFight);
    }
    //提交
    public void OnReferClick()
    {
        //string dicStr = "目前没有实现";
        string dicStr = StrDictionary.GetClientDictionaryString("#{2900}");
        MessageBoxLogic.OpenOKCancelBox(dicStr, "", OnAccountOK, OnMessageBoxNO);
    }
    //返回
    public void OnRetuenClick()
    {
        SystemStupWindow.SetActive(true);
        QuestionWindow.SetActive(false);
        CustomerWindow.SetActive(false);
//     public GameObject SystemStupWindow;
//     public GameObject QuestionWindow;
//     public GameObject CustomerWindow;
    }

    void OnUserCenter()
    {
        PlatformHelper.EnterUserCenter();
    }

    //更新名字板
    private void UpdateNameBoard()
    {
        Dictionary<string, Obj> targets = Singleton<ObjManager>.GetInstance().ObjPools;
        foreach (Obj targetObj in targets.Values)
        {
            if (targetObj != null)
            {
                Obj_Character ObjChar = targetObj.gameObject.GetComponent<Obj_Character>();
                if (ObjChar == null)
                {
                    continue;
                }
                if (ObjChar.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
                    ObjChar.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
				    ObjChar.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC||ObjChar.ObjType== GameDefine_Globe.OBJ_TYPE.OBJ_FELLOW|| ObjChar.ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER)
                {
                    if (PlayerPreferenceData.SystemNameBoard == 0)
                    {
                        ObjChar.CloseNameBoard();
                    }
                    else
                    {
                        ObjChar.ShowNameBoard();
                    }
                }
            }
        }
    }

    public void OnHideOtherPlayerSliderChange()
    {
        m_curShowPlayerNum = (int)(m_SliderHideOtherPlayer.value * m_ShowPlayerNumMax);
        if (m_curShowPlayerNum >= m_ShowPlayerNumMax)
        {
            //m_LabelHideOtherPlayerNum.text = "全部";
            m_LabelHideOtherPlayerNum.text = StrDictionary.GetClientDictionaryString("#{2901}");
        }
        else
        {
            m_LabelHideOtherPlayerNum.text = m_curShowPlayerNum.ToString();
        }
    }
    //充值异常
    private float m_fSendCYpay = 0; //判断发送异常充值
    public void OnRechargeAnomalyClick()
    {
        if (Time.realtimeSinceStartup - m_fSendCYpay > 2.0f)
        {
            m_fSendCYpay = Time.realtimeSinceStartup;
            PlatformListener.SendCYPay(2);
        }       
    }

    void OnCallCenterClick()
    {
        //PlatformHelper.ShowCallCenter();
    }
}
