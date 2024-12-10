using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Games.GlobeDefine;
using GCGame.Table;
using Games.Item;
using Games.ChatHistory;
using Games.TitleInvestitive;
using System.IO;
using Games.Fellow;
using Module.Log;
using System.Xml;
using Games.SwordsMan;
#if UNITY_WP8
using UnityPortMD5;
using UnityPort;
using System.Text;
using System.Xml.Serialization;
#else
using System.Security.Cryptography;
#endif

namespace GCGame
{
    public class Utils
    {
       
        /// <summary>
        /// 按照规则严格进行分割
        /// </summary>
        /// <param name="str">原始字符</param>
        /// <param name="nTypeList">字符串类型</param>
        /// <param name="regix">规则词，只有一个</param>
        /// <returns>返回分割的词</returns>
        public static string[] MySplit(string str, string[] nTypeList, string regix)
        {

            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            String[] content = new String[nTypeList.Length];
            int nIndex = 0;
            int nstartPos = 0;
            while (nstartPos <= str.Length)
            {
                int nsPos = str.IndexOf(regix, nstartPos);
                if (nsPos < 0)
                {
                    String lastdataString = str.Substring(nstartPos);
                    if (string.IsNullOrEmpty(lastdataString) && nTypeList[nIndex].ToLower() != "string")
                    {
                        content[nIndex++] = "--";
                    }
                    else
                    {
                        content[nIndex++] = lastdataString;
                    }
                    break;
                }
                else
                {
                    if (nstartPos == nsPos)
                    {
                        if (nTypeList[nIndex].ToLower() != "string")
                        {
                            content[nIndex++] = "--";
                        }
                        else
                        {
                            content[nIndex++] = "";
                        }
                    }
                    else
                    {
                        content[nIndex++] = str.Substring(nstartPos, nsPos - nstartPos);
                    }
                    nstartPos = nsPos + 1;
                }
            }

            return content;

        }
        /// <summary>
        /// 将GameObject的transform归0
        /// </summary>
        /// <param name="go"></param>
        public static void ZeroTrans(GameObject go) 
        {
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
        }
        // 复制一个obj并绑定到父节点
        public static GameObject BindObjToParent(GameObject resObject, GameObject parentObject, string name = null)
        {
            if (null == resObject || null == parentObject)
            {
                return null;
            }

            GameObject newObj = GameObject.Instantiate(resObject) as GameObject;
            newObj.transform.parent = parentObject.transform;
            newObj.transform.localPosition = Vector3.zero;
            newObj.transform.localScale = Vector3.one;
            if (null != name)
            {
                newObj.name = name;
            }
            return newObj;
        }

        public static Color GetColorByString(string strColor)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            if (strColor.Length == 8 &&
                strColor.IndexOf("[") == 0 &&
                strColor.IndexOf("]") == 7)
            {
                strColor = strColor.Substring(1, 6);
            }
            if (strColor.Length == 6)
            {
                string strR = strColor[0].ToString() + strColor[1].ToString();
                string strG = strColor[2].ToString() + strColor[3].ToString();
                string strB = strColor[4].ToString() + strColor[5].ToString();
                r = Convert.ToInt32(strR, 16);
                g = Convert.ToInt32(strG, 16);
                b = Convert.ToInt32(strB, 16);
            }
            return new Color((float)r / 255, (float)g / 255, (float)b / 255);
        }
        
        public static string GetItemNameColor(int nQuality)
        {
            string strColor = "[FFFFFF]";
            switch ((ItemQuality)nQuality)
            {
                case ItemQuality.QUALITY_WHITE:
                    {
                        strColor = "[FFFFFF]";
                    }
                    break;
                case ItemQuality.QUALITY_GREEN:
                    {
                        strColor = "[33CC66]";
                    }
                    break;
                case ItemQuality.QUALITY_BLUE:
                    {
                        strColor = "[33CCFF]";
                    }
                    break;
                case ItemQuality.QUALITY_PURPLE:
                    {
                        strColor = "[CC66FF]";
                    }
                    break;
                case ItemQuality.QUALITY_ORANGE:
                    {
                        strColor = "[FF9933]";
                    }
                    break;
                default:
                    break;
            }
            return strColor;
        }

        public static string GetFellowNameColor(int nQuality)
        {
            string strColor = "[FFFFFF]";
            switch ((FELLOWQUALITY)nQuality)
            {
                case FELLOWQUALITY.WHITE:
                    {
                        strColor = "[FFFFFF]";
                    }
                    break;
                case FELLOWQUALITY.GREEN:
                    {
                        strColor = "[33CC66]";
                    }
                    break;
                case FELLOWQUALITY.BLUE:
                    {
                        strColor = "[33CCFF]";
                    }
                    break;
                case FELLOWQUALITY.PURPLE:
                    {
                        strColor = "[CC66FF]";
                    }
                    break;
                case FELLOWQUALITY.ORANGE:
                    {
                        strColor = "[FF9933]";
                    }
                    break;
                default:
                    break;
            }
            return strColor;
        }

        public static string GetFellowQualityName(int nQuality)
        {
            string str = "普通";
            switch ((FELLOWQUALITY)nQuality)
            {
                case FELLOWQUALITY.WHITE:
                    {
                        str = "普通";
                    }
                    break;
                case FELLOWQUALITY.GREEN:
                    {
                        str = "优秀";
                    }
                    break;
                case FELLOWQUALITY.BLUE:
                    {
                        str = "稀有";
                    }
                    break;
                case FELLOWQUALITY.PURPLE:
                    {
                        str = "史诗";
                    }
                    break;
                case FELLOWQUALITY.ORANGE:
                    {
                        str = "传奇";
                    }
                    break;
                default:
                    break;
            }
            return str;
        }

