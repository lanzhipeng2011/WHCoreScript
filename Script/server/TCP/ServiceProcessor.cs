using System;
using ProtoCmd;
using System.Threading;
using System.IO;
//using ProtoBuf;
using UnityEngine;

public class ServiceProcessor
{
    private TcpServer m_tcp_client;
    private ByteBuffer m_recv_buffer;
    private MessageQueue m_mess_queue;

    private ushort m_game;
    private ushort m_zone;
    private string m_version;

    private uint m_id;
    private uint m_login_id;
    private string m_account;
    private string m_passwd;

    private ProcessorState m_state;

    public delegate void ConnectServerSelfBreak();
    public ConnectServerSelfBreak OnConnectServerSelfBreak;


    /// <summary>
    /// 服务处理
    /// </summary>
    public ServiceProcessor()
    {
        m_tcp_client = new TcpServer();
        m_recv_buffer = new ByteBuffer();
        m_mess_queue = new MessageQueue();

        TcpServer.OnReceiveFunction rece_fun = new TcpServer.OnReceiveFunction(Receive);

        m_tcp_client.Attach(rece_fun);
        m_tcp_client.OnDisconnect = ServerDisconnect;

        //m_state = ProcessorState.State_None;
        ChangeProcessorState(ProcessorState.State_None);
    }

    public void SetID(uint id)
    {
        m_id = id;
    }

    public uint GetID()
    {
        return m_id;
    }

    public void SetLoginID(uint id)
    {
        m_login_id = id;
    }

    public uint GetLoginID()
    {
        return m_login_id;
    }


    /// <summary>
    /// 发送登陆平台服信息
    /// </summary>
    public void Login(string account, string pwd)
    {
        PlayerRequestLoginClientCmd send = new PlayerRequestLoginClientCmd();

        send.account = account;
        send.password = pwd;
        send.internal_test = 1;

        Debug.LogError("需要设置发送平台id");

        //if (BaseGamePlatform.Instance != null)
        //    send.platform_id = (uint)BaseGamePlatform.PlatformID;
        //else
        //    send.platform_id = (uint)PlatformMgr.GetInstance().m_platformType;


        Debug.Log("准备登陆平台服： account --  " + account.ToString() + "   pwd -- " + pwd.ToString() + "  internal_test:" + send.internal_test.ToString());

        GameNetwork.Instance.SendCmd(CmdNumber.PlayerRequestLoginClientCmd_C, send);

        //MemoryStream mem5 = new MemoryStream();

        //DTOSerializer dtoSerializer2 = new DTOSerializer();

        //dtoSerializer2.Serialize(mem5, send);

        //byte[] bytes = CommonTools.MemoryStreamToBytes(mem5, 0);

        //Message sendmsg = new Message(CmdNumber.PlayerRequestLoginClientCmd_C, bytes);

        //if (GameNetwork.m_service != null)
        //    GameNetwork.m_service.Send(sendmsg);
        //else
        //    Debug.LogError("SendChatInfo error! net work is not init");

    }

    /// <summary>
    /// 第三方平台账户登录
    /// </summary>
    public void ThirdGameLogin() 
    {
        
		//PlatformMgr platformMgr = PlatformMgr.GetInstance();

//#if CommonSDK
//        PlayerRequestLoginClientCmd send = CommonSDKPlaform.Instance.GetLoginData();
//#else
//        //PlayerRequestLoginClientCmd send = PlatformMgr.GetInstance().m_Platform.GetLoginRequestCmd();
//#endif
        PlayerRequestLoginClientCmd send = CommonSDKPlaform.Instance.GetLoginData();
        


        Debug.LogError("准备登陆平台服： platform_id --  " + send.platform_id.ToString() + "   send.app_loginkey -- " + send.app_loginkey.ToString() + "   send.app_uid--" + send.app_uid.ToString() + "  key:" + send.internal_test.ToString());
        GameNetwork.Instance.SendCmd(CmdNumber.PlayerRequestLoginClientCmd_C, send);

    }




    ///// <summary>
    ///// 中青宝SDK登陆
    ///// </summary>
    //public void ZQLogin()
    //{
    //    PlayerRequestLoginClientCmd send = new PlayerRequestLoginClientCmd();

    //    send.platform_id = (uint)ClientPlatformType.ClientPlatformType_ZQGame;
    //    send.app_loginkey = ZqgamePlatform.Instance.m_strToken;
    //    send.app_uid = ZqgamePlatform.Instance.m_strUserID;

    //    Debug.Log("准备登陆平台服： platform_id --  " + send.platform_id.ToString() + "   send.app_loginkey -- " + send.app_loginkey.ToString() + "   send.app_uid--" + send.app_uid.ToString());

