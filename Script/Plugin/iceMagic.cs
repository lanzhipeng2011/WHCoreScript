using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Module.Log;
public class iceMagic : MonoBehaviour {

    public enum PlayMode
    {
        ONCE,
        LOOP,
    }
    public Texture[] frames;   //����һ�����飬�����ͼ����������inspector�ῴ��һ��frames�����飬����ĳ��ȿ����Լ����1���ʹ���ֻ��1��ͼ�����԰�һ��texture�Ͻ�ȥ����2�ʹ���2�ţ��Դ�����
	public float TailTime = 0.05f;
    public int framesPerSecond = 10;  //����fps,ÿ�벥�ż�֡��Ӱ�춯�����ٶȡ�

    public int xCount;
    public int yCount;
    public PlayMode curMode = PlayMode.LOOP;
    private List<Texture2D> curTextrueList = new List<Texture2D>();
    private int cellFrameCount = 0;
	private int m_curIndex = 0;
    private float m_curTimer;

	private float m_delayTimer = 0;

	// Use this for initialization
	void OnEnable()
	{
		m_curIndex = 0;
        m_curTimer = 0;
		if (gameObject.particleSystem != null) {
			m_delayTimer = gameObject.particleSystem.startDelay;
			if(frames.Length > 0)
			{
				framesPerSecond = (int)(frames.Length / (gameObject.particleSystem.startLifetime - TailTime));
			}
			else
			{
				framesPerSecond = (int)(xCount * yCount / (gameObject.particleSystem.startLifetime - TailTime));
			}

		}
	}

	void Start () 
	{
        if (frames.Length > 0)
        {
            m_curIndex = 0;
            m_curTimer = 0;
            cellFrameCount = 0;
        }
        else
        {
            m_curIndex = 0;
            m_curTimer = 0;
            if (xCount > 0 && yCount > 0 && null != renderer)
            {
                Texture2D resTexture = renderer.material.mainTexture as Texture2D;
                int cellWidth = resTexture.width / xCount;
                int cellHeight = resTexture.height / yCount;
                cellFrameCount = xCount * yCount;
                for (int j = 0; j < yCount; j++)
                {
                    for (int i = 0; i < xCount; i++)
                    {
                        Texture2D curTexture = new Texture2D(cellWidth, cellHeight, resTexture.format, false);
                        curTexture.SetPixels(0, 0, cellWidth, cellHeight, resTexture.GetPixels(i * cellWidth, resTexture.height - (j + 1) * cellHeight, cellWidth, cellHeight));
                        curTexture.Apply();
                        curTextrueList.Add(curTexture);
                    }
                }
            }
        }
		
	}

    // Update is called once per frame
	void Update() 
    {
		if (m_delayTimer > 0) {
			m_delayTimer -= Time.deltaTime;
			return;
				}
        if (cellFrameCount > 0)
        {
            if (curMode == PlayMode.LOOP)
            {
                m_curIndex = (int)(Time.time * framesPerSecond) % cellFrameCount;
                if (null != renderer)
                    renderer.material.SetTexture("_MainTex", curTextrueList[m_curIndex]);
            }
            else
            {
                m_curTimer += Time.deltaTime;
                m_curIndex = (int)(framesPerSecond * m_curTimer);

                if (null != renderer)
                {
                    if (m_curIndex >= curTextrueList.Count)
                    {
                        renderer.material.SetTexture("_MainTex", curTextrueList[curTextrueList.Count - 1]);
                    }
                    else
                    {
                        renderer.material.SetTexture("_MainTex", curTextrueList[m_curIndex]);
                    }
                }
            }
            
			
        }
        else if (null != frames && frames.Length > 0)
        {
            if (curMode == PlayMode.LOOP)
            {
                m_curIndex = (int)(Time.time * framesPerSecond) % frames.Length;
                if (null != renderer)
                    renderer.material.mainTexture = frames[m_curIndex];                      //��Ⱦ�����ͼ
            }
            else
            {
                m_curTimer += Time.deltaTime;
                m_curIndex = (int)(framesPerSecond * m_curTimer);
                if (null != renderer)
                {
                    if (m_curIndex >= frames.Length)
                    {
                        renderer.material.SetTexture("_MainTex", frames[frames.Length - 1]);
                    }
                    else
                    {
                        renderer.material.SetTexture("_MainTex", frames[m_curIndex]);
                    }
                }
            }
            
			
        }
	}
}
