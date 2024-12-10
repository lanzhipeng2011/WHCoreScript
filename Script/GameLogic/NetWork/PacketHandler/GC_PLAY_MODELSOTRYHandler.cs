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
    public class GC_PLAY_MODELSOTRYHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_PLAY_MODELSOTRY packet = (GC_PLAY_MODELSOTRY)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            if (packet.ModelStoryID == GlobeVar.YanMenGuan_ModelStoryID)
            {               
                Obj_MainPlayer mainplayer = Singleton<ObjManager>.Instance.MainPlayer;
                if (mainplayer == null)
                {
                    return (uint)PACKET_EXE.PACKET_EXE_ERROR;
                }
                //mainplayer.CameraController.InitCameraTrack(mainplayer.Position, curBoss.Position);
                mainplayer.ModelStoryID = packet.ModelStoryID;

                mainplayer.StopMove();
                Vector3 posTarget = new Vector3(13.9f, 0, 27.4f);
                Vector3 posTargetTerrain = ActiveScene.GetTerrainPosition(posTarget);
                mainplayer.MoveTo(posTargetTerrain);           
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
