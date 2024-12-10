using UnityEngine;
using System.Collections;
using GCGame.Table;
using System.Collections.Generic;
using System.Text;
using Games.GlobeDefine;

public class VipData 
{
    public static int m_MaxShowLevel = 17;

    public static int GetVipLv()
    {
        int viplevel = 0;
        if (Singleton<ObjManager>.Instance.MainPlayer != null)
        {
            int nCost = Singleton<ObjManager>.Instance.MainPlayer.VipCost;
            int nLeft = 0;
            VipData.GetVipLevel(nCost, ref viplevel, ref nLeft);
        }
        return viplevel;
    }

    public static void GetVipLevel(int nCost, ref int rLevel, ref int rLeft)
    {
        if (nCost < 0)
        {
            rLeft = 0;
            rLevel = 0;
            return;
        }
        Dictionary<int, List<Tab_VipBook>> allData = TableManager.GetVipBook();
        foreach (int key in allData.Keys)
        {
            Tab_VipBook tBook = allData[key][0];
            if (nCost >= tBook.VipCost)
            {
                nCost -= tBook.VipCost;
                rLevel = tBook.Id;
                rLeft = nCost;
            }
            else
            {
                break;
            }
        }
    }

    public static string GetStarIconByLevel(int nLevel)
    {
        if (nLevel >= 1 && nLevel <= m_MaxShowLevel)
        {
            StringBuilder stringbuilt = new StringBuilder("");
            stringbuilt.AppendFormat("v{0}", nLevel);
            return stringbuilt.ToString();
        }
        else
        {
            return "";
        }
    }

    public static string GetVipImage(int nCost)
    {
        if (nCost < 0)
            return "";

        int nLevel = 0;
        int nLeft = 0;
        GetVipLevel(nCost, ref nLevel, ref nLeft);
        return GetStarIconByLevel(nLevel);
    }

	public static int GetVipStamina(int nLevel)
	{
		int nFinalCount = 0;
		foreach (KeyValuePair<int, List<Tab_StaminaBuyRule>> pair in TableManager.GetStaminaBuyRule())
		{
			Tab_StaminaBuyRule tabBuyRule = pair.Value[0];
			if (tabBuyRule == null)
			{
				continue;
			}
			
			if (tabBuyRule.VIPRequire <= nLevel)
			{
				nFinalCount = tabBuyRule.BuyNumMax;
			}
		}
		return nFinalCount;
	}

    public static int GetDeskIndex(int nLevel)
    {
        int lastIndex = -1;
        for (int n = 0; n < TableManager.GetRestaurantDesk().Count; ++n)
        {
            Tab_RestaurantDesk tab = TableManager.GetRestaurantDeskByID(n,0);
            if (tab != null && tab.OpenConditionType == 3)
            {
                if (nLevel >= tab.OpenConditionValue)
                {
                    lastIndex = n + 1;//名字叫做第n+1个桌子
                }                
            }
        }
        return lastIndex;
    }

	public static int GetVipCopySceneNum(int sceneid)
	{
        int nId = -1;
		if (sceneid == (int)GameDefine_Globe.TLI_COPYSCENEID.TLI_NUHAICHUJIAN)//22 过关斩将
        {
			nId = (int)Games.UserCommonData.USER_COMMONDATA.CD_VIPCP_NHCJ;//39  过关斩将
        }
		else if (sceneid == (int)GameDefine_Globe.TLI_COPYSCENEID.TLI_JUXIANZHUAN)//16 虎牢关
        {
			nId = (int)Games.UserCommonData.USER_COMMONDATA.CD_VIPCP_JXZ;//34 虎牢关
        }
		else if (sceneid == (int)GameDefine_Globe.TLI_COPYSCENEID.TLI_YANZIWU)//19 护送美人
		{
			nId = (int)Games.UserCommonData.USER_COMMONDATA.CD_VIPCP_YZW;//36 护送美人
        }
		else if (sceneid == (int)GameDefine_Globe.TLI_COPYSCENEID.TLI_YANWANGGUMO)//21夜袭大营
		{
			nId = (int)Games.UserCommonData.USER_COMMONDATA.CD_VIPCP_YWGM;//38  夜袭大营
		}
		else if (sceneid == (int)GameDefine_Globe.TLI_COPYSCENEID.TLI_SHAOSHISHAN)//31
        {
            nId = (int)Games.UserCommonData.USER_COMMONDATA.CD_VIPCP_SSS;//40
        }
		else if (sceneid == (int)GameDefine_Globe.TLI_COPYSCENEID.TLI_ZHENLONGQIJU)//28 七星玄阵
		{
			nId = (int)Games.UserCommonData.USER_COMMONDATA.CD_VIPCP_ZLQJ;//37 七星玄阵
		}
		else
        {
            return 0;
        }
        return GameManager.gameManager.PlayerDataPool.CommonData.GetCommonData(nId);
	}    

    public static int GetVipMaxCopySceneNum(int sceneid, int nCost)
    {
        if (nCost < 0)
            return 0;

        int nLevel = 0;
        int nLeft = 0;
        GetVipLevel(nCost, ref nLevel, ref nLeft);
        Tab_VipBook tBook = TableManager.GetVipBookByID(nLevel, 0);
        if (tBook != null)
        {
            if (sceneid == (int)GameDefine_Globe.TLI_COPYSCENEID.TLI_NUHAICHUJIAN)
            {
                return tBook.VipNHCJ;
            }
            else if (sceneid == (int)GameDefine_Globe.TLI_COPYSCENEID.TLI_JUXIANZHUAN)
            {
                return tBook.VipJXZ;
            }
            else if (sceneid == (int)GameDefine_Globe.TLI_COPYSCENEID.TLI_YANZIWU)
            {
                return tBook.VipYZW;
            }
            else if (sceneid == (int)GameDefine_Globe.TLI_COPYSCENEID.TLI_YANWANGGUMO)
            {
                return tBook.VipYWGM;
            }
            else if (sceneid == (int)GameDefine_Globe.TLI_COPYSCENEID.TLI_SHAOSHISHAN)
            {
                return tBook.VipSSS;
            }
            else if (sceneid == (int)GameDefine_Globe.TLI_COPYSCENEID.TLI_ZHENLONGQIJU)
            {
                return tBook.VipZLQJ;
            }
            else
                return 0;
        }
        return 0;
    }

    public static string MakeVipString(int nSceneID)
    {
        StringBuilder builder = new StringBuilder();        
        if (Singleton<ObjManager>.Instance.MainPlayer != null)
        {
            int nCost = Singleton<ObjManager>.Instance.MainPlayer.VipCost;
            if (nCost >= 0)
            {
                int nMax = 0;
                int nVip = 0;
                if (nSceneID > 0)
                {
                    nMax = GetVipMaxCopySceneNum(nSceneID, nCost);
                    nVip = GetVipCopySceneNum(nSceneID);
                }
                builder.AppendFormat(" VIP:{0}/{1}", nVip, nMax);
            }
        }
        return builder.ToString();
    }
}
