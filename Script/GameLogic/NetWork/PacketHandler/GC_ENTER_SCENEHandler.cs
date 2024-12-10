//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.LogicObj;
using Games.Events;
using Games.GlobeDefine;
using UnityEngine;

namespace SPacket.SocketInstance
{
    public class GC_ENTER_SCENEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ENTER_SCENE packet = (GC_ENTER_SCENE)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            
            //enter your logic
            //EnterScene为玩家登陆数据的一部分，另外一部分在CreatePlayer消息包中发送
            //记录登录场景的位置
//
            GameTrigger.Instance.Destory();


            float fEnterPosX = ((float)packet.PosX)/100;
            float fEnterPosZ = ((float)packet.PosZ)/100;

//		    float fEnterPosX = 29;
//		    float fEnterPosZ = 118;
            GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterScenePos = new UnityEngine.Vector3(fEnterPosX, 0, fEnterPosZ);
            GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterSceneServerID = packet.Mainplayerserverid;
            GameManager.gameManager.PlayerDataPool.EnterSceneCache.EnterSceneSceneID = packet.Sceneclass;
            SceneData.SceneInst = packet.Sceneinst;

            //进入切场景流程
            NetWorkLogic.GetMe().CanProcessPacket = false;
            GameEvent _event = new GameEvent(GameDefine_Globe.EVENT_DEFINE.EVENT_CHANGESCENE);
            _event.AddIntParam(packet.Sceneclass);

            Singleton<EventSystem>.GetInstance().PushEvent(_event);

			if (GameManager.gameManager.ActiveScene != null&&packet.Sceneclass != 21) 
            {
                GameManager.gameManager.ActiveScene.OnLoad(packet.Sceneclass);
            }
			//????????

			GameManager.gameManager.ChangeTimeScal (1.0f);
				return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
