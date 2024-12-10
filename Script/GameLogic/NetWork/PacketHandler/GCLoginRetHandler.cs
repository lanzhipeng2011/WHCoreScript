/********************************************************************************
 *	文件名：GCLoginRetHandler.cs
 *	全路径：	\NetWork\PacketHandler\GCLoginRetHandler.cs
 *	创建人：	王华
 *	创建时间：2013-11-29
 *
 *	功能说明： GCLoginRet包的处理逻辑
 *	       
 *	修改记录：
*********************************************************************************/

using System;
using System.Text;
using Google.ProtocolBuffers;

namespace SPacket.SocketInstance
{
	public enum LOGINSTATUS_ID
	{
		RET_PASSWORDERROR = -1,// use password errror
		RET_USERNOTEXISTS = -2,// user not exists
		RET_UIDERROR = -3,// use unique id error
		RET_SUCCESS = 1,// login success       
	}

    public class GCLoginRetHandler : Ipacket
    {
		public uint Execute(PacketDistributed packet)
        {
            GC_LOGIN_RET rPacket = (GC_LOGIN_RET)packet;

            Console.WriteLine();
            Console.WriteLine("OY, GameServer Send Messages:");
            Console.WriteLine("Result: " + rPacket.Result);
          
			 return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
            
        }
    }

}
