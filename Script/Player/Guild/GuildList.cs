/********************************************************************************
 *	文件名：	GuildList.cs
 *	全路径：	\Script\Player\Guild\GuildList.cs
 *	创建人：	李嘉
 *	创建时间：2014-04-24
 *
 *	功能说明：帮会列表基础数据
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.GlobeDefine;

public class GuildList
{
    public GuildList()
    {
        CleanUp();
    }

    public void CleanUp()
    {
        if (null == m_GuildInfoList)
        {
            m_GuildInfoList = new List<GuildPreviewInfo>();
        }

        m_GuildInfoList.Clear();
    }

    private List<GuildPreviewInfo> m_GuildInfoList;
    public List<GuildPreviewInfo> GuildInfoList
    {
        get { return m_GuildInfoList; }
        set { m_GuildInfoList = value; }
    }

    public void UpdateData(GC_GUILD_RET_LIST list)
    {
        m_GuildInfoList.Clear();
        for (int i = 0; i < list.guildGuidCount; ++i)
        {
            GuildPreviewInfo info = new GuildPreviewInfo();
            info.GuildGuid = list.GetGuildGuid(i);
            if (info.GuildGuid == GlobeVar.INVALID_GUID)
            {
                continue;
            }

            if (i < list.guildNameCount)
            {
                info.GuildName = list.GetGuildName(i);
            }

            if (i < list.guildLevelCount)
            {
                info.GuildLevel = list.GetGuildLevel(i);
            }

            if (i < list.guildChiefNameCount)
            {
                info.GuildChiefName = list.GetGuildChiefName(i);
            }

            if (i < list.guildMemberNumCount)
            {
                info.GuildCurMemberNum = list.GetGuildMemberNum(i);
            }
            if (i < list.guildCombatCount)
            {
                info.GuildCombatValue = list.GetGuildCombat(i);
            }
            if (i< list.isEnemyGuildCount)
            {
                info.IsEnemyGuild = (list.GetIsEnemyGuild(i) == 1);
            }
            if (i < list.guildApplyNumCount)
            {
                info.GuildCurApplyNum = (list.GetGuildApplyNum(i));
            }
           // Debug.Log(info.GuildLevel);
          //  Debug.Log(info.GuildMaxMemberNum);
            m_GuildInfoList.Add(info);
        }
    }
}
