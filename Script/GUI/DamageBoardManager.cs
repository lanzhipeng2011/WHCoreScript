//********************************************************************
// 文件名: DamageBoardManager.cs
// 描述: 玩家伤害信息板逻辑 替换之前的InfoBoardLogic.cs 挂在玩家/NPC中的DamageBoard上
// 作者: WangZhe
// 创建时间: 2013-11-29
//
// 修改历史:
// 2013-11-29 王喆创建
// 2014-05-21 李嘉：将伤害面板从名字面板中分离，将DamageBoard改名为DamageBoardManager，进行统一的创建和复用管理
//                  并建立新的DamageBoard类作为伤害面板实体
//********************************************************************
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.LogicObj;
using GCGame.Table;
using GCGame;
using Games.GlobeDefine;

public class DamageBoardManager : MonoBehaviour 
{
    //鼠标按键枚举
    public enum MOUSE_BUTTON
    {
        MOUSE_BUTTON_LEFT,
        MOUSE_BUTTON_RIGHT,
        MOUSE_BUTTON_MIDDLE,
    }
    
    //伤害板改为全局，每一种类型根据配表定上限
    static private Dictionary<int, List<DamageBoard>> m_EnabledDictionary = null;   // 使用中的信息板 改用dictionary
    static private Dictionary<int, List<DamageBoard>> m_DisabledDictionary = null; // 未使用的信息板 循环使用 不必每次都new 改用dictionary
	
	static public Dictionary<int, List<Tab_DamageBoardType>> DamageBoardType = null;

	static public void ClearDamageDictionary()
	{
		if (null != m_EnabledDictionary)
		{
            m_EnabledDictionary.Clear();
			m_EnabledDictionary = null;
		}
		
		if (null != m_DisabledDictionary)
		{
            m_DisabledDictionary.Clear();
			m_DisabledDictionary = null;
		}
	}

	// Use this for initialization
    //void Awake () 
    //{
    //    if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN)
    //    {
    //        return;
    //    }
    //}
	
	// Update is called once per frame
	void FixedUpdate() 
    {
		DeleteDamageBoard();
	}

    //预加载一些伤害板
    static public void PreloadDamageBoard()
    {
        if (null == DamageBoardType)
        {
            DamageBoardType = TableManager.GetDamageBoardType();
        }
        
        ClearDamageDictionary();

        if (null == m_EnabledDictionary)
        {
            m_EnabledDictionary = new Dictionary<int, List<DamageBoard>>();
        }

        if (null == m_DisabledDictionary)
        {
            m_DisabledDictionary = new Dictionary<int, List<DamageBoard>>();
        }

        Vector3 cachePos = new Vector3(0, 0, 0);
        for (int i = 0; i < DamageBoardType.Count; i++)
        {
            if (false == m_EnabledDictionary.ContainsKey(i))
            {
                List<DamageBoard> nValue = new List<DamageBoard>();
                m_EnabledDictionary.Add(i, nValue);
            }

            if (false == m_DisabledDictionary.ContainsKey(i))
            {
                List<DamageBoard> nValue = new List<DamageBoard>();
                m_DisabledDictionary.Add(i, nValue);
            }

            //DamageBoardLoadInfo info = new DamageBoardLoadInfo(i, "PreCache", cachePos, true);
            //UIManager.LoadItem(UIInfo.DamageBoard, OnLoadDamageBoard, info);
            //BundleManager.LoadUI(UIInfo.DamageBoard, null, null, null);
        }
    }

    /// <summary>
    /// 显示伤害信息板
    /// </summary>
    /// <param name="nType">伤害信息板类型 对应DamageBoardType表ID</param>
    /// <param name="nValue">显示的内容</param>
    public void ShowDamageBoard(int nType, string strValue, Vector3 pos, bool isProfessionSkill = true)
    {
        if (PlayerPreferenceData.SystemDamageBoardEnable == false)
        {

             return ;
        }
        if (false == DebugHelper.m_bShowDamageBoard)
        {
             return ;
        }

        if (DamageBoardType.ContainsKey(nType) == false)
        {
             return ;
        }
        StartCoroutine(DelayShowDamageBoard(nType,strValue,  pos,  isProfessionSkill));
       
    }

