using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Clojure;
using GCGame.Table;
public class CreateBatchNpcAction : BlockAction
{
    int batchId;
   
    int CreateNumber;
    int addNumber;
    public override void OnLoad(List<ASTNode> args)
    {
        this.batchId = (int)((NumberNode)args[1]).NumberVal;
    }

    public override void OnExec()
    {
        ObjManager.Instance.CreateCharacterCallBack += OnCreateCharacter;

        List<Tab_NewPlayTutorialNpc> lstNpc = TableManager.GetNewPlayTutorialNpcByID(batchId);
        if (lstNpc == null)
        {
            Debug.LogError("NewPlayTutorialNpc表中不存在 " + batchId.ToString() + " 波");
            return;
        }
        for (int i = 0; i < lstNpc.Count; i++)
        {
            Tab_NewPlayTutorialNpc npcData = lstNpc[i];
            CG_ASK_NEWPLAYER_CREATE_NPC packet = (CG_ASK_NEWPLAYER_CREATE_NPC)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_NEWPLAYER_CREATE_NPC);
            packet.NpcID = npcData.DataID;
            packet.NpcX = (int)npcData.PosX ;
            packet.NpcZ = (int)npcData.PosZ ;
            packet.NpcFacedir = (int)npcData.FaceDirection;
            packet.SendPacket();
        }
        CreateNumber = lstNpc.Count;
    }

    public void OnCreateCharacter(Obj_Init_Data data) 
    {
        addNumber++;
      //如果X值和本波最后一个怪X值相等
        if (addNumber == CreateNumber)
        {
            this.IsFinish = true;
        }
    }

    public override void OnExit()
    {
        ObjManager.Instance.CreateCharacterCallBack -= OnCreateCharacter;
    }
}
