/********************************************************************
	文件名: 	ConsignSaleData.cs
	创建时间:	2014/06/12 13:09
	全路径:	\Version\Main\Project\Client\Assets\MLDJ\Script\Player\ConsignSale
	创建人:		luoy
	功能说明:	寄售行数据结构
	修改记录:
*********************************************************************/
using Games.Item;
using UnityEngine;
using System.Collections;
namespace Games.ConsignSale
{
    public struct ConsignSaleSearchInfo
    {
        private GameItem m_ItemInfo;
        public Games.Item.GameItem ItemInfo
        {
            get { return m_ItemInfo; }
            set { m_ItemInfo = value; }
        }
        private int m_nPrice;
        public int Price
        {
            get { return m_nPrice; }
            set { m_nPrice = value; }
        }

        private string m_OwnerName;
        public string OwnerName
        {
            get { return m_OwnerName; }
            set { m_OwnerName = value; }
        }
        public void CleanUp()
        {
            m_ItemInfo = new GameItem();
            m_nPrice = 0;
            m_OwnerName = "";
        }
    }
    public struct MyConsignSaleItemInfo
    {
        private GameItem m_ItemInfo;
        public Games.Item.GameItem ItemInfo
        {
            get { return m_ItemInfo; }
            set { m_ItemInfo = value; }
        }
        private int m_nRemainTime;
        public int RemainTime
        {
            get { return m_nRemainTime; }
            set { m_nRemainTime = value; }
        }
        private int m_nPrice;
        public int Price
        {
            get { return m_nPrice; }
            set { m_nPrice = value; }
        }


        public void CleanUp()
        {
            m_ItemInfo = new GameItem();
            m_nRemainTime = 0;
            m_nPrice = 0;
        }
    }
    /// <summary>
    /// !!!与服务器保持一致
    /// </summary>
    public enum ConsignSale_SearchClass //寄售行分类
    {
        INVAILD_TYPE = -1,
        WEAPON = 0,          //武器
        FANGJU,             //防具
        CHARM,              //饰品
        MEDIC,              //药品
        MATERIAL,           //材料(生活技能材料 打造图)
        ALL,                //全部
    }
    /// <summary>
    /// !!!与服务器保持一致
    /// </summary>
    public enum ConsignSale_SearchSubClass //寄售行筛选子分类
    {
        INVAILD_TYPE = -1,
        WEAPON_SHAOLIN = 0,  //棍
        WEAPON_TIANSHAN,    //双短
        WEAPON_DALI,        //单手剑
        WEAPON_XIAOYAO,     //符
        CUFF,               //护腕
        HEAD,               //帽子
        ARMOR,              //上衣
        LEG_GUARD,          //裤子
        SHOES,              //鞋子
        CHARM1,             //吊坠
        CHARM2,             //戒指
        HPMEDIC,            //血药
        MPMEDIC,            //蓝药
        AMULET,             //护符
        LUEPRINT,           //打造图
        LIFE_MATERIAL,      //生活材料
        ATTRMEDIC,          //属性药
        ALL,                //全部
    }
    /// <summary>
    /// !!!与服务器保持一致
    /// </summary>
    public enum ConsignSale_SortClass //排序类别
    {
        QUALITY = 0,         //品质排序
        LEVEL,             //等级排序
        COUNT,             //数量排序
        PRICE,             //价格排序
        MAXNUM,            //总数
    }
    /// <summary>
    /// !!!与服务器保持一致
    /// </summary>
    public enum ConsignSale_SortType //查询方式
    {
        UP = 0, //顺序由小到大
        DOWN, //逆序  由大到小
    }
    /// <summary>
    /// !!!与服务器保持一致
    /// </summary>
    public enum ConSignSaleSearchSelfCode //查询上架物品的方式
    {
        INVAILD_CODE = -1,
        CLIENTASK = 0,           //客户端请求数据
        SALECHECK,              //上架前的查询
        SALEUPDATE,             //上架成功的更新
        CANCELUPDATE,           //下架成功的更新
    }
    /// <summary>
    /// !!!与服务器保持一致
    /// </summary>
    public enum ConsignSaleData
    {
        MAXCOUNTSANDTOCLIENT = 20,
    }
}
