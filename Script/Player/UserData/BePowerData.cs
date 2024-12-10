using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using System.Collections.Generic;
using Games.LogicObj;
using GCGame;
using GCGame.Table;

public class BePowerData
{
    public enum BePowerType
    {
        BPTDEFINE_Null = 0,
        BPTDEFINE_EQUIP,
        BPTDEFINE_BELLE,
        BPTDEFINE_GEM,
        BPTDEFINE_XIAKE,
        BPTDEFINE_SKILL,
        BPTDEFINE_FELLOW,
        BPTDEFINE_MAX,
    }


    public enum BePowerWayDefine
    {
        EBPWD_Null = 0,
        EBPWD_COPYSCENE_JUXIANZHUANG = 1,
        EBPWD_COPYSCENE_CANGJINGE=2,
        EBPWD_COPYSCENE_SHAOSHISHAN=3,
        EBPWD_EQUIP_UPGRADE=4,
        EBPWD_EQUIP_STAR=5,
        EBPWD_BELLE_TUJIAN=6,
        EBPWD_BELLE_ZHENFA=7,
        EBPWD_SHOP_QIANGHUA=8,
        EBPWD_SKILL_LEVELUP=9,
        EBPWD_FELLOW_GET=10,
        EBPWD_FELLOW_UPGRADE=11,
        EBPWD_FELLOW_STAR=12,
        EBPWD_FELLOW_SKILL=13,
        EBPWD_XIAKE=14,
        EBPWD_COPYSCENE_ZHENLONGQIJU = 15,
        EBPWD_COPYSCENE_NUHAICHUJIAN = 16,
        EBPWD_COPYSCENE_YANZIWU = 17,
        EBPWD_COPYSCENE_YANWANGGUMU = 18,
        EBPWD_WAY_MAX
    }

    public enum BePowerLevel
    {
        EBPL_LEVEL_0 = 0,
        EBPL_LEVEL_1 = 1,
        EBPL_LEVEL_2 = 2,
        EBPL_LEVEL_3 = 3,
        EBPL_LEVEL_4 = 4,
        EBPL_LEVEL_5 = 5,
    }

    public struct BePowerListItemData
    {

        public BePowerListItemData( int _type, int _titleDict,
            int _descDict, int _btnDict, int _function, int _btnDict2, int _function2)
        {
            type = _type;
            titleDict = _titleDict;
            descDict = _descDict;
            btnDict = _btnDict;
            function = _function;
            btnDict2 = _btnDict2;
            function2 = _function2;
        }

        public int type;
        public int titleDict;
        public int descDict;
        public int btnDict;
        public int function;
        public int btnDict2;
        public int function2;
    }

    public static int curBePowerType { set; get; }
    public static List<BePowerListItemData> curDataList = new List<BePowerListItemData>();
    public delegate void ShowBePowerItemListDelegate();
    public static ShowBePowerItemListDelegate delegateShowBePowerItemList;

    public struct BePowerUpData
    {
        public BePowerUpData(int _t, int _val, int lv)
        {
            type = _t;
            value = _val;
            level = lv;
        }

        public int type;
        public int value;
        public int level;
    }
    public static List<BePowerUpData> curScoreList = new List<BePowerUpData>();
    public delegate void ShowBePowerLeftDelegate();
    public static ShowBePowerLeftDelegate delegateShowBePowerLeft;

    public static void ReciveResPowerData(GC_RES_POWERUP msg)
    {
        curScoreList.Clear();
        for (int i = 0; i < msg.typeCount; i++)
        {
            curScoreList.Add(new BePowerUpData(msg.GetType(i), msg.GetValue(i), msg.GetLevel(i)));
        }

        if (delegateShowBePowerLeft != null)
        {
            delegateShowBePowerLeft();
        }
        else
        {
            UIManager.ShowUI(UIInfo.CheckPowerRoot, AfterLoadUI);
        }
    }

    public static void AfterLoadUI(bool bSuccess, object param)
    {
        if (delegateShowBePowerLeft != null)
        {
            delegateShowBePowerLeft();
        }


        if (delegateShowBePowerItemList != null)
        {
            delegateShowBePowerItemList();
        }
    }

    public static void ShowBePowerWithData(GC_POWERUP_LIST msg)
    {
        int type = msg.Type;
        curDataList.Clear();
        if (type > (int)BePowerType.BPTDEFINE_Null && type < (int)BePowerType.BPTDEFINE_MAX)
        {
            // fill
            for (int i = 0; i < msg.titleDictCount; i++)
            {
                curDataList.Add(new BePowerListItemData(
                    msg.Type, msg.GetTitleDict(i), 
                    msg.GetDescDict(i), msg.GetBtnDict1(i), msg.GetFuncType1(i), msg.GetBtnDict2(i), msg.GetFuncType2(i)));
            }
            //.. 

            if (delegateShowBePowerItemList != null)
            {
                delegateShowBePowerItemList();
            }
        }
    }
}
