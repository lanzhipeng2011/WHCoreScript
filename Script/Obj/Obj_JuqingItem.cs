/********************************************************************
	文件名: 	Obj_JuqingItem.cs
	创建时间:	2015/12/28 14:55
	全路径:	TLOL\Version\Main\Project\Client\Assets\MLDJ\Script\Obj
	创建人:		
	功能说明:	夜袭大营辎重
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

public class Obj_JuqingItem : Obj
{
		private int m_ID =-1;
		public int ID
		{
			get { return m_ID; }
			set { m_ID = value; }
		}

		//是否被激活
		private bool  m_IsActive =false;
		public bool  IsActive
		{
			get { return m_IsActive; }
			set { m_IsActive = value; }
		}

		public Obj_JuqingItem()
		{
			m_ObjType = GameDefine_Globe.OBJ_TYPE.OBJ_JUQINGITEM;
		}
		public bool Init(ObjJuqingItem_Init_Data initData)
		{
			ID = initData.m_ID;
			if (null == m_ObjTransform)
				m_ObjTransform = transform;
			if (ObjEffectLogic == null)
			{
				ObjEffectLogic = gameObject.AddComponent<EffectLogic>();
				InitEffect();
			}
			PlayEffect (304);
			//SetOutLine();
			return true;
		}
		public void  DelaysendMsg()
		{
			//CG_JUQINGITEM_PLAYEFFECT msg=
			Invoke ("sendMsg", 1.0f);
			m_IsActive = false;
		}
		public void sendMsg()
		{
			if(Singleton<ObjManager>.GetInstance().MainPlayer.isHideWeapon==true)
				Singleton<ObjManager>.GetInstance().MainPlayer.HideOrShowWeanpon();
			PlayEffect (300);

		}
		public void  DelayRemoveEffct()
		{
			//CG_JUQINGITEM_PLAYEFFECT msg=
			Invoke ("Removeeffect", 2.0f);

		}
		public void Removeeffect()
		{
		 
			StopEffect (304);
		}



}
}