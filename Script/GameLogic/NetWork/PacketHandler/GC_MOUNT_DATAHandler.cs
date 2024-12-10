//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.LogicObj;
namespace SPacket.SocketInstance
{
     public class GC_MOUNT_DATAHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_MOUNT_DATA packet = (GC_MOUNT_DATA )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             int nObjServerID = packet.ObjServerID;
             int nMountID = packet.MountID;
             
             int EnterSceneServerID = GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterSceneServerID;
             // ÇÐ³¡¾°»º´æ
             if (EnterSceneServerID == nObjServerID)
             {
                 GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterSceneMountID = nMountID;
             }
             else
             {
                 Obj_Character obj = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(nObjServerID);
                 if (obj)
                 {
                     if (obj.ObjType == Games.GlobeDefine.GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER
                         || obj.ObjType == Games.GlobeDefine.GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
                     {
                         Obj_OtherPlayer Player = obj as Obj_OtherPlayer;
                         Player.RideOrUnMount(nMountID);
                     }
                 }
             }
             
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
}
