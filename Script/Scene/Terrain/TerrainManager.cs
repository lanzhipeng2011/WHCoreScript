/********************************************************************************
 *	文件名：	TerrainManager.cs
 *	全路径：	\Script\GameLogic\GameManager\TerrainManager.cs
 *	创建人：	李嘉
 *	创建时间：2014-04-02
 *
 *	功能说明：游戏地形数据管理器。
 *	         由于取消了场景中的Terrain节点，所以地形相关数据由此类管理。
 *	         每进入一个场景则初始化一次，在ActiveScene中进行初始化操作。
 *	         目前主要提供获得地表高度接口
 *	修改记录：
*********************************************************************************/

using Games.LogicObj;
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using Module.Log;

public class TerrainManager
{
    private int m_nTerrainHeightMapLength;            //地形长度
    private int m_nTerrainHeightMapWidth;             //地形宽度
    private int m_nTerrainHeightMax;                  //地形高度最大值
    private float m_fTerrainLenRate = 1.0f;           //地图真是x坐标在高度图中的位置换算比例
    private float m_fTerrainWidRate = 1.0f;           //地图真是z坐标在高度图中的位置换算比例

    private byte[,] m_TerrianRawData = null;

    //从地形Raw文件读取地形高度数据
    public bool InitTerrianData(string path, int nHeightMapLength, int nHeightMapWidth, int nHeightMax, int nMapRealLength, int nMapRealWidth)
    {
        //只能初始化一次，所以RawData必须为空
        if (nHeightMapLength <= 0 || nHeightMapWidth <= 0 || m_TerrianRawData != null)
        {
            return false;
        }

        m_nTerrainHeightMapLength = nHeightMapLength;
        m_nTerrainHeightMapWidth = nHeightMapWidth;
        m_nTerrainHeightMax = nHeightMax;

        //得到长和宽的换算比例
        if (nMapRealLength > 0)
        {
            m_fTerrainLenRate = ((float)m_nTerrainHeightMapLength) / nMapRealLength;
        }
        if (nMapRealWidth > 0)
        {
            m_fTerrainWidRate = ((float)m_nTerrainHeightMapWidth) / nMapRealWidth;
        }

        m_TerrianRawData = new byte[m_nTerrainHeightMapLength, m_nTerrainHeightMapWidth];

#if UNITY_ANDROID && !UNITY_EDITOR
        // Raw 资源加载.回调方法
        GameManager.gameManager.rawDataCallback = RawDataComplete;
        GameManager.gameManager.GetRawData(path);
#else
        return IOSAndPCReadRaw(path);
#endif
        
        return true;
    }

    private bool IOSAndPCReadRaw(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                LogModule.DebugLog("Can't Open Raw File " + path);
                return false;
            }

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            int nSeek = 0;
            byte[] byteData = new byte[1];
            //读数据
            for (int lenIdx = 0; lenIdx < m_nTerrainHeightMapLength; ++lenIdx)
            {
                for (int widIdx = 0; widIdx < m_nTerrainHeightMapWidth; ++widIdx)
                {
                    fileStream.Seek(nSeek, SeekOrigin.Begin);
                    fileStream.Read(byteData, 0, 1);
                    nSeek++;

                    //写入short
                    m_TerrianRawData[widIdx, lenIdx] = byteData[0];
                }
            }

            fileStream.Close();
            return true;
        }
        catch (IOException e)
        {
            Debug.Log("An IO exception has been thrown!");
            Debug.Log(e.ToString());
        }

        return true;
    }

    public void RawDataComplete(byte[] bytes)
    {
        GameManager.gameManager.rawDataCallback -= RawDataComplete;

        if (bytes == null || bytes.Length <= 0)
        {
            return;
        }

        int nSeek = 0;
        //byte[] byteData = new byte[1];
        //读数据
        for (int lenIdx = 0; lenIdx < m_nTerrainHeightMapLength; ++lenIdx)
        {
            nSeek = lenIdx * m_nTerrainHeightMapWidth;

            for (int widIdx = 0; widIdx < m_nTerrainHeightMapWidth; ++widIdx)
            {
                //写入short
                m_TerrianRawData[widIdx, lenIdx] = bytes[nSeek + widIdx];
            }
        }
    }

    public float GetTerrianHeight(Vector3 pos)
    {
        int nX = (int)(pos.x * m_fTerrainLenRate);
        int nZ = (int)(pos.z * m_fTerrainWidRate);
        if (nX < 0 || nX > m_nTerrainHeightMapLength || nZ < 0 || nZ > m_nTerrainHeightMapWidth)
        {
            return 0;
        }

        //8bit灰度图可表示数值0~255,对应高度具体数值的0~m_nTerrainHeightMax
        //所以根据所在灰度图区间来映射到对应高度
        return ((float)(m_TerrianRawData[nX, nZ]) / 255) * m_nTerrainHeightMax;
    }

    public float GetTerrianHeightSample(Vector3 pos)
    {
        int nX = (int)(pos.x * m_fTerrainLenRate);
        int nZ = (int)(pos.z * m_fTerrainWidRate);
        int nX1 = nX + 1;
        int nZ1 = nZ + 1;
        if (nX < 0 || nX > m_nTerrainHeightMapLength || nZ < 0 || nZ > m_nTerrainHeightMapWidth)
        {
            return 0;
        }

        if (nX1 > m_nTerrainHeightMapLength)
        {
            nX1 = nX;
        }

        if (nZ1 > m_nTerrainHeightMapWidth)
        {
            nZ1 = nZ;
        }

        float LeftBottomHeight = ((float)(m_TerrianRawData[nX, nZ]) / 255) * m_nTerrainHeightMax;
        float LeftTopHeight = ((float)(m_TerrianRawData[nX, nZ1]) / 255) * m_nTerrainHeightMax;
        float RightBottomHeight = ((float)(m_TerrianRawData[nX1, nZ]) / 255) * m_nTerrainHeightMax;
        float RightTopHeight = ((float)(m_TerrianRawData[nX1, nZ1]) / 255) * m_nTerrainHeightMax;
        float retValue = LeftBottomHeight;

        if (nX1 - pos.x > pos.z - nZ)
        {
            if (nX1 == pos.x)
            {
                retValue = LeftBottomHeight + (LeftTopHeight - LeftBottomHeight) * (pos.z - nZ);
            }
            else
            {
                float k = (nZ1 - pos.z) / (nX1 - pos.x);
                float b = nZ1 - k * nX1;
                float nZTarget = k * nX + b;
                float precent = nZTarget - nZ;
                retValue = LeftBottomHeight + (LeftTopHeight - LeftBottomHeight) * precent;
            }
            
        }
        else
        {
            if (pos.x == nX)
            {
                retValue = RightBottomHeight + (RightTopHeight - RightBottomHeight) * (pos.z - nZ);
            }
            else
            {
                float k = (pos.z - nZ) / (pos.x - nX);
                float b = nZ - k * nX;
                float nZTarget = k * nX1 + b;
                float precent = nZTarget - nZ;
                retValue = RightBottomHeight + (RightTopHeight - RightBottomHeight) * precent;
            }
        }
       
       
        return retValue;
    }
}
