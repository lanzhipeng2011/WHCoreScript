//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.LogicObj;
using Games.GlobeDefine;
namespace SPacket.SocketInstance
{
    public class GC_SEND_CURFASHIONHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_SEND_CURFASHION packet = (GC_SEND_CURFASHION )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            // �ı���Ϣ��Ϊ����������Լ�ͬ����ǰʱװID ���㲥
            if (FashionLogic.Instance() != null)
            {
                FashionLogic.Instance().HandleSendCurFashion(packet.CurFashionID);               
            }          

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
