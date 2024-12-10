//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using UnityEngine;
using Games.GlobeDefine;
namespace SPacket.SocketInstance
{
     public class GC_RET_QUIT_GAMEHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_RET_QUIT_GAME packet = (GC_RET_QUIT_GAME )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             LoginUILogic.m_LoginSelect = 0;
             if (packet.Type == (int)GC_RET_QUIT_GAME.GameSelectType.GAMESELECTTYPE_ACCOUNT)
             {
                 if (GameManager.gameManager.RunningScene != (int)(int)GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN)
                 {
                     LoginUILogic.m_LoginSelect = 1;
                     LoadingWindow.LoadScene(GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN);
                     OnClearAccount();
                 }
                 else
                 {
                     // 如果在登陆场景，直接返回服务器选择界面 
                     if (LoginUILogic.Instance() != null)
                     {
                         LoginUILogic.Instance().EnterServerChoose();
                     }
                 }
                 
             }
             else if (packet.Type == (int)GC_RET_QUIT_GAME.GameSelectType.GAMESELECTTYPE_ROLE)
             {
                 LoginUILogic.m_LoginSelect = 2;
                 LoadingWindow.LoadScene(GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN);
                 OnClearRole();
             }
             else if (packet.Type == (int)GC_RET_QUIT_GAME.GameSelectType.GAMESELECTTYPE_QUIT)
             {
                 Application.Quit();
             }

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
         public void OnClearAccount()   //清空账号相关的信息
         {
             OnClearRole();
             
         }
         public void OnClearRole()      //清空角色相关的信息
         {
//              if (Singleton<ObjManager>.Instance.MainPlayer)
//              {
//                  
//              }
             GameManager.gameManager.PlayerDataPool.TeamInfo.CleanUp();
             GameManager.gameManager.PlayerDataPool.CommonData.ClearData();
             GameManager.gameManager.PlayerDataPool.DailyLuckyDrawData.CleanUp();
             GameManager.gameManager.PlayerDataPool.ChatHistory.ClearData();
             GameManager.gameManager.PlayerDataPool.TitleInvestitive.ClearData();
             GameManager.gameManager.PlayerDataPool.ClearFashionData();             
             GameManager.gameManager.PlayerDataPool.GuildInfo.CleanUp();
             GameManager.gameManager.PlayerDataPool.guildList.CleanUp();
             GameManager.gameManager.PlayerDataPool.FriendList.CleanUp();
             GameManager.gameManager.PlayerDataPool.BlackList.CleanUp();
             GameManager.gameManager.PlayerDataPool.HateList.CleanUp();
             GameManager.gameManager.PlayerDataPool.TellChatSpeakers.ClearData();
             GameManager.gameManager.PlayerDataPool.LastTellGUID = GlobeVar.INVALID_GUID;
             GameManager.gameManager.PlayerDataPool.LastTellName = "";
         }
     }
 }
