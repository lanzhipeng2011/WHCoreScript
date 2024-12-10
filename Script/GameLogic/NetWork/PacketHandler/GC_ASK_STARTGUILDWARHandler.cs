//This code create by CodeEngine

using System;
using Games.LogicObj;
using GCGame.Table;
using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using UnityEngine;

namespace SPacket.SocketInstance
 {
    public class GC_ASK_STARTGUILDWARHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ASK_STARTGUILDWAR packet = (GC_ASK_STARTGUILDWAR )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
           
            GuildWarPushMessageInfo messageInfo =new GuildWarPushMessageInfo();
            messageInfo.MessageType = packet.MessaegType;
            messageInfo.WarType = packet.WarType;
            messageInfo.PointType = packet.PointType;
            if (packet.HasChallengeGuildName)
            {
                messageInfo.ChallengeGuildName = packet.ChallengeGuildName;
            }
            if (packet.HasChallengeGuildGuid)
            {
                messageInfo.ChallengeGuildGUID = packet.ChallengeGuildGuid;
            }
            messageInfo.PushTime = Time.time;
            int nMessagecount = GameManager.gameManager.PlayerDataPool.WarPushMessaeg.Count;
            GameManager.gameManager.PlayerDataPool.WarPushMessaeg.Insert(nMessagecount,messageInfo);
            Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
            //提示
            if (_mainPlayer)
            {
                switch (messageInfo.MessageType)
                {
                    //海选推送信息
                    case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.STARTPREMINARY:
                        {
                           _mainPlayer.SendNoticMsg(false,"#{2578}");
                        }
                        break;
                    //守卫据点推送信息
                    case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.PROTECTPOINT:
                        {
                            string strPointName = GuildWarInfoLogic.GetWarPointNameByType(messageInfo.PointType);
                            _mainPlayer.SendNoticMsg(false,StrDictionary.GetClientDictionaryString("#{2580}", strPointName));
                        }
                        break;
                    //抢夺据点推送信息
                    case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.ROBPOINT:
                        {
                            string strPointName = GuildWarInfoLogic.GetWarPointNameByType(messageInfo.PointType);
                            _mainPlayer.SendNoticMsg(false,StrDictionary.GetClientDictionaryString("#{2582}", strPointName));
                        }
                        break;
                    //回应是否接受约战(副本战)
                    case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.ASKCHALLENGE:
                        {
                            _mainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2611}", messageInfo.ChallengeGuildName));
                        }
                        break;
                    //回应是否接受野外宣战
                    case (int)GC_ASK_STARTGUILDWAR.MESSAGETYPE.ASKWILDWAR:
                        {
                            _mainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{3163}", messageInfo.ChallengeGuildName));
                        }
                        break;
                }
            }
           
            //如果推送按钮没显示 则打开
            if (GuildWarPushMessageLogic.Instance() ==null)
            {
                UIManager.ShowUI(UIInfo.GuilWarPushMessage);
            }
            else
            {
                GuildWarPushMessageLogic.Instance().UpdateMessageNum();
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
