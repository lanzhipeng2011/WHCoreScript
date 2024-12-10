/********************************************************************
	日期:	2014-9-26
	文件: 	RestaurantConfigData.cs
	路径:	D:\work\code\mldj\Version\Main\Project\Client\Assets\MLDJ\Script\Player\UserData\RestaurantConfigData.cs
	描述:	酒楼设置
	修改:	
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RestaurantConfigData
{

   
    private int[] m_nRestaurantFilterLv = new int[RestaurantData.FoodLevelMax];
    private int m_nRestaurantFilterCoin;
    private int m_nRestaurantFilterExp;
    private int m_nRestaurantFilterMeterial;

    public RestaurantConfigData()
    {
        
        m_nRestaurantFilterCoin = 1;
        m_nRestaurantFilterExp = 1;
        m_nRestaurantFilterMeterial = 1;
        for (int i = 0; i < m_nRestaurantFilterLv.Length; i++)
        {
            m_nRestaurantFilterLv[i] = 1;
        }
    }

    public  void SetRestaurantFilterLv(int nFilterLevel, int nFilter)
    {
        if (nFilterLevel > 0 && nFilterLevel <= m_nRestaurantFilterLv.Length)
        {
            m_nRestaurantFilterLv[nFilterLevel - 1] = nFilter;
        }
    }

    public  int GetRestaurantFilterLv(int nFilterLevel)
    {
        if (nFilterLevel > 0 && nFilterLevel <= m_nRestaurantFilterLv.Length)
        {
            return m_nRestaurantFilterLv[nFilterLevel - 1];
        }
        return 1;
    }


    public  int RestaurantFilterExp
    {
        set
        {
            m_nRestaurantFilterExp = value ;
        }
        get
        {
            return m_nRestaurantFilterExp;
        }
    }
    public  int RestaurantFilterCoin
    {
        set
        {
            m_nRestaurantFilterCoin = value; 
        }
        get
        {
            return m_nRestaurantFilterCoin;
        }
    }
    public  int RestaurantFilterMeterial
    {
        set
        {
            m_nRestaurantFilterMeterial = value; 
        }
        get
        {
            return m_nRestaurantFilterMeterial;
        }
    }
}

