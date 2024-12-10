/********************************************************************************
 *	文件名：	GuildListItemLogic.cs
 *	全路径：	\Script\GUI\GuildListItemLogic.cs
 *	创建人：	李嘉
 *	创建时间：2014-04-24
 *
 *	功能说明：帮会列表项
 *	修改记录：
*********************************************************************************/

using Games.LogicObj;
using UnityEngine;
using System.Collections;
using System;
using Games.GlobeDefine;
using GCGame.Table;

public class GuildListItemLogic : MonoBehaviour 
{
    //帮会列表项控件
    public UILabel m_GuildIndexLable;        //帮会在列表中的索引
    public UILabel m_GuildNameLabel;         //帮会名称
    public UILabel m_GuildLevelLabel;        //帮会等级
    public UILabel m_GuildChiefNameLabel;    //帮主姓名
    public UILabel m_GuildMemberNumLabel;    //帮会成员数量
    public UILabel m_GuildCombatLable;    //帮会战力
    public UISprite m_EnemyGuildSpirit; //敌对帮会的标记
    public UIImageButton m_GuildJoinButton;  //加入帮会按钮
    public UIImageButton m_GuildChallengeButton;  //约战帮会按钮
    //帮会非UI数据
    private UInt64 m_GuildGuid;         //帮会GUID
    private bool m_canBeJoined = true;      // 是否可以被加入
    private bool m_applyListIsFull = false; // 帮会申请队列是否满

    //根据数据初始化控件
    public void Init(GuildPreviewInfo info, int nIndex)
    {
        //判断数据合法性
        if (info.GuildGuid == GlobeVar.INVALID_GUID)
        {
            return;
        }
        if (m_GuildJoinButton == null || m_GuildChallengeButton == null || m_EnemyGuildSpirit == null)
        {
            return;
        }
        m_GuildGuid = info.GuildGuid;
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer ==null)
        {
            return;
        }
        int GuildJob = GameManager.gameManager.PlayerDataPool.GuildInfo.GetMemberJob(_mainPlayer.GUID);
        m_GuildChallengeButton.gameObject.SetActive(false);
        m_EnemyGuildSpirit.gameObject.SetActive(false);
        m_GuildJoinButton.gameObject.SetActive(false);
        //如果是敌对状态 则显示
        if (info.IsEnemyGuild)
        {
            m_EnemyGuildSpirit.gameObject.SetActive(true);
        }
        //如果玩家自己没有帮会并且该帮会不是玩家已经提交申请的帮会，则显示加入帮会按钮
        else if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid == GlobeVar.INVALID_GUID)
        {
            // 如果  帮会的人数满   或者   帮会待审批列表满，则申请加入按钮变灰
            if (info.GuildCurMemberNum >= info.GuildMaxMemberNum  || info.GuildCurApplyNum >= info.GuildMaxApplyNum)
            {
                m_canBeJoined = false;
                m_applyListIsFull = (info.GuildCurApplyNum >= info.GuildMaxApplyNum);
            }
            else
            {
                m_canBeJoined = true;
                m_applyListIsFull = false;
            }

            m_GuildJoinButton.gameObject.SetActive(true);
        }
        //会长或者副会长可以看到 约战按钮
        else if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid != GlobeVar.INVALID_GUID &&
                GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid != m_GuildGuid && //不是自己的帮会
                (GuildJob == (int) (int) GameDefine_Globe.GUILD_JOB.CHIEF ||
                 GuildJob == (int) (int) GameDefine_Globe.GUILD_JOB.VICE_CHIEF))
        {
            m_GuildChallengeButton.gameObject.SetActive(true);
        }
       
        //填充数据
        if (null != m_GuildIndexLable)
        {
            m_GuildIndexLable.text = nIndex.ToString();
        }
        if (null != m_GuildNameLabel)
        {
            m_GuildNameLabel.text = info.GuildName;
        }

        if (null != m_GuildChiefNameLabel)
        {
            m_GuildChiefNameLabel.text = info.GuildChiefName;
        }

        if (null != m_GuildLevelLabel)
        {
            m_GuildLevelLabel.text = info.GuildLevel.ToString();
        }

        if (null != m_GuildMemberNumLabel)
        {
            m_GuildMemberNumLabel.text = info.GuildCurMemberNum.ToString() + "/" + info.GuildMaxMemberNum.ToString();
        }
        if (null != m_GuildCombatLable)
        {
            if (info.GuildCombatValue < 10000)
            {
                m_GuildCombatLable.text = info.GuildCombatValue.ToString();
            }
            else
            {
                m_GuildCombatLable.text = StrDictionary.GetClientDictionaryString("#{2972}", info.GuildCombatValue /10000);
            }
        }
    }

    void OnClickJoinGuildBtn()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        if (!m_canBeJoined)
        {
            if (m_applyListIsFull)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3180}");
                return;
            }
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1774}");
            return;
        }

        if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid != GlobeVar.INVALID_GUID)
        {
            //如果在一个帮会中，但是暂时为待审批帮众，则可以去申请其他帮会
            GuildMember mySelfGuildInfo = GameManager.gameManager.PlayerDataPool.GuildInfo.GetMainPlayerGuildInfo();
            if (null != mySelfGuildInfo)
            {
                if (mySelfGuildInfo.Job == (int)GameDefine_Globe.GUILD_JOB.RESERVE)
                {
                    //只能同时申请一个帮会，将替换原来的请求，是否继续？
                    string dicStr = StrDictionary.GetClientDictionaryString("#{1861}");
                    MessageBoxLogic.OpenOKCancelBox(dicStr, "", AgreeChangeJoinGuildRequest, null);
                    return;
                }
            }

            //否则提示“你已属于一个帮会不能加入”
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1781}");
            return;
        }

        if (GameManager.gameManager.PlayerDataPool.GuildInfo.PreserveGuildGuid > 0
            && GameManager.gameManager.PlayerDataPool.GuildInfo.PreserveGuildGuid != m_GuildGuid)
        {
            string str =  StrDictionary.GetClientDictionaryString("#{1861}");
            MessageBoxLogic.OpenOKCancelBox(str, "", AgreeChangeJoinGuildRequest, null);
            return;
        }

        Singleton<ObjManager>.GetInstance().MainPlayer.ReqJoinGuild(m_GuildGuid);
    }

    void AgreeChangeJoinGuildRequest()
    {
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.ReqJoinGuild(m_GuildGuid);
        }
    }
    void OnClickChallengeGuildBtn()
    {
        if (GuildWindow.Instance() !=null)
        {
            GuildWindow.Instance().CurChallengeGuildGuid = m_GuildGuid;
            GuildWindow.Instance().ShowChallengeRoot();
        }
    }

    
}
