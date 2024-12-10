//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_TEAM_SYNC_MEMBERINFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_TEAM_SYNC_MEMBERINFO packet = (GC_TEAM_SYNC_MEMBERINFO)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            
            //直接调用MainPlayer的更新队员操作
            if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                //是否指更新HP标记位
                bool bJustUpdateHP = false;

                //拼接TeamMember结构
                TeamMember member = new TeamMember();
                if (packet.HasMemberGuid)
                {
                    member.Guid = packet.MemberGuid;
                    bJustUpdateHP = false;
                }
                else
                {
                    bJustUpdateHP = true;
                }

                if (packet.HasMemberName)
                {
                    member.MemberName = packet.MemberName;
                }
                if (packet.HasMemberLevel)
                {
                    member.Level = packet.MemberLevel;
                }
                if (packet.HasMemberProf)
                {
                    member.Profession = packet.MemberProf;
                }

                member.HP = packet.MemberHP;
                member.MaxHP = packet.MemberMaxHP;

                Singleton<ObjManager>.GetInstance().MainPlayer.UpdateTeamMemberInfo(packet.Index, member, bJustUpdateHP);

                //更新组队界面（如果未打开，则在UpdateTeamInfo中会不处理）
                if (null != GUIData.delFriendDataUpdate) GUIData.delFriendDataUpdate();

                if (ChatInfoLogic.Instance() != null)
                {
                    ChatInfoLogic.Instance().UpdateSpeakerList_Team();
                }
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
