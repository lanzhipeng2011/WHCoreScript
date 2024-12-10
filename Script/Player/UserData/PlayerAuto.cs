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

public class PlayerAuto
{
    //public int AutoFightOpenToggle;   //自动打怪
    public int AutoPickUp;            //自动捡东西
    public int AutoTaem;              //自动组队
	public int AutoAcceptTeam;//   队长自动接收入队申请
    public int AutoHpPercent;         //血百分比
    public int AutoMpPercent;         //蓝百分比
    public int AutoHpID;                //血ID
    public int AutoMpID;                //蓝ID
    public int AutoBuyDrug;           //自动买药
    //public int AutoRadius;            //怪的范围       
    public int AutoNotice;            //每天只弹出一次公告
    public int AutoIsSelectDrug;        //是否选择过药品
    public UInt64 AutoEquipGuid;
	public int [] AutoSkills=new int[10];//挂机时使用技能
    public PlayerAuto(int pickup,bool team,bool  acceptteam,int hp,int mp,bool buy,int notice,int hpid, int mpid, bool selectDrug, UInt64 EquipGuid ,bool []autoskills)
    {
        //AutoFightOpenToggle = fight? 1 : 0;    //自动打怪
        AutoPickUp = pickup;           //自动捡东西
        AutoTaem = team ? 1 : 0;             //自动组队
		AutoAcceptTeam = acceptteam ? 1 : 0;             //队长自动组队
        AutoHpPercent = hp;         //血百分比
        AutoMpPercent = mp;         //蓝百分比
        AutoBuyDrug = buy ? 1 : 0;           //自动买药
        //AutoRadius = radius;            //怪的范围       
        AutoNotice = notice;            //每天只弹出一次公告
        AutoHpID = hpid;
        AutoMpID = mpid;
        AutoIsSelectDrug = selectDrug ? 1 : 0;   //是否选择过药品
        AutoEquipGuid = EquipGuid;
		for (int i=0; i<10; i++) 
		{
		AutoSkills[i]=autoskills[i]?1:0;
		}
    }


   // public static List<PlayerAuto> PlayerAutoList = new List<PlayerAuto>();
}