    //    GameNetwork.Instance.SendCmd(CmdNumber.PlayerRequestLoginClientCmd_C, send);

    //}





    /// <summary>
    /// 连接平台服
    /// </summary>
    public bool ConnectPlatform(string ip, int port)
    {
		//ServiceClient.Info("登陆平台服状态：" + m_state.ToString());
		Logger.Info(LoggerType.LOG_SERVICE, "登陆平台服状态：" + m_state.ToString());

        if (m_state == ProcessorState.State_None)
        {

            Debug.Log("开始登陆平台服:" + ip + "   " + port);

            //这个地方在主线程连接，如果服务器没有立即响应的话，会卡死

            //初始化回调函数
            // m_tcp_client.OnConnectSuccess = new TcpServer.OnConnectSuccessFunc(CallBackTryConnect);

            // m_tcp_client.TryConnect(ip, port);


            if (m_tcp_client.Connect(ip, port))
            {

                PlayerVerifyVerLoginClientCmd send = new PlayerVerifyVerLoginClientCmd();

                send.game = m_game;
                send.zone = m_zone;
                send.version = ECommonTool.Utf8StringToBytes(m_version);




                


                MemoryStream mem5 = new MemoryStream();

                ProtoBuf.Serializer.Serialize<PlayerVerifyVerLoginClientCmd>(mem5, send);

                //DTOSerializer dtoSerializer2 = new DTOSerializer();

                //dtoSerializer2.Serialize(mem5, send);


                byte[] bytes = MemoryStreamToBytes(mem5, 0);

                Message sendmsg = new Message(CmdNumber.PlayerVerifyVerLoginClientCmd_C, bytes);



                Send(sendmsg);




                //PlayerVerifyVerLoginClientCmd reqList = PlayerVerifyVerLoginClientCmd.CreateBuilder()
                //                                        .SetGame(m_game)
                //                                        .SetZone(m_zone)
                //                                        .SetVersion(m_version)
                //                                        .Build();

                //Message message = new Message(CmdNumber.PlayerVerifyVerLoginClientCmd_C, reqList.ToByteArray());
                //Send(message);
                ChangeProcessorState(ProcessorState.State_Platform);
                //m_state = ProcessorState.State_Platform;
                //Debug.LogError("set state:" + m_state);
				//ServiceClient.Info("连接平台成功...");
				Logger.Info(LoggerType.LOG_SERVICE, "连接平台成功...");
                Debug.Log("连接平台成功!");
                return true;
            }
        }

        return false;
    }






    //监听连接服务器函数
    public delegate void OnConnectExSuccessFunc();
    public OnConnectExSuccessFunc OnConnectExSuccess;
    public delegate void OnConnectFailFunc();
    public OnConnectFailFunc OnConnectFailSuccess;


    /// <summary>
    /// 连接平台服
    /// </summary>
    public void ConnectPlatformEx(string ip, int port)
    {
        Debug.Log("m_state:" + m_state);
        CmdNumber cmd = CmdNumber.AccountRegisterClientCmd_CS;
        if (m_state == ProcessorState.State_None)
        {

            Debug.Log("开始登陆平台服:" + ip + "   " + port);

            //这个地方在主线程连接，如果服务器没有立即响应的话，会卡死

            //初始化回调函数
            m_tcp_client.OnConnectSuccess = new TcpServer.OnConnectSuccessFunc(CallBackTryConnect);
            m_tcp_client.OnConnectFail = new TcpServer.OnConnectFailFunc(CallBackTryConnectFail);
            m_tcp_client.TryConnect(ip, port);
        }

    }

    public void CallBackTryConnect()
    {
        Debug.Log("CallBackTryConnect Sucess :  version -- " + m_version.ToString() + "   zone-- "+m_zone.ToString() + "   game-- "+m_game.ToString());
        
        PlayerVerifyVerLoginClientCmd send = new PlayerVerifyVerLoginClientCmd();

        send.game = m_game;
        send.zone = m_zone;
        send.version = ECommonTool.Utf8StringToBytes(m_version);

        MemoryStream mem5 = new MemoryStream();
        ProtoBuf.Serializer.Serialize<PlayerVerifyVerLoginClientCmd>(mem5, send);
        //DTOSerializer dtoSerializer2 = new DTOSerializer();

        //dtoSerializer2.Serialize(mem5, send);

        byte[] bytes = MemoryStreamToBytes(mem5, 0);

        Message sendmsg = new Message(CmdNumber.PlayerVerifyVerLoginClientCmd_C, bytes);

		//ServiceClient.Info("开始发送...");

		Logger.Info(LoggerType.LOG_SERVICE, "开始发送...");

        Send(sendmsg);

		//ServiceClient.Info("发送完了...");
		Logger.Info(LoggerType.LOG_SERVICE, "发送完了...");
        ChangeProcessorState(ProcessorState.State_Platform);
        //m_state = ProcessorState.State_Platform;
        //Debug.LogError("set state:" + m_state);

        OnConnectExSuccess();

		//ServiceClient.Info("连接平台成功...");
		Logger.Info(LoggerType.LOG_SERVICE, "连接平台成功...");
    }

    private void ChangeProcessorState(ProcessorState state) 
    {
        //if (state == ProcessorState.State_None && m_state == ProcessorState.State_Platform) 
        //{
        //    Debug.LogWarning("state is platform ,change state to none error");
        //    return;
        //}
        m_state = state;
    }


    public void CallBackTryConnectFail()
    {
        OnConnectFailSuccess();

        Debug.Log("CallBackTryConnectFail!");
    }
    
    public void ConnectAccessEx(string ip, ushort port, TcpServer.OnConnectSuccessFunc fun1, TcpServer.OnConnectFailFunc fun2)
    {
        Debug.Log("m_state:" + m_state + " ip:" + ip + " port:" + port + ",state:" + m_state);
        //if (m_state == ProcessorState.State_Platform)
        //{
            Debug.Log("DisConnectPlatform and ConnectAccess !");
            DisConnect();

            //初始化回调函数
            m_tcp_client.OnConnectSuccess = fun1;
            m_tcp_client.OnConnectFail = fun2;
            m_tcp_client.TryConnect(ip, port);
        //}
    }

    public void UpdateCreateTcpTimeout() 
    {
        if (m_tcp_client != null) 
        {
            m_tcp_client.UpdateCheckTcpConnectTimeOut();
        }
    }


    public void ReConnectAccessService(string ip, ushort port, TcpServer.OnConnectSuccessFunc _onConnectSuccess, TcpServer.OnConnectFailFunc _onConnectFail) 
    {
        Debug.Log("ReConnectAccessService m_state:" + m_state + " ip:" + ip + " port:" + port);
        if (m_state == ProcessorState.State_None) 
        {
            m_tcp_client.OnConnectSuccess = _onConnectSuccess;
            m_tcp_client.OnConnectFail = _onConnectFail;
            m_tcp_client.TryConnect(ip, port);
        }
    }


    /// <summary>
    /// 成功连接接入服后，由主线程调用发起登陆信息
    /// </summary>
    public void ConnectAccessSuccess()
    {
        LoginAccessLoginClientCmd send = new LoginAccessLoginClientCmd();

        send.login_id = (int)m_login_id;
        send.account_id = (int)m_id;

		//ServiceClient.Info("帐号：" + m_account + "    密码：" + m_passwd + "   版本:" + m_version.ToString());
		Logger.Info(LoggerType.LOG_SERVICE, "帐号：" + m_account + "    密码：" + m_passwd + "   版本:" + m_version.ToString());

        send.password = m_passwd;
        send.account = m_account;
        send.version = ECommonTool.Utf8StringToBytes(m_version);

        GameNetwork.Instance.SendCmd(CmdNumber.LoginAccessLoginClientCmd_C, send);
        
        //m_state = ProcessorState.State_Access;
        ChangeProcessorState(ProcessorState.State_Access);
		//ServiceClient.Info("连接网关成功...  成功发送登陆信息 ... LoginAccessLoginClientCmd_C");
		Logger.Info(LoggerType.LOG_SERVICE, "连接网关成功...  成功发送登陆信息 ... LoginAccessLoginClientCmd_C");
    }


    
    public bool CheckConnect()
    {
        return m_tcp_client.CheckConnect();
    }

    public void DestoryThread()
    {
        Debug.Log("ServiceProcessor Destory Thread");
        m_tcp_client.DestoryThread();
        ChangeProcessorState(ProcessorState.State_None);
        //m_state = ProcessorState.State_None;
    }

    public void DisConnect()
    {

        m_tcp_client.DisConnect();
    }

    public void SetGameInfo(ushort game, ushort zone, string version)
    {
        Debug.Log("SetGameInfo:game--" + game.ToString() + "   ");
        m_game = game;
        m_zone = zone;
        m_version = version;
    }


    public void SetAccontAndPassword(string accont, string password)
    {
        m_account = accont;
        m_passwd = password;
    }

    public bool Process(ServiceClient service)
    {
        while (!m_mess_queue.IsEmpty)
        {
            service.Parse(m_mess_queue.Dequeue());
        }

        return CheckConnect();
    }