        public static string GetTitleColor(int nColorLevel)
        {
            string strColor = "[FFFFFF]";
            switch ((TITLE_COLORLEVEL)nColorLevel)
            {
                case TITLE_COLORLEVEL.COLOR_WHITE:
                    {
                        strColor = "[FFFFFF]";
                    }
                    break;
                case TITLE_COLORLEVEL.COLOR_GREEN:
                    {
                        strColor = "[33CC66]";
                    }
                    break;
                case TITLE_COLORLEVEL.COLOR_BLUE:
                    {
                        strColor = "[33CCFF]";
                    }
                    break;
                case TITLE_COLORLEVEL.COLOR_PURPLE:
                    {
                        strColor = "[CC66FF]";
                    }
                    break;
                case TITLE_COLORLEVEL.COLOR_ORANGE:
                    {
                        strColor = "[FF9933]";
                    }
                    break;
                default:
                    break;
            }
            return strColor;
        }

        public static string GetItemType(int nClassID, int nSubClassID)
        {
            string strType = "无";
            switch ((GameDefine_Globe.ITEM_CLASS)nClassID)
            {
                case GameDefine_Globe.ITEM_CLASS.CLASS_TEST:
                    {
                        switch((GameDefine_Globe.ITEM_SUBCLASS)nSubClassID)
                        {
                            case GameDefine_Globe.ITEM_SUBCLASS.SUBCLASS_TEST:
                                {
                                    strType = "最强GM剑";
                                }
                                break;
                            case GameDefine_Globe.ITEM_SUBCLASS.SUBCLASS_INVALID:
                            default:
                                break;
                        }
                    }
                    break;
                case GameDefine_Globe.ITEM_CLASS.CLASS_INVALID:
                default:
                    break;
            }
            return strType;
        }
        /// <summary>
        /// 过滤字
        /// </summary>
        /// <param name="str">原始字符</param>
        /// <param name="nFilterType">过滤类型参考STRFILTER_TYPE</param>
        /// <returns>返回空未没有找到，找到后返回相应字符</returns>
        public static string GetStrFilter(string str, int nFilterType)
        {
            if (nFilterType < (int)GameDefine_Globe.STRFILTER_TYPE.STRFILTER_CHAT || nFilterType > (int)GameDefine_Globe.STRFILTER_TYPE.STRFILTER_NAME)
            {
                return null;
            }
            if (str == null)
            {
                return null;
            }

            foreach (KeyValuePair<int, List<Tab_StrFilter>> filter in TableManager.GetStrFilter())
            {
                if (filter.Value.Count > 0)
                {
                    Tab_StrFilter tableList = filter.Value[0];
                    if (null != tableList)
                    {
                        //找到了
                        if (str.IndexOf(tableList.SzString) != -1 && tableList.GetTypebyIndex(nFilterType) == true)
                        {
                            return tableList.SzString;
                        }
                    }
                }
            }
            return null;         
        }

        public static void SendCGChatPak(string text, ChatHistoryItem historyReply)
        {
            CG_CHAT packet = (CG_CHAT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CHAT);
			if(ChatInfoLogic.Instance())
            	packet.Chattype = ChatInfoLogic.Instance().GetCurChannelType();     

            if (packet.Chattype == (int)CG_CHAT.CHATTYPE.CHAT_TYPE_TELL ||
                packet.Chattype == (int)CG_CHAT.CHATTYPE.CHAT_TYPE_FRIEND)
            {
                UInt64 tellGuid = GlobeVar.INVALID_GUID;
                string tellName = "";
                if (packet.Chattype == (int)CG_CHAT.CHATTYPE.CHAT_TYPE_TELL)
                {
                    tellGuid = ChatInfoLogic.Instance().TellChatReceiverGuid;
                    tellName = ChatInfoLogic.Instance().TellChatReceiverName;
                }
                if (packet.Chattype == (int)CG_CHAT.CHATTYPE.CHAT_TYPE_FRIEND)
                {
                    tellGuid = ChatInfoLogic.Instance().FriendChatReceiverGuid;
                    tellName = ChatInfoLogic.Instance().FriendChatReceiverName;
                }
                // 设置私聊对象同时去掉聊天信息中的"/对象名"
                packet.Receiverguid = tellGuid;
                packet.Receivername = tellName;
                string strTellChatHead = "/" + tellName + " ";
                if (text.Contains(strTellChatHead))
                {
                    text = text.Replace(strTellChatHead, "");

                    if (packet.Chattype == (int)CG_CHAT.CHATTYPE.CHAT_TYPE_TELL)
                    {
                        // 加入最近私聊玩家记录
                        if (!GameManager.gameManager.PlayerDataPool.TellChatSpeakers.IsExist(tellGuid))
                        {
                            GameManager.gameManager.PlayerDataPool.TellChatSpeakers.Add(tellGuid, tellName);
                        }
                    }
                }
                else
                {
                    // 如果私聊接收人名字被删掉 按附近频道处理
                    packet.Chattype = (int)CG_CHAT.CHATTYPE.CHAT_TYPE_NORMAL;
                }
            }
            packet.Chatinfo = text;
            if (!historyReply.IsValid())
            {
				if(ChatInfoLogic.Instance())
                	WriteLinkToCGChat(packet, ChatInfoLogic.Instance().ChatLinkType);
            }
            else
            {
                CopyLinkFromHistoryReply(packet, historyReply);
            }
            packet.SendPacket();

            GameManager.gameManager.PlayerDataPool.ChatHistory.AddReplyHistory(packet);
            if (FastReplyLogic.Instance() != null)
            {
                FastReplyLogic.Instance().UpdateHistoryReply();
            }
			if(ChatInfoLogic.Instance())
            	ChatInfoLogic.Instance().ClearLinkBuffer();

        }

        public static void SendLoudSpeaker(string text, int num)
        {
            CG_CHAT packet = (CG_CHAT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CHAT);
            packet.Chattype = (int)CG_CHAT.CHATTYPE.CHAT_TYPE_LOUDSPEAKER;
            text = LoudSpeakerLogic.Instance().InsertLinkSymbols(text);
            packet.Chatinfo = text;
            packet.LoudSpeakerNum = num;
            WriteLinkToCGChat(packet, LoudSpeakerLogic.Instance().ChatLinkType, true);
            LoudSpeakerLogic.Instance().ClearLinkBuffer();
            packet.SendPacket();
        }

        public static void SendGMCommand(string cmd)
        {
            CG_GMCOMMAND packet = (CG_GMCOMMAND)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GMCOMMAND);
            packet.Cmd = cmd;
            packet.SendPacket();
        }

        /// <summary>
        /// 向CG包中写入连接信息
        /// </summary>
        /// <param name="pak">消息包</param>
        /// <param name="eChatLinkType">聊天链接类型</param>
        public static void WriteLinkToCGChat(CG_CHAT pak, ChatInfoLogic.LINK_TYPE eChatLinkType, bool isLoudSpeaker = false)
        {
            // 写入链接信息时需要注意server端GameDefine_Chat.h中定义的数据数量上限宏LINK_INT_MAX和LINK_STRING_MAX
            switch (eChatLinkType)
            {
                case ChatInfoLogic.LINK_TYPE.LINK_TYPE_ITEM:
                    {
                        WriteLinkToCGChat_Item(pak, isLoudSpeaker);
                    }
                    break;
                case ChatInfoLogic.LINK_TYPE.LINK_TYPE_EQUIP:
                    {
                        WriteLinkToCGChat_Equip(pak, isLoudSpeaker);
                    }
                    break;
                case ChatInfoLogic.LINK_TYPE.LINK_TYPE_GUILD:
                    {
                        WriteLinkToCGChat_Guild(pak, isLoudSpeaker);
                    }
                    break;
//                 case ChatInfoLogic.LINK_TYPE.LINK_TYPE_COPYTEAM:
//                     {
//                         WriteLinkToCGChat_CopyTeam(pak, nCopySceneID, nDifficulty);
//                     }
//                     break;
                default:
                    {
                        pak.Linktype = (int)CG_CHAT.LINKTYPE.LINK_TYPE_INVALID;
                    }
                    break;
            }
        }

        /// <summary>
        /// 向CG包中写入物品链接
        /// </summary>
        /// <param name="pak">消息包</param>
        static void WriteLinkToCGChat_Item(CG_CHAT pak, bool isLoudSpeaker)
        {
            pak.Linktype = (int)CG_CHAT.LINKTYPE.LINK_TYPE_ITEM;
            GameItem item = null;
            if (isLoudSpeaker)
            {
                item = LoudSpeakerLogic.Instance().ItemBuffer;
            }
            else
            {
                item = ChatInfoLogic.Instance().ItemBuffer;
            }

            if (item == null || !item.IsValid())
            {
                return;
            }

            pak.AddIntdata(item.DataID);
            int nBindFlag = item.BindFlag ? 1 : 0;
            pak.AddIntdata(nBindFlag);
            for (int i = 0; i < item.DynamicData.Length; i++)
            {
                pak.AddIntdata(item.DynamicData[i]);
            }
        }

        /// <summary>
        /// 向CG包中写入装备链接
        /// </summary>
        /// <param name="pak">消息包</param>
        static void WriteLinkToCGChat_Equip(CG_CHAT pak, bool isLoudSpeaker)
        {
            pak.Linktype = (int)CG_CHAT.LINKTYPE.LINK_TYPE_EQUIP;
            GameItem equip = null;
            if (isLoudSpeaker)
            {
                equip = LoudSpeakerLogic.Instance().EquipBuffer;
            }
            else
            {
                equip = ChatInfoLogic.Instance().EquipBuffer;
            }

            if (equip == null || !equip.IsValid())
            {
                return;
            }

            pak.AddIntdata(equip.DataID);
            int nBindFlag = equip.BindFlag ? 1 : 0;
            pak.AddIntdata(nBindFlag);
            pak.AddIntdata(equip.StarLevel);
            pak.AddIntdata(equip.EnchanceLevel);
            for (int i = 0; i < equip.DynamicData.Length; i++)
            {
                pak.AddIntdata(equip.DynamicData[i]);
            }
            pak.AddIntdata(equip.EnchanceExp);
        }

        /// <summary>
        /// 向CG包中写入帮会链接
        /// </summary>
        /// <param name="pak">消息包</param>
        static void WriteLinkToCGChat_Guild(CG_CHAT pak, bool isLoudSpeaker)
        {
            pak.Linktype = (int)CG_CHAT.LINKTYPE.LINK_TYPE_GUILDCRUITE;
            UInt64 item = 0;
            if (isLoudSpeaker)
            {
                if (null != LoudSpeakerLogic.Instance())
                    item = LoudSpeakerLogic.Instance().GuildBuffer;
            }
            else
            {
                if (null != ChatInfoLogic.Instance())
                    item = ChatInfoLogic.Instance().GuildBuffer;
            }

            Int32 highValue = Convert.ToInt32(item >> 32);
       
            Int32 lowValue = (Int32)((item << 32 >> 32));

            pak.AddIntdata(highValue);
            pak.AddIntdata(lowValue);
            
        }
        /// <summary>
        /// 从聊天历史中读出物品链接
        /// </summary>
        /// <param name="history">聊天历史</param>
        /// <param name="item">物品结构 存入信息</param>
        public static void ReadLinkFromHistory_Item(ChatHistoryItem history, GameItem item, int linkindex)
        {
            int intstart = GetChatLinkIntDataStart(history, linkindex);
            if (intstart == GlobeVar.INVALID_ID)
            {
                return;
            }

            item.DataID = history.IntData[intstart + 0];
            item.BindFlag = history.IntData[intstart + 1] == 1 ? true : false;
            for (int i = 0; i < item.DynamicData.Length && i < 8; i++)
            {
                item.DynamicData[i] = history.IntData[intstart + 2 + i];
            }
        }

        /// <summary>
        /// 从聊天历史中读出装备链接
        /// </summary>
        /// <param name="history">聊天历史</param>
        /// <param name="equip">物品结构 存入信息</param>
        public static void ReadLinkFromHistory_Equip(ChatHistoryItem history, GameItem equip, int linkindex)
        {
            int intstart = GetChatLinkIntDataStart(history, linkindex);
            if (intstart == GlobeVar.INVALID_ID)
            {
                return;
            }

            equip.DataID = history.IntData[intstart + 0];
            equip.BindFlag = history.IntData[intstart + 1] == 1 ? true : false;
            equip.StarLevel = history.IntData[intstart + 2];
            equip.EnchanceLevel = history.IntData[intstart + 3];
            for (int i = 0; i < equip.DynamicData.Length && i < 8; i++)
            {
                equip.DynamicData[i] = history.IntData[intstart + 4 + i];
            }
            equip.EnchanceExp = history.IntData[intstart + 12];
        }

        public static void ReadLinkFromHistory_Team(ChatHistoryItem history, out UInt64 guid)
        {
            guid = history.SenderGuid;
        }

        public static void ReadLinkFromHistory_MoveTo(ChatHistoryItem history, ref ChatLinkLogic.MoveToLinkInfo info, int linkindex)
        {
            int intstart = GetChatLinkIntDataStart(history, linkindex);
            if (intstart == GlobeVar.INVALID_ID)
            {
                return;
            }

            info.SceneClassID = history.IntData[intstart + 0];
            info.SceneInstID = history.IntData[intstart + 1];
            info.PosX = history.IntData[intstart + 2];
            info.PosZ = history.IntData[intstart + 3];
        }

        public static void ReadLinkFromHistory_PlayerInfo(ChatHistoryItem history, out UInt64 guid, int linkindex)
        {
            int intstart = GetChatLinkIntDataStart(history, linkindex);
            if (intstart == GlobeVar.INVALID_ID)
            {
                guid = 0;
                return;
            }
            guid = ((UInt64)history.IntData[intstart + 0] << 32) + (UInt64)history.IntData[intstart + 1];
        }

        public static void ReadLinkFromHistory_GuildInfo(ChatHistoryItem history, out UInt64 guildId, int linkindex)
        {
            int intstart = 0;
            /*int intstart = GetChatLinkIntDataStart(history, linkindex);
            if (intstart == GlobeVar.INVALID_ID)
            {
                guildId = 0;
                return;
            }*/
            if (history.IntData.Count < 2)
            {
                guildId = 0;
                return;
            }
            guildId = ((UInt64)history.IntData[intstart + 0] << 32) + (UInt64)history.IntData[intstart + 1];
        }

        /// <summary>
        /// 从聊天历史中读出侠客链接
        /// </summary>
        /// <param name="history">聊天历史</param>
        /// <param name="oSwordsman">侠客 存入信息</param>
        public static void ReadLinkFromHistory_SwordsMan(ChatHistoryItem history, SwordsMan oSwordsman, int linkindex)
        {
            oSwordsman.CleanUp();
            int intstart = GetChatLinkIntDataStart(history, linkindex);
            if (intstart == GlobeVar.INVALID_ID)
            {
                return;
            }
            oSwordsman.DataId = history.IntData[intstart + 0];
            oSwordsman.Level = history.IntData[intstart + 1];
            oSwordsman.Exp = history.IntData[intstart + 2];
            Tab_SwordsManAttr SwordsManTable = TableManager.GetSwordsManAttrByID(oSwordsman.DataId, 0);
            if (null == SwordsManTable)
            {
                return;
            }
            oSwordsman.Quality = SwordsManTable.Quality;
            oSwordsman.Name = SwordsManTable.Name;
            oSwordsman.Locked = false;
            Tab_SwordsManLevelUp SwordsLevelUpTable = TableManager.GetSwordsManLevelUpByID(oSwordsman.DataId, 0);
            if (null == SwordsLevelUpTable)
            {
                oSwordsman.MaxExp = oSwordsman.Exp;
            }
            else
            {
                oSwordsman.MaxExp = SwordsLevelUpTable.GetExpNeedLvbyIndex(oSwordsman.Level);
            }
        }

        public static int GetChatLinkIntDataStart(ChatHistoryItem history, int linkindex)
        {
            int intstart = 0;
            for (int i = 0; i < linkindex; i++)
            {
                intstart += history.GetLinkIntDataCountByIndex(i);
            }

            if (intstart + history.GetLinkIntDataCountByIndex(linkindex) > history.IntData.Count)
            {
                intstart = GlobeVar.INVALID_ID;
            }

            return intstart;
        }

        public static void CopyLinkFromHistoryReply(CG_CHAT pak, ChatHistoryItem history)
        {
            pak.Linktype = (int)history.ELinkType[0];
            for (int i = 0; i < history.IntData.Count; ++i)
            {
                pak.AddIntdata(history.IntData[i]);
            }
            for (int i = 0; i < history.StrData.Count; ++i)
            {
                pak.AddStrdata(history.StrData[i]);
            }
        }

        public static bool IsContainEmotion(string text)
        {
            return text.Contains("[em=") && text.Substring(text.IndexOf("[em=")).Contains("]");
        }

        public static bool IsContainChatLink(string text)
        {
            return text.Contains("<a>") && text.Contains("</a>");
        }

        public static string GetLinkColor(ChatHistoryItem history, int index)
        {
            if (index >= history.ELinkType.Count)
            {
                return "";
            }

            switch (history.ELinkType[index])
            {
                case GC_CHAT.LINKTYPE.LINK_TYPE_ITEM:
                case GC_CHAT.LINKTYPE.LINK_TYPE_EQUIP:
                    {
                        int intstart = 0;
                        for (int i = 0; i < index; i++ )
                        {
                            intstart += history.GetLinkIntDataCountByIndex(i);
                        }
                        int nDataID = history.IntData[intstart + 0];
                        Tab_CommonItem tabItem = TableManager.GetCommonItemByID(nDataID, 0);
                        if (tabItem != null)
                        {
                            return Utils.GetItemNameColor(tabItem.Quality);
                        }
                    }
                    break;
                case GC_CHAT.LINKTYPE.LINK_TYPE_SWORDSMAN:
                    {
                        int intstart = 0;
                        for (int i = 0; i < index; i++)
                        {
                            intstart += history.GetLinkIntDataCountByIndex(i);
                        }
                        int nDataID = history.IntData[intstart + 0];
                        Tab_SwordsManAttr tabSwordsMan = TableManager.GetSwordsManAttrByID(nDataID, 0);
                        if (tabSwordsMan != null)
                        {
                            return Utils.GetItemNameColor(tabSwordsMan.Quality +1);
                        }
                    }
                    break;
                case GC_CHAT.LINKTYPE.LINK_TYPE_COPYTEAM:
                    return "[FFFF10]";
                        break;
                case GC_CHAT.LINKTYPE.LINK_TYPE_PLAYERINFO:
                    return "[FFE401]";
                        break;
                default:
                    break;
            }
            return "[FFFF10]";
        }

        public static string GetChannelColor(ChatHistoryItem history)
        {
            switch (history.EChannel)
            {
                case GC_CHAT.CHATTYPE.CHAT_TYPE_NORMAL:
                    return "[FFFFFF]";
                case GC_CHAT.CHATTYPE.CHAT_TYPE_TELL:
                    return "[C776FF]";
                case GC_CHAT.CHATTYPE.CHAT_TYPE_TEAM:
                    return "[39D84F]";
                case GC_CHAT.CHATTYPE.CHAT_TYPE_GUILD:
                    return "[63BBFF]";
                case GC_CHAT.CHATTYPE.CHAT_TYPE_MASTER:
                    return "[FF9000]";
                case GC_CHAT.CHATTYPE.CHAT_TYPE_WORLD:
                    return "[FCC0C4]";
                case GC_CHAT.CHATTYPE.CHAT_TYPE_SYSTEM:
                    return "[F23232]";
                case GC_CHAT.CHATTYPE.CHAT_TYPE_FRIEND:
                    return "[FF6EE2]";
                case GC_CHAT.CHATTYPE.CHAT_TYPE_LOUDSPEAKER:
                    return "[FCFF00]";
                default:
                    return "";
            }
        }

        public static int GetStrCharNum(string text)
        {
            int curCharNum = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if ((int)text[i] < 128)
                {
                    curCharNum += 1;
                }
                else
                {
                    curCharNum += 2;
                }
            }
            return curCharNum;
        }

        public static int GetStrTextNum(string text)
        {
            int curCharNum = GetStrCharNum(text);
            return Mathf.CeilToInt((float)curCharNum / 2);
        }
     
        // 将时间间隔格式化对应标签上
        public static void SetTimeDiffToLabel(UILabel label, int timeDiff)
        {
            if (timeDiff <= 0)
            {
                label.text = "00:00:00";
            }
            else
            {
                label.text = string.Format("{0,2:D2}:{1,2:D2}:{2,2:D2}", timeDiff / 3600, (timeDiff % 3600) / 60, timeDiff % 60);
            }
        } 
        public static string GetTimeBySecond(int second) 
        {
            second *= 60;
            if (second >= 24 * 3600)
            {
                return StrDictionary.GetClientDictionaryString("#{2833}", Mathf.RoundToInt((float)second / 3600.0f / 24.0f));
            }
            else if (second >= 60 * 60)
            {
                return StrDictionary.GetClientDictionaryString("#{2834}", Mathf.RoundToInt((float)second / 60.0f / 60.0f));
            }
            else if (second > 0)
            {
                return StrDictionary.GetClientDictionaryString("#{2448}", second);
            }
            return "";
        }
        public static string GetTimeDiffFormatString(int timeDiff)
        {
            if (timeDiff <= 0)
            {
                return "00:00:00";
            }
            else
            {
                return string.Format("{0,2:D2}:{1,2:D2}:{2,2:D2}", timeDiff / 3600, (timeDiff % 3600) / 60, timeDiff % 60);
            }
        }

        // 清除GRID所有子ITEM
        public static void CleanGrid(GameObject grid)
        {
            for (int i = 0, count = grid.transform.childCount; i < count; i++)
            {
                GameObject.Destroy(grid.transform.GetChild(i).gameObject);
            }

            grid.transform.DetachChildren();
        }

        public static string StrFilter_Chat(string strChat)
        {
            string text = strChat;
            string strFilter = GetStrFilter(text, (int)GameDefine_Globe.STRFILTER_TYPE.STRFILTER_CHAT);
            while (strFilter != null)
            {
                text = text.Replace(strFilter, "*");
                strFilter = GetStrFilter(text, (int)GameDefine_Globe.STRFILTER_TYPE.STRFILTER_CHAT);
            }
            return text;
        }

        public static string StrFilter_Mail(string strMailText)
        {
            string text = strMailText;
            string strFilter = GetStrFilter(text, (int)GameDefine_Globe.STRFILTER_TYPE.STRFILTER_CHAT);
            while (strFilter != null)
            {
                text = text.Replace(strFilter, "*");
                strFilter = GetStrFilter(text, (int)GameDefine_Globe.STRFILTER_TYPE.STRFILTER_CHAT);
            }
            return text;
        }

        public static GameObject LoadUIItem(GameObject parent, string name, UIPathData uiData)
        {
            GameObject resObj = ResourceManager.LoadResource(uiData.path) as GameObject;
            GameObject curItem = Utils.BindObjToParent(resObj, parent);
            curItem.name = name;
            return curItem;
        }

        // 简单的获取表中的一个ID对应的文字。
        public static string GetDicByID(int dicID)
        {
            return StrDictionary.GetClientDictionaryString("#{" + dicID.ToString() + "}");
        }

        public static Quaternion DirServerToClient(float rad)
        {
            return Quaternion.Euler(0, 90.0f - rad * 180.0f / Mathf.PI, 0);
        }

        public static float DirClientToServer(Quaternion rotate)
        {
            return Mathf.PI * 0.5f - rotate.eulerAngles.y * Mathf.PI / 180.0f;
        }

        //转化到0-2PI范围内
        public static float NormaliseDirection(float fDirection)
        {
            float _2PI = (float)(Math.PI * 2);
            float fRetValue = fDirection;

            if (fRetValue >= _2PI)
            {
                fRetValue -= ((float)((int)(fDirection / _2PI)) * _2PI);
                fRetValue = (fRetValue > 0.0F) ? fRetValue : 0.0f;
                fRetValue = (fRetValue < _2PI) ? fRetValue : _2PI;
            }
            else if (fRetValue < 0)
            {
                fRetValue += ((float)((int)(-fDirection / _2PI) + 1) * _2PI);
                fRetValue = (fRetValue > 0.0F) ? fRetValue : 0.0f;
                fRetValue = (fRetValue < _2PI) ? fRetValue : _2PI;
            }
            return fRetValue;
        }

        //获取当前平台StreamingAsset路径
        public static string GetStreamingAssetPath()
        {
            string strStreamingPath = "";
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                strStreamingPath = Application.dataPath + "/Raw";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                strStreamingPath = Application.streamingAssetsPath;
            }
            else
            {
                //strStreamingPath = "file://" + Application.streamingAssetsPath; WWW 
                strStreamingPath = Application.streamingAssetsPath;  //File
            }

            return strStreamingPath;
        }

        // 获取属性类型字典
        public static string GetAttrTypeString(int type)
        {
            if (CharacterDefine.AttrTable.ContainsKey(type))
            {
                return Utils.GetDicByID(CharacterDefine.AttrTable[type]);
            }

            return "未知";
        }

		public static void CheckTargetPath(string targetPath)
		{
            targetPath = targetPath.Replace('\\', '/');

            int dotPos = targetPath.LastIndexOf('.');
            int lastPathPos = targetPath.LastIndexOf('/');

            if (dotPos > 0 && lastPathPos < dotPos)
            {
                targetPath = targetPath.Substring(0, lastPathPos);
            }
			if(Directory.Exists(targetPath))
			{
				return;
			}

			
			string[] subPath = targetPath.Split('/');
			string curCheckPath = "";
			int subContentSize = subPath.Length;
			for(int i=0; i<subContentSize; i++)
			{
				curCheckPath += subPath[i] + '/';
				if(!Directory.Exists(curCheckPath))
				{
					Directory.CreateDirectory(curCheckPath);
				}
			}
		}

        public static void DeleteFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }
			Debug.Log("del folder" + path);
            string[] strTemp;
            //先删除该目录下的文件
            strTemp = System.IO.Directory.GetFiles(path);
            foreach (string str in strTemp)
            {
                System.IO.File.Delete(str);
            }
            //删除子目录，递归
            strTemp = System.IO.Directory.GetDirectories(path);
            foreach (string str in strTemp)
            {
                DeleteFolder(str);
            }

            System.IO.Directory.Delete(path);
        }

        // 拷贝一个路径下所有的文件，不包含子目录
        public static void CopyPathFile(string srcPath, string distPath)
        {
            if (!Directory.Exists(srcPath))
            {
                return;
            }

            Utils.CheckTargetPath(distPath);

            string[] strLocalFile = System.IO.Directory.GetFiles(srcPath);
            foreach (string curFile in strLocalFile)
            {
                System.IO.File.Copy(curFile, distPath + "/" + Path.GetFileName(curFile), true);
            }
        }

        // 获取MD5
        public static string GetMD5Hash(string pathName)
        {

            string strResult = "";
            string strHashData = "";
#if !UNITY_WP8
            byte[] arrbytHashValue;
#endif
            System.IO.FileStream oFileStream = null;
            MD5CryptoServiceProvider oMD5Hasher = new MD5CryptoServiceProvider();
            try
            {
                oFileStream = new System.IO.FileStream(pathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
#if UNITY_WP8
                strHashData = oMD5Hasher.ComputeHash(oFileStream);
                oFileStream.Close();
#else
                arrbytHashValue = oMD5Hasher.ComputeHash(oFileStream);
                oFileStream.Close();
                strHashData = System.BitConverter.ToString(arrbytHashValue);
                strHashData = strHashData.Replace("-", "");
#endif
                
                strResult = strHashData;
            }
            catch (System.Exception ex)
            {
                LogModule.ErrorLog("read md5 file error :" + pathName + " e: " + ex.ToString());
            }

            return strResult;

        }

        //登录场景播放音乐,其他地方不要使用
        public static void PlaySceneMusic(int nSoundID)
        {
            if (null == GameManager.gameManager.SoundManager)
            {
                return;
            }

            Tab_Sounds soundsTab = TableManager.GetSoundsByID(nSoundID, 0);
            if (soundsTab == null)
            {
                return;
            }

            GameManager.gameManager.SoundManager.PlayBGMusic(nSoundID, soundsTab.FadeOutTime, soundsTab.FadeInTime);
        }

        public static string ConvertLargeNumToString(int num)
        {
            if (num >= 100000000) 
            {
                return StrDictionary.GetClientDictionaryString("#{5561}", num / 100000000);
            }
            else if (num >= 1000000)
            {
                // 超过100w的显示xx万
                return StrDictionary.GetClientDictionaryString("#{2224}", num / 10000);
            }
            else
            {
                return num.ToString();
            }
        }

        public static string ConvertLargeNumToString_10w(int num)
        {
            if (num >= 100000)
            {
                // 超过10w的显示xx万
                return StrDictionary.GetClientDictionaryString("#{2224}", num / 10000);
            }
            else
            {
                return num.ToString();
            }
        }

        public static int GetIntNumber(int src, int start, int len)
        {
            if (start < 0 || start > 9)
            {
                return GlobeVar.INVALID_ID;
            }
            if (len < 1)
            {
                return GlobeVar.INVALID_ID;
            }

            int result = 0;
            for (int i = 0; i < len; i++ )
            {
                result += (src / (int)Mathf.Pow(10, start + i) % 10) * (int)Mathf.Pow(10, i);
            }
            return result;
        }

        public static bool SetIntNumber(ref int src, int start, int len, int val)
        {
            if (start < 0 || start > 9)
            {
                return false;
            }
            if (len < 1)
            {
                return false;
            }
            if (val >= Mathf.Pow(10, len))
            {
                return false;
            }

            for (int i = 0; i < len; i++ )
            {
                src -= (src / (int)Mathf.Pow(10, start + i) % 10) * (int)Mathf.Pow(10, start + i);
                src += (val / (int)Mathf.Pow(10, i) % 10) * (int)Mathf.Pow(10, start + i);
            }

            return true;
        }

		public static bool GetFileInt(string path, out int retInt)
		{
			try
			{
				if(!File.Exists(path))
				{
					retInt = 0;
					return false;
				}
				FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
				StreamReader sr = new StreamReader(fs);
				string text = sr.ReadToEnd();
				sr.Close();
				fs.Close();

				if(!int.TryParse(text, out retInt))
				{
					LogModule.ErrorLog("parse int error path:" + path);
					return false;
				}

				return true;
			}
			catch(Exception ex)
			{
				LogModule.ErrorLog(ex.ToString());
				retInt = 0;
				return false;
			}

			return false;
		}

		public static bool WriteStringToFile(string path, string text)
		{
			try
			{
				FileStream fs = new FileStream(path, FileMode.Create);
				StreamWriter sw = new StreamWriter(fs);
				sw.WriteLine(text);
				
				sw.Close();
				fs.Close();

				return true;
			}
			catch(Exception ex)
			{
				LogModule.ErrorLog(ex.ToString());
				return false;
			}

			return false;
		}

#if UNITY_WP8
    public class FileInformation : IXmlSerializable
    {
        public string path { get; set; }
        public string md5 { get; set; }
        public string size { get; set; }
        public string level { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.Read();
            reader.ReadStartElement("path");
            path = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("md5");
            md5 = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("size");
            size = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("level");
            level = reader.ReadContentAsString();
            reader.ReadEndElement();

        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("path", path);
            writer.WriteElementString("md5", md5);
            writer.WriteElementString("size", size);
        }
    }


    public class FileList : IXmlSerializable
    {
        public List<FileInformation> list { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            list = new List<FileInformation>();

            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "FileList")
            {
                reader.Read();
                while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "FileInfo")
                {
                    FileInformation info = new FileInformation();
                    info.ReadXml(reader);
                    list.Add(info);
                    reader.Read();
                }
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (FileInformation info in list)
            {
                writer.WriteStartElement("FileInfo");
                info.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
    }
#endif


        public static bool GenerateResFileList(string path, Dictionary<string, UpdateHelper.FileInfo> dicFileInfo)
		{
			try
			{
				Utils.CheckTargetPath(path);
#if UNITY_WP8
                FileList fileList = new FileList();
                fileList.list = new List<FileInformation>();

                foreach (string keys in dicFileInfo.Keys)
                {
                    FileInformation fileInfo = new FileInformation();
                    fileInfo.path = keys;
                    fileInfo.md5 = dicFileInfo[keys].md5.ToString();
                    fileInfo.size = dicFileInfo[keys].size.ToString();
                    fileInfo.level = dicFileInfo[keys].level.ToString();

                    fileList.list.Add(fileInfo);
                }

                XmlHelper.XmlSerializeToFile(fileList, path, Encoding.UTF8);

#else
                XmlDocument xmlRemote = new XmlDocument();
				xmlRemote.AppendChild(xmlRemote.CreateComment("mtlbb update file"));
				XmlElement fileRootRemote = xmlRemote.CreateElement("FileList");
				xmlRemote.AppendChild(fileRootRemote);
				
				foreach (string keys in dicFileInfo.Keys)
				{
					XmlElement curFileNode = xmlRemote.CreateElement("FileInfo");
					
					XmlElement curFilePath = xmlRemote.CreateElement("path");
					curFilePath.AppendChild(xmlRemote.CreateTextNode(keys));
					curFileNode.AppendChild(curFilePath);
					
					XmlElement curFileMd5 = xmlRemote.CreateElement("md5");
					curFileMd5.AppendChild(xmlRemote.CreateTextNode(dicFileInfo[keys].md5));
					curFileNode.AppendChild(curFileMd5);
					
					XmlElement curFileSize = xmlRemote.CreateElement("size");
					curFileSize.AppendChild(xmlRemote.CreateTextNode(dicFileInfo[keys].size.ToString()));
					curFileNode.AppendChild(curFileSize);
					
					XmlElement curFileLoadLevel = xmlRemote.CreateElement("level");
					curFileLoadLevel.AppendChild(xmlRemote.CreateTextNode(dicFileInfo[keys].level.ToString()));
					curFileNode.AppendChild(curFileLoadLevel);
					
					fileRootRemote.AppendChild(curFileNode);
				}
				
				xmlRemote.Save(path);
#endif

				return true;
			}
			catch (Exception ex)
			{
				LogModule.ErrorLog(ex.ToString());
				return false;
			}

			return false;

		}

        public static void GetCopySceneCounts(int copySceneId, int mode, int difficult, ref int curCount, ref int maxCount)
        {            
            curCount = 0; maxCount = 0;

            int nLevel = 0;
            if ( Singleton<ObjManager>.GetInstance().MainPlayer )
            {
                nLevel = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Level;
            }
            Tab_SceneClass pSceneClass = TableManager.GetSceneClassByID(copySceneId, 0);
            if (pSceneClass == null)
            {
                return;
            }

            Tab_CopyScene pCopyScene = TableManager.GetCopySceneByID(pSceneClass.CopySceneID, 0);
            if (pCopyScene == null)
            {
                return;
            }

            Tab_CopySceneRule pCopySceneRule;
            if (mode == 1)
            {
                pCopySceneRule = TableManager.GetCopySceneRuleByID(pCopyScene.GetRulebyIndex(difficult - 1), 0);
            }
            else
            {
                pCopySceneRule = TableManager.GetCopySceneRuleByID(pCopyScene.GetRuleTeambyIndex(difficult - 1), 0);
            }
            if (pCopySceneRule == null)
            {
                return;
            }
            if (nLevel < pCopySceneRule.Level)
            {
                return;
            }

            int nTabNum = pCopySceneRule.Number;
            int ExtraNum = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneExtraNumber(copySceneId, mode);
            int nMul = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneMultiple(copySceneId);
            int nNum = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneNumber(copySceneId, mode);
            int nVip = 0;
            if (mode == 1)
            {
                nVip = VipData.GetVipCopySceneNum(copySceneId);
            }
            maxCount = nTabNum * nMul + ExtraNum + nVip;
            curCount = maxCount - nNum;            
        }
        public static void GetCopySceneCountsAll(int copySceneId,ref int curCount, ref int maxCount)
        {
            curCount = 0;
            maxCount = 0;
            int nCur = 0;
            int nMax = 0;
            GetCopySceneCounts(copySceneId, 1, 1, ref nCur, ref nMax);
            curCount += nCur;
            maxCount += nMax;
            GetCopySceneCounts(copySceneId, 2, 1, ref nCur, ref nMax);
            curCount += nCur;
            maxCount += nMax;
        }
        public static void GetSweepCounts(ref int curCount, ref int maxCount)
        {
            curCount = 0;
            maxCount = 0;
            int nSweep = GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData((int)Games.UserCommonData.USER_COMMONDATA.CD_COPYSCENE_CANGJINGGE_SWEEP);
			int nMul = GameManager.gameManager.PlayerDataPool.CommonData.GetCopySceneMultiple((int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA);
            maxCount = nMul * 3;
            //策划要求不显示花费元宝的那些次数，计算参照OnSweepClick
            //curCount = maxCount - nSweep;
            if (nSweep >= nMul)
            {
                curCount = 0;
            }
            else
            {
                curCount = nMul - nSweep;
            }
        }
        public static int GetActivityAwardBonusLeft()
        {
            int nCount = 0;
            int nActiveness = GameManager.gameManager.PlayerDataPool.AwardActivityData.Activeness;
            int nMaxRecordCount = TableManager.GetActivenessAward().Count;
            for (int i = 0; i < nMaxRecordCount; ++i)
            {
                Tab_ActivenessAward pAward = TableManager.GetActivenessAwardByID(i, 0);
                if (pAward != null)
                {
                    bool bFlag = GameManager.gameManager.PlayerDataPool.AwardActivityData.GetActivenessAwardFlag(i);
                    if (bFlag == false && nActiveness >= pAward.MiniActiveness)
                    {
                        nCount++;
                    }
                }
            }
            return nCount;
        }

        public static string GenCodeWithSelfGuid(ShareType nShareType)
        {
            if (Singleton<ObjManager>.Instance.MainPlayer != null)
            {
                UInt64 guid = Singleton<ObjManager>.Instance.MainPlayer.GUID;
                UInt32 high = (UInt32)(guid >> 32);
                UInt32 serial = (UInt32)(guid & 0xffffffff);
                UInt16 worldId = (UInt16)((high >> 16) & 0xffff);
                Byte carry = (Byte)(high & 0xff);
                UInt32 randKey = (UInt32)UnityEngine.Random.Range(1000, 10000);
                randKey = (UInt32)(randKey & 0xfffc7fff);
                if (nShareType == ShareType.ShareType_SNS)
                {
                    randKey = (UInt32)(randKey | 0x00000000);
                }
                else if (nShareType == ShareType.ShareType_NanGua)
                {
                    randKey = (UInt32)(randKey | 0x00008000);
                }
                else
                {
                    randKey = (UInt32)(randKey | 0x00038000);
                }
                return string.Format("{0}-{1}-{2}-{3}", worldId, carry, serial, randKey);
            }
            return "";
        }

        public static string GenServerNameWithSelfGuid()
        {
            if (Singleton<ObjManager>.Instance.MainPlayer != null)
            {
                UInt64 guid = Singleton<ObjManager>.Instance.MainPlayer.GUID;
                UInt32 high = (UInt32)(guid >> 32);
                UInt16 worldId = (UInt16)((high >> 16) & 0xffff);
                LoginData.ServerListData curData = LoginData.GetServerListDataByID(worldId);
                if (null != curData)
                {
                    return curData.m_name;
                }
            }
            return "";

        }


    }
}
