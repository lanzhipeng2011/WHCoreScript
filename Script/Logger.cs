using System;
using System.Text;
using System.IO;
using System.Globalization;
using UnityEngine;

// 用于表示打印的类型 by lsj
public enum LoggerType
{
	LOG_CLIENT,
	LOG_SERVICE,
}

public class Logger
{
	// 更改为静态参数 by lsj
	private static string m_info = " INFO: ";
	private static string m_error = " ERROR: ";
	private static string m_warn = " WARNING: ";
	//private static Locker m_lock = new Locker();
	private static Locker m_client_lock = new Locker();
	private static Locker m_service_lock = new Locker();

	// 客户端log by lsj
	private static StreamWriter m_client_writer;
	// 服务类log by lsj
	private static StreamWriter m_service_writer;
	private static bool m_state = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.OSXPlayer
			|| Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor? false : true;

	//private static Logger m_instance;

	//public static Logger GetSingleton()
	//{
	//    if (m_instance == null)
	//    {
	//        m_instance = new Logger();
	//    }
	//    return m_instance;
	//}

	//private static StreamWriter m_file_writer;


	//// 使用单件
	//private Logger()
	//{
	//    m_info = " INFO: ";
	//    m_error = " ERROR: ";
	//    m_warn = " WARNING: ";
	//    m_lock = new Locker();
	//    m_state = true;
	//}

	//public static void SetState(bool value)
	//{
	//    m_state = value;

	//    if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.OSXPlayer
	//        || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor)
	//    {
	//        m_state = false;
	//    }
	//}

    public static void Info(LoggerType type, string format, params object[] args)
    {
		Write(type, m_info + string.Format(format, args));
    }

	public static void Error(LoggerType type, string format, params object[] args)
    {
		Write(type, m_error + string.Format(format, args));
    }

	public static void Warn(LoggerType type, string format, params object[] args)
    {
		Write(type, m_warn + string.Format(format, args));
    }

	private static void Write(LoggerType type, string msg)
    {
        if (m_state)
        {
            DateTime date = DateTime.Now;
            msg = date.ToString() + msg;

			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                Console.WriteLine(msg);
            }

            Debug.Log(msg);

			if (type == LoggerType.LOG_SERVICE)
			{
				m_service_lock.Lock();
				WriteServiceFile(msg);
				m_service_lock.UnLock();
			}
			else if (type == LoggerType.LOG_CLIENT)
			{
				m_client_lock.Lock();
				WriteClientFile(msg);
				m_client_lock.UnLock();
			}
        }
    }

	/// <summary>
	/// 写服务日志
	/// </summary>
	/// <param name="msg"></param>
	private static void WriteServiceFile(string msg)
    {
        try
        {
			if (m_service_writer == null)
            {
                FileStream fs = new FileStream("service.log", FileMode.Create);
				m_service_writer = new StreamWriter(fs, Encoding.UTF8);
            }
			m_service_writer.WriteLine(msg);
			m_service_writer.Flush();
        }
        catch (System.Exception ex)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                Console.WriteLine("写入日志文件失败: {0}", ex.Message);
            }
        }
    }

	/// <summary>
	/// 写客户端日志
	/// </summary>
	/// <param name="msg"></param>
	private static void WriteClientFile(string msg)
	{
		try
		{
			if (m_client_writer == null)
			{
				FileStream fs = new FileStream("Client.log", FileMode.Create);
				m_client_writer = new StreamWriter(fs, Encoding.UTF8);
			}
			m_client_writer.WriteLine(msg);
			m_client_writer.Flush();
		}
		catch (System.Exception ex)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
			{
				Console.WriteLine("写入日志文件失败: {0}", ex.Message);
			}
		}
	}
}
