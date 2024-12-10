using UnityEngine;
using System.Collections;
using GCGame.Table;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class RollNotice : MonoBehaviour
{

    public UILabel objNoticeLables;
    public GameObject BackGround;

    public float ClipHeight; // 切片高度
    public float ShowTime = 5.0f;  // 展示时长
    public float AnimiTime = 1.0f;      // 动画时长

    private float m_ShowTime_Timer = 0; // 显示时长计时器
    private float m_Animi_Timer = 0;   // 动画时长


    private static RollNotice m_instance;
    public static RollNotice Instance()
    {
        return m_instance;
    }

    private bool bPlayRollFlag = false;
    public void AddRollNotice(string strNotice)
    {
        if (string.IsNullOrEmpty(strNotice))
        {
            return;
        }

        List<string> NoticeList = GameManager.gameManager.PlayerDataPool.RollNoticeList;
        NoticeList.Add(strNotice);
        if (NoticeList.Count > 0)
        {
            ShowRollNotice();
        }
    }

    void Awake()
    {
        m_instance = this;
    }

    // Use this for initialization
    void Start()
    {
        CleanUp();
    }

    // Update is called once per frame
    void Update()
    {
        // 动画
        if (m_Animi_Timer > 0)
        {
            Vector3 curLabelPos = objNoticeLables.transform.localPosition;
            m_Animi_Timer -= Time.deltaTime;
            curLabelPos.y += Time.deltaTime * ClipHeight;
            objNoticeLables.transform.localPosition = curLabelPos;

            if (curLabelPos.y > ClipHeight)
            {
                List<string> NoticeList = GameManager.gameManager.PlayerDataPool.RollNoticeList;
                if (NoticeList.Count > 0)
                {
                    bPlayRollFlag = false;
                    ShowRollNotice();
                }
                else
                {
                    CleanUp();
                }
                return;
            }

            if (m_Animi_Timer <= 0)
            {
                objNoticeLables.transform.localPosition = new Vector3(0, 0, 0);
                m_ShowTime_Timer = ShowTime;
            }
        }

        // 停留
        if (m_ShowTime_Timer > 0)
        {
            m_ShowTime_Timer -= Time.deltaTime;

            if (m_ShowTime_Timer <= 0)
            {
                m_Animi_Timer = AnimiTime;
            }
        }

    }

    void OnDestroy()
    {
        m_instance = null;
    }

    void Init()
    {
        if (objNoticeLables && BackGround)
        {
            objNoticeLables.transform.localPosition = new Vector3(0, -ClipHeight, 0);
            objNoticeLables.text = "";
            if (BackGround.activeInHierarchy == false)
            {
                BackGround.SetActive(true);
            }
        }

        m_Animi_Timer = AnimiTime;
    }

    void CleanUp()
    {
        bPlayRollFlag = false;
        if (objNoticeLables && BackGround)
        {
            objNoticeLables.text = "";
            BackGround.SetActive(false);
        }
    }

    public void ShowRollNotice()
    {
        if (bPlayRollFlag)
        {
            return;
        }
        List<string> NoticeList = GameManager.gameManager.PlayerDataPool.RollNoticeList;
        if (NoticeList.Count <= 0)
        {
            return;
        }
        string StrDic = NoticeList[0];
        if (objNoticeLables)
        {
            if (!string.IsNullOrEmpty(StrDic))
            {
                Init();
                string str = "";
                char firstChar = StrDic[0];
                if (firstChar != '#')
                {
                    str = StrDic;
                }
                else
                {
					str = DeletLink(StrDictionary.GetServerErrorString(StrDic));
                }
                objNoticeLables.text = str;
            }
        }

        bPlayRollFlag = true;
        NoticeList.RemoveAt(0);
    }

    string DeletLink(string str)
    {
        string str1 = Regex.Replace(str, @"<a>", "");
        return Regex.Replace(str1, @"</a>", "");
    }
}