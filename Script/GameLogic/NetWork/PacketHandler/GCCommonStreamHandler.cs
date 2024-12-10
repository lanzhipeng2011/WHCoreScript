/********************************************************************************
 *	文件名：GCCommonStreamHandler.cs
 *	全路径：	\NetWork\PacketHandler\GCCommonStreamHandler.cs
 *	创建人：	王华
 *	创建时间：2013-11-29
 *
 *	功能说明： Server返回给客户端的公共包的处理
 *	       
 *	修改记录：
*********************************************************************************/

using System;
using System.Collections.Generic;
using Google.ProtocolBuffers;

 namespace SPacket.SocketInstance
{
    //Server回包
	 public class GCLoginRet : PacketFactory
    {
		 public MessageID GetPacketID()
		 {
			 return MessageID.PACKET_GC_LOGIN_RET;
		 }
        public Ipacket CreatePacket()
        {
            return new GCLoginRetHandler();
        }
    }


}