    /// <summary>
    /// 接受数据处理(监听自TCP)
    /// </summary>
    private bool Receive(byte[] cmd, int len)
    {

        //ServiceClient.Info("收到数据, 长度: {0}", len);
        //Debug.Log("收到数据");

        m_recv_buffer.Push(cmd, (uint)len);

        while (m_recv_buffer.CheckOK())
        {
            Message message = UnPackCmd(m_recv_buffer.GetData());
			//ServiceClient.Info("收到消息: {0}", message.m_cmd_no);
			//Logger.Info(LoggerType.LOG_SERVICE, "收到消息: {0}", message.m_cmd_no);
            Debug.Log("收到消息: " + message.m_cmd_no.ToString());
            //if (Application.platform == RuntimePlatform.Android)
            //{
            //    Debug.Log("===========================收到数据：" + message.m_cmd_no.ToString() + "===============================");
            //}

            m_mess_queue.Enqueue(message);
            m_recv_buffer.Pop();
        }
        return true;
    }


    /// <summary>
    /// 监听服务器主动断线
    /// </summary>
    /// <returns></returns>
    public void ServerDisconnect()
    {
        //模拟一个数据让客户端处理断线
        Debug.Log("服务器主动断线,Procerssor处理。");
        ChangeProcessorState(ProcessorState.State_None);
        //m_state = ProcessorState.State_None;
        OnConnectServerSelfBreak();
    }

    /// <summary>
    /// 打包发送数据
    /// </summary>
    public bool Send(Message message)
    {
        //if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        //{
        //    Debug.Log("===========================发送消息：" + message.m_cmd_no.ToString() + "===============================");
        //}      

        Debug.Log("发送消息：" + message.m_cmd_no.ToString());

		//Logger.Info(LoggerType.LOG_SERVICE, "发送消息：{0}", message.m_cmd_no);
        byte[] cmd = PackCmd(message);
        return m_tcp_client.Send(cmd, cmd.Length);
    }

    /// <summary>
    /// 打包数据
    /// </summary>
    private byte[] PackCmd(Message message)
    {
        //只发命令
        if (message.m_cmd == null)
        {
            ushort cmd_len = 0;
            ushort cmd_no = (ushort)message.m_cmd_no;
            int head_len = sizeof(int);
            int pack_len = cmd_len + head_len;

            byte[] send = new byte[pack_len];
            byte[] len_byte = BitConverter.GetBytes(cmd_len);
            byte[] no_byte = BitConverter.GetBytes(cmd_no);

            Array.Copy(len_byte, 0, send, 0, len_byte.Length); // 2byte: length
            Array.Copy(no_byte, 0, send, len_byte.Length, no_byte.Length); // 2byte: cmdno length
            //Array.Copy(message.m_cmd, 0, send, head_len, cmd_len); // content
            return send;
        }
        else 
        {
            ushort cmd_len = (ushort)message.m_cmd.Length;
            ushort cmd_no = (ushort)message.m_cmd_no;
            int head_len = sizeof(int);
            int pack_len = cmd_len + head_len;

            byte[] send = new byte[pack_len];
            byte[] len_byte = BitConverter.GetBytes(cmd_len);
            byte[] no_byte = BitConverter.GetBytes(cmd_no);

            Array.Copy(len_byte, 0, send, 0, len_byte.Length); // 2byte: length
            Array.Copy(no_byte, 0, send, len_byte.Length, no_byte.Length); // 2byte: cmdno length
            Array.Copy(message.m_cmd, 0, send, head_len, cmd_len); // content
            return send;
        }
    }

    /// <summary>
    /// 解析数据
    /// </summary>
    private Message UnPackCmd(byte[] cmd)
    {
        ushort data_size = ByteBuffer.GetAllDataSize(cmd);
        if (data_size <= cmd.Length)
        {
            int len = data_size - sizeof(uint);
            byte[] ret_buffer = new byte[len];
            Array.Copy(cmd, sizeof(int), ret_buffer, 0, len);

            Message message = new Message();
            message.m_cmd_no = (CmdNumber)BitConverter.ToUInt16(cmd, sizeof(ushort));
            message.m_cmd = ret_buffer;
            return message;
        }
        return null;
    }

    /// <summary>
    /// 内存流转换至二进制
    /// </summary>
    public byte[] MemoryStreamToBytes(MemoryStream memStream, int offset)
    {
        memStream.Seek(offset, SeekOrigin.Begin);
        int buffLength = (int)memStream.Length - offset;
        if (buffLength < 0)
            buffLength = 0;

        byte[] bytes = new byte[buffLength];
        memStream.Read(bytes, 0, buffLength);
        memStream.Seek(0, SeekOrigin.Begin);

        return bytes;
    }

}
