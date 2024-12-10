using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Games.LogicObj;
using GCGame;
using Games;

public class CreateGuildLogic : MonoBehaviour
{
    public UILabel m_GuildNameLable;

    void OnCreateGuild()
    {
        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        //判断帮会名称控件是否存在
        if (null == m_GuildNameLable)
        {
            return;
        }

        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (null == mainPlayer)
        {
            return;
        }

        //判断帮会名称是否合法
        string szGuildName = m_GuildNameLable.text;

        //过短
        if (szGuildName.Length <= 0)
        {
            mainPlayer.SendNoticMsg(false, "#{1761}");    //请输入帮会名称
            return;
        }

        //过长
        int nTextCount = 0;
        for (int i = 0; i < szGuildName.Length; i++)
        {
//             if (szGuildName[i] >= 128)
//             {
//                 nTextCount += 2;
//             }
//             else if (szGuildName[i] >= 65 && szGuildName[i] <= 90)
//             {
//                 nTextCount += 2;
//             }
//             else
//             {
//                 nTextCount += 1;
//             }
            nTextCount += 2;
        }
        if (nTextCount > GlobeVar.MAX_GUILD_NAME)
        {
            mainPlayer.SendNoticMsg(false, "#{1279}");     //名字过长
            return;
        }

        //玩家等级判断
        if (mainPlayer.BaseAttr.Level < GlobeVar.CREATE_GUILD_LEVEL)
        {
            mainPlayer.SendNoticMsg(false, "#{1771}");    //你的人物等级不足30级，无法创建帮会
            return;
        }

        //有帮会无法申请
        if (GameManager.gameManager.PlayerDataPool.GuildInfo.GuildGuid != GlobeVar.INVALID_GUID)
        {
            mainPlayer.SendNoticMsg(false, "#{1772}");        //你已属于一个帮会，不能创建帮会
            return;
        }
        
        if (null != Utils.GetStrFilter(szGuildName, (int)GameDefine_Globe.STRFILTER_TYPE.STRFILTER_NAME))
        {
            mainPlayer.SendNoticMsg(false, "#{1278}");        // 包含非法字符
            return;
        }

        mainPlayer.ReqCreateGuild(szGuildName);
        UIManager.CloseUI(UIInfo.CreateGuild);
    }

    void OnClose()
    {
        UIManager.CloseUI(UIInfo.CreateGuild);
    }
}
