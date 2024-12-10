//////////////////////////////////////////////////////////////////////////
// 修改历史:
//    2014-5-28 Lijia: 客户端效率优化，把DailyMission从class改为struct
//////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Games.Mission;
using Module.Log;

namespace Games.DailyMissionData
{
    public struct DailyMission
    {
        private int m_nMissionID; // 日常任务ID
        public int MissionID
        {
            get { return m_nMissionID; }
            set { m_nMissionID = value; }
        }
        private byte m_yQuality;  // 品质
        public byte Quality
        {
            get { return m_yQuality; }
            set { m_yQuality = value; }
        }

        public void CleanUp()
        {
            m_nMissionID = GlobeVar.INVALID_ID;
            m_yQuality = 0;
        }
    }

    public class DailyMissionData
    {
        public const int MAX_DAILYMISSION_KINDS = 10;   // 最大种类数

        private DailyMission[] m_DailyMissionList;  // 日常任务
        private int m_nDailyMissionDoneCount;   // 每天所做日常任务数
        public int DailyMissionDoneCount
        {
            get { return m_nDailyMissionDoneCount; }
            set { 
                m_nDailyMissionDoneCount = value; 
                if (ActivityController.Instance())
                {
                    ActivityController.Instance().UpDateDoneCount(m_nDailyMissionDoneCount);
                }
            }
        }

        public DailyMissionData()
        {
            m_DailyMissionList = new DailyMission[MAX_DAILYMISSION_KINDS];
            m_nDailyMissionDoneCount = 0;
        }

        void CleanUp()
        {
            for (int nIndex = 0; nIndex < MAX_DAILYMISSION_KINDS; nIndex++)
            {
                m_DailyMissionList[nIndex].CleanUp();
            }
            m_nDailyMissionDoneCount = 0;
        }

        public DailyMission GetDailyMissionByKind(int nKind)
        {
            if (nKind >= 0 && nKind < MAX_DAILYMISSION_KINDS)
            {
                return m_DailyMissionList[nKind];
            }

            DailyMission mission = new DailyMission();
            mission.CleanUp();
            return mission;
        }

        public void AskUpdateDailyMission(int nKind)
        {
            if (nKind < 0 || nKind > MAX_DAILYMISSION_KINDS)
            {
                return;
            }
            CG_DAILYMISSION_UPDATE askPacket = (CG_DAILYMISSION_UPDATE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_DAILYMISSION_UPDATE);
			askPacket.SetType((uint)nKind);
            askPacket.SendPacket();
        }

        public void HandlePacket(GC_DAILYMISSION_UPDATE_RET packet)
        {
            DailyMissionDoneCount = packet.Donecount;
            int nKind = packet.Type;
            if (nKind == MAX_DAILYMISSION_KINDS)
            {
                for (int i = 0; i < packet.missionIDCount; i++)
                {
                    DailyMission data = new DailyMission();
                    data.CleanUp();
                    data.MissionID  = packet.GetMissionID(i);
                    data.Quality = (byte)packet.GetQualityType(i);
                    if (i < m_DailyMissionList.Length)
                        m_DailyMissionList[i] = data;
                }

                if (ActivityController.Instance())
                {
                    ActivityController.Instance().UpdateDailyMissionList();
                }
            }
            else if (nKind > GlobeVar.INVALID_ID && nKind < MAX_DAILYMISSION_KINDS)
            {
                DailyMission data = new DailyMission();
                data.CleanUp();
                data.MissionID = packet.GetMissionID(0);
                data.Quality = (byte)packet.GetQualityType(0);
                m_DailyMissionList[nKind] = data;
                if (ActivityController.Instance())
                {
                    ActivityController.Instance().UpdateMissionItemByKind(nKind);
                }
            }
        }
    }

}
