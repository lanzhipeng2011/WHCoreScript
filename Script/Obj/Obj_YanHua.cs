
/********************************************************************
    文件名: 	Obj_YanHua.cs
    创建时间:	2014-8-22
    全路径:	    E:\MDLJ\Main\Project\Client\Assets\MLDJ\Script\Obj
    创建人:		grx
    功能说明:	烟花
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
    public class Obj_YanHua : Obj
    {
        public Obj_YanHua()
        {
            m_ObjType = GameDefine_Globe.OBJ_TYPE.OBJ_YANHUA;
        }

        private int m_nOwerobjID = -1;
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

        private int m_nYanHuaId = -1;
        public int YanHuaId
        {
            get { return m_nYanHuaId; }
            set { m_nYanHuaId = value; }
        }

        public bool Init(ObjYanHua_Init_Data initData)
        {
            if (null == m_ObjTransform)
                m_ObjTransform = transform;
            //服务器发过来的信息               
            ServerID = initData.m_ServerID;
            YanHuaId = initData.m_nYanHuaID;
            OwerobjID = initData.m_OwnerObjId;
            OwnerGuid = initData.m_OwerGuid;
            m_ObjTransform.position = ActiveScene.GetTerrainPosition(new Vector3(initData.m_fX, 0, initData.m_fZ));
            Obj_Character ownerCharacter = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(OwerobjID);
            
            if (ownerCharacter != null)
            {
                Vector3 newPosVector3 = m_ObjTransform.position;
                newPosVector3.y = ownerCharacter.ObjTransform.position.y;
                m_ObjTransform.position = newPosVector3;
            }

            //初始化特效
            if (ObjEffectLogic == null)
            {
                ObjEffectLogic = gameObject.AddComponent<EffectLogic>();
                InitEffect();
            }
            //播放特效
            Tab_YanHua TabYanHua = TableManager.GetYanHuaByID(YanHuaId, 0);
            if (TabYanHua != null)
            {
                int AliveEffectId = TabYanHua.AliveEffectId;
                if (AliveEffectId != -1 && ObjEffectLogic != null)
                {
                    PlayEffect(AliveEffectId);
                }
            }
            return true;
        }
    }
}
