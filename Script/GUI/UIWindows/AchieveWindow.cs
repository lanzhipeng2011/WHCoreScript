//********************************************************************
// 文件名: NoticeLogic.cs
// 描述: 公告里的活动UI控件
// 作者: YangXin
// 创建时间: 2012-11-11
//********************************************************************
using UnityEngine;
using System.Collections;
using GCGame.Table;
using Module.Log;
using GCGame;

public class AchieveWindow : MonoBehaviour {

    public GameObject ItemParent;
    // Use this for initialization
    void Start()
    {
        UIManager.LoadItem(UIInfo.AchieveItem, OnLoadItem);
    }

    void OnLoadItem(GameObject achieveItem, object param)
    {
        if (null == achieveItem)
        {
            LogModule.DebugLog("load achieve item error");
            return;
        }

        int signItemCount = TableManager.GetAchieveNotice().Count;
        for (int i = 0; i < signItemCount; i++)
        {

            GameObject newItem = Utils.BindObjToParent(achieveItem, ItemParent);
            Tab_AchieveNotice pLin = TableManager.GetAchieveNoticeByID(i, 0);
            if (pLin != null && null != newItem)
            {
                Transform iconTransform = newItem.transform.FindChild("Icon");
                if (null != iconTransform && null != iconTransform.gameObject.GetComponent<UISprite>())
                    iconTransform.gameObject.GetComponent<UISprite>().spriteName = pLin.Icon;

                Transform nameTransform = newItem.transform.FindChild("Name");
                if (null != nameTransform && null != nameTransform.gameObject.GetComponent<UILabel>())
                    nameTransform.gameObject.GetComponent<UILabel>().text = pLin.Name;
            }
        }
        ItemParent.GetComponent<UIGrid>().repositionNow = true;
    }
  
}
