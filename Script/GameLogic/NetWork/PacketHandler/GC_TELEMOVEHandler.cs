//This code create by CodeEngine

using System;
using Games.GlobeDefine;
using Games.LogicObj;
using GCGame;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using UnityEngine;
using Games.Scene;

namespace SPacket.SocketInstance
 {
    public class GC_TELEMOVEHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_TELEMOVE packet = (GC_TELEMOVE )ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
            Obj_Character _objChar = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(packet.ObjId);
            if (_objChar ==null)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE; 
            }
            Vector3 targetPos =new Vector3(packet.TargetPosX/100.0f,0,packet.TargetPosZ/100.0f);

			targetPos  = ActiveScene.GetTerrainPosition(targetPos);
			//targetPos=Singleton<ObjManager>.GetInstance().MainPlayer.gameObject.transform.position;
            //�Ƿ���Ҫ�ı䳯��
            if (packet.NeedChangeFaceto ==1)
            {
                _objChar.FaceTo(targetPos);
            }
            else
            {
                _objChar.IsMoveToNoFaceTo = true;
            }
            AutoMove autoMove = _objChar.GetComponent<AutoMove>();
            if (autoMove!=null)
            {
                autoMove.ResetAutoMove();
            }
            //��Ŀ����ƶ�
            _objChar.MoveTo(targetPos, null, 2.0f);
            //�Ƿ���Ҫ���Ÿ��Ӷ���
            if (packet.HasAnimaId && _objChar.AnimLogic!=null)
            {
                _objChar.AnimLogic.Stop();
                _objChar.AnimLogic.Play(packet.AnimaId);
                //�������ﲥ�Ż���Ч�� todo��ʱ��
                _objChar.PlayEffect(121);
            }
           
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
 }
