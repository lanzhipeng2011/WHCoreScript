using System;
using Games.GlobeDefine;
using Games.LogicObj;
using GCGame.Table;
using UnityEngine;
using System.Collections;

public class GuildWarPonitItemLogic : MonoBehaviour
{

    // Use this for initialization
    private GuildWarPointInfo m_pointInfo;
    public UILabel m_PointNameLabel;
    public UILabel m_PointScoreLabel;
    public UISprite m_StateSprite;

    public GameObject m_FightIcon;

    private void Start()
    {

    }

    public void InitInfo(GuildWarPointInfo pointInfo)
    {
        m_pointInfo = pointInfo;
        m_PointNameLabel.text = GuildWarInfoLogic.GetWarPointNameByType(m_pointInfo.PointType);
        m_PointScoreLabel.text = String.Format("+{0}", m_pointInfo.PointScore);
        //中立点
        if (m_pointInfo.PointOwnGuildGuid == GlobeVar.INVALID_GUID)
        {
            m_StateSprite.spriteName = "daizhanling";
            m_StateSprite.MakePixelPerfect();
        }
            //本帮占领
        else if (m_pointInfo.PointOwnGuildGuid == GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid)
        {
            m_StateSprite.spriteName = "youfangzhanling";
            m_StateSprite.MakePixelPerfect();
        }
            //对方帮会占领
        else
        {
            m_StateSprite.spriteName = "difangzhanling";
            m_StateSprite.MakePixelPerfect();
        }
        if (pointInfo.IsFighting)
        {
            m_FightIcon.SetActive(true);
        }
        else
        {
            m_FightIcon.SetActive(false);
        }
    }

    private void ClickPointBt()
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer)
        {
            //正在抢夺中
            if (m_pointInfo.IsFighting)
            {
                _mainPlayer.SendNoticMsg(false, "#{2506}");
                return;
            }
            //已经是本帮占领的
            if (m_pointInfo.PointOwnGuildGuid == GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid)
            {
                _mainPlayer.SendNoticMsg(false, "#{2507}");
                return;
            }
            //弹出MessageBox
            string strPointName = GuildWarInfoLogic.GetWarPointNameByType(m_pointInfo.PointType);
            string dicStr = StrDictionary.GetClientDictionaryString("#{2583}", strPointName);
            MessageBoxLogic.OpenOKCancelBox(dicStr, "", OnOkFight, null);
        }
    }

    private void OnOkFight()
    {
        if (GuildWarInfoLogic.Instance())
        {
            CG_FIGHTGUILDWARPOINT infoPak =(CG_FIGHTGUILDWARPOINT) PacketDistributed.CreatePacket(MessageID.PACKET_CG_FIGHTGUILDWARPOINT);
            infoPak.SetWarType(GuildWarInfoLogic.Instance().CurWarType);
            infoPak.SetPointType(m_pointInfo.PointType);
            infoPak.SendPacket();
        }
    }
}
