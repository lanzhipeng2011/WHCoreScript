using ProtoCmd;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ServiceClient
{
    public delegate bool ParseCommand(Message message);
    public delegate void OnResult(LoginReturn ret_code);

    //服务器主动断线处理
    public delegate void OnServerDisconnect();
    public OnServerDisconnect m_OnServerDisconnect;

	//private static Logger m_logger = new Logger();
	private static Logger m_logger;

    private Dictionary<CmdNumber, ParseCommand> m_parser;

    public ServiceProcessor m_processor;


    private OnResult m_result_fun;
    private MessageQueue m_delay_mess_queue;
    private bool bPraseDelayMsg = false;

    private MessageQueue m_delay_mess_battlequeue;
    private bool bPraseBattleDelayMsg = false;

    //战斗的时候如果断线,然后客户端已经发消息了(没发送成功),缓存到这里,在重连成功后会再次发送这个消息
    private MessageQueue m_cache_no_send_msg;

    public ServiceClient()
    {
		m_parser = new Dictionary<CmdNumber, ParseCommand>();
        m_processor = new ServiceProcessor();

        m_delay_mess_queue = new MessageQueue();
        m_delay_mess_battlequeue = new MessageQueue();

        m_cache_no_send_msg = new MessageQueue();

        RegAllFun();
    }

    /// <summary>
    /// 登录平台服
    /// </summary>
    public void Login(string account, string pwd)
    {
		Logger.Info(LoggerType.LOG_SERVICE, "开始登陆: [{0}, {1}]", account, pwd);

        m_processor.Login(account, pwd);
    }



    public void LoginByPlatform() 
    {
        m_processor.ThirdGameLogin();
    }


    ///// <summary>
    ///// 根据平台进行登陆
    ///// </summary>
    ///// <param name="platformId">平台ID</param>
    //public void LoginByPlatform(ClientPlatformType platformId)
    //{
    //    switch (platformId)
    //    {
    //        case ClientPlatformType.ClientPlatformType_ZQGame:
    //            //m_processor.ZQLogin();
    //            break;
    //        default:
    //            Debug.LogError("No this platform : "+platformId.ToString());
    //            break;
    //    }
    //}



    public void PushDelayMsg(Message message)
    {
        m_delay_mess_queue.Enqueue(message);
    }

    public void ClearDelayMsg()
    {
        while (!m_delay_mess_queue.IsEmpty)
        {
            m_delay_mess_queue.Dequeue();
        }
    }

    public void BreakDelayMsg()
    {
        bPraseDelayMsg = false;
    }

    public void HandDelayMsg()
    {
        bPraseDelayMsg = true;

		Logger.Info(LoggerType.LOG_SERVICE, "HandDelayMsg Start");

        while (!m_delay_mess_queue.IsEmpty && bPraseDelayMsg)
        {
            Parse(m_delay_mess_queue.Dequeue());
        }

        bPraseDelayMsg = false;

		Logger.Info(LoggerType.LOG_SERVICE, "HandDelayMsg End");
    }

    public void PushBattleDelayMsg(Message message)
    {
        m_delay_mess_battlequeue.Enqueue(message);
    }

    public void ClearBattleDelayMsg()
    {
        while (!m_delay_mess_battlequeue.IsEmpty)
        {
            m_delay_mess_battlequeue.Dequeue();
        }
    }

    public void BreakBattleDelayMsg()
    {
        bPraseBattleDelayMsg = false;
    }

    public void HandBattleDelayMsg()
    {
        bPraseBattleDelayMsg = true;

		Logger.Info(LoggerType.LOG_SERVICE, "HandBattleDelayMsg Start");

        //while (!m_delay_mess_battlequeue.IsEmpty && bPraseBattleDelayMsg)
        //{
        //    Message message = m_delay_mess_battlequeue.Dequeue();
            
        //    Parse(message);
        //}
        int count = m_delay_mess_battlequeue.GetCount( );
        while (count-- > 0 && bPraseBattleDelayMsg)
        {
            Message message = m_delay_mess_battlequeue.Dequeue( );

            Parse( message );
        }

        bPraseBattleDelayMsg = false;

		Logger.Info(LoggerType.LOG_SERVICE, "HandBattleDelayMsg End");
    }

    public void PushNoSendMsg(Message message) 
    {
        m_cache_no_send_msg.Enqueue(message);
    }

    public void HandNoSendMsg() 
    {
        while (!m_cache_no_send_msg.IsEmpty) 
        {
            Message message = m_cache_no_send_msg.Dequeue();
            Send(message);
        }
    }
    public void ClearNoSendMsg() 
    {
        while (!m_cache_no_send_msg.IsEmpty)
        {
            m_cache_no_send_msg.Dequeue();
        }
    }
    
    public bool RegParseFun(ParseCommand parse_fun, CmdNumber cmd_no)
    {
        if (m_parser.ContainsKey(cmd_no))
        {
            //ServiceClient.Error("注册消息解析函数失败{0}", cmd_no);
            //return false;
            m_parser.Remove(cmd_no);
        }
        m_parser[cmd_no] = parse_fun;
        return true;
    }

    public void DetachParseFun( CmdNumber cmd_no )
    {
        if( m_parser.ContainsKey(cmd_no) )
        {
            //ServiceClient.Error("注册消息解析函数失败{0}", cmd_no);
            //return false;
            m_parser[cmd_no] = null;
            m_parser.Remove(cmd_no);
        }
    }

    public void Attach(OnResult result)
    {
        m_result_fun = result;
    }

    public void Initilize(ushort game, ushort zone, string version)//, bool need_log)
    {
        m_processor.SetGameInfo(game, zone, version);
		//m_logger.SetState(need_log);
    }

    public bool Connect(string ip, int port)
    {
        return m_processor.ConnectPlatform(ip, port);
    }

    public void ConnectEx(string ip, int port)
    {

        m_processor.ConnectPlatformEx(ip, port);
    }



    public void DestoryThread()
    {
		Logger.Info(LoggerType.LOG_SERVICE, "sockt 断开!!!!!!!!!!!!!!!!!!!");
        m_processor.DestoryThread();
    }

    public void DisConnect()
    {
        m_processor.DisConnect();
    }
    
    public bool CheckConnect()
    {
        return m_processor.CheckConnect();
    }

    public bool Send(Message message)
    {
        return m_processor.Send(message);
    }


    public bool Process()
    {
        if (m_processor.Process(this))
        {
            return true;
        }

        //这个地方不知道为何调用了一下登陆的东西，暂时被我注释。xzp
        //m_result_fun(0);
        return false;
    }

    public void Parse(Message message)
    {
        if (m_parser.ContainsKey(message.m_cmd_no))
        {
            Debug.Log("处理消息:" + message.m_cmd_no.ToString());
            ParseCommand parse_fun = m_parser[message.m_cmd_no];
            parse_fun(message);
            GameNetwork.Instance.ResetHeartbeat();  //重置心跳
            return;
        }

        Debug.Log("未处理消息:"+ message.m_cmd_no.ToString());
		//Logger.Error(LoggerType.LOG_SERVICE, "未处理消息{0}", message.m_cmd_no);
    }


    private void RegAllFun()
    {
        Debug.Log("RegAllFun WangGuan");

        RegParseFun(ParseStartupResult, CmdNumber.ServerReturnLoginClientCmd_S);
        //RegParseFun(ParseReturnLogin, CmdNumber.ServerReturnLoginSuccessLoginClientCmd_S);
    }

    private bool ParseStartupResult(Message message)
    {
       // ServerReturnLoginClientCmd rev = ServerReturnLoginClientCmd.ParseFrom(message.m_cmd);
        Debug.Log("ParseStartupResult");

        ServerReturnLoginClientCmd rev = (ServerReturnLoginClientCmd)GameNetwork.Instance.AnalysisMessage(message, typeof(ServerReturnLoginClientCmd)); //new ServerReturnLoginClientCmd();


        Debug.Log("RetCode =" + rev.ret_code);

        m_result_fun(rev.ret_code);

        //ServerReturnLoginClientCmd rev = ServerReturnLoginClientCmd.ParseFrom(message.m_cmd);

        //m_result_fun(rev.RetCode);
        return true;
    }

}