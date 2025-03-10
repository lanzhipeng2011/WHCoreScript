using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;



public class TcpServer
{

    //接收数据函数监听
    public delegate bool OnReceiveFunction(byte[] data, int len);
    public OnReceiveFunction OnReceive;

    //服务器主动断线监听
    public delegate void OnDisconnectFunc();
    public OnDisconnectFunc OnDisconnect;


    //监听连接服务器函数
    public delegate void OnConnectSuccessFunc();
    public OnConnectSuccessFunc OnConnectSuccess;
    public delegate void OnConnectFailFunc();
    public OnConnectFailFunc OnConnectFail;


    private volatile TcpState mState = TcpState.State_None;
    private TcpClient mTcpClient;

    private ServiceThread mThread;

    //连接线程
    private Thread m_ConnectThread;


    private float m_fStartConnectTime;      //开始创建连接的时间
    private float m_fConnectTimeout = 5.0f; //创建tcp连接超时时间
    private bool m_bIsStartConnect = false; //是否开始连接了

     

    //private volatile bool bForceStop;
        

    private void StartTcp()
    {
        if (mThread == null)
        {
            Debug.Log("为TCP层创建新线程");
            mThread = new ServiceThread();
            mThread.Start(this);
        }
    }

    /// <summary>
    /// 初始化连接
    /// </summary>
    private void InitTcpClient(int send_size, int receive_size)
    {
        if (mTcpClient == null)
        {
            Debug.Log("Recreate Tcp Client!");
            mTcpClient = new TcpClient(AddressFamily.InterNetwork);
            mTcpClient.SendBufferSize = send_size;
            mTcpClient.ReceiveBufferSize = receive_size;
        }
    }

    public TcpServer()
    {
        mState = TcpState.State_None;
        //bForceStop=false;
    }

    /// <summary>
    /// 绑定监听函数
    /// </summary>
    /// <param name="rece_fun"></param>
    public void Attach(OnReceiveFunction rece_fun)
    {
        OnReceive = rece_fun;
    }

    /// <summary>
    /// 检测与服务器是否在连接状态
    /// </summary>
    public bool CheckConnect()
    {
        return (mState == TcpState.State_Connect);
    }



    /// <summary>
    /// 销毁线程和连接。
    /// </summary>
    public void DestoryThread()
    {
		//ServiceClient.Info("Destory Tcp Client!!");
		Logger.Info(LoggerType.LOG_SERVICE, "Destory Tcp Client!!");

        if (mTcpClient != null) 
        {
            //mTcpClient.EndConnect(asynResult);

            mTcpClient.Close();
        }
        

        mTcpClient = null;

        mThread = null;

        if (connectThread != null) 
        {
            connectThread = null;
        }

        mState = TcpState.State_Disconnect;
    }


   

    /// <summary>
    /// 合并线程，断掉已有的连接。
    /// </summary>
    public void DisConnect()
    {
		//ServiceClient.Info("将TCP线程合并到主线程!");
		Logger.Info(LoggerType.LOG_SERVICE, "将TCP线程合并到主线程!");

        mThread.Join();

        DestoryThread();

        //mState=TcpState.State_Disconnect;
        //CommonTools.DebugShowLog("DisConnect b4 join");
        //CommonTools.DebugShowLog("DisConnect after join");
        //mTcpClient.Close();
        //mTcpClient = null;
        //mThread = null;
    }

    private void onStartConnect() 
    {
        m_bIsStartConnect = true;
        m_fStartConnectTime = Time.realtimeSinceStartup;
    }


    public void UpdateCheckTcpConnectTimeOut() 
    {
        if (!m_bIsStartConnect) 
        {
            return;
        }
        if (Time.frameCount % 10 != 0)
            return;
        float time = Time.realtimeSinceStartup;
        //Debug.LogError("UpdateCheckTcpConnectTimeOut: " + (time - m_fStartConnectTime));
        
        if (time - m_fStartConnectTime >= m_fConnectTimeout) 
        {
            Debug.LogError("create tcp time out!!!");
            connectDone.Set();
            dealwithConnectResult();
            //连接超时,按失败处理
            
        }

    }

    public ManualResetEvent connectDone = new ManualResetEvent(false);

    string s_IP;
    int i_Port;
    Thread connectThread;


    /// <summary>
    /// 新线程连接
    /// </summary>
    public void TryConnect(string ip, int port)
    {
        s_IP = ip;
        i_Port = port;
        
        if (connectThread == null)
        {
            
            connectThread = new Thread(SyncTryConnect);
            connectThread.Start();
            onStartConnect();
        }
    }
    
    private void SyncTryConnect()
    {
        try
        {
            Debug.Log("TryConnect Tcp Client In New Thread!");

            mTcpClient = new TcpClient();
            mTcpClient.SendBufferSize = 1024;
            mTcpClient.ReceiveBufferSize = 1024;
            mTcpClient.ReceiveTimeout = 10;


            //mTcpClient.Connect(s_IP, i_Port);


            connectDone.Reset();


            Debug.Log("BeginConnect  -----   s_IP:" + s_IP + "  i_Port :" + i_Port);

            mTcpClient.BeginConnect(s_IP, i_Port, new AsyncCallback(CallBackTryConnect), mTcpClient);

            connectDone.WaitOne();
            //mTcpClient.Connect(ip, port);

        }
        catch (Exception err)
        {
			//ServiceClient.Error("连接[{0}, {1}]失败: {2}", s_IP, i_Port, err.Message);
			Logger.Error(LoggerType.LOG_SERVICE, "连接[{0}, {1}]失败: {2}", s_IP, i_Port, err.Message);
        }
    }

    public void CallBackTryConnect(IAsyncResult ar)
    {
        Debug.Log("CallBackTryConnect -----  ConnectThread:" + connectThread.ThreadState);

        connectDone.Set();

        TcpClient t = (TcpClient)ar.AsyncState;
        t.EndConnect(ar);

        dealwithConnectResult();

    }

    private void dealwithConnectResult() 
    {
        m_bIsStartConnect = false;
        if (connectThread != null) 
        {
            connectThread.Join();
        }
        connectThread = null;

        if (mThread != null)
        {
            mThread = null;
        }

        if ((mTcpClient != null) && (mTcpClient.Connected))
        {
            Debug.Log("connect tcp success");
            //ServiceClient.Info("连接成功!");
            Logger.Info(LoggerType.LOG_SERVICE, "连接成功!");
            mState = TcpState.State_Connect;
            StartTcp();
            OnConnectSuccess();
        }
        else
        {
            Debug.Log("connect tcp fail");
            //ServiceClient.Info("连接失败!");
            Logger.Info(LoggerType.LOG_SERVICE, "连接失败!");
            OnConnectFail();
        }
    }




    /// <summary>
    /// 开始连接
    /// </summary>
    public bool Connect(string ip, int port)
    {
        try
        {
            InitTcpClient(1024, 1024);
            mTcpClient.Connect(ip, port);
            mState = TcpState.State_Connect;
            StartTcp();
        }
        catch (Exception err)
        {
			//ServiceClient.Error("连接[{0}, {1}]失败: {2}", ip, port, err.Message);
			Logger.Error(LoggerType.LOG_SERVICE, "连接[{0}, {1}]失败: {2}", ip, port, err.Message);
            return false;
        }

        return true;
    }


    /// <summary>
    /// 接收数据
    /// </summary>
    public void Receive()
    {
        Debug.Log("======================= Listening the Tcp Reveive Func In New Thread ===================================");

        while (CheckConnect())
        {
            if (mState == TcpState.State_Connect)
            {
                try
                {
                    byte[] revcev_bytes = new byte[mTcpClient.ReceiveBufferSize];
                    int num = mTcpClient.GetStream().Read(revcev_bytes, 0, revcev_bytes.Length);
                    if (num != 0)
                    {
                        OnReceive(revcev_bytes, num);
                        //m_rece_fun(revcev_bytes, num);
                    }
                    else
                    {
                        //服务器主动断线(接收数据失败，连接断开)
                        Debug.Log("服务器要求断线");

                        //TCP处理断线
                        //DisConnect();

                        //回调处理断线
                        OnDisconnect();

                        mState = TcpState.State_Disconnect;
						//ServiceClient.Error("接收数据失败，连接断开");
						Logger.Error(LoggerType.LOG_SERVICE, "接收数据失败，连接断开");
                    }
                }
                catch (System.Exception err)
                {
					//ServiceClient.Error("读取数据失败：{0}", err.Message);
                    Logger.Error(LoggerType.LOG_SERVICE, "读取数据失败：{0}", err.Message);
                    mState = TcpState.State_Disconnect;
                    GameNetwork.Instance.m_bReceiveError = true;

                }
            }

            Thread.Sleep(100);
        }
    }

    /// <summary>
    /// 发送信息
    /// </summary>
    public bool Send(byte[] send, int len)
    {
        if (CheckConnect())
        {
            try
            {

                mTcpClient.GetStream().Write(send, 0, len);               
                //ServiceClient.Info("发送数据, 长度：{0}\n", len);

                return true;
            }
            catch (Exception err)
            {
                //ServiceClient.Error("发送数据失败：{0}", err.Message);
                Logger.Error(LoggerType.LOG_SERVICE, "发送数据失败：{0}", err.Message);
                Debug.Log("发送数据失败：" + err.Message);
                mState = TcpState.State_Disconnect;
                return false;
                //return true;
            }
        }
        else
        {
            Debug.Log("尚未创立连接!:" + mState);
        }

        return false;
    }
}

