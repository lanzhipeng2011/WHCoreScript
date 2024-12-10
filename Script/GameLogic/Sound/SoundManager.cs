/********************************************************************************
 *	�ļ�����	SoundManager.cs
 *	ȫ·����	\Script\Scene\SoundManager.cs
 *	�����ˣ�	����
 *	����ʱ�䣺2013-11-21
 *
 *	����˵��������������
 *	�޸ļ�¼��
 *	2013-11-22 ��ȥ��Э�̣�����������طֳ�2����һ��������أ����米�����֣���һ���ǳ�����صģ��������＼�ܣ�
 *	������ص�3D��Դ�����������ϣ��������ƶ�
 *	2013-12-12 ����������һ�и�Ϊ���ƣ��������ֹ����������
 *	2013-12-18 �ǳ������֣����缼�����֣������ĵ������������,����PanLevel��Spread
 *	2013-12-24 PanLevel��Spread�������
 *	2014-03-13 �������ֺͳ���������Чʹ��ͬһ�����ӣ����Ӵ�С�̶��������ʱ��δʱ�伴�滻�㷨���³������ݣ�
 *	��ʼʱ����������m_SFXChannelsCount��AudioSource�����ڲ��ű������ֺ���Ч��ͬʱ�ܹ����ŵ����������Ҳ��m_SFXChannelsCount����
 *	���⣬AudioSource�ҽ���SoundManager���������
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

    //�������
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

    //����ʱ����
    public float m_LastActiveTime = 0.0f;  //�ϴλ�Ծʱ��,�ϴβ���ʱ��
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

    private Dictionary<int, SoundClip> m_SoundClipMap = new Dictionary<int,SoundClip>();    //��Ч�б������������MAX

    /// <summary>
    /// �����������Ƶõ�SoundClip�������ڻ��Զ����
    /// </summary>
    /// <param name="name">����</param>
    /// <returns></returns>
    public void GetSoundClip(int nSoundId, GetSoundClipDelegate delFun, SoundClipParam param)
    {
        if (nSoundId >= 0)
        {
            if (m_SoundClipMap.ContainsKey(nSoundId))
            {
                //�����ϴβ���ʱ��
                m_SoundClipMap[nSoundId].m_LastActiveTime = Time.realtimeSinceStartup;
                if (null != delFun) delFun(m_SoundClipMap[nSoundId], param);
                return;
            }
            else
            {
                if (m_SoundClipMap.Count > SoundManager.m_SFXChannelsCount) //�������ֵ��ɾ���ʱ��δʹ�õ�
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
    /// ɾ���ʱ��δʹ�õ���Ŀ
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

        LogModule.DebugLog("RemoveLastUnUsedClip( " + smallestId.ToString() + " )");  //�Ժ�ע�͵�

        m_SoundClipMap.Remove(smallestId);
    }

    //add by sunyu 2014-07-31
    //force Remove clip from pool, special for bgmusic
    public void ForceRemoveClipByID(short uid)
    {
        if(uid != -1)
        {
            int nNeeddelId = -1;
            foreach (SoundClip clip in m_SoundClipMap.Values) //dictionary �� foreachȥ����
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
        public short m_uID;         //����������Ψһ��ʾ��
        public AudioSource m_AudioSource;
    }

    public SoundClipPools m_SoundClipPools; //�������ݳ� 
    private MyAudioSource m_BGAudioSource = new MyAudioSource(); //��������Դ
    private float       m_CurBGVolume = 0; //��ǰ������������
    public static int m_SFXChannelsCount = 30; //�����������
    private MyAudioSource[] m_SFXChannel = new MyAudioSource[m_SFXChannelsCount]; //Sound FX��������Ч��
    private SoundClip m_NextSoundClip = null; 
    public static bool m_EnableBGM = false;     //�Ƿ����ñ�������
    public static bool m_EnableSFX = false;      //�Ƿ����û�����Ч
	private short m_lastMusicID = 1001;//-1;     //�ϴβ��ŵĳ�����������Id�������жϺ��ز�

    public float m_sfxVolume = 1.0f;        //��Ч����ϵ��
    public float m_bgmVolume = 1.0f;        //����������������ϵ��

    private enum FadeMode //����ģʽ
    {
        None,
        FadeIn, //����
        FadeOut //����
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

    ////////////////////////////////////����ʵ��//////////////////////////////////////////
	
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
            if (Math.Abs(m_fadeOutTime) < 0.001f)  return;  //��������

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
            if (Math.Abs(m_fadeInTime) < 0.001f) return; //��������

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
    /// ���뵭�����ű�������
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="fadeInTime">����ʱ��</param>
    /// <param name="fadeOutTime">����ʱ��</param>
    private void PlayBGMWithFade(int nSoundclipId, float fadeOutTime, float fadeInTime)
    {
        m_SoundClipPools.GetSoundClip(nSoundclipId, OnPlayBGMWithFade, new SoundClipPools.SoundClipParam(nSoundclipId, fadeOutTime, fadeInTime));
    }

    void OnPlayBGMWithFade(SoundClip bgSoundClip, SoundClipPools.SoundClipParam param)
    {
        if (m_BGAudioSource != null && bgSoundClip != null)
        {
            m_lastMusicID = param.m_clipID;
            if (m_BGAudioSource.m_AudioSource.isPlaying) //����������һ�ױ�������
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
            else //û�ڲ��ţ�ֱ�Ӳ���
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
    /// ���뵭�����ű�������
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="fadeOutTime">����ʱ��</param>
    /// <param name="fadeInTime">����ʱ��</param>
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
    /// ֹͣ��������
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
    /// ֹͣ��Ч
    /// </summary>
    /// <param name="name">����,��������һ��</param>
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


    //////////////////////////////////������Ч////////////////////////////////////
    /// <summary>
    /// ����Ŀ�������ͽ����ߵ�����listenerPos�ľ�����ȷ������,���ڼ�����Ч
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
            return;     //�������;ͷ���
        }
        else if (volume > 1.0f)
        {
            volume = 1.0f;
        }

        PlaySoundEffect(nSoundID, volume);
    }

    //////////////////////////////////������Ч////////////////////////////////////
    /// <summary>
    /// ����Ŀ�������ͽ����ߵ�����listenerPos�ľ�����ȷ�������������ܻ�������
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
            return;     //�������;ͷ���
        }
        else if (volume > 1.0f)
        {
            volume = 1.0f;
        }

        PlaySoundEffect(nSoundID, volume);
    }


    /// <summary>
    /// ������Ч��Ĭ����������ϵ�����Բ�����ֵ
    /// </summary>
    /// <param name="name"></param>
    /// <param name="volumeFactor">��������ϵ��</param>
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
    /// ����soundClip�е���Ч
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
            int nCurMaxPlayingCount = 0; //��󲥷Ŵ���
            int nFirstEmptyIndex = -1; //��һ����λ
            int nFirstSameClipValidIndex = -1; //��һ���Ѿ�ֹͣ���ϴβ��Ź���λ��

            for (int nIndex = 0; nIndex < m_SFXChannelsCount; ++nIndex)   //�����Ѿ��ڲ��Ż��߲��Ź���
            {
                if (m_SFXChannel[nIndex] == null) return; //error

                if (nFirstEmptyIndex == -1 && !m_SFXChannel[nIndex].m_AudioSource.isPlaying)  //��¼��һ�����õĿ�λ��
                {
                    nFirstEmptyIndex = nIndex;
                }

                if (m_SFXChannel[nIndex].m_AudioSource.clip == null) continue;

                if (m_SFXChannel[nIndex].m_uID == soundClip.m_uID) //�в��Ź�������
                {
                    if (nFirstSameClipValidIndex == -1 && !m_SFXChannel[nIndex].m_AudioSource.isPlaying)  //��¼��һ���Ѿ�ֹͣ���ϴβ��Ź���λ��
                    {
                        nFirstSameClipValidIndex = nIndex;
                    }

                    if (m_SFXChannel[nIndex].m_AudioSource.isPlaying) ++nCurMaxPlayingCount;  //���ڲ��ŵļ���
                    if (nCurMaxPlayingCount >= soundClip.m_curMaxPlayingCount) //�Ѿ������ˣ���������
                    {
                        break;
                    }
                }

                if (m_SFXChannel[leastImportantIndex].m_AudioSource.priority < m_SFXChannel[nIndex].m_AudioSource.priority)  //��¼������ȼ�
                {
                    leastImportantIndex = nIndex;
                }
            }

            if (nCurMaxPlayingCount < soundClip.m_curMaxPlayingCount)    //û�������������ƣ�ֱ�Ӳ���
            {
                int nValidIndex = -1;        //��ѡ���Ѿ�ֹͣ���ŵģ����û�У�ѡ��һ���յģ����Ҳû�пյģ��滻����������ߵ�
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
                //�ﵽ�������ޣ�������
                //LogModule.DebugLog("Warning PlaySoundEffect " + soundClip.m_name + " PlayingCount = " + nCurMaxPlayingCount);
            }

        }
    }

    //��ֵ
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

    //////////////////////////////////������Ч����////////////////////////////////////
	
}