    static IEnumerator DelayShowDamageBoard(int nType, string strValue, Vector3 pos, bool isProfessionSkill = true) 
    {
       
       

        Tab_DamageBoardType tabDamageBoardType = DamageBoardType[nType][0];

        yield return new WaitForSeconds(tabDamageBoardType.DelayTime);

        if (tabDamageBoardType == null)
        {
            tabDamageBoardType = DamageBoardType[0][0];
        }

        int nMaxNum = tabDamageBoardType.MaxNum;                    // 最大显示数量

        DamageBoard damageBoardInfo = null;
        if (m_DisabledDictionary[nType].Count > 0)
        {
            // 若DisabledDictionary对应list中有元素 则不创建新的信息板 直接使用旧的
            damageBoardInfo = m_DisabledDictionary[nType][m_DisabledDictionary[nType].Count - 1];
            damageBoardInfo.Reuse(pos);
            m_DisabledDictionary[nType].RemoveAt(m_DisabledDictionary[nType].Count - 1);
            m_EnabledDictionary[nType].Add(damageBoardInfo);
        }
        else
        {
            // 若DisabledDictionary对应list中没有元素
            if (m_EnabledDictionary[nType].Count < nMaxNum)
            {
                // 若EnabledDictionary对应list数量未达到最大值 则直接创建
                //damageBoardInfo = new DamageBoard();
                //damageBoardInfo.Create(m_Font, gameObject.transform.position);
                //m_EnabledDictionary[nType].Add(damageBoardInfo);
                DamageBoardLoadInfo info = new DamageBoardLoadInfo(nType, strValue, pos, isProfessionSkill);
                UIManager.LoadItem(UIInfo.DamageBoard, OnLoadDamageBoard, info);
            }
            else
            {
                // 若EnabledDictionary对应list数量已达到最大值 则Remove第一个 再Add出来一个
                damageBoardInfo = m_EnabledDictionary[nType][m_EnabledDictionary[nType].Count - 1];
                damageBoardInfo.Reuse(pos);
                m_EnabledDictionary[nType].RemoveAt(m_EnabledDictionary[nType].Count - 1);
                m_EnabledDictionary[nType].Add(damageBoardInfo);

            }
        }

        if (null != damageBoardInfo)
        {
            damageBoardInfo.ActiveDamageBoard(nType, strValue, pos, isProfessionSkill);
        }
    }

    static void OnLoadDamageBoard(GameObject resObj, object param)
    {
        if (PlayerPreferenceData.SystemDamageBoardEnable == false)
        {
            return;
        }
        if (null == resObj || null == param)
        {
            return;
        }

        if (null == GameManager.gameManager.ActiveScene ||
            null == GameManager.gameManager.ActiveScene.DamageBoardRoot)
        {
            return;
        }

        GameObject resInst = GameObject.Instantiate(resObj) as GameObject;

        resInst.transform.parent = GameManager.gameManager.ActiveScene.DamageBoardRoot.transform;

        DamageBoard board = resInst.GetComponent<DamageBoard>();
        if (null == board)
        {
            return;
        }

        DamageBoardLoadInfo info = param as DamageBoardLoadInfo;
        if (board.ActiveDamageBoard(info.Type, info.Value, info.Pos, info.IsProfessionSkill) && m_EnabledDictionary.ContainsKey(info.Type))
        {
            m_EnabledDictionary[info.Type].Add(board);
        }
    }

 
    /// 将播放完Tween动画的信息板加入DisabledList中 并从EnabledList删除
    /// </summary>
    void DeleteDamageBoard()
    {
		if (null == m_EnabledDictionary || null == m_DisabledDictionary)
		{
			return;
		}

        float fCurTime = Time.time;
        for (int i = 0; i < DamageBoardType.Count; i++)
        {
            for (int j = 0; j < m_EnabledDictionary[i].Count; j++)
            {
                DamageBoard info = m_EnabledDictionary[i][j];
               
                if (info.gameObject.activeSelf)
                {
                    if (fCurTime - info.ShowTime > 1.0f)
                    {
                        info.Remove();
                        m_DisabledDictionary[i].Add(info);
                        m_EnabledDictionary[i].RemoveAt(j);

                        //由于遍历中进行了数据移动，会造成后续问题，所以每次Delete的时候，每种类型只删除一个即可
                        break;
                    }
                }
            }
        }
    }
}
