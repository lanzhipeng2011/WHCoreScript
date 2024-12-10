// 修改记录：
//     2014-5-28 Lijia: 客户端效率优化，把OwnSkillData从class改为struct
using System.Runtime.CompilerServices;
using Games.GlobeDefine;
using UnityEngine;
using System.Collections;
using GCGame.Table;
using System.Collections.Generic;
namespace Games.SkillModle
{
	public enum SKILLSELLOGIC
	{
		SELLOGIC_INVALID = -1,
		SELLOGIC_SINGLE = 0,
		SELLOGIC_MULTI = 1,
		SELLOGIC_ALL = 2,
		
	}
	
	public enum SKILLUSEFAILTYPE
	{
		INVALID =-1,
		DISTANCE =0,
	}
	public enum SKILLFINISHREASON
	{
		BREAK = 1,
		FINISH = 2,
	}
	
	public enum SKILLUSETYPE
	{
		SHUNFA = 0,//瞬发
		YINCHANG = 1,//吟唱
		BEIDONG =2,//被动
	}
	public enum SKILLBASEID
	{
		XFZBASEID = 2006,//摩诃无量的的baseID
		YTWJID = 307,//以退为进baseID
		YGSDID = 204,//阳关三叠baseID
		HDWLID = 306,//画地为牢 
		ZLZ = 404,  //祝融掌
	}
	public enum SKILLDEFINE
	{
		MAX_SKILLNUM = 15, //主角最大技能数
		PUBLICCDID = 14,//公共冷却ID
	}
	//每个int位表示一种类别 符合两种及以上的累加
	public enum SKILLCLASS
	{
		INITIATIVE=1,//主动技
		PASSIVITY =2,//被动技
		AUTOREPEAT =4,//自动连续技
		XP =8,//XP技
		CHONGFENG =16,//冲锋技
		DUTIAO =32,//读条
		MASTERSKILL = 64,   //师门技能
		GAINSKILL = 128,    //加持技能
	}
	
	//技能范围特效 类别
	public enum SKILLRANGEEFFECTTYPE
	{
		INVALID =-1,
		RING =0,//环
		CIRCLE =1,//圆
		RECT =2,//矩形
		ARROWS =3,//箭头
		SICIRCLE=4,//半圆
	}
	
	public enum SKILLRANGEEFFECTTAR
	{
		INVAILD =-1,
		SELF =0,//自身
		SELECTTARGET =1,//选中目标
	}
	public enum SKILLRANGEEFFECTID
	{
		INVALID =-1,
		RINGEFFECTID =100,//环
		CIRCLEEFFECTID = 101,//圆
		RECTEFFECTID = 102,//矩形
		ARROWSEFFECTID = 103,//箭头
		SICIRCLEEFFECTID = 104,//半圆圆

	}
	public struct OwnSkillData
	{

		public bool IsValid()
		{
			return (m_nSkillId != -1);
		}
		private int m_nSkillId;
		public int SkillId
		{
			get { return m_nSkillId; }
			set { m_nSkillId = value; }
		}
		private int m_nCDTime; //单位：毫秒
		public int CDTime
		{
			get { return m_nCDTime; }
			set { m_nCDTime = value; }
		}
		public void CleanUp()
		{
			m_nSkillId = -1;
			m_nCDTime = 0;
			m_PriorityAutoCombat = -1;
			m_CanAutoCombat = true;
		}
		
		private int m_PriorityAutoCombat; //挂机时使用的优先级
		public int PriorityAutoCombat
		{
			get { return m_PriorityAutoCombat; }
			set { m_PriorityAutoCombat = value; }
		}
		private bool  m_CanAutoCombat; //挂机时能否使用默认都能使用
		public bool CanAutoCombat
		{
			get { return m_CanAutoCombat; }
			set { m_CanAutoCombat = value; }
		}
	}
	
	public  struct MultiShowDamageBoard //多次显示 
	{
		public void CleanUp()
		{
			m_nShowValue = 0;
			m_nShowTimes = -1;
			m_fShowInter = -1.0f;
			m_nDamageType = GameDefine_Globe.DAMAGEBOARD_TYPE.PLAYER_TYPE_INVALID;
			m_fLastShowTime = 0;

		}
		private int m_nShowValue;
		//public List<int> ShowValuList;
		public int ShowValue
		{
			get { return m_nShowValue; }
			set { m_nShowValue = value; }
		}
		private int m_nShowTimes;
		public int ShowTimes
		{
			get { return m_nShowTimes; }
			set { m_nShowTimes = value; }
		}
		private float m_fShowInter;
		public float ShowInter
		{
			get { return m_fShowInter; }
			set { m_fShowInter = value; }
		}
		
		private GameDefine_Globe.DAMAGEBOARD_TYPE m_nDamageType;
		public GameDefine_Globe.DAMAGEBOARD_TYPE DamageType
		{
			get { return m_nDamageType; }
			set { m_nDamageType = value; }
		}
		
		private float m_fLastShowTime;
		public float LastShowTime
		{
			get { return m_fLastShowTime; }
			set { m_fLastShowTime = value; }
		}
	}
	
}

