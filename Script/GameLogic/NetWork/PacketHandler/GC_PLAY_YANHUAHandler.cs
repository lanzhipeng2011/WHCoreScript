//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using Games.GlobeDefine;
using Games.Fellow;
using Games.LogicObj;
using GCGame.Table;
namespace SPacket.SocketInstance
{
    public class GC_PLAY_YANHUAHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_PLAY_YANHUA packet = (GC_PLAY_YANHUA)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            int nEffectID =  packet.Effectid;
            if (BackCamerControll.Instance() != null)
            {
                BackCamerControll.Instance().PlaySceneEffect(nEffectID);
            }
            Tab_CommonItem TabItem= TableManager.GetCommonItemByID(packet.Itemid, 0);
            if (TabItem != null)
            {
                string strMsg = StrDictionary.GetClientDictionaryString("#{3050}", packet.Username, TabItem.Name);              
                GUIData.AddNotifyData(strMsg);
            }
           
            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
