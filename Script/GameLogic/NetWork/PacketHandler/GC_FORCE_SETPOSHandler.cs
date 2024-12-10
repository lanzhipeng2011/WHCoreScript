//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.LogicObj;
using UnityEngine;
using Games.Scene;

namespace SPacket.SocketInstance
{
    public class GC_FORCE_SETPOSHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_FORCE_SETPOS packet = (GC_FORCE_SETPOS )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            Obj obj = Singleton<ObjManager>.Instance.FindObjInScene(packet.ServerID);
            if (obj != null)
            {
                if (obj.ObjType == Games.GlobeDefine.GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
                    obj.ObjType == Games.GlobeDefine.GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
                    obj.ObjType == Games.GlobeDefine.GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER ||
                    obj.ObjType == Games.GlobeDefine.GameDefine_Globe.OBJ_TYPE.OBJ_FELLOW ||
                    obj.ObjType == Games.GlobeDefine.GameDefine_Globe.OBJ_TYPE.OBJ_NPC)
                {
                    Obj_Character objChar = obj as Obj_Character;
                    Vector3 vec = new Vector3((float)packet.PosX / 100, 0, (float)packet.PosZ / 100);

                    if (objChar.NavAgent != null)
                    {
                        UnityEngine.GameObject.DestroyImmediate(objChar.NavAgent);
                    }
                    objChar.Position = ActiveScene.GetTerrainPosition(vec);
                    if (objChar.NavAgent == null)
                    {
                        objChar.InitNavAgent();
                    }                    
                }                
            }
            
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
