//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_RESTAURANT_VISITFRIENDINFOHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RESTAURANT_VISITFRIENDINFO packet = (GC_RESTAURANT_VISITFRIENDINFO)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic   
            if (RestaurantData.m_PlayerRestaurantInfo != null && 
                RestaurantData.m_PlayerRestaurantInfo.m_VisitFrindList != null)
            {
                RestaurantData.m_PlayerRestaurantInfo.m_VisitFrindList.Clear();
                for (int i = 0; i < packet.friendGuidCount; i++)
                {
                    RestaurantData.m_PlayerRestaurantInfo.m_VisitFrindList.Add(packet.GetFriendGuid(i));
                }
                if (RestaurantController.Instance() != null)
                {
                    RestaurantController.Instance().UpdateVisitFriendInfo();
                }
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
