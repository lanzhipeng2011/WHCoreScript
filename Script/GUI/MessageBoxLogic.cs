//********************************************************************
// 文件名: MessageBoxLogic.cs
// 描述: 消息框脚本
// 作者: WangZhe
//********************************************************************

using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using GCGame.Table;
using GCGame;

public class MessageBoxLogic : UIControllerBase<MessageBoxLogic> {

    public enum PASSWORD
    {
        INVALID = -1,
        MARRYROOT = 0,
    }
    
    public UILabel labelTitle;                            // 提示标题
    public UILabel labelText;                             // 提示内容
    public GameObject m_MessageBoxOKButton;               // 确定按钮
    public GameObject m_MessageBoxCancelButton;           // 取消按钮
    public GameObject detailBandRoot;                     // 详细信息节点
    public UISprite sprBackGround;                        // 背景
    public UIFont m_Font;
    public GameObject m_MessageInfo;

    public Vector3 btnLelfPos;                            // 按钮在左边的位置
    public Vector3 btnCenterPos;                          // 按钮在中间的位置
    public Vector3 btnRightPos;                           // 按钮在右边的位置

    public delegate void OnOKClick();
    private OnOKClick deleOK;                             // 确定按钮响应函数托管
    public delegate void OnCancelClick();
    private OnCancelClick deleCancel;                     // 取消按钮响应函数托管
    public delegate void OnWaitTimeOut();
    private OnWaitTimeOut delWaitTimeOut;                 // 等待超时
    private float m_waitTimer = 0;
    private float m_delayTimer = 0;
    private float m_fCountDown = -1;  //倒计时时间 -1代表无倒计时,秒标记
    private float TextHeight = GlobeVar.INVALID_ID;
    private PASSWORD m_ePassword = PASSWORD.INVALID;
	private bool  isIcon = false;
    public PASSWORD EPassword
    {
        get { return m_ePassword; }
        set { m_ePassword = value; }
    }

	
	// 新手指引相关
	private int m_NewPlayerGuideFlag_Step = -1;
	public int NewPlayerGuideFlag_Step
	{
		get { return m_NewPlayerGuideFlag_Step; }
		set { m_NewPlayerGuideFlag_Step = value; }
	}

    private string[] symbols = new string[4] { "#q1", "#q2", "#q3", "#q5" };
    private string[] icons = new string[4] { "bi", "yuanbao1", "yuanbao2", "qian5" };
    void Awake()
    {
        SetInstance(this);
    }

    //void Start()
    //{
        
    //}

    //void OnDestroy()
    //{
        
    //}

	void Update () {

        if (m_delayTimer > 0)
        {
            m_delayTimer -= Time.deltaTime;
            if (m_delayTimer <= 0)
            {
                ShowBox();
            }

            return;
        }
        if (m_waitTimer > 0)
        {
            m_waitTimer -= Time.deltaTime;
            if (m_waitTimer <= 0)
            {
                UIManager.CloseUI(UIInfo.MessageBox);
                if (null != delWaitTimeOut) delWaitTimeOut();
            }
        }
        if (m_fCountDown > 0)
        {
            m_fCountDown -= Time.deltaTime;
            if (m_fCountDown <= 0 )
            {
                UIManager.CloseUI(UIInfo.MessageBox);
                if (null != deleCancel)
                {
                    deleCancel();
                }
            }
        }       	
	}

    void MessageBoxOK()
    {
        UIManager.CloseUI(UIInfo.MessageBox);
        if (null != deleOK)
        {
            deleOK();
        }
    }

    void MessageBoxCancel()
    {
        foreach (UISprite sprite in m_MessageInfo.GetComponentsInChildren<UISprite>())
        {
            Destroy(sprite.gameObject);
        }
        UIManager.CloseUI(UIInfo.MessageBox);
        if (null != deleCancel)
        {
            deleCancel();
        }
    }

    void CleanData()
    {
        deleOK = null;
        deleCancel = null;
        delWaitTimeOut = null;
        m_waitTimer = 0;
        m_delayTimer = 0;
        m_ePassword = PASSWORD.INVALID;
        ShowBox();
    }

    private static Color transColor = new Color(1, 1, 1, 0);
    void HideBox()
    {
        sprBackGround.color = transColor;
        detailBandRoot.SetActive(false);
    }

    void ShowBox()
    {
        sprBackGround.color = Color.white;
        detailBandRoot.SetActive(true);
    }
    /// <summary>
    /// 显示MessageBox
    /// </summary>
    /// <param name="strInfo">提示信息</param>
    /// <param name="eType">MessageBox类型</param>
    /// <param name="funcOKClicked">确定按钮响应函数</param>
    /// <param name="funcCancelClicked">取消按钮响应函数</param>
    public void ShowMessageBox(string title, string text, GameDefine_Globe.MESSAGEBOX_TYPE eType)
    {
        title = title == ""||title==null ? "提示信息" : title;
        if (TextHeight < 0)
        {
            TextHeight = m_Font.CalculatePrintedSize(" ", true, UIFont.SymbolStyle.Uncolored).y;
        }
        labelTitle.text = title;

        SetTextAnalyseSymbol(text);        
        if (isIcon==false) 
		{
			Vector3 pos=new Vector3(6.0f,m_MessageInfo.transform.localPosition.y,m_MessageInfo.transform.localPosition.z);
			m_MessageInfo.transform.localPosition=pos;
		}
        switch (eType)
        {
            case GameDefine_Globe.MESSAGEBOX_TYPE.TYPE_OK:
                {
                    m_MessageBoxOKButton.SetActive(true);
                    m_MessageBoxCancelButton.SetActive(false);
                    m_MessageBoxOKButton.transform.localPosition = btnCenterPos;

                }
                break;
            case GameDefine_Globe.MESSAGEBOX_TYPE.TYPE_OKCANCEL:
                {
                    m_MessageBoxOKButton.SetActive(true);
                    m_MessageBoxCancelButton.SetActive(true);
                    m_MessageBoxOKButton.transform.localPosition = btnLelfPos;
                    m_MessageBoxCancelButton.transform.localPosition = btnRightPos;
                }
                break;
            case GameDefine_Globe.MESSAGEBOX_TYPE.TYPE_WAIT:
                {
                    m_MessageBoxOKButton.SetActive(false);
                    m_MessageBoxCancelButton.SetActive(false);
                }
                break;
            case GameDefine_Globe.MESSAGEBOX_TYPE.TYPE_INVALID:
            default:
                break;
        }
        gameObject.SetActive(true);
    }

    public void SetTextAnalyseSymbol(string text)
    {
        foreach (UISprite sprite in m_MessageInfo.GetComponentsInChildren<UISprite>())
        {
            Destroy(sprite.gameObject);
        }
        for (int i = 0; i < 4; i++ )
        {
            int iconindex = text.IndexOf(symbols[i]);
            if (iconindex > 0)
            {
				isIcon=true;
                // prewidth-图标前面的总宽度
                float prewidth = m_Font.CalculatePrintedSize(text.Substring(0, iconindex), true, UIFont.SymbolStyle.Uncolored).x;
                // prelines-图标前面的行数 不包括图标行
                int prelines = Mathf.FloorToInt(prewidth / (float)labelText.width);
                // wholewidth-总宽度
				float wholewidth = m_Font.CalculatePrintedSize(text, true, UIFont.SymbolStyle.Uncolored).x;
                // lines-总行数
                int lines = Mathf.CeilToInt(wholewidth / (float)labelText.width);
                // 此时可以先确定图标Y位置
                float POSY = TextHeight / 2 * (lines - 1) - prelines * TextHeight;
				
				//POSY+=10;
                // 创建图标
                GameObject icon = ResourceManager.LoadMessageIcon(m_MessageInfo);
                if (icon != null && icon.GetComponent<UISprite>() != null)
                {
                    icon.GetComponent<UISprite>().spriteName = icons[i];                    

                    string strSpace = "";
					float spaceWidth = m_Font.CalculatePrintedSize("　", true, UIFont.SymbolStyle.Uncolored).x;
                    int iconWidth = icon.GetComponent<UISprite>().width;
                    int spaceNum = Mathf.CeilToInt((float)iconWidth / spaceWidth);
                    for (int j = 0; j < spaceNum; j++)
                    {
						if(icons[i] == "bi")
						{
                        	strSpace += "   ";
						}
						else 
						{
							strSpace += "    ";
						}
                    }   
					strSpace += "  ";
                    text = text.Replace(symbols[i], strSpace);

                    // 此时确定图标的X位置 为所加空格的中点
                    // prelineswidth-图标前面的行的宽度
                    float prelineswidth = 0;
                    if (prelines > 0)
                    {
                        for (int k = 0; k < iconindex; k++ )
                        {
							float width = m_Font.CalculatePrintedSize(text.Substring(0, k), true, UIFont.SymbolStyle.Uncolored).x;
                            if (width > labelText.width * prelines)
                            {
                                break;
                            }
                            prelineswidth = width;
                        }
                    }
                    // leftwidth-图标行左侧文字的宽度
                    float leftwidth = prewidth - prelineswidth;

                    // curlinewidth-图标行的宽度 此时图标已被空格替换
                    float curlinewidth = 0;
                    for (int k = iconindex + 1; k < text.Length; k++ )
                    {
						float width = m_Font.CalculatePrintedSize(text.Substring(0, k), true, UIFont.SymbolStyle.Uncolored).x - prelineswidth;
                        if (width > labelText.width)
                        {
                            break;
                        }
                        curlinewidth = width;
                    }

                    float POSX = 0;

					float StartPosX = - curlinewidth / 2.0f;
					if(icons[i] == "bi")
					{
						POSX = StartPosX + leftwidth + (float)(spaceNum * spaceWidth) / 4.0f;
					}
					else 
					{
						POSX = StartPosX + leftwidth + (float)(spaceNum * spaceWidth) / 2.0f;
					}
					//Debug.Log("StartPosX" + StartPosX + "   POSX" + POSX + "   curlinewidth" + curlinewidth + "   wholewidth" + wholewidth + "   leftwidth" + leftwidth);
                    // 因为文本居中显示 所以减去图标行行宽的一半
                    //POSX -= curlinewidth / 2.0f;
					//POSX+=10;
                    icon.transform.localPosition = new Vector3(POSX, POSY, 0);
					labelText.text = text;

                    break;
                }
            }
            else if (i == 3)
            {

                labelText.text = text;
            }
        }       
    }

    public static void CloseBox()
    {
        if (null != MessageBoxLogic.Instance())
        {
            MessageBoxLogic.Instance().CleanData();
        }
        UIManager.CloseUI(UIInfo.MessageBox);
    }

    private class OKCancelInfo
    {
        public OKCancelInfo(string text, string title = null, OnOKClick funcOKClicked = null, OnCancelClick funcCancelClicked = null, float fCountDown = GlobeVar.INVALID_ID, PASSWORD password = PASSWORD.INVALID)
        {
            _text = text;
            _title = title;
            _delOkClick = funcOKClicked;
            _delCancelClick = funcCancelClicked;
            _fCountDown = fCountDown;
            _password = password;
        }

        public string _text;
        public string _title;
        public float _fCountDown;
        public PASSWORD _password;

        public OnOKClick _delOkClick;
        public OnCancelClick _delCancelClick;
    }


    // 只有一个确定按钮
    public static void OpenOKBox(string text, string title = null, OnOKClick funcOKClicked = null, PASSWORD password = PASSWORD.INVALID)
    {
        OKCancelInfo curInfo = new OKCancelInfo(text, title, funcOKClicked, null, GlobeVar.INVALID_ID, password);
        UIManager.ShowUI(UIInfo.MessageBox, OnOpenOkBox, curInfo);
        
    }
    static void OnOpenOkBox(bool bSuccess , object param)
    {
        if (!bSuccess)
        {
            return;
        }
        if (MessageBoxLogic.Instance() != null)
        {
            OKCancelInfo curInfo = param as OKCancelInfo;
            MessageBoxLogic.Instance().CleanData();
            MessageBoxLogic.Instance().deleOK = curInfo._delOkClick;
            MessageBoxLogic.Instance().m_ePassword = curInfo._password;
            MessageBoxLogic.Instance().ShowMessageBox(curInfo._title, curInfo._text, GameDefine_Globe.MESSAGEBOX_TYPE.TYPE_OK);
        }
    }

    
    // 有确定取消按钮
    public static void OpenOKCancelBox(string text, string title = null, OnOKClick funcOKClicked = null, OnCancelClick funcCancelClicked = null, int nCountDown = GlobeVar.INVALID_ID, PASSWORD password = PASSWORD.INVALID)
    {
        OKCancelInfo curInfo = new OKCancelInfo(text, title, funcOKClicked, funcCancelClicked, nCountDown, password);
        UIManager.ShowUI(UIInfo.MessageBox, OnOpenOkCancelBox, curInfo);
    }

    static void OnOpenOkCancelBox(bool bSuccess, object param)
    {
        if (!bSuccess)
        {
            return;
        }
        if (MessageBoxLogic.Instance() != null)
        {
            OKCancelInfo curInfo = param as OKCancelInfo;
            MessageBoxLogic.Instance().CleanData();
            MessageBoxLogic.Instance().deleOK = curInfo._delOkClick;
            MessageBoxLogic.Instance().deleCancel = curInfo._delCancelClick;
            MessageBoxLogic.Instance().m_fCountDown = curInfo._fCountDown;
            MessageBoxLogic.Instance().m_ePassword = curInfo._password;
            MessageBoxLogic.Instance().ShowMessageBox(curInfo._title, curInfo._text, GameDefine_Globe.MESSAGEBOX_TYPE.TYPE_OKCANCEL);
        }
    }

