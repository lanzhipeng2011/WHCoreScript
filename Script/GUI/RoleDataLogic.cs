//********************************************************************
// 文件名: RoleDataLogic.cs
// 描述: 人物属性界面逻辑
// 作者: WangZhe
// 创建时间: 2013-10-31
//
// 修改历史:
// 2013-10-31 王喆创建
// 2013-11-9 将人物属性和任务信息的打开和关闭 交由UIButton Activate控件控制
//********************************************************************

using UnityEngine;
using System.Collections;

public class RoleDataLogic : MonoBehaviour {

    private static RoleDataLogic m_Instance = null;
    public static RoleDataLogic Instance()
    {
        return m_Instance;
    }

    void Awake()
    {
        m_Instance = this;
    }

    void OnDestroy()
    {
        m_Instance = null;
    }
	
}
