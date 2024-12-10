//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_CREATEROLE_RETHandler : Ipacket
    {
        public delegate void CreateRoltFailRet(GC_CREATEROLE_RET.CREATEROLE_RESULT result);
        public static CreateRoltFailRet retCreateRoleFail;
        public uint Execute(PacketDistributed ipacket)
        {
            GC_CREATEROLE_RET packet = (GC_CREATEROLE_RET)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            if (packet.Result == (int)GC_CREATEROLE_RET.CREATEROLE_RESULT.CREATEROLE_SUCCESS)
            {
       
                LoginData.loginRoleList.Add(new LoginData.PlayerRoleData(packet.PlayerGuid, packet.Profession, packet.PlayerName, 1, -1, -1, -1,0));
				LoginData.UpdateLoginRoleInfo(packet.PlayerGuid);
                PlatformHelper.OnRoleCreate();
                UserConfigData.AddRoleInfo();
               // NGUIDebug.Log2(packet.PlayerGuid.ToString());

            }
            else
            {
                if (null != retCreateRoleFail) retCreateRoleFail((GC_CREATEROLE_RET.CREATEROLE_RESULT)packet.Result);
            }
            
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}