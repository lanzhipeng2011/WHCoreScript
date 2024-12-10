//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_UI_OPERATIONHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_UI_OPERATION packet = (GC_UI_OPERATION)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            if (packet.Operation == (int)GC_UI_OPERATION.UIOPERATION.UI_SHOW)
            {
                UIManager.ShowUIByID(packet.TableID);
            }
            else if (packet.Operation == (int)GC_UI_OPERATION.UIOPERATION.UI_CLOSE)
            {
                UIManager.CloseUIByID(packet.TableID);
            }
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
