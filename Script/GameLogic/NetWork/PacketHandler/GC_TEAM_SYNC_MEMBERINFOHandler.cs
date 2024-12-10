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
            
            //ֱ�ӵ���MainPlayer�ĸ��¶�Ա����
            if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
            {
                //�Ƿ�ָ����HP���λ
                bool bJustUpdateHP = false;

                //ƴ��TeamMember�ṹ
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

                //������ӽ��棨���δ�򿪣�����UpdateTeamInfo�л᲻����
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
