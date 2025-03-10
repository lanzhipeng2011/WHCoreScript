using System.Threading;

public class ServiceThread
{
    private ThreadState m_state;
    private Thread m_thread;

    public ServiceThread()
    {
        m_state = ThreadState.State_None;
    }

    public bool Start(TcpServer tcp)
    {
        if (m_state == ThreadState.State_None)
        {
            //ParameterizedThreadStart start = new ParameterizedThreadStart(CallFun);
            //m_thread = new Thread(start);

            m_thread = new Thread(new ParameterizedThreadStart(CallFun));
            m_thread.Start(tcp);
            m_state = ThreadState.State_Run;
            return true;
        }
        return false;
    }

    public void Final()
    {
        if (m_state == ThreadState.State_Run)
        {
            Join();
        }
    }

    public void Join()
    {
        m_thread.Join();
        m_state = ThreadState.State_Final;
    }


    public bool CheckRun()
    {
        return (m_state == ThreadState.State_Run);
    }

    private void CallFun(object parm)
    {
		//ServiceClient.Info("ServiceThread  CallFun  Begin ");
		Logger.Info(LoggerType.LOG_SERVICE, "ServiceThread  CallFun  Begin ");
        TcpServer tcp = parm as TcpServer;
        tcp.Receive();
    }
}

