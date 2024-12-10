/********************************************************************************
 *	文件名：	Team.cs
 *	全路径：	\Script\Player\Team\Team.cs
 *	创建人：	李嘉
 *	创建时间：2014-01-09
 *
 *	功能说明：全部的组队数据
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Games.LogicObj;

public class Team
{
    public Team()
    {
        m_nTeamID = GlobeVar.INVALID_ID;
        m_TeamMember = new TeamMember[GlobeVar.MAX_TEAM_MEMBER];
        for (int i = 0; i < GlobeVar.MAX_TEAM_MEMBER; ++i )
        {
            m_TeamMember[i] = new TeamMember();
        }
    }

    public void CleanUp()
    {
        m_nTeamID = GlobeVar.INVALID_ID;
        for (int i = 0; i < GlobeVar.MAX_TEAM_MEMBER; ++i)
        {
            m_TeamMember[i].CleanUp();
        }
    }

    //更新全部队伍信息
    public void UpdateTeamInfo(GC_TEAM_SYNC_TEAMINFO packet)
    {
        if (null == packet)
        {
            return;
        }

        bool bNeedUpdateNameBoardColor = false;     //是否更新名字版颜色，加入队伍的时候调用
        //如果收到最新的队伍信息，而玩家之前没有队伍的话，则进行提示“你进入了一个队伍，可获得更多收益”
        if (GlobeVar.INVALID_ID == m_nTeamID && GlobeVar.INVALID_ID != packet.TeamID)
        {
            if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2229}");
            }

            bNeedUpdateNameBoardColor = true;
        }

        CleanUp();

        //更新数据
        m_nTeamID = packet.TeamID;

        //如果队伍ID为-1，则说明解散队伍，清除信息即可
        if (m_nTeamID == GlobeVar.INVALID_ID)
        {
            //退出组队跟随状态
            if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.LeaveTeamFollow();
            }
            return;
        }

        //由于是整组填充，所以用guid的数量作为整组数据的数量即可
		for (int i = 0; i < packet.memberGuidList.Count; ++i)
        {
            if (i >= m_TeamMember.Length)
                continue;

            if (i < packet.memberGuidList.Count)
                m_TeamMember[i].Guid = packet.memberGuidList[i];

            if (i < packet.memberNameList.Count)
                m_TeamMember[i].MemberName = packet.memberNameList[i];

            if (i < packet.memberLevelList.Count)
                m_TeamMember[i].Level = packet.memberLevelList[i];

            if (i < packet.memberProfList.Count)
                m_TeamMember[i].Profession = packet.memberProfList[i];

            if (i < packet.memberHPList.Count)
                m_TeamMember[i].HP = packet.memberHPList[i];

            if (i < packet.memberMaxHPList.Count)
                m_TeamMember[i].MaxHP = packet.memberMaxHPList[i];

            if (i < packet.memberCombatList.Count)
                m_TeamMember[i].CombatNum = packet.memberCombatList[i];

            if (i < packet.sceneclassList.Count)
                m_TeamMember[i].SceneClassID = packet.sceneclassList[i];

            if (i < packet.sceneinstList.Count)
                m_TeamMember[i].SceneInstID = packet.sceneinstList[i];

            float fEnterPosX = 0;
            if (i < packet.posXList.Count)
                fEnterPosX = ((float)packet.posXList[i]) / 100;
            float fEnterPosZ = 0;
            if (i < packet.posZList.Count)
                 fEnterPosZ = ((float)packet.posZList[i]) / 100;
            m_TeamMember[i].ScenePos = new UnityEngine.Vector3(fEnterPosX, 0, fEnterPosZ);

            //更新队长队员,索引为0即队长
            if (i == 0)
                m_TeamMember[i].TeamJob = 0;
            else
                m_TeamMember[i].TeamJob = 1;

            //如果队员就在本场景内，则更新名字版信息
            if (bNeedUpdateNameBoardColor && m_TeamMember[i].IsValid())
            {
                Obj_OtherPlayer objTeam = Singleton<ObjManager>.GetInstance().FindOtherPlayerInScene(m_TeamMember[i].Guid);
                if (null != objTeam)
                {
                    objTeam.SetNameBoardColor();
                }
            }
        }
    }

    //根据索引更新某一个队员的信息
    public void UpdateMember(int nIndex, TeamMember member, bool bJustUpdateHP)
    {
        if (nIndex < 0 || nIndex >= m_TeamMember.Length)
        {
            return;
        }

        if (false == bJustUpdateHP)
        {
            //更新全部信息
            m_TeamMember[nIndex] = member;
        }
        else
        {
            //只更新HP
            m_TeamMember[nIndex].HP     = member.HP;
            m_TeamMember[nIndex].MaxHP  = member.MaxHP;
        }
    }

    //获得某个队员信息
    public TeamMember GetTeamMember(int nIndex)
    {
        if (nIndex < 0 || nIndex >= GlobeVar.MAX_TEAM_MEMBER)
        {
            //TeamMember member = new TeamMember();
            //member.CleanUp();
            //return member;
            return null;
        }

        return m_TeamMember[nIndex];
    }

    //队伍是否已满
    public bool IsFull()
    {
        return m_TeamMember[GlobeVar.MAX_TEAM_MEMBER - 1].IsValid();
    }

    public int GetTeamMemberCount()
    {
        int count = 0;
        for (int i = 0; i < GlobeVar.MAX_TEAM_MEMBER; i++)
        {
            if (m_TeamMember[i].IsValid())
            {
                count += 1;
            }
            else
            {
                break;
            }
        }
        return count;
    }

    private int m_nTeamID;                  //队伍ID
    public int TeamID
    {
        get { return m_nTeamID; }
        set { m_nTeamID = value; }
    }
    private TeamMember[] m_TeamMember;      //队员信息
    public TeamMember[] teamMember
    {
        get { return m_TeamMember; }
        set { m_TeamMember = value; }
    }
}
