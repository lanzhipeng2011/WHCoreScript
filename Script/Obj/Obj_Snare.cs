/********************************************************************
	文件名: 	Obj_Snare.cs
	创建时间:	2014/05/11 14:55
	全路径:	TLOL\Version\Main\Project\Client\Assets\MLDJ\Script\Obj
	创建人:		罗勇
	功能说明:	陷阱
	修改记录:
*********************************************************************/

using System;
using Games.Animation_Modle;
using Games.GlobeDefine;
using GCGame.Table;
using UnityEngine;
using System.Collections;
using Games.Scene;

namespace Games.LogicObj
{
    public class Obj_Snare : Obj
    {
        public Obj_Snare()
        {
            m_ObjType = GameDefine_Globe.OBJ_TYPE.OBJ_SNARE;
        }

        private int m_nOwerobjID =-1;
        public int OwerobjID
        {
            get { return m_nOwerobjID; }
            set { m_nOwerobjID = value; }
        }
        private UInt64 m_OwnerGuid = GlobeVar.INVALID_GUID;
        public System.UInt64 OwnerGuid
        {
            get { return m_OwnerGuid; }
            set { m_OwnerGuid = value; }
        }

        private int m_nSnareId = -1;
        public int SnareId
        {
            get { return m_nSnareId; }
            set { m_nSnareId = value; }
        }

        public bool Init(ObjSnare_Init_Data initData)
        {
            if (null == m_ObjTransform)
            {
                m_ObjTransform = transform;
            }

            //服务器发过来的信息               
            ServerID = initData.m_ServerID;
            SnareId = initData.m_SnareID;
            OwerobjID = initData.m_OwnerObjId;
            OwnerGuid = initData.m_OwerGuid;
            m_ObjTransform.position = ActiveScene.GetTerrainPosition(new Vector3(initData.m_fX, 0, initData.m_fZ));
            Obj_Character ownerCharacter = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(OwerobjID);
            //修正陷阱的高度和陷阱释放者一样
            if (ownerCharacter !=null)
            {
                Vector3 newPosVector3 = m_ObjTransform.position;
                newPosVector3.y = ownerCharacter.ObjTransform.position.y;
                m_ObjTransform.position =newPosVector3;
            }
           
            //初始化特效
            if (ObjEffectLogic == null)
            {
                ObjEffectLogic =gameObject.AddComponent<EffectLogic>();
                InitEffect();
            }
            
            Tab_SnareObjInfo SnareInfo = TableManager.GetSnareObjInfoByID(SnareId, 0);
            if (SnareInfo!=null)
            {
                //播放生存期特效
                for (int i = 0; i < SnareInfo.getAliveEffectIdCount(); i++)
                {
                    int AliveEffectId = SnareInfo.GetAliveEffectIdbyIndex(i);
                    if (AliveEffectId != -1 && ObjEffectLogic != null)
                    {
                        PlayEffect(AliveEffectId);
                    }
                }
                //播放生存期音效
                for (int i = 0; i < SnareInfo.getAliveSoundIdCount(); i++)
                {
                    int AliveSoundId = SnareInfo.GetAliveSoundIdbyIndex(i);
                    if (AliveSoundId != -1)
                    {
                       GameManager.gameManager.SoundManager.PlaySoundEffect(AliveSoundId);
                    }
                }
            }
            return true;
        }
    }
}
