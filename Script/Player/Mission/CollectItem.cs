/********************************************************************************
 *	文件名：CollectItem.cs
 *	全路径：	\Script\Mission\CollectItem.cs
 *	创建人：	贺文鹏
 *	创建时间：2014-02-17
 *
 *	功能说明： 客户端采集物品逻辑，刷新场景内物品 CollectItem.txt表配置。
 *	       
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Games.Mission;
using Module.Log;
using Games.LogicObj;
using Games.GlobeDefine;
using Games.Events;
public class CollectItem : Singleton<CollectItem>
{
    private GameObject m_ItemObj = null;
    private int m_MissionID;

    public CollectItem()
    {
        CleanUp();
    }

    public void CleanUp()
    {
        m_MissionID = -1;
    }

    // 创建item
    public void InitCollectItem(int nSceneID)
    {
        if (nSceneID < 0)
        {
            return;
        }

        CleanUp();

        List<Tab_CollectItem> CollectItemList = TableManager.GetCollectItemByID(nSceneID);
        if (CollectItemList == null)
        {
            return;
        }
        foreach (Tab_CollectItem CollectItem in CollectItemList)
        {
            if (CollectItem != null)
            {
                for (int i = 0; i < CollectItem.Count; i++ )
                {
                    string strName = "CollectItem" + CollectItem.Index.ToString()+i;
                    if (IsItemExist(strName))
                    {
                        continue;
                    }

                    Singleton<ObjManager>.GetInstance().CreateCollectItem(CollectItem, strName, i);
                }
            }
        }
    }

    // 开始从场景中移除物品
    public void RemoveItem(GameObject ItemObj)
    {
        if (ItemObj)
        {            
            // 玩家任务检测
            List<int> nMissionIDList = GameManager.gameManager.MissionManager.GetAllMissionID();
            int nMissionCount = nMissionIDList.Count;
            if (nMissionCount <= 0)
            {
                return;
            }
            // 遍历任务
            foreach (int nMissionID in nMissionIDList)
            {
                Tab_MissionBase MissionTab = TableManager.GetMissionBaseByID(nMissionID, 0);
                if (MissionTab == null)
                {
                    continue;
                }
                if (MissionTab.LogicType != (int)TableType.Table_CollectItem)
                {
                    continue;
                }

                // 任务状态 已完成
                if (2 == GameManager.gameManager.MissionManager.GetMissionState(nMissionID))
                {
                    continue;
                }

                Tab_MissionCollectItem CItem = TableManager.GetMissionCollectItemByID(MissionTab.Id, 0);
                if (CItem == null)
                {
                    continue;
                }

                GameObject gItemObj = Singleton<ObjManager>.GetInstance().FindOtherGameObj(ItemObj.name);
                if (gItemObj)
                {
                    Obj_OtherGameObj otherGameObj = gItemObj.GetComponent<Obj_OtherGameObj>();
                    if (otherGameObj && otherGameObj.GetIntParamByIndex(0) == CItem.CharModelID)
                    {
                        m_MissionID = nMissionID;
                    }
                    break;
                }
            }

            // 无任务
            if (m_MissionID < 0)
            {
                return;
            }

            m_ItemObj = ItemObj;
            GameManager.gameManager.SoundManager.PlaySoundEffect(9);        //collect
            // 通知采集条
            UIManager.ShowUI(UIInfo.CollectItemSlider);
        }
    }

    // 采集条出发 真正删除物品
    public void SafeDeleteItem()
    {
        if (m_ItemObj)
        {
            if (m_MissionID >= 0)
            {
                Tab_MissionBase MissionTab = TableManager.GetMissionBaseByID(m_MissionID, 0);
                if (MissionTab == null)
                {
                    return;
                }
                if (MissionTab.LogicType != (int)TableType.Table_CollectItem)
                {
                    return;
                }
                Tab_MissionCollectItem CItem = TableManager.GetMissionCollectItemByID(MissionTab.Id, 0);
                if (CItem == null)
                {
                    return;
                }
                GameObject gItemObj = Singleton<ObjManager>.GetInstance().FindOtherGameObj(m_ItemObj.name);
                if (gItemObj)
                {
                    Obj_OtherGameObj otherGameObj = gItemObj.GetComponent<Obj_OtherGameObj>();
                    if (otherGameObj && otherGameObj.GetIntParamByIndex(0) == CItem.CharModelID)
                    {
                        int nParam = GameManager.gameManager.MissionManager.GetMissionParam(m_MissionID, 0);
                        if (nParam >= CItem.ItemCount)
                        {
                            return;
                        }
                        nParam += 1;
                        GameManager.gameManager.MissionManager.SetMissionParam(m_MissionID, 0, nParam);
                        if (nParam >= CItem.ItemCount)
                        {
                            GameManager.gameManager.MissionManager.SetMissionState(m_MissionID, 2);
                        }

                        // 移除 隐藏处理
                        gItemObj.SetActive(false);
                        m_ItemObj = null;
                        m_MissionID = -1;

                        ReSeedItemEvent(GameManager.gameManager.RunningScene, otherGameObj.GetIntParamByIndex(1), otherGameObj.GetIntParamByIndex(2));
                    }
                }
            }
        }
    }

    bool IsItemExist(string strName)
    {
        if (strName == null)
        {
            return false;
        }

        GameObject gItemObj = Singleton<ObjManager>.GetInstance().FindOtherGameObj(strName);
        if (gItemObj)
        {
            return true;
        }

        return false;
    }

    void ReSeedItemEvent(int nSceneID, int nSceneIndex, int nItemIndex)
    {
        Tab_CollectItem cItem = TableManager.GetCollectItemByID(nSceneID, nSceneIndex);
        if (cItem == null)
        {
            return;
        }

        GameEvent _event = new GameEvent(GameDefine_Globe.EVENT_DEFINE.EVENT_COLLECTITEM_RESEED);
        _event.IsDelay = true;
        _event.DelayTime = (float)cItem.AutoLifeTime;
        _event.AddIntParam(nSceneID);
        _event.AddIntParam(nSceneIndex);
        _event.AddIntParam(nItemIndex);
        Singleton<EventSystem>.GetInstance().PushEvent(_event);
    }

    public void ReSeedItems(int nSceneID, int nSceneIndex, int nItemIndex)
    {
        if (nSceneID < 0 || nSceneID != GameManager.gameManager.RunningScene)
        {
            return;
        }

        Tab_CollectItem cItem = TableManager.GetCollectItemByID(nSceneID, nItemIndex);
        if (cItem == null)
        {
            return;
        }

        string strName = "CollectItem" + cItem.Index.ToString() + nItemIndex;
        GameObject gItemObj = Singleton<ObjManager>.GetInstance().FindOtherGameObj(strName);
        if (gItemObj)
        {
            gItemObj.SetActive(true);
        }
    }
}
