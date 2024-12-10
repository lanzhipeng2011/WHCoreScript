//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using Games.LogicObj;
using GCGame.Table;
namespace SPacket.SocketInstance
 {
     public class GC_USE_ITEM_RETHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_USE_ITEM_RET packet = (GC_USE_ITEM_RET )ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
             //enter your logic
             Obj_MainPlayer MainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
             if (null == MainPlayer) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

             int success = packet.Success;
             int dataid = packet.Dataid;

             int _totalCDTime = 0;

             Tab_UsableItem _UsableInfo = TableManager.GetUsableItemByID(dataid, 0);
             if (_UsableInfo != null)
             {
                 int nCoolId = _UsableInfo.CoolDownId;
                 Tab_CoolDownTime _CDTimeInfo = TableManager.GetCoolDownTimeByID(nCoolId, 0);
                 if (_CDTimeInfo != null)
                 {
                     _totalCDTime = _CDTimeInfo.CDTime;
                 }
             }
             if (success == 1)
             {
                 if (_totalCDTime > 0 && PlayerFrameLogic.Instance())
                 {
                     if (MainPlayer.AutoHpID == dataid)
                     {
                         //PlayerFrameLogic.Instance().InitAutoFight();
                         GameManager.gameManager.PlayerDataPool.HpItemCDTime = _totalCDTime;

                     }  
                     else if (MainPlayer.AutoMpID == dataid)
                     {
                         //PlayerFrameLogic.Instance().InitAutoFight();
                         GameManager.gameManager.PlayerDataPool.MpItemCDTime = _totalCDTime;
                     }       
                 }                         
             }
         return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
