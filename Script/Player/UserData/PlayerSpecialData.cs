/********************************************************************
	日期:	2014/03/05
	文件: 	PlayerAuto.cs
	路径:	D:\work\code\mldj\Version\Main\Project\Client\Assets\MLDJ\Script\Player\UserData\PlayerAuto.cs
	作者:	YangXin
	描述:	玩家挂机设置
	修改:	
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PlayerSpecialData
{
	//public int AutoFightOpenToggle;   //自动打怪
	public bool  m_bisFirstYeXiDaYing;            //自动捡东西

	public PlayerSpecialData( bool  isFirstYeXiDaYing)
	{
		//AutoFightOpenToggle = fight? 1 : 0;    //自动打怪
		m_bisFirstYeXiDaYing = isFirstYeXiDaYing;           //自动捡东西

	}
	

}

