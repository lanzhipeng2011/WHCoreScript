//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using System.Collections.Generic;
using Games.LogicObj;
namespace SPacket.SocketInstance
{
    public class GC_DELETE_OBJHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_DELETE_OBJ packet = (GC_DELETE_OBJ)ipacket;
            if (null == packet) 
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            int serverId = packet.ServerId;
            if (serverId < 0)
            {
                return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            }

            Obj obj = Singleton<ObjManager>.GetInstance().FindObjInScene(serverId);
            if (null != obj)
            {
                //如果删除的目标为选择框目标，则取消选择框
                if (obj.gameObject == GameManager.gameManager.ActiveScene.SelectObj)
                {
                    GameManager.gameManager.ActiveScene.DeactiveSelectCircle();
					//obj.CancelOutLine();
                }

                //如果删除的目标为当前头像，则取消
                if (null != Singleton<ObjManager>.GetInstance().MainPlayer &&
                    Singleton<ObjManager>.GetInstance().MainPlayer.SelectTarget == obj)
                {
                    Singleton<ObjManager>.GetInstance().MainPlayer.OnSelectTarget(null);
                }

                //清除身上的绑定，防止显示错乱
                Obj_Character charobj = obj as Obj_Character;
                if (charobj != null)
                {
				
                    charobj.BindParent = -1;
                    List<int> emptylist = new List<int>();
                    charobj.UpdateBindChildren(emptylist);
//					if(charobj.m_IsMJ==true)
//					{
//						GameManager.gameManager.ChangeTimeScal(1.0f);
//						charobj.m_IsMJ=false;
//					}
                }
            }
		
            Singleton<ObjManager>.GetInstance().RemoveObj(serverId);

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
