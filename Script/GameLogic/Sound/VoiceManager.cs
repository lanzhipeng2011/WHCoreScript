using UnityEngine;
using System.Collections;
using YunvaIM;
public class VoiceManager :Singleton<VoiceManager>
{
    public uint AppId = 1000420;
    string[] wildCard = new string[] {"0x001"};
    string filePath;

    string recordPath = string.Empty;//返回录音地址
    string recordUrlPath = string.Empty;//返回录音url地址

    public bool IsEnable = false;
    public bool IsInit = false;
    public VoiceManager()
    {
#if UNITY_ANDROID && !UNITY_EDITOR  && CommonSDK
       IsEnable = true;
#endif
    }
    public void Init()
    {
        if (!IsEnable) return;

      
        if (this.IsInit == false)
        {
#if UNITY_ANDROID && !UNITY_EDITOR && CommonSDK
            AndroidJavaClass YvLoginInit = new AndroidJavaClass("com.yunva.im.sdk.lib.YvLoginInit");
            AndroidJavaObject activity = CommonSDKPlaform.Instance.sdkBase.Call<AndroidJavaObject>("getParentActivity");
            AndroidJavaObject application = activity.Call<AndroidJavaObject>("getApplication");
            //AndroidJavaClass boolean = new AndroidJavaClass("com.yunva.im.sdk.lib.YvLoginInit");
            bool b =  YvLoginInit.CallStatic<bool>("initApplicationOnCreate", application, AppId.ToString());
            Debug.Log("Call platform Init Vlice??"+b.ToString());
#endif
            this.IsInit = true;
            
            YunVaImSDK.instance.YunVa_Init(0, AppId, Application.persistentDataPath, false);
        }
    }

    public void Login(string roleName,string uid,string serverId)
    {
        if (!IsEnable) return;
        string tt = "{\"uid\":\"" + uid + "\",\"nickname\":\"" + roleName + "\"}";
       
        YunVaImSDK.instance.YunVaOnLogin(tt,serverId, wildCard,0,(data)=> 
        {
            if (data.result != 0)
            {
                Debug.LogError(data.msg);
            }
        });
    }

    public void LoginOut()
    {
        if (!IsEnable) return;
        YunVaImSDK.instance.YunVaLogOut();
    }

    /// <summary>
    /// 开始录音
    /// </summary>
    public void StartRecordRequest()
    {
        if (!IsEnable) return;
        filePath = string.Format("{0}/{1}.amr", Application.persistentDataPath, System.DateTime.Now.ToFileTime());
        YunVaImSDK.instance.RecordStartRequest(filePath);
    }
    /// <summary>
    /// 停止录音
    /// </summary>
    public void StopRecordRequest(System.Action<ImRecordStopResp> fn)
    {
        if (!IsEnable) return;
        YunVaImSDK.instance.RecordStopRequest(fn);
    }
    /// <summary>
    /// 播放语音
    /// </summary>
    public void StartPlayRecord(string voicePath)
    {
        YunVaImSDK.instance.RecordStartPlayRequest(voicePath, "",System.DateTime.Now.ToFileTime().ToString(),(data)=> 
        {
            if (data.result != 0) { Debug.LogError(data.describe); }
        });
    }
    /// <summary>
    /// 停止播放
    /// </summary>
    public void StopPlayRecord()
    {
        YunVaImSDK.instance.RecordStopPlayRequest();
    }
    /// <summary>
    /// 上传
    /// </summary>
    public string UploadFileRequest(string path,System.Action<ImUploadFileResp> uploadFn)
    {
        string TimeStr = System.DateTime.Now.ToFileTime().ToString();
        YunVaImSDK.instance.UploadFileRequest(path, TimeStr, uploadFn); 
        return TimeStr;
    }

    public void DownLoadFileRequest(string urlPath,System.Action<ImDownLoadFileResp> fn)
    {  
        string DownLoadfilePath = string.Format("{0}/{1}.amr", Application.persistentDataPath,System.DateTime.Now.ToFileTime());
        string fileid = System.DateTime.Now.ToFileTime().ToString();
        YunVaImSDK.instance.DownLoadFileRequest(urlPath, DownLoadfilePath, fileid, fn);
    }
}
