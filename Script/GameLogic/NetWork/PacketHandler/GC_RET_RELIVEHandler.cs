//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.LogicObj;
using UnityEngine;
using Games.GlobeDefine;
using Games.Scene;

namespace SPacket.SocketInstance
 {
     public class GC_RET_RELIVEHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_RET_RELIVE packet = (GC_RET_RELIVE )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic

             Obj_MainPlayer MainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
             if (null == MainPlayer) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

             int objId = packet.ObjId;
             Obj_Character TargetObj = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(objId);
             if (TargetObj == null) return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;

             if(objId == MainPlayer.ServerID)
             {
                 // 自己
                 if (packet.HasReviveTime)
                 {
                     MainPlayer.ReliveEntryTime = packet.ReviveTime;
                 }
             }
             if (packet.HasPos_x && packet.HasPos_z && packet.HasFacedir)
             {
                 float fEnterPosX = ((float)packet.Pos_x) / 100;
                 float fEnterPosZ = ((float)packet.Pos_z) / 100;
                 Vector3 newPosition = new UnityEngine.Vector3(fEnterPosX, 0, fEnterPosZ);
//                 if (GameManager.gameManager.ActiveScene.IsT4MScene())
//                 {
//                     newPosition.y = GameManager.gameManager.ActiveScene.GetTerrainHeight(newPosition);
//                 }
//                 else if (null != Terrain.activeTerrain)
//                 {
//                     newPosition.y = GameManager.gameManager.ActiveScene.GetNavSampleHeight(newPosition);
//                 }

                 //Temp，保证版本正确性
                 newPosition = ActiveScene.GetTerrainPosition(newPosition);
//                  if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_ERHAI)
//                  {
//                      newPosition.y = 18.0f;
//                  }
//                  else if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YANGWANGGUMU
//                      || GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WANGYOUGU
//                      || GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_TIANSHAN)
//                  {
//                      newPosition.y = 22.5f;
//                  }
                 
                 TargetObj.Position = newPosition;
             }

             //主角位置特殊处理
             if (objId == MainPlayer.ServerID)
             {
                 MainPlayer.LastSyncPos = MainPlayer.Position;
                 if (null == MainPlayer.NavAgent)
                 {
                     MainPlayer.InitNavAgent();
                     MainPlayer.NavAgent.destination = MainPlayer.Position;
                 }
             }

             TargetObj.OnRelife();
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }

