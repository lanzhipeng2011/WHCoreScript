//********************************************************************
// 文件名: JoyStickLogic.cs
// 描述: 摇杆逻辑
// 作者: WangZhe
// 创建时间: 2013-11-5
//
// 修改历史:
// 2013-11-5 王喆创建
//********************************************************************

using UnityEngine;
using System.Collections;
using Games.LogicObj;
using Module.Log;
using GCGame.Table;

public class JoyStickLogic : MonoBehaviour
{
    private static JoyStickLogic m_Instance = null;
    public static JoyStickLogic Instance()
    {
        return m_Instance;
    }

    public UISprite m_JoyStickSprite;                                         // 摇杆sprite
    private Vector3 m_nTouchPos = new Vector3(0, 0, 0);				// 手指位置 demo里是鼠标
    private Vector3 m_nJoyStickOrigin = new Vector3(0, 0, 0);		// 摇杆原位置 设置后不再更改
    public float m_MaxRadius = 50.0f;											// 摇杆最大拖动半径
    //private float m_MoveRadius;												// 判断玩家移动方向的距离 根据m_MaxRadius推算出来 让一个圆可以等分成8份
    private ProcessInput m_ProcessInput;                                    // 玩家输入
    private Vector3 m_MouseBuffer = new Vector3(0, 0, 0);         // 鼠标位置缓存
    //private bool m_IsPressed = false;

#if UNITY_ANDROID && !UNITY_EDITOR
    private float _differenceX = 1f;
    private float _differenceY = 1f;
    private float _difTag = 0.77f;
#endif

    private int m_FingerID = -1;
    public int FingerID
    {
        get { return m_FingerID; }
    }

    private int m_NewPlayerStep = 0;

    void Awake()
    {
        m_Instance = this;
    }
    // Use this for initialization
    void Start()
    {
        m_nJoyStickOrigin.x = gameObject.transform.localPosition.x;
        m_nJoyStickOrigin.y = gameObject.transform.localPosition.y;
        //m_MoveRadius = m_MaxRadius * Mathf.Cos(67.5f*Mathf.PI/180);
        m_ProcessInput = ProcessInput.Instance();
        m_JoyStickSprite.alpha = 0.5f;
       // m_BackGround1.alpha = 0.5f;
      //  m_BackGround2.alpha = 0.5f;
      //  m_BackGround1.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
      //  m_BackGround1.gameObject.GetComponent<Spin>().enabled = false;
        //m_IsPressed = false;
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Singleton<ObjManager>.Instance.MainPlayer && !Singleton<ObjManager>.Instance.MainPlayer.IsInModelStory
            && !Singleton<ObjManager>.Instance.MainPlayer.QingGongState)
        {
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
            if (!Singleton<ObjManager>.Instance.MainPlayer.IsDie() && !UIManager.IsSubUIShow())
            {
                if (Input.touchCount > 0)
                {
           
                    UpdateWithTouch();
                }
                else
                {
                    // 无触摸点 fingerID又不是-1 应对真机上使用摇杆同时按HOME键退出再进入的情况
                    if(m_FingerID != -1)
                    {
                        LogModule.DebugLog("iPhone HOME come back!");
                        m_FingerID = -1;
                        JoyStickEndMove();
                    }
                }
            }        
#endif
        }
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    void UpdateWithTouch()
    {
       
        if (m_FingerID == -1)
        {
            if (null == UICamera.mainCamera)
            {
                return;
            }

            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                Vector3 vecTouchPos = touch.position;
                Ray rayTouch = UICamera.mainCamera.ScreenPointToRay(vecTouchPos);
                RaycastHit hitTouch;
                if (Physics.Raycast(rayTouch, out hitTouch))
                {
                    // 如果点中二级界面 拦截 打开二级菜单时摇杆隐藏 取消判断 防止出错
                     if (hitTouch.collider.gameObject.tag == "SubUI")
                     {
                         return;
                     }
                    // 点中摇杆
                    if (hitTouch.collider.gameObject.transform.parent == gameObject.transform.parent)
                    {
                        if (NewPlayerGuidLogic.Instance() != null && NewPlayerGuidLogic.Instance().CurShowType == "JoyStick")
                        {
                            NewPlayerGuidLogic.CloseWindow();
                        }
                        if (touch.phase == TouchPhase.Began)
                        {
                            // 记录fingerID
                            m_FingerID = touch.fingerId;
                            ProcessInput.Instance().SceneTouchFingerID = -1;
                            //m_JoyStickCamera.GetComponent<UICamera>().JoyStickFingerID = m_FingerID;
                            // 摇杆开始移动
                            JoyStickStartMove();

#if UNITY_ANDROID && !UNITY_EDITOR
                            _differenceX = touch.position.x;
                            _differenceY = touch.position.y;
#endif
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.fingerId == m_FingerID && touch.phase == TouchPhase.Moved)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    JoyStickInMoving(touch.position);
#else
                    JoyStickInMoving(touch.deltaPosition);
#endif
                }
                else if (touch.fingerId == m_FingerID && touch.phase == TouchPhase.Ended)
                {
                    m_FingerID = -1;
                    //m_JoyStickCamera.GetComponent<UICamera>().JoyStickFingerID = m_FingerID;
                    JoyStickEndMove();
                }
            }
        }
    }

    /// <summary>
    /// 拖动响应
    /// </summary>
    void OnDrag()
    {
        // 改为只对电脑操作时鼠标响应 真机上的触摸响应写在update
#if UNITY_EDITOR
        if (!Singleton<ObjManager>.GetInstance().MainPlayer.IsDie() && !UIManager.IsSubUIShow())
        {
            m_nTouchPos.x = Input.mousePosition.x;
            m_nTouchPos.y = Input.mousePosition.y;
            // 两次之间的差值
            float nDeltaX = m_nTouchPos.x - m_MouseBuffer.x;
            float nDeltaY = m_nTouchPos.y - m_MouseBuffer.y;
            // 新的XY 先不设置
            float nNewX = gameObject.transform.localPosition.x + nDeltaX;
            float nNewY = gameObject.transform.localPosition.y + nDeltaY;
            // 计算距离
            float nDistance = GetDistance(nNewX, nNewY, m_nJoyStickOrigin.x, m_nJoyStickOrigin.y);
            if (nDistance <= m_MaxRadius)
            {
                // 若拖动位置在最大半径以内 则直接设置newXY
                gameObject.transform.localPosition = new Vector3(nNewX, nNewY, 0);
            }
            else
            {
                // 若拖动位置超出最大半径 则设置连线和圆的交点位置
                Vector3 nResult = new Vector3(0, 0, 0);
                // 计算鼠标和摇杆连线的直线方程 与摇杆移动范围为半径的圆的交点坐标
                if (nNewX == m_nJoyStickOrigin.x)
                {
                    // 若直线和x轴垂直
                    nResult.x = m_nJoyStickOrigin.x;
                    if (nNewY > m_nJoyStickOrigin.y)
                    {
                        nResult.y = m_nJoyStickOrigin.y + m_MaxRadius;
                    }
                    else
                    {
                        nResult.y = m_nJoyStickOrigin.y - m_MaxRadius;
                    }
                }
                else
                {
                    // 直线斜率
                    float k = (nNewY - m_nJoyStickOrigin.y) / (nNewX - m_nJoyStickOrigin.x);
                    // 圆的方程(x-m)^2+(y-n)^2=R^2 此处m为m_nJoyStickOrigin.x, n为m_nJoyStickOrigin.y
                    // 联立配成一元二次方程标准型 ax^2+bx+c=0 求出a b c
                    float a = Mathf.Pow(k, 2) + 1;
                    float b = 2 * k * (nNewY - k * nNewX - m_nJoyStickOrigin.y) - 2 * m_nJoyStickOrigin.x;
                    float c = Mathf.Pow(m_nJoyStickOrigin.x, 2) + Mathf.Pow(nNewY - k * nNewX - m_nJoyStickOrigin.y, 2) - Mathf.Pow(m_MaxRadius, 2);
                    // 根据求根公式算出两个解
                    float x1 = (-b + Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / (2 * a);
                    float x2 = (-b - Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / (2 * a);
                    // 鼠标在摇杆左右来确定x
                    if (nNewX > m_nJoyStickOrigin.x)
                    {
                        nResult.x = x1;
                    }
                    else
                    {
                        nResult.x = x2;
                    }
                    // 代入直线方程求y
                    nResult.y = nNewY - k * nNewX + k * nResult.x;
                }
                gameObject.transform.localPosition = new Vector3(nResult.x, nResult.y, 0);
            }
            SendMoveDirection();
            // 更新鼠标位置
            m_MouseBuffer = Input.mousePosition;
        }
#endif
    }

    /// <summary>
    /// 按下/抬起响应
    /// </summary>
    /// <param name='bPressed'>
    /// 是否被按下
    /// </param>
    void OnPress(bool bPressed)
    {

       
#if UNITY_EDITOR
        if (ObjManager.Instance.MainPlayer != null && ObjManager.Instance.MainPlayer.m_playerHeadInfo != null)
        {
            ObjManager.Instance.MainPlayer.m_playerHeadInfo.ToggleGuaJi(false);
            ObjManager.Instance.MainPlayer.m_playerHeadInfo.ToggleXunLu(false);
			ObjManager.Instance.MainPlayer.AutoXunLu=false;
        }
        if (NewPlayerGuidLogic.Instance() != null && NewPlayerGuidLogic.Instance().CurShowType == "JoyStick")
        {
            NewPlayerGuidLogic.CloseWindow();
        }

        if (!Singleton<ObjManager>.GetInstance().MainPlayer.IsDie() && !UIManager.IsSubUIShow())
        {
            if (!Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.Die)
            {
                if (bPressed)
                {
                    // 重置移动后事件
                    if (Singleton<ObjManager>.GetInstance().MainPlayer)
                    {
                        Singleton<ObjManager>.GetInstance().MainPlayer.MoveOverEvent.Reset();
                    }

                    // 记录鼠标位置
                    m_MouseBuffer = Input.mousePosition;
                    // 拖动时重设精灵透明度
                    m_JoyStickSprite.alpha = 1.0f;
                    //m_BackGround1.alpha = 1.0f;
                   // m_BackGround2.alpha = 1.0f;
                   // m_BackGround1.gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
                   // if (null != m_BackGround1.gameObject.GetComponent<Spin>())
                   //     m_BackGround1.gameObject.GetComponent<Spin>().enabled = true;

                    // 重置Tween动画
                    TweenPosition nTween = gameObject.GetComponent<TweenPosition>();
                    if (null != nTween)
                    {
                        nTween.from = gameObject.transform.localPosition;
                        nTween.Reset();
                    }
                }
                else
                {
                    // 恢复摇杆精灵透明度
                    m_JoyStickSprite.alpha = 0.5f;
                  //  m_BackGround1.alpha = 0.5f;
                  //  m_BackGround2.alpha = 0.5f;
                //    m_BackGround1.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    //if (null != m_BackGround1.gameObject.GetComponent<Spin>())
                   //     m_BackGround1.gameObject.GetComponent<Spin>().enabled = false;
                    // 发送摇杆状态-复位 玩家不再移动
                    m_ProcessInput.ResetStickState();

                    // 播放Tween动画摇杆回归
                    TweenPosition nTween = gameObject.GetComponent<TweenPosition>();
                    if (null != nTween)
                    {
                        nTween.from = gameObject.transform.localPosition;
                        nTween.Play();
                    }
                }
            }
        }
#endif
    }

    void JoyStickStartMove()
    {
        if (ObjManager.Instance.MainPlayer != null && ObjManager.Instance.MainPlayer.m_playerHeadInfo!=null) 
        {
            ObjManager.Instance.MainPlayer.m_playerHeadInfo.ToggleGuaJi(false);
            ObjManager.Instance.MainPlayer.m_playerHeadInfo.ToggleXunLu(false);
			ObjManager.Instance.MainPlayer.AutoXunLu=false;
        }
        // 重置移动后事件
        Singleton<ObjManager>.GetInstance().MainPlayer.MoveOverEvent.Reset();
        // 拖动时重设精灵透明度
        m_JoyStickSprite.alpha = 1.0f;

        // 重置Tween动画
        TweenPosition nTween = gameObject.GetComponent<TweenPosition>();
        if (null != nTween)
        {
            nTween.from = gameObject.transform.localPosition;
            nTween.Reset();
        }
    }

    void JoyStickInMoving(Vector3 vecDeltaPos)
    {
        // 两次之间的差值
        float nDeltaX = vecDeltaPos.x;
        float nDeltaY = vecDeltaPos.y;

#if UNITY_ANDROID && !UNITY_EDITOR
        nDeltaX -= _differenceX;
        nDeltaY -= _differenceY;

        nDeltaX *= _difTag;
        nDeltaY *= _difTag;

        _differenceX = vecDeltaPos.x;
        _differenceY = vecDeltaPos.y;
#endif

        // 新的XY 先不设置
        float nNewX = gameObject.transform.localPosition.x + nDeltaX;
        float nNewY = gameObject.transform.localPosition.y + nDeltaY;

        // 计算距离
        float nDistance = GetDistance(nNewX, nNewY, m_nJoyStickOrigin.x, m_nJoyStickOrigin.y);

        if (nDistance <= m_MaxRadius)
        {
            // 若拖动位置在最大半径以内 则直接设置newXY
            gameObject.transform.localPosition = new Vector3(nNewX, nNewY, 0);
        }
        else
        {
            // 若拖动位置超出最大半径 则设置连线和圆的交点位置
            Vector3 nResult = new Vector3(0, 0, 0);
            // 计算鼠标和摇杆连线的直线方程 与摇杆移动范围为半径的圆的交点坐标
            if (nNewX == m_nJoyStickOrigin.x)
            {
                // 若直线和x轴垂直
                nResult.x = m_nJoyStickOrigin.x;
                if (nNewY > m_nJoyStickOrigin.y)
                {
                    nResult.y = m_nJoyStickOrigin.y + m_MaxRadius;
                }
                else
                {
                    nResult.y = m_nJoyStickOrigin.y - m_MaxRadius;
                }
            }
            else
            {
                // 直线斜率
                float k = (nNewY - m_nJoyStickOrigin.y) / (nNewX - m_nJoyStickOrigin.x);
                // 圆的方程(x-m)^2+(y-n)^2=R^2 此处m为m_nJoyStickOrigin.x, n为m_nJoyStickOrigin.y
                // 联立配成一元二次方程标准型 ax^2+bx+c=0 求出a b c
                float a = Mathf.Pow(k, 2) + 1;
                float b = 2 * k * (nNewY - k * nNewX - m_nJoyStickOrigin.y) - 2 * m_nJoyStickOrigin.x;
                float c = Mathf.Pow(m_nJoyStickOrigin.x, 2) + Mathf.Pow(nNewY - k * nNewX - m_nJoyStickOrigin.y, 2) - Mathf.Pow(m_MaxRadius, 2);
                // 根据求根公式算出两个解
                float x1 = (-b + Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / (2 * a);
                float x2 = (-b - Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / (2 * a);
                // 鼠标在摇杆左右来确定x
                if (nNewX > m_nJoyStickOrigin.x)
                {
                    nResult.x = x1;
                }
                else
                {
                    nResult.x = x2;
                }
                // 代入直线方程求y
                nResult.y = nNewY - k * nNewX + k * nResult.x;
            }

            gameObject.transform.localPosition = new Vector3(nResult.x, nResult.y, 0);
        }

        SendMoveDirection();
    }

    public void JoyStickEndMove()
    {
        // 恢复摇杆精灵透明度
        m_JoyStickSprite.alpha = 0.5f;
        // 新摇杆取消
//         m_BackGround1.alpha = 0.5f;
//         m_BackGround2.alpha = 0.5f;
//         m_BackGround1.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
//         m_BackGround1.gameObject.GetComponent<Spin>().enabled = false;
        // 发送摇杆状态-复位 玩家不再移动
        m_ProcessInput.ResetStickState();
        // 播放Tween动画摇杆回归
        TweenPosition nTween = gameObject.GetComponent<TweenPosition>();
        if (null != nTween)
        {
            nTween.from = gameObject.transform.localPosition;
            nTween.Play();
        }

    }

    /// <summary>
    /// 求两点距离
    /// </summary>
    /// <param>
    /// 两点的坐标
    /// </param>
    float GetDistance(float x1, float y1, float x2, float y2)
    {
        return Mathf.Sqrt(Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2));
    }

    /// <summary>
    /// 发送摇杆状态
    /// </summary>
    void SendMoveDirection()
    {
        float nPosX = gameObject.transform.localPosition.x;
        float nPosY = gameObject.transform.localPosition.y;


        if (null == Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            return;
        }

        ThirdPersonController controller = Singleton<ObjManager>.GetInstance().MainPlayer.GetComponent<ThirdPersonController>();
        if (null == controller)
        {
            return;
        }

        controller.VerticalRaw = nPosY / m_MaxRadius;
        controller.HorizonRaw = nPosX / m_MaxRadius;

        //if ( nPosX >= m_nJoyStickOrigin.x + m_MoveRadius )
        //{
        //    // 右侧超过范围向右移动
        //    m_ProcessInput.SetStickState(ProcessInput.STICK_STATE.STICK_STATE_RIGHT, true);
        //    m_ProcessInput.SetStickState(ProcessInput.STICK_STATE.STICK_STATE_LEFT, false);
        //}
        //if ( nPosX < m_nJoyStickOrigin.x + m_MoveRadius && nPosX >= m_nJoyStickOrigin.x - m_MoveRadius )
        //{
        //    // 在范围中 左右方向不移动
        //    m_ProcessInput.SetStickState(ProcessInput.STICK_STATE.STICK_STATE_RIGHT, false);
        //    m_ProcessInput.SetStickState(ProcessInput.STICK_STATE.STICK_STATE_LEFT, false);
        //}
        //if ( nPosX < m_nJoyStickOrigin.x - m_MoveRadius )
        //{
        //    // 左侧超过范围向左移动
        //    m_ProcessInput.SetStickState(ProcessInput.STICK_STATE.STICK_STATE_RIGHT, false);
        //    m_ProcessInput.SetStickState(ProcessInput.STICK_STATE.STICK_STATE_LEFT, true);
        //}

        //if ( nPosY >= m_nJoyStickOrigin.y + m_MoveRadius )
        //{
        //    // 前方超过范围向前移动
        //    m_ProcessInput.SetStickState(ProcessInput.STICK_STATE.STICK_STATE_FORWARD, true);
        //    m_ProcessInput.SetStickState(ProcessInput.STICK_STATE.STICK_STATE_BACKWARD, false);
        //}
        //if ( nPosY < m_nJoyStickOrigin.y + m_MoveRadius && nPosY >= m_nJoyStickOrigin.y - m_MoveRadius )
        //{
        //    // 在范围中 前后方向不移动
        //    m_ProcessInput.SetStickState(ProcessInput.STICK_STATE.STICK_STATE_FORWARD, false);
        //    m_ProcessInput.SetStickState(ProcessInput.STICK_STATE.STICK_STATE_BACKWARD, false);
        //}
        //if ( nPosY < m_nJoyStickOrigin.y - m_MoveRadius )
        //{
        //    // 后方超过范围向后移动
        //    m_ProcessInput.SetStickState(ProcessInput.STICK_STATE.STICK_STATE_FORWARD, false);
        //    m_ProcessInput.SetStickState(ProcessInput.STICK_STATE.STICK_STATE_BACKWARD, true);
        //}
    }

    // 特殊 JoyStickLogic脚本是挂在JoyStick上 而非挂在JoyStickRoot上
    // 关闭和开启时需要找到Root节点来进行操作
    public void CloseWindow()
    {
        ReleaseJoyStick();
        gameObject.transform.parent.parent.gameObject.SetActive(false);
    }

    public void OpenWindow()
    {
        gameObject.transform.parent.parent.gameObject.SetActive(true);
    }

    public void ReleaseJoyStick()
    {
        JoyStickEndMove();
        if (m_FingerID != -1)
        {
            m_FingerID = -1;
            // 恢复摇杆精灵透明度
            m_JoyStickSprite.alpha = 0.5f;
            // 发送摇杆状态-复位 玩家不再移动
            m_ProcessInput.ResetStickState();
            // 播放Tween动画摇杆回归
            gameObject.transform.localPosition = m_nJoyStickOrigin;
        }
    }

    public void NewPlayerGuide(int nIndex)
    {
        m_NewPlayerStep = nIndex;
        switch (nIndex)
        {
            case 1:
                //NewPlayerGuidLogic.OpenWindow(gameObject, 224, 224, "按住中心不放，左右移动", "right",0, true);
                NewPlayerGuidLogic.OpenWindow(gameObject, 224, 224, StrDictionary.GetClientDictionaryString("#{2841}"), "right", 2, true);
                break;
            default:
                break;
        }
    }
}
