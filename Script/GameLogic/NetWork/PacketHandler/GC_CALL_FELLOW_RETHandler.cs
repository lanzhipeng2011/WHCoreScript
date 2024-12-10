//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using Games.GlobeDefine;
using Games.Fellow;
using Games.LogicObj;
namespace SPacket.SocketInstance
{
    public class GC_CALL_FELLOW_RETHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_CALL_FELLOW_RET packet = (GC_CALL_FELLOW_RET)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            UInt64 fellowGuid = packet.Guid;
            int fellowObjId = packet.Objid;
            if (fellowGuid == GlobeVar.INVALID_GUID)
            {
                return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            }

            FellowContainer container = GameManager.gameManager.PlayerDataPool.FellowContainer;
            if (container != null)
            {
                Fellow fellow = container.GetFellowByGuid(fellowGuid);
                if (fellow != null)
                {
                    //����Ϊ�ٳ�
                    fellow.Called = true;

                    if (PartnerFrameLogic.Instance() != null)
                    {
                        if (PartnerFrameLogic_Info.Instance()
                            && PartnerFrameLogic_Info.Instance().NewPlayerGuideFlag_Step == 2)
                        {
//                            if (MenuBarLogic.Instance())
//                            {
//                                MenuBarLogic.Instance().NewPlayerGuide(101);
//                            }
                        }
                        //UIManager.CloseUI(UIInfo.PartnerAndMountRoot);
                        PartnerFrameLogic.Instance().UpdatePartnerFrame();
                    }
                }
            }
            //���MainPlayer�жϣ���ȷ���ڵ���ʱ��Ϣ˳���ǹ����������
			if(Singleton<ObjManager>.GetInstance().MainPlayer)
           		Singleton<ObjManager>.GetInstance().MainPlayer.CurFellowObjId = fellowObjId;
            //�����ٳ���Ч
            GameManager.gameManager.PlayerDataPool.FellowPlayerEffect = true;
            //Obj_Character charObj = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(fellowObjId);
            //if (charObj != null)
            //{
            //    charObj.PlayEffect(52);
            //}

            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