    private class WaitBoxInfo
    {
        public WaitBoxInfo(string text, float duration, float delay, OnWaitTimeOut delWaitTimeOutFun, PASSWORD password)
        {
            _text = text;
            _duration = duration;
            _delay = delay;
            _delWaitTimeOut = delWaitTimeOutFun;
            _password = password;
        }

        public string _text;
        public float _duration;
        public float _delay;
        public OnWaitTimeOut _delWaitTimeOut;
        public PASSWORD _password;
    }
    // 等待界面
    // duration 等待时间，如果<=0 则无限等待,
    // delay 延时弹出时间，如有延时，则会先以透明底版的形式弹出，延时结束后显示内容
    public static void OpenWaitBox(string text, float duration = 0, float delay = 0,OnWaitTimeOut delWaitTimeOutFun = null, PASSWORD password = PASSWORD.INVALID)
    {
        WaitBoxInfo curInfo = new WaitBoxInfo(text, duration, delay, delWaitTimeOutFun, password);
        UIManager.ShowUI(UIInfo.MessageBox, OnOpenWaitBox, curInfo);
        
    }

    static void OnOpenWaitBox(bool bSuccess, object param)
    {
        if (!bSuccess)
        {
            return;
        }
        if (MessageBoxLogic.Instance() != null)
        {
            WaitBoxInfo curInfo = param as WaitBoxInfo;

            MessageBoxLogic.Instance().CleanData();
            MessageBoxLogic.Instance().delWaitTimeOut = curInfo._delWaitTimeOut;
            MessageBoxLogic.Instance().m_waitTimer = curInfo._duration;
            MessageBoxLogic.Instance().m_delayTimer = curInfo._delay;
            MessageBoxLogic.Instance().m_ePassword = curInfo._password;
            MessageBoxLogic.Instance().ShowMessageBox("", curInfo._text, GameDefine_Globe.MESSAGEBOX_TYPE.TYPE_WAIT);
            if (curInfo._delay > 0)
            {
                MessageBoxLogic.Instance().HideBox();
            }
        }
    }

	public void NewPlayerGuide(int nIndex)
	{
		if (nIndex < 0) 
		{
			return;		
		}
		
		NewPlayerGuidLogic.CloseWindow();
		
		m_NewPlayerGuideFlag_Step = nIndex;
		
		switch (m_NewPlayerGuideFlag_Step)
		{
		case 1:
			if(m_MessageBoxOKButton != null)
			{
				NewPlayerGuidLogic.OpenWindow(m_MessageBoxOKButton, 180, 64, "", "right", 2, true, true);
			}
			break;
		case 2:
			if(m_MessageBoxCancelButton != null)
			{
				NewPlayerGuidLogic.OpenWindow(m_MessageBoxCancelButton, 180, 64, "", "right", 2, true, true);
			}
			break;
		}
	}

    //----------------------------------重载便捷函数-------------------------------------------------------/
    public static void OpenOKBox(int textDicID, int titleDicID = -1, OnOKClick funcOKClicked = null, PASSWORD password = PASSWORD.INVALID)
    {
        MessageBoxLogic.OpenOKBox(Utils.GetDicByID(textDicID), titleDicID == -1 ? "" : Utils.GetDicByID((int)titleDicID), funcOKClicked, password);
    }

    public static void OpenOKCancelBox(int textDicID, int titleDicID = -1, OnOKClick funcOKClicked = null, OnCancelClick funcCancelClicked = null, PASSWORD password = PASSWORD.INVALID)
    {
        MessageBoxLogic.OpenOKCancelBox(Utils.GetDicByID(textDicID), Utils.GetDicByID((int)titleDicID), funcOKClicked, funcCancelClicked, GlobeVar.INVALID_ID, password);
    }

    public static void OpenWaitBox(int textDicID, float duration = GameDefines.CONNECT_TIMEOUT, float delay = GameDefines.CONNECT_WAIT_DELAY, OnWaitTimeOut delWaitTimeOutFun = null, PASSWORD password = PASSWORD.INVALID)
    {
        
        MessageBoxLogic.OpenWaitBox(Utils.GetDicByID(textDicID), duration, delay, delWaitTimeOutFun, password);
    }
}
