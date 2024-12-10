//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using GCGame.Table;
using Games.GlobeDefine;
using GCGame;

namespace SPacket.SocketInstance
 {
     public class GC_RET_RANKHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_RET_RANK packet = (GC_RET_RANK )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic

             PVPData.ClearRankData();
             int curPage = 0;
             if (packet.HasCurPage)
             {
                 curPage = packet.CurPage;
                 PVPData.RankCurPage = curPage;
             }

             if (packet.HasTotalPage)
             {
                 PVPData.RankTotalPage = packet.TotalPage;
             }
             PVPData.RankIsPage = (packet.nameCount < 20);
             PVPData.RankType = packet.Type;
             if (packet.HasMerank && packet.Merank != -1)
             {
                 PVPData.meRank = packet.Merank.ToString();
             }
             else
             {
                 PVPData.meRank = Utils.GetDicByID(2059);
             }

             if (packet.Type == (int) GameDefine_Globe.RANKTYPE.TYPE_CANGJINGGE)
             {
                 for (int i =0; i < packet.nameCount; i++)
                 {  
                     string szRank = (curPage*20+(i+1)).ToString();
                     string szName = packet.GetName(i);
                     string szlevel = packet.GetLevel(i).ToString();
                     //string szPro = StrDictionary.GetClientDictionaryString("#{" + CharacterDefine.PROFESSION_DICNUM[packet.GetPro(i)].ToString() + "}");
                     string szTier = packet.GetTier(i).ToString();
                     string Sec = (packet.GetTime(i) % 60).ToString();
                     if (packet.GetTime(i) % 60 < 10)
                     {
                         Sec = "0" + (packet.GetTime(i) % 60).ToString();
                     }
                     string szTime = (packet.GetTime(i)/60).ToString() + ":" + Sec;
                     PVPData.AddRankDataItem(szRank, szName, szlevel,/* szPro,*/ szTier, szTime);
                 }   
             }
             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_PRELIMINARYGUILDWARRANK)
             {
                 if (GuildWarInfoLogic.Instance())
                 {
                    GuildWarInfoLogic.Instance().UpdateGuildWarPreliminaryRankInfo(packet);
                 }
             }
             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_PRELIMINARYGUILDWARKILLRANK)
             {
                 if (GuildWarInfoLogic.Instance())
                 {
                     GuildWarInfoLogic.Instance().UpdateGuildWarKillRankInfo(packet);
                 }
             }

             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANZHANJI)
             {
                 for (int i = 0; i < packet.nameCount; i++)
                 {

                     string szRank = (curPage * 20 + (i + 1)).ToString();

                     PVPData.AddRankDataItem(szRank, packet.GetName(i), packet.GetZhanji(i).ToString());
                 }
             }

             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANJINYAODAI)
             {
                 for (int i = 0; i < packet.nameCount; i++)
                 {

                     string szRank = (curPage * 20 + (i + 1)).ToString();

                     PVPData.AddRankDataItem(szRank, packet.GetName(i), packet.GetJinyaodai(i).ToString());
                 }
             }

             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_USERLEVELRANK)
             {
                 for (int i = 0; i < packet.nameCount; i++)
                 {

                     string szRank = (curPage * 20 + (i + 1)).ToString();

                     PVPData.AddRankDataItem(szRank, packet.GetName(i), packet.GetLevel(i).ToString());
                 }
             }

             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOMBATRANK)
             {
                 for (int i = 0; i < packet.nameCount; i++)
                 {

                     string szRank = (curPage * 20 + (i + 1)).ToString();

                     PVPData.AddRankDataItem(szRank, packet.GetName(i), packet.GetCombatnum(i).ToString());
                 }
             }
             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_USERHPRANK)
             {
                 for (int i = 0; i < packet.nameCount; i++)
                 {

                     string szRank = (curPage * 20 + (i + 1)).ToString();

                     PVPData.AddRankDataItem(szRank, packet.GetName(i), packet.GetHp(i).ToString());
                 }
             }
			else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_ATTACK)
			{
				for (int i = 0; i < packet.nameCount; i++)
				{
					
					string szRank = (curPage * 20 + (i + 1)).ToString();
					
					PVPData.AddRankDataItem(szRank, packet.GetName(i), packet.GetZhanji(i).ToString());
				}
			}
             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_SHAOLINREPUTATION ||
                      packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_TIANSHANREPUTATION ||
                      packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_DALIREPUTATION ||
                      packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_XIAOYAOREPUTATION)
             {
                 for (int i = 0; i < packet.nameCount; i++)
                 {

                     string szRank = (curPage * 20 + (i + 1)).ToString();

                     PVPData.AddRankDataItem(szRank, packet.GetName(i), packet.GetZhanji(i).ToString());
                 }
             }
             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_GUILDCOMBAT)
             {
                for (int i = 0; i < packet.nameCount ; i++)
                {
                         
                    string szRank = (curPage * 20 + (i + 1)).ToString();

                    PVPData.AddRankDataItem(szRank, packet.GetName(i), packet.GetCombatnum(i).ToString());
                }
             }
             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_HUASHANPOS)
             {
                 for (int i = 0; i < packet.nameCount; i++)
                 {

                     string szRank = (curPage * 20 + (i + 1)).ToString();

                     PVPData.AddRankDataItem(szRank, packet.GetName(i));
                 }
             }
             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOIN)
             {
                 for (int i = 0; i < packet.nameCount; i++)
                 {

                     string szRank = (curPage * 20 + (i + 1)).ToString();

                     PVPData.AddRankDataItem(szRank, packet.GetName(i), packet.GetCoin(i).ToString());
                 }
             }
             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_MASTER)
             {
                 for (int i = 0; i < packet.nameCount; i++)
                 {

                     string szRank = (curPage * 20 + (i + 1)).ToString();

                     PVPData.AddRankDataItem(szRank, packet.GetName(i), packet.GetMastername(i), packet.GetTouchvalue(i).ToString());
                 }
             }
             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_USERCOSTYUANBAO)
             {
                 for (int i = 0; i < packet.nameCount; i++)
                 {

                     string szRank = (curPage * 20 + (i + 1)).ToString();
                     PVPData.AddRankDataItem(szRank, packet.GetName(i), packet.GetTotalcostyuanbao(i).ToString());
                 }
             }
             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_CHONGZHI)
             {
                 for (int i = 0; i < packet.nameCount; i++)
                 {
                     string szRank = (curPage * 20 + (i + 1)).ToString();
                     PVPData.AddRankDataItem(szRank, packet.GetName(i), packet.GetTotalChongZhi(i).ToString());
                 }
             }
             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_MASTERAVTIVECOMBAT)
             {
                 for (int i = 0; i < packet.nameCount; i++)
                 {

                     string szRank = (curPage * 20 + (i + 1)).ToString();

                     PVPData.AddRankDataItem(szRank, packet.GetName(i), packet.GetGuid(i).ToString());
                 }
             }
             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_SHAOSHISHAN)
             {
                 for (int i = 0; i < packet.nameCount; i++)
                 {

                     string szRank = (curPage * 20 + (i + 1)).ToString();
                     int nDiffcult = packet.GetTier(i);
                     string szDiffcult = "";
                     if (1 == nDiffcult)
                     {
                        szDiffcult = StrDictionary.GetClientDictionaryString("#{3041}");
                     }
                     else if (2 == nDiffcult)
                     {
                         szDiffcult = StrDictionary.GetClientDictionaryString("#{3042}");
                     }
                     else if (3 == nDiffcult)
                     {
                         szDiffcult = StrDictionary.GetClientDictionaryString("#{3043}");
                     }                                                        
                     string Sec = (packet.GetTime(i) % 60).ToString();
                     if (packet.GetTime(i) % 60 < 10)
                     {
                         Sec = "0" + (packet.GetTime(i) % 60).ToString();
                     }
                     string szTime = (packet.GetTime(i) / 60).ToString() + ":" + Sec;

                     PVPData.AddRankDataItem(szRank, packet.GetName(i), packet.GetLevel(i).ToString(), szDiffcult, szTime);
                 }
             }
             else if (packet.Type == (int)GameDefine_Globe.RANKTYPE.TYPE_TOTALONLINETIME)
             {
                 int nTotalOnlineTime = 0;
                 int nHour = 0;
                 int nMinute = 0;
                 int nSecond = 0;
                 for (int i = 0; i < packet.nameCount; i++)
                 {
                     string szRank = (curPage * 20 + (i + 1)).ToString();
                     nTotalOnlineTime = packet.GetTotalonlinetime(i);
                     nHour = nTotalOnlineTime / 3600;
                     nMinute = (nTotalOnlineTime % 3600)/60;
                     nSecond = nTotalOnlineTime %60;
                     string szOnlineTime = "";
                     szOnlineTime = StrDictionary.GetClientDictionaryString("#{3195}", nHour, nMinute, nSecond);
                     PVPData.AddRankDataItem(szRank, packet.GetName(i), szOnlineTime);
                 }
             }

             if (packet.Type != (int)GameDefine_Globe.RANKTYPE.TYPE_PRELIMINARYGUILDWARKILLRANK &&
                packet.Type  != (int)GameDefine_Globe.RANKTYPE.TYPE_PRELIMINARYGUILDWARRANK )
             {
				//====如果出现家族战跳转出排行榜界面，则因为前后端此枚举未对应上

                PVPData.ShowRank();
             }
                 

             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
