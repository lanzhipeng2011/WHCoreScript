/********************************************************************************
 *	文件名：	UILoadResource.cs
 *	全路径：	\Script\GUI\UILoadResource.cs
 *	创建人：	王华
 *	创建时间：2013-11-21
 *
 *	功能说明：UI已载入资源列表
 *	修改记录：
*********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

public class UILoadResource
{
    private Dictionary<string, UIAtlas> m_LoadedAtlasMap = new Dictionary<string, UIAtlas>();

    public UILoadResource()
    {
        m_LoadedAtlasMap.Clear();
    }

    public UIAtlas GetAtlas(string atlasName)
    {
        if (string.IsNullOrEmpty(atlasName))
        {
            return null;
        }

        if (m_LoadedAtlasMap.ContainsKey(atlasName))
        {
            return m_LoadedAtlasMap[atlasName];
        }
        else
        {
            UIAtlas uiAtlas  = ResourceManager.LoadResource(atlasName) as UIAtlas;
            if (uiAtlas)
            {
                m_LoadedAtlasMap[atlasName] = uiAtlas;
                return uiAtlas;
            }
            return null;
        }
    }
}

