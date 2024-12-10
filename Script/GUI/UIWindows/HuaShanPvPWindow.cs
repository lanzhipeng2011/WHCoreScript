using UnityEngine;
using System.Collections;
using GCGame;

public class HuaShanPvPWindow : MonoBehaviour
{

	// Use this for initialization

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    public void ReqHuaShanMemberList( )
    {
        CG_HUASHAN_PVP_MEMBERLIST packet = (CG_HUASHAN_PVP_MEMBERLIST)PacketDistributed.CreatePacket(MessageID.PACKET_CG_HUASHAN_PVP_MEMBERLIST);
        packet.None = 0;
        packet.SendPacket();
    }
}
