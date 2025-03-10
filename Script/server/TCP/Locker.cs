using System.Threading;

public class Locker
{
    private object m_sync_obj;

    public Locker()
    {
        m_sync_obj = new object();
    }

    public void Lock()
    {
        if (m_sync_obj != null)
        {
            Monitor.Enter(m_sync_obj);
        }
    }

    public void UnLock()
    {
        if (m_sync_obj != null)
        {
            Monitor.Exit(m_sync_obj);
        }
    }
}

