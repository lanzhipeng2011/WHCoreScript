//This code create by CodeEngine

using System;
using System.Collections.Generic;
using Games.ImpactModle;
using Games.LogicObj;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using UnityEngine;

namespace SPacket.SocketInstance
{
	public class GC_UPDATE_NEEDIMPACTINFOHandler : Ipacket
	{
		public uint Execute(PacketDistributed ipacket)
		{
			GC_UPDATE_NEEDIMPACTINFO packet = (GC_UPDATE_NEEDIMPACTINFO) ipacket;
			if (null == packet) return (uint) PACKET_EXE.PACKET_EXE_ERROR;
			//enter your logic
		//	GameManager.gameManager.PlayerDataPool.ClientImpactInfo.Clear ();
			
			for (int nImpactIndex = 0; nImpactIndex < packet.impactIdCount; nImpactIndex++)
			{
				if (packet.GetImpactId(nImpactIndex) == -1)
				{
					continue;
				}
				//??
				if (nImpactIndex < packet.isAddCount && packet.GetIsAdd(nImpactIndex) == 1)
				{
					ClientImpactInfo _impactInfo = new ClientImpactInfo();
					_impactInfo.CleanUp();
					_impactInfo.ImpactId = packet.GetImpactId(nImpactIndex);
					if (nImpactIndex < packet.impactLogicIdCount)
					{
						_impactInfo.ImpactLogicId = packet.GetImpactLogicId(nImpactIndex);
					}
					if (nImpactIndex < packet.isForeverCount)
					{
						_impactInfo.IsForever = (packet.GetIsForever(nImpactIndex) == 1 ? true : false);
					}
					if (nImpactIndex < packet.remainTimeCount)
					{
						_impactInfo.RemainTime = packet.GetRemainTime(nImpactIndex)/1.0f;
					}
					GameManager.gameManager.PlayerDataPool.ClientImpactInfo.Add(_impactInfo);
				}
				else //??
				{
					for (int i = 0; i < GameManager.gameManager.PlayerDataPool.ClientImpactInfo.Count; i++)
					{
						if (GameManager.gameManager.PlayerDataPool.ClientImpactInfo[i].ImpactId == packet.GetImpactId(nImpactIndex))
						{
							ClientImpactInfo _TmpInfo = new ClientImpactInfo();
							_TmpInfo.CleanUp();
							GameManager.gameManager.PlayerDataPool.ClientImpactInfo[i] =_TmpInfo;
						}
					}
					
				}
			}
			//?????
			List<ClientImpactInfo> impactList = new List<ClientImpactInfo>();
			for (int i = 0; i < GameManager.gameManager.PlayerDataPool.ClientImpactInfo.Count; ++i)
			{
				if (GameManager.gameManager.PlayerDataPool.ClientImpactInfo[i].IsVaild() == false)
				{
					impactList.Add(GameManager.gameManager.PlayerDataPool.ClientImpactInfo[i]);
				}
			}
			
			for (int i = 0; i < impactList.Count; ++i)
			{
				GameManager.gameManager.PlayerDataPool.ClientImpactInfo.Remove(impactList[i]);
			}
			//????????
			if (PlayerFrameLogic.Instance() != null)
			{
				PlayerFrameLogic.Instance().UpdateBuffIcon();
			}
			return (uint) PACKET_EXE.PACKET_EXE_CONTINUE;
		}
	}
}
