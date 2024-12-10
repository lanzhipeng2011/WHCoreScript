using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using Module.Log;
using System.Threading;
using GCGame;
using System.Collections.Generic;

#if UNITY_WP8
using System;
#endif


public class DownloadHelper
{
    public delegate void DelegateDownloadFinish(bool bSuccess);
    public long AlreadyDownloadSize { get { return m_alreadyDownloadSize; } }


    private long m_alreadyDownloadSize = 0;


    private string[] m_curUrlArray;
    private string[] m_curFilePathArray;

    private DelegateDownloadFinish m_curDelFun;
    private int m_curDownloadSize = 0;

    //private Thread m_curThread = null;
    private bool m_bStop = false;
    public static DownloadHelper StartDownload(MonoBehaviour monoBehavior, string url, bool bRemote, string fileSavePath, DelegateDownloadFinish delFun = null)
    {
        DownloadHelper helper = new DownloadHelper(url, bRemote, fileSavePath, delFun);
		monoBehavior.StartCoroutine(helper.DownloadFile());
        return helper;
    }

    public static DownloadHelper StartDownload(MonoBehaviour monoBehavior, List<string> urlList, bool bRemote, List<string> fileSavePathList, DelegateDownloadFinish delFun = null)
    {
        DownloadHelper helper = new DownloadHelper(urlList, bRemote, fileSavePathList, delFun);
		monoBehavior.StartCoroutine(helper.DownloadFile());
        return helper;
    }

    public void Stop()
    {
        m_bStop = true;
    }

    private DownloadHelper(string url, bool bRemote, string fileSavePath, DelegateDownloadFinish delFun)
    {
        m_curUrlArray = new string[1];
        m_curFilePathArray = new string[1];
        m_curFilePathArray[0] = fileSavePath;
        if (bRemote)
        {
            m_curUrlArray[0] = DownloadHelper.AddTimestampToUrl(url);
        }
        else
        {
            m_curUrlArray[0] = url;
        }
        
        m_curDelFun = delFun;
        m_bStop = false;
    }

    private DownloadHelper(List<string> urlList, bool bRemote, List<string> fileSavePathList, DelegateDownloadFinish delFun)
    {
        m_curUrlArray = new string[urlList.Count];
        for (int i = 0; i < urlList.Count; i++)
        {
            if (bRemote)
            {
                m_curUrlArray[i] = DownloadHelper.AddTimestampToUrl(urlList[i]);
            }
            else
            {
                m_curUrlArray[i] = urlList[i];
            }
           
        }

        m_curFilePathArray = new string[fileSavePathList.Count];
        for (int i = 0; i < fileSavePathList.Count; i++)
        {
            m_curFilePathArray[i] = fileSavePathList[i];
        }

        m_curDelFun = delFun;
		m_bStop = false;
    }
    
	private IEnumerator DownloadFile()
    {
        if (null == m_curUrlArray || null == m_curFilePathArray)
        {
            LogModule.ErrorLog("url array or file array is null ");
            if (null != m_curDelFun) m_curDelFun(false);
            yield break;
        }

        if (m_curUrlArray.Length != m_curFilePathArray.Length)
        {
            LogModule.ErrorLog("url array size is not equal file path size");
            if (null != m_curDelFun) m_curDelFun(false);
            yield break;
        }
		for (int i = 0; i < m_curUrlArray.Length; i++)
		{
			if (m_bStop)
			{
				yield break;
			}

			WWW wwwCurFile = new WWW(m_curUrlArray[i]);
			yield return wwwCurFile;
			if(!string.IsNullOrEmpty(wwwCurFile.error))
			{
				LogModule.ErrorLog("download file fail" + m_curUrlArray[i] + " error:" + wwwCurFile.error);
				if (null != m_curDelFun) m_curDelFun(false);
				yield break;
			}
			else
			{
				try
				{
					Utils.CheckTargetPath(m_curFilePathArray[i]);
					FileStream fs = new FileStream(m_curFilePathArray[i], FileMode.OpenOrCreate);
					fs.Write(wwwCurFile.bytes, 0, wwwCurFile.bytesDownloaded);
					fs.Close();
					m_alreadyDownloadSize += wwwCurFile.bytesDownloaded;
				}
				catch (System.Exception ex)
				{
					LogModule.ErrorLog("download file error:" + ex.ToString());
					if (null != m_curDelFun) m_curDelFun(false);
					m_bStop = true;
				}

			}
		}
		
		if (null != m_curDelFun) m_curDelFun(!m_bStop);
	}

    public static string AddTimestampToUrl(string url)
    {
        return url + "?" + System.DateTime.Now.Millisecond.ToString();
    }

}