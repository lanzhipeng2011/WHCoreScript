//This code create by CodeEngine

using System;
using Games.LogicObj;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using UnityEngine;
namespace SPacket.SocketInstance
{
    public class GC_OP_QINGGONGPOINTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_OP_QINGGONGPOINT packet = (GC_OP_QINGGONGPOINT)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            string strName = packet.Name;
            bool bShowQingGongPoint = packet.IsShow == 1 ? true : false;
            
            GameObject[] QingGongPointList = GameManager.gameManager.ActiveScene.QingGongPointList;
            for (int i = 0; i < QingGongPointList.Length; ++i )
            {
                if (null != QingGongPointList[i] && QingGongPointList[i].name == strName)
                {
                    QingGongPointList[i].SetActive(bShowQingGongPoint);
                    if (Singleton<ObjManager>.Instance.MainPlayer)
                    {
                        Singleton<ObjManager>.Instance.MainPlayer.AutoMovetoQGPointId = i; //挂机状态需要自动寻路过去的轻功点
                    }
                  //  Singleton<ObjManager>.Instance.MainPlayer.AutoFightFlyInYanZiWu();
                    return (uint)PACKET_EXE.PACKET_EXE_CONTINUE; ;
                }
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
