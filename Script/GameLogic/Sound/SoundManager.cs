/********************************************************************************
 *	文件名：	SoundManager.cs
 *	全路径：	\Script\Scene\SoundManager.cs
 *	创建人：	王华
 *	创建时间：2013-11-21
 *
 *	功能说明：声音管理器
 *	修改记录：
 *	2013-11-22 先去掉协程，把声音缓冲池分成2个，一个场景相关（比如背景音乐），一个非场景相关的（比如人物技能）
 *	场景相关的3D音源绑在人物身上，随人物移动
 *	2013-12-12 把声音表格第一列改为名称，背景音乐挂在摄像机上
 *	2013-12-18 非场景音乐（比如技能音乐）的中心点挂在主角身上,设置PanLevel和Spread
 *	2013-12-24 PanLevel和Spread配表设置
 *	2014-03-13 背景音乐和场景各种音效使用同一个池子，池子大小固定，采用最长时间未时间即替换算法更新池子内容，
 *	初始时，立即创建m_SFXChannelsCount个AudioSource，用于播放背景音乐和音效，同时能够播放的最大声音数也是m_SFXChannelsCount个，
 *	另外，AudioSource挂接在SoundManager这个物体上
*********************************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Games.LogicObj;
using Games.GlobeDefine;
using Module.Log;
using Games.Scene;
using GCGame;


[Serializable]
public class SoundClip
{
    private AudioClip m_Audioclip;
    public AudioClip Audioclip
    {
        get { return m_Audioclip; }
        set { m_Audioclip = value; }
    }

    //表格数据
    public short m_priority = 128;
    public string m_name = string.Empty;
    public string m_path = string.Empty;
    public float m_minDistance = 10;
    public float m_volume = 1.0f;
    public float m_delay = 0.0f;
    public float m_panLevel = 0.0f;
    public float m_spread = 0.0f;
    public bool m_isLoop = false;
    public short m_curMaxPlayingCount = 1;
    public short m_uID = -1;

    //运行时数据
    public float m_LastActiveTime = 0.0f;  //上次活跃时间,上次播放时间
}

public class SoundClipPools
{
    public class SoundClipParam
    {
        public SoundClipParam(float volumeFactor)
        {
            m_volumeFactor = volumeFactor;
            m_fadeInTime = 0;
            m_fadeOutTime = 0;
        }

        public SoundClipParam(int clipId, float fadeOutTime, float fadeInTime)
        {
            m_volumeFactor = 1;
            m_fadeInTime = fadeInTime;
            m_fadeOutTime = fadeOutTime;
            m_clipID = (short)clipId;
        }

        public float m_volumeFactor;
        public float m_fadeOutTime;
        public float m_fadeInTime;
        public short m_clipID;
    }
    public delegate void GetSoundClipDelegate(SoundClip soundClip, SoundClipParam param);

    private Dictionary<int, SoundClip> m_SoundClipMap = new Dictionary<int,SoundClip>();    //音效列表，限制最大数量MAX

    /// <summary>
    /// 根据声音名称得到SoundClip，不存在会自动添加
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns></returns>
    public void GetSoundClip(int nSoundId, GetSoundClipDelegate delFun, SoundClipParam param)
    {
        if (nSoundId >= 0)
        {
            if (m_SoundClipMap.ContainsKey(nSoundId))
            {
                //更新上次播放时间
                m_SoundClipMap[nSoundId].m_LastActiveTime = Time.realtimeSinceStartup;
                if (null != delFun) delFun(m_SoundClipMap[nSoundId], param);
                return;
            }
            else
            {
                if (m_SoundClipMap.Count > SoundManager.m_SFXChannelsCount) //超过最大值，删除最长时间未使用的
                {
                    //LogModule.DebugLog("Warnning m_SoundClipList.Count > " + SoundManager.m_SFXChannelsCount);
                    RemoveLastUnUsedClip();
                }

                Tab_Sounds soundsTab = TableManager.GetSoundsByID(nSoundId,0);
                if (soundsTab == null)
                {
                    LogModule.DebugLog("sound id " + nSoundId.ToString() + " is null");
                    if (null != delFun) delFun(null, param);
                    return;
                }

                string fullsoundName = soundsTab.FullPathName;
                if (string.IsNullOrEmpty(fullsoundName))
                {
                    if (null != delFun) delFun(null, param);
                    return;
                }

                if (GameManager.gameManager.SceneLogic == null)
                {
                    if (null != delFun) delFun(null, param);
                    return;
                }
				AudioClip curSound = ResourceManager.LoadResource(fullsoundName) as AudioClip;

				if(curSound!=null)
				{
					OnLoadSound(fullsoundName,curSound, soundsTab, delFun, param);
					return  ;
				}
                GameManager.gameManager.SceneLogic.StartCoroutine(BundleManager.LoadSound(fullsoundName, OnLoadSound, soundsTab, delFun, param));

            }
        }
    }

    void OnLoadSound(string soundPath, AudioClip curAudioClip, object param1, object param2, object param3 = null)
    {
        SoundClip clip = new SoundClip();
        clip.Audioclip = curAudioClip;
        GetSoundClipDelegate delFun = param2 as GetSoundClipDelegate;
        SoundClipParam soundClipParam = param3 as SoundClipParam;
        Tab_Sounds soundsTab = param1 as Tab_Sounds;
        if (null == clip.Audioclip)
        {
            LogModule.DebugLog("sound clip " + soundPath + " is null");
            if (null != delFun) delFun(null, soundClipParam);
            return;
        }

        if (!clip.Audioclip.isReadyToPlay)
        {
            LogModule.DebugLog("Cann't decompress the sound resource " + soundPath);
            if (null != delFun) delFun(null, soundClipParam);
            return;
        }
        clip.m_LastActiveTime = Time.realtimeSinceStartup;
        clip.m_delay = soundsTab.Delay;
        clip.m_minDistance = soundsTab.MinDistance;
        clip.m_panLevel = soundsTab.PanLevel;
        clip.m_spread = soundsTab.Spread;
        clip.m_volume = soundsTab.Volume;
        clip.m_isLoop = soundsTab.IsLoop;
        clip.m_path = soundPath;
        clip.m_name = soundsTab.Name;
        clip.m_uID = (short)soundsTab.Id;
        clip.m_curMaxPlayingCount = soundsTab.CurMaxPlayingCount;

        if (!m_SoundClipMap.ContainsKey(soundsTab.Id))
        {
            m_SoundClipMap.Add(soundsTab.Id, clip);
        }


        if (null != delFun) delFun(clip, soundClipParam);
    }
    /// <summary>
    /// 删除最长时间未使用的条目
    /// </summary>
    private void RemoveLastUnUsedClip()
    {
        float fSmallestTime = 99999999.0f;
        int smallestId = -1;
        foreach (SoundClip clip in m_SoundClipMap.Values)
        {
            if (fSmallestTime > clip.m_LastActiveTime)
            {
                smallestId = clip.m_uID;
                fSmallestTime = clip.m_LastActiveTime;
            }
        }

        LogModule.DebugLog("RemoveLastUnUsedClip( " + smallestId.ToString() + " )");  //以后注释掉

        m_SoundClipMap.Remove(smallestId);
    }

    //add by sunyu 2014-07-31
    //force Remove clip from pool, special for bgmusic
    public void ForceRemoveClipByID(short uid)
    {
        if(uid != -1)
        {
            int nNeeddelId = -1;
            foreach (SoundClip clip in m_SoundClipMap.Values) //dictionary 的 foreach去不掉
            {
                if(clip.m_uID == uid)
                {
                    nNeeddelId = clip.m_uID;
                    break;
                }
            }
            m_SoundClipMap.Remove(nNeeddelId);
        }
    }

}

public class SoundManager : MonoBehaviour 
{
    public class MyAudioSource
    {
        public MyAudioSource()
        {
            m_uID = -1;
            m_AudioSource = null;
        }
        public short m_uID;         //声音表格配的唯一标示符
        public AudioSource m_AudioSource;
    }

    public SoundClipPools m_SoundClipPools; //声音数据池 
    private MyAudioSource m_BGAudioSource = new MyAudioSource(); //背景音乐源
    private float       m_CurBGVolume = 0; //当前背景音乐音量
    public static int m_SFXChannelsCount = 30; //最大声道数量
    private MyAudioSource[] m_SFXChannel = new MyAudioSource[m_SFXChannelsCount]; //Sound FX，声音特效。
    private SoundClip m_NextSoundClip = null; 
    public static bool m_EnableBGM = false;     //是否启用背景音乐
    public static bool m_EnableSFX = false;      //是否启用环境音效
	private short m_lastMusicID = 1001;//-1;     //上次播放的场景背景音乐Id，用于中断后重播

    public float m_sfxVolume = 1.0f;        //音效音量系数
    public float m_bgmVolume = 1.0f;        //场景背景音乐音量系数

    private enum FadeMode //播放模式
    {
        None,
        FadeIn, //淡入
        FadeOut //淡出
    }
    private FadeMode m_fadeMode;
    private float m_fadeOutTime;
    private float m_fadeOutTimer;
    private float m_fadeInTime;
    private float m_fadeInTimer;

    public bool EnableSFX 
	{
		get{ return m_EnableSFX; }
		set{ m_EnableSFX = value; }
	}
	
	public bool EnableBGM
	{
		get{ return m_EnableBGM; }
		set
		{
			if( m_EnableBGM && !value )
			{
				if(m_BGAudioSource!=null)
				{
					if(m_BGAudioSource.m_AudioSource.isPlaying)
					{
						m_BGAudioSource.m_AudioSource.Stop();
					}
				}
			}
			else if(!m_EnableBGM && value)
			{
				if(m_BGAudioSource!=null)
				{
                    PlayBGMWithFade(m_lastMusicID, 0.1f, 0);
				}
			}
            m_EnableBGM = value;
		}
	}

    ////////////////////////////////////方法实现//////////////////////////////////////////
	
	void Awake()
	{
        DontDestroyOnLoad(this.gameObject);

        for (int i = 0; i < m_SFXChannelsCount; ++i)
        {
            m_SFXChannel[i] = new MyAudioSource();
            m_SFXChannel[i].m_uID = -1;
            m_SFXChannel[i].m_AudioSource = gameObject.AddComponent<AudioSource>() as AudioSource;
        }
        m_BGAudioSource.m_AudioSource = gameObject.AddComponent<AudioSource>() as AudioSource;
        m_BGAudioSource.m_uID = -1;
        m_SoundClipPools = new SoundClipPools();
	}
	

	void Start()
	{
        if (PlayerPreferenceData.SystemMusic == 0)
        {
            //TODO
            //====SystemLogic Open times  set  EnableBGM = false   
            EnableBGM = false;
        }
        else 
        {
            EnableBGM = true;
        }
        if (PlayerPreferenceData.SystemSoundEffect == 0)
        {
            EnableSFX = false;
        }
        else 
        {

            EnableSFX = true;
        }
	}
	
    void FixedUpdate()
    {
        UpdateBGMusic();
    }

    void UpdateBGMusic()
    {
        if (m_fadeMode == FadeMode.FadeOut)
        {
            if (Math.Abs(m_fadeOutTime) < 0.001f)  return;  //保护代码

            m_fadeOutTimer += Time.deltaTime;
            m_BGAudioSource.m_AudioSource.volume = (1 - m_fadeOutTimer / m_fadeOutTime) * m_bgmVolume;
            if (m_fadeOutTimer >= m_fadeOutTime)
            {
                //add by sunyu 2014-07-31
                //force Remove clip from pool, special for bgmusic
                short deluid = m_BGAudioSource.m_uID;
                SetAudioSource(ref m_BGAudioSource, m_NextSoundClip, 1.0f);
                m_SoundClipPools.ForceRemoveClipByID(deluid);
                if (m_fadeInTime > 0)
                {
                    m_fadeMode = FadeMode.FadeIn;
                    m_fadeOutTimer = 0;
                    m_BGAudioSource.m_AudioSource.volume = 0;
                }
                else
                {
                    m_fadeMode = FadeMode.None;
                    //m_BGAudioSource.volume = m_bgmVolume;
                }
                m_BGAudioSource.m_AudioSource.Play();
            }
        }
        else if (m_fadeMode == FadeMode.FadeIn)
        {
            if (Math.Abs(m_fadeInTime) < 0.001f) return; //保护代码

            m_fadeInTimer += Time.deltaTime;
            m_BGAudioSource.m_AudioSource.volume = m_fadeInTimer / m_fadeInTime * m_bgmVolume;
            if (m_fadeInTimer >= m_fadeInTime)
            {
                m_fadeMode = FadeMode.None;
                m_fadeInTimer = 0;
                m_BGAudioSource.m_AudioSource.volume = m_CurBGVolume;
            }
        }
    }	

    /// <summary>
    /// 淡入淡出播放背景音乐
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="fadeInTime">淡入时间</param>
    /// <param name="fadeOutTime">淡出时间</param>
    private void PlayBGMWithFade(int nSoundclipId, float fadeOutTime, float fadeInTime)
    {
        m_SoundClipPools.GetSoundClip(nSoundclipId, OnPlayBGMWithFade, new SoundClipPools.SoundClipParam(nSoundclipId, fadeOutTime, fadeInTime));
    }

    void OnPlayBGMWithFade(SoundClip bgSoundClip, SoundClipPools.SoundClipParam param)
    {
        if (m_BGAudioSource != null && bgSoundClip != null)
        {
            m_lastMusicID = param.m_clipID;
            if (m_BGAudioSource.m_AudioSource.isPlaying) //正常播放上一首背景音乐
            {
                if (m_NextSoundClip != null && m_NextSoundClip.m_uID == bgSoundClip.m_uID)
                {
                    return;
                }
                m_fadeOutTime = param.m_fadeOutTime;
                m_fadeInTime = param.m_fadeInTime;
                m_fadeOutTimer = 0;
                m_fadeInTimer = 0;
                if (m_fadeOutTime <= 0)
                {
                    //add by sunyu 2014-07-31
                    //force Remove clip from pool, special for bgmusic
                    short deluid = m_BGAudioSource.m_uID;
                    SetAudioSource(ref m_BGAudioSource, bgSoundClip, 1.0f);
                    m_SoundClipPools.ForceRemoveClipByID(deluid);

                    m_CurBGVolume = bgSoundClip.m_volume;
                    if (m_fadeInTime <= 0)
                    {
                        m_BGAudioSource.m_AudioSource.Play();
                        m_fadeMode = FadeMode.None;
                    }
                    else
                    {
                        m_BGAudioSource.m_AudioSource.volume = 0;
                        m_BGAudioSource.m_AudioSource.Play();
                        m_fadeMode = FadeMode.FadeIn;
                    }
                }
                else
                {
                    m_NextSoundClip = bgSoundClip;
                    m_fadeMode = FadeMode.FadeOut;
                }
            }
            else //没在播放，直接播放
            {
                m_fadeInTime = param.m_fadeInTime;
                m_fadeInTimer = 0;

                //add by sunyu 2014-07-31
                //force Remove clip from pool, special for bgmusic
                short deluid = m_BGAudioSource.m_uID;
                SetAudioSource(ref m_BGAudioSource, bgSoundClip, 1.0f);
                m_SoundClipPools.ForceRemoveClipByID(deluid);

                m_CurBGVolume = bgSoundClip.m_volume;
                if (m_fadeInTime <= 0)
                {
                    m_BGAudioSource.m_AudioSource.Play();
                    m_fadeMode = FadeMode.None;
                }
                else
                {
                    m_BGAudioSource.m_AudioSource.volume = 0;
                    m_BGAudioSource.m_AudioSource.Play();
                    m_fadeMode = FadeMode.FadeIn;
                }
            }
        }
    }

    /// <summary>
    /// 淡入淡出播放背景音乐
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="fadeOutTime">淡出时间</param>
    /// <param name="fadeInTime">淡入时间</param>
    public void PlayBGMusic(int nClipID, float fadeOutTime, float fadeInTime)
    {
		if ( AudioListener.volume == 0 )
			return;

        if (nClipID < 0)
        {
            LogModule.ErrorLog("PlayBGM id < 0");
            return;
        }

        m_lastMusicID = (short)nClipID;

        if (m_EnableBGM)
        {
            PlayBGMWithFade(nClipID, fadeOutTime, fadeInTime);
        }
        else
		{
			 //m_BGAudioSource.clip = null;
		}
    }

    

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    /// <param name="_fadeTime"></param>
    public void StopBGM(float _fadeTime)
    {
        if (m_EnableBGM)
        {
            StartCoroutine(StopBGMWithFade(_fadeTime));
        }
    }

    private IEnumerator StopBGMWithFade(float _fadeTime)
    {
        if (m_BGAudioSource != null)
        {
            if (m_BGAudioSource.m_AudioSource.isPlaying)
            {
                float time = _fadeTime;
                while (time > 0)
                {
                    m_BGAudioSource.m_AudioSource.volume = (time / m_fadeOutTime) * m_bgmVolume;
                    time -= Time.deltaTime;

                    yield return null;
                }

                m_BGAudioSource.m_AudioSource.Stop();
            }
        }
    }

    /// <summary>
    /// 停止音效
    /// </summary>
    /// <param name="name">名称,声音表格第一列</param>
    public void StopSoundEffect(int nSoundID)
    {
        if (m_SFXChannel == null)
        {
            return;
        }

        for (int nIndex = 0; nIndex < m_SFXChannelsCount; nIndex++)
        {
            if (m_SFXChannel[nIndex].m_AudioSource == null)
            {
                continue;
            }

            if (m_SFXChannel[nIndex].m_uID == nSoundID)
            {
                m_SFXChannel[nIndex].m_AudioSource.Stop();
            }
        }
    }

    public void StopAllSoundEffect()
    {
        if (m_SFXChannel != null)
        {
            for (int nIndex = 0; nIndex < m_SFXChannelsCount; ++nIndex)
            {
                if (m_SFXChannel[nIndex] != null)
                {
                    m_SFXChannel[nIndex].m_AudioSource.Stop();
                }
            }
        }
    }


    //////////////////////////////////播放音效////////////////////////////////////
    /// <summary>
    /// 根据目标的坐标和接收者的坐标listenerPos的距离来确定音量,用于技能音效
    /// </summary>
    /// <param name="nSoundID"></param>
    /// <param name="playSoundPos"></param>
    /// <param name="listenerPos"></param>
    public void PlaySoundEffectAtPos(int nSoundID, Vector3 playSoundPos, Vector3 listenerPos)
    {
        if (nSoundID < 0)
        {
            return;
        }

        float volume = 1.0f;
        listenerPos.y = 0;
        playSoundPos.y = 0;

        float distance = Vector3.Distance(listenerPos, playSoundPos);

        volume = (0.4f - distance / 10.0f)*2.5f;
        if (volume < 0.01f)
        {
            volume = 0.01f;
            return;     //声音过低就返回
        }
        else if (volume > 1.0f)
        {
            volume = 1.0f;
        }

        PlaySoundEffect(nSoundID, volume);
    }

    //////////////////////////////////播放音效////////////////////////////////////
    /// <summary>
    /// 根据目标的坐标和接收者的坐标listenerPos的距离来确定音量，用于受击、死亡
    /// </summary>
    /// <param name="nSoundID"></param>
    /// <param name="playSoundPos"></param>
    /// <param name="listenerPos"></param>
    public void PlaySoundEffectAtPos2(int nSoundID, Vector3 playSoundPos, Vector3 listenerPos)
    {
        if (nSoundID < 0)
        {
            return;
        }

        float volume = 1.0f;
        listenerPos.y = 0;
        playSoundPos.y = 0;

        float distance = Vector3.Distance(listenerPos, playSoundPos);

        volume = (0.6f - distance / 10.0f)*1.67f;
        if (volume < 0.05f)
        {
            volume = 0.05f;
            return;     //声音过低就返回
        }
        else if (volume > 1.0f)
        {
            volume = 1.0f;
        }

        PlaySoundEffect(nSoundID, volume);
    }


    /// <summary>
    /// 播放音效，默认音量缩放系数可以不填，配表值
    /// </summary>
    /// <param name="name"></param>
    /// <param name="volumeFactor">音量缩放系数</param>
    /// <returns></returns>
    public void PlaySoundEffect(int nSoundID, float volumeFactor = 1.0f)
    {
	
        if (AudioListener.volume == 0)
            return;

        if (!m_EnableSFX)
        {
            return;
        }

        if (name == null)
        {
            LogModule.ErrorLog("PlaySoundEffect name is null");
            return;
        }

        name = name.Trim();

        m_SoundClipPools.GetSoundClip(nSoundID, OnPlaySoundEffect, new SoundClipPools.SoundClipParam(volumeFactor));           
    }

    void OnPlaySoundEffect(SoundClip soundClip, SoundClipPools.SoundClipParam param)
    {
        if (soundClip == null)
        {
            LogModule.ErrorLog("soundClip is null");
            return;
        }

        PlaySoundEffect(soundClip, param.m_volumeFactor);
    }



    /// <summary>
    /// 播放soundClip中的音效
    /// </summary>
    /// <param name="soundClip"></param>
    /// <param name="volume"></param>
    /// <returns></returns>
    private void PlaySoundEffect(SoundClip soundClip, float volumeFactor)
    {
        if (m_EnableSFX && !string.IsNullOrEmpty(soundClip.m_path))
        {
            if (soundClip.Audioclip == null)
            {
                LogModule.ErrorLog("PlaySoundEffect soundClip.Audioclip is null");
                return;
            }

            int leastImportantIndex = 0;
            int nCurMaxPlayingCount = 0; //最大播放次数
            int nFirstEmptyIndex = -1; //第一个空位
            int nFirstSameClipValidIndex = -1; //第一个已经停止的上次播放过的位置

            for (int nIndex = 0; nIndex < m_SFXChannelsCount; ++nIndex)   //先找已经在播放或者播放过的
            {
                if (m_SFXChannel[nIndex] == null) return; //error

                if (nFirstEmptyIndex == -1 && !m_SFXChannel[nIndex].m_AudioSource.isPlaying)  //记录第一个可用的空位置
                {
                    nFirstEmptyIndex = nIndex;
                }

                if (m_SFXChannel[nIndex].m_AudioSource.clip == null) continue;

                if (m_SFXChannel[nIndex].m_uID == soundClip.m_uID) //有播放过的内容
                {
                    if (nFirstSameClipValidIndex == -1 && !m_SFXChannel[nIndex].m_AudioSource.isPlaying)  //记录第一个已经停止的上次播放过的位置
                    {
                        nFirstSameClipValidIndex = nIndex;
                    }

                    if (m_SFXChannel[nIndex].m_AudioSource.isPlaying) ++nCurMaxPlayingCount;  //正在播放的计数
                    if (nCurMaxPlayingCount >= soundClip.m_curMaxPlayingCount) //已经超过了，不播放了
                    {
                        break;
                    }
                }

                if (m_SFXChannel[leastImportantIndex].m_AudioSource.priority < m_SFXChannel[nIndex].m_AudioSource.priority)  //记录最低优先级
                {
                    leastImportantIndex = nIndex;
                }
            }

            if (nCurMaxPlayingCount < soundClip.m_curMaxPlayingCount)    //没到播放数量限制，直接播放
            {
                int nValidIndex = -1;        //先选择已经停止播放的，如果没有，选第一个空的，如果也没有空的，替换优先数字最高的
                if (nFirstSameClipValidIndex != -1)
                {
                    nValidIndex = nFirstSameClipValidIndex;
                }
                else
                {
                    if (nFirstEmptyIndex != -1)
                    {
                        nValidIndex = nFirstEmptyIndex;
                    }
                    else
                    {
                        nValidIndex = leastImportantIndex;
                    }
                }

                if (nValidIndex >= 0 && nValidIndex < m_SFXChannelsCount)
                {
                    m_SFXChannel[nValidIndex].m_AudioSource.Stop();
                    SetAudioSource(ref m_SFXChannel[nValidIndex], soundClip, volumeFactor);
                    m_SFXChannel[nValidIndex].m_AudioSource.PlayDelayed(soundClip.m_delay);

                    return;// m_SFXChannel[nValidIndex]; 
                }
            }
            else
            {
                //达到播放上限，不播放
                //LogModule.DebugLog("Warning PlaySoundEffect " + soundClip.m_name + " PlayingCount = " + nCurMaxPlayingCount);
            }

        }
    }

    //赋值
    private void SetAudioSource(ref MyAudioSource audioSource, SoundClip clip, float volumeFactor)
    {
        if (clip == null)
        {
            LogModule.ErrorLog("Error Clip null, please resolve");
            return;
        }
        audioSource.m_AudioSource.clip = clip.Audioclip;
        audioSource.m_AudioSource.volume = clip.m_volume * volumeFactor * m_sfxVolume;
        audioSource.m_AudioSource.spread = clip.m_spread;
        audioSource.m_AudioSource.priority = clip.m_priority;
        audioSource.m_AudioSource.panLevel = clip.m_panLevel;
        audioSource.m_AudioSource.minDistance = clip.m_minDistance;
        audioSource.m_AudioSource.loop = clip.m_isLoop;
        audioSource.m_uID = clip.m_uID;
    }

    //////////////////////////////////播放音效结束////////////////////////////////////
	
}
