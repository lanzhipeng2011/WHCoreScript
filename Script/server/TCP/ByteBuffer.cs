using System;
using System.Collections;


public class ByteBuffer
{
    private uint m_size;
    private uint m_cur_size;
    private byte[] m_buffer;

    public ByteBuffer()
    {
        m_size = 256;
        m_cur_size = 0;
        m_buffer = new byte[m_size];
    }
    public ByteBuffer(uint size)
    {
        m_size = size;
        m_cur_size = 0;
        m_buffer = new byte[size];
    }

    public byte[] GetData()
    {
        ushort data_size = GetAllDataSize(m_buffer);
        if (data_size <= m_cur_size)
        {
            byte[] ret_buffer = new byte[data_size];
            Array.Copy(m_buffer, 0, ret_buffer, 0, data_size);
            return ret_buffer;
        }
        return null;
    }

    public static ushort GetDataSize(byte[] buffer)
    {
        return BitConverter.ToUInt16(buffer, 0);
    }

    public static ushort GetAllDataSize(byte[] buffer)
    {                   
        return (ushort)(BitConverter.ToUInt16(buffer, 0) + sizeof(uint));
    }

    public uint GetCurSize()
    {
        return m_cur_size;
    }

    public bool CheckOK()
    {
        return ((m_cur_size != 0) && (m_cur_size >= GetAllDataSize(m_buffer)));
    }

    public void Push(byte[] val, uint len)
    {
        if ((m_size - m_cur_size) < len)
        {
            m_size += len;
            byte[] new_buffer = new byte[m_size];
            Array.Copy(m_buffer, 0, new_buffer, 0, m_cur_size);
            m_buffer = new_buffer;
        }
        Array.Copy(val, 0, m_buffer, m_cur_size, len);
        m_cur_size += len;
    }

    public void Pop()
    {
        ushort data_size = GetAllDataSize(m_buffer);
        if (data_size <= m_cur_size)
        {
            m_cur_size -= data_size;
            Array.Copy(m_buffer, data_size, m_buffer, 0, m_cur_size);
        }
    }
}