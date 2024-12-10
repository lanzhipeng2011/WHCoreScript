/********************************************************************************
 *	文件名：SocketInputStream.cs
 *	全路径：	\NetWork\SocketAPI\SocketInputStream.cs
 *	创建人：	王华
 *	创建时间：2013-11-29
 *
 *	功能说明： Socket的应用层输入缓冲区
 *	       
 *	修改记录：
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Net;
#if UNITY_WP8
using UnityPortSocket;
#else
using System.Net.Sockets;
#endif

namespace SPacket.SocketInstance
{
    public class SocketInputStream
    {
        public const uint SOCKET_ERROR = 0xFFFFFFFF;
        public const uint DEFAULT_SOCKET_INPUT_BUFFER_SIZE = 1024 * 256;
        public const uint MAX_SOCKET_INPUT_BUFFER_SIZE = 1024 * 512;

        public SocketInputStream(SocketInstance socket,
            uint BufferLen = DEFAULT_SOCKET_INPUT_BUFFER_SIZE,
            uint MaxBufferLen = MAX_SOCKET_INPUT_BUFFER_SIZE)
        {
            m_pSocket = socket;
            m_BufferLen = BufferLen;
            m_MaxBufferLen = MaxBufferLen;
            m_Head = 0;
            m_Tail = 0;
            m_Buffer = new Byte[BufferLen];
            m_BufferTemp = new Byte[BufferLen];
        }
        void ClearBufferTemp()
        {
            // 判断临时数组是否为空
            if (m_BufferTemp != null)
            {
                // 将有用长度之后的清理掉
                int nCount = m_BufferTemp.Length;
                Array.Clear(m_BufferTemp, 0, nCount);
            }
        }

        public uint Length()
        {
            if (m_Head < m_Tail)
                return m_Tail - m_Head;

            else if (m_Head > m_Tail)
                return m_BufferLen - m_Head + m_Tail;

            return 0;
        }

        public uint Read(Byte[] buf, uint len)
        {
            // 判断读取长度 是否为0
            if (len == 0)
                return 0;
            // 判断读取长度是否大于当前长度
            if (len > Length())
                return 0;
            // 头在前的情况
            if (m_Head < m_Tail)
            {
                for (uint i = 0; i < len; ++i)
                {
                    buf[i] = m_Buffer[m_Head + i];
                }
            }
            else// 头在后的情况
            {
                // 计算数组后面要拿走的长度
                uint rightLen = m_BufferLen - m_Head;
                // 如果要拿走的长度小于等于头部到数组结尾的长度直接拿
                if (len <= rightLen)
                {
                    for (uint i = 0; i < len; ++i)
                    {
                        buf[i] = m_Buffer[m_Head + i];
                    }
                }
                else // 如果要拿走的长度大于头部到数组结尾的长度先拿后面  再到开头拿
                {
                    for (uint i = 0; i < rightLen; ++i)
                    {
                        buf[i] = m_Buffer[m_Head + i];
                    }
                    for (uint i = 0; i < len - rightLen; ++i)
                    {
                        buf[i + rightLen] = m_Buffer[i];
                    }
                }
            }
            // 拿完数据重置头部
            m_Head = (m_Head + len) % m_BufferLen;

            return len;
        }
        public bool Peek(Byte[] buf, uint len)
        {

            if (len == 0)
                return false;

            if (len > Length())
                return false;

            if (m_Head < m_Tail)
            {

                for (uint i = 0; i < len; ++i)
                {
                    buf[i] = m_Buffer[m_Head + i];
                }

            }
            else
            {
                uint rightLen = m_BufferLen - m_Head;
                if (len <= rightLen)
                {

                    for (uint i = 0; i < len; ++i)
                    {
                        buf[i] = m_Buffer[m_Head + i];
                    }
                }
                else
                {
                    for (uint i = 0; i < rightLen; ++i)
                    {
                        buf[i] = m_Buffer[m_Head + i];
                    }
                    for (uint i = 0; i < len - rightLen; ++i)
                    {
                        buf[rightLen + i] = m_Buffer[i];
                    }
                }
            }

            return true;
        }
        public bool Skip(uint len)
        {
            if (len == 0)
                return false;

            if (len > Length())
                return false;

            m_Head = (m_Head + len) % m_BufferLen;

            return true;
        }

        public uint Fill()
        {
            // 这一次接收的总长度
            uint nFilled = 0;
            // 一次接收的长度
            uint nReceived = 0;
            // 剩余的长度
            uint nFree = 0;
            // 判断Socket实例是否为空
            if (m_pSocket == null) return 0;
            // 判断头的位置是否小于等于尾
            if (m_Head <= m_Tail)
            {
                // 如果头的位置在0
                if (m_Head == 0)
                {
                    //
                    // H   T		LEN=10
                    // 0123456789
                    // abcd......
                    //
                    // 定义一个接收长度
                    nReceived = 0;
                    // 计算剩余的长度
                    nFree = m_BufferLen - m_Tail - 1;
                    if (nFree != 0)
                    {
                        ClearBufferTemp();
                        // 开始接收消息
                        nReceived = m_pSocket.receive(m_BufferTemp, (int)nFree);
                        if (nReceived == SOCKET_ERROR) return SOCKET_ERROR;
                        // 将接收到的消息 从临时储存放到缓存数组中
                        for (int i = 0; i < nReceived; ++i)
                        {
                            m_Buffer[m_Tail + i] = m_BufferTemp[i];
                        }

                        // if (nReceived==SOCKET_ERROR_WOULDBLOCK) return 0 ; 
                        // if (nReceived==SOCKET_ERROR) return SOCKET_ERROR-1 ;

                        // 更新尾部信息
                        m_Tail += nReceived;
                        // 更新长度信息
                        nFilled += nReceived;
                    }
                    // 判断接收的长度和剩余长度相等  说明数组满了
                    if (nReceived == nFree)
                    {
                        // 获取Socket中可供读取的数据的字节数
                        uint available = m_pSocket.available();
                        // 判断是否还有可读的数据
                        if (available > 0)
                        {
                            // 判断如果当前流的长度加上Socket可读的长度大于最大长度
                            if ((m_BufferLen + available + 1) > m_MaxBufferLen)
                            {
                                // 初始化流
                                Initsize();
                                return SOCKET_ERROR;
                            }
                            // 重新设置大小
                            if (!Resize(available + 1))
                                return 0;
                            // 清理缓存
                            ClearBufferTemp();
                            // 继续接收消息
                            nReceived = m_pSocket.receive(m_BufferTemp, (int)available);
                            if (nReceived == SOCKET_ERROR) return SOCKET_ERROR;
                            // 数据从临时缓存写入缓存中
                            for (int i = 0; i < nReceived; ++i)
                            {
                                m_Buffer[m_Tail + i] = m_BufferTemp[i];
                            }

                            // if (nReceived==SOCKET_ERROR_WOULDBLOCK) return 0 ; 
                            // if (nReceived==SOCKET_ERROR) return SOCKET_ERROR-4 ;
                            //if (nReceived==0) return SOCKET_ERROR-5;
                            // 设置尾部
                            m_Tail += nReceived;
                            // 更新长度信息
                            nFilled += nReceived;
                        }
                    }

                }
                else // 如果头的位置不在0
                {
                    //
                    //    H   T		LEN=10
                    // 0123456789
                    // ...abcd...
                    //
                    // 剩余的长度
                    nFree = m_BufferLen - m_Tail;

                    ClearBufferTemp();
                    nReceived = m_pSocket.receive(m_BufferTemp, (int)nFree);
                    if (nReceived == SOCKET_ERROR) return SOCKET_ERROR;

                    for (int i = 0; i < nReceived; ++i)
                    {
                        m_Buffer[m_Tail + i] = m_BufferTemp[i];
                    }

                    //nReceived = m_pSocket->receive( &m_Buffer[m_Tail], nFree );
                    // if( nReceived==SOCKET_ERROR_WOULDBLOCK ) return 0 ; 
                    // if( nReceived==SOCKET_ERROR ) return SOCKET_ERROR-6 ;
                    // if( nReceived==0 ) return SOCKET_ERROR-7 ;

                    m_Tail = (m_Tail + nReceived) % m_BufferLen;
                    nFilled += nReceived;

                    if (nReceived == nFree)
                    {
                        //				Assert( m_Tail == 0 );

                        nReceived = 0;
                        nFree = m_Head - 1;
                        if (nFree != 0)
                        {
                            ClearBufferTemp();
                            nReceived = m_pSocket.receive(m_BufferTemp, (int)nFree);
                            if (nReceived == SOCKET_ERROR) return SOCKET_ERROR;

                            for (int i = 0; i < nReceived; ++i)
                            {
                                m_Buffer[i] = m_BufferTemp[i];
                            }

                            //  nReceived = m_pSocket->receive( &m_Buffer[0] , nFree );
                            // if( nReceived==SOCKET_ERROR_WOULDBLOCK ) return 0 ; 
                            // if( nReceived==SOCKET_ERROR ) return SOCKET_ERROR -8;
                            // if( nReceived==0 ) return SOCKET_ERROR-9 ;

                            m_Tail += nReceived;
                            nFilled += nReceived;
                        }

                        if (nReceived == nFree)
                        {
                            uint available = m_pSocket.available();
                            if (available > 0)
                            {
                                if ((m_BufferLen + available + 1) > m_MaxBufferLen)
                                {
                                    Initsize();
                                    return SOCKET_ERROR;
                                }
                                if (!Resize(available + 1))
                                    return 0;

                                ClearBufferTemp();
                                nReceived = m_pSocket.receive(m_BufferTemp, (int)available);
                                if (nReceived == SOCKET_ERROR) return SOCKET_ERROR;

                                for (int i = 0; i < nReceived; ++i)
                                {
                                    m_Buffer[m_Tail + i] = m_BufferTemp[i];
                                }

                                // nReceived = m_pSocket->receive( &m_Buffer[m_Tail] , available );
                                //if (nReceived==SOCKET_ERROR_WOULDBLOCK) return 0 ; 
                                //if (nReceived==SOCKET_ERROR) return SOCKET_ERROR-11 ;
                                // if (nReceived==0) return SOCKET_ERROR-12;

                                m_Tail += nReceived;
                                nFilled += nReceived;
                            }
                        }
                    }

                }

            }
            else
            {
                //
                //     T  H		LEN=10
                // 0123456789
                // abcd...efg
                //

                nReceived = 0;
                nFree = m_Head - m_Tail - 1;
                if (nFree != 0)
                {
                    ClearBufferTemp();
                    nReceived = m_pSocket.receive(m_BufferTemp, (int)nFree);
                    if (nReceived == SOCKET_ERROR) return SOCKET_ERROR;

                    for (int i = 0; i < nReceived; ++i)
                    {
                        m_Buffer[m_Tail + i] = m_BufferTemp[i];
                    }

                    // nReceived = m_pSocket->receive( &m_Buffer[m_Tail], nFree ) ;
                    // if( nReceived==SOCKET_ERROR_WOULDBLOCK ) return 0 ; 
                    //if( nReceived==SOCKET_ERROR ) return SOCKET_ERROR-13 ;
                    //if( nReceived==0 ) return SOCKET_ERROR-14 ;

                    m_Tail += nReceived;
                    nFilled += nReceived;
                }
                if (nReceived == nFree)
                {
                    uint available = m_pSocket.available();
                    if (available > 0)
                    {
                        if ((m_BufferLen + available + 1) > m_MaxBufferLen)
                        {
                            Initsize();
                            return SOCKET_ERROR;
                        }
                        if (!Resize(available + 1))
                            return 0;
                        ClearBufferTemp();
                        nReceived = m_pSocket.receive(m_BufferTemp, (int)available);
                        if (nReceived == SOCKET_ERROR) return SOCKET_ERROR;

                        for (int i = 0; i < nReceived; ++i)
                        {
                            m_Buffer[m_Tail + i] = m_BufferTemp[i];
                        }

                        // nReceived = m_pSocket->receive( &m_Buffer[m_Tail], available ) ;
                        // if( nReceived==SOCKET_ERROR_WOULDBLOCK ) return 0 ; 
                        // if( nReceived==SOCKET_ERROR ) return SOCKET_ERROR-16 ;
                        // if( nReceived==0 ) return SOCKET_ERROR-17 ;

                        m_Tail += nReceived;
                        nFilled += nReceived;
                    }
                }

            }

            return nFilled;
        }
        public void Initsize(uint BufferSize = DEFAULT_SOCKET_INPUT_BUFFER_SIZE)
        {
            // 重置头尾
            m_Head = 0;
            m_Tail = 0;
            // 设置长度
            if (BufferSize > m_MaxBufferLen)
            {
                BufferSize = m_MaxBufferLen;
            }
            // 重新new数组
            m_Buffer = new Byte[BufferSize];
            // 设置最大长度
            m_BufferLen = BufferSize;
        }
        public bool Resize(uint size)
        {
            // 比较可读取长度+1和流长度右移一位
            size = Math.Max(size, (uint)(m_BufferLen >> 1));
            // 计算新的缓存长度
            uint newBufferLen = (uint)(m_BufferLen + size);
            // 获取当前使用的长度
            uint len = Length();
            // 判断size是否小于0
            if (size < 0)
            {
                // 判断新长度是否小于0或者新长度小于当前使用的长度
                if (newBufferLen < 0 || newBufferLen < len)
                    return false;
            }
            // 重新new出来消息数组
            Byte[] newBuffer = new Byte[newBufferLen];
            // 转移数据
            if (m_Head < m_Tail)
            {
                //memcpy(newBuffer, &m_Buffer[m_Head], m_Tail - m_Head);
                for (int i = 0; i < m_Tail - m_Head; ++i)
                {
                    newBuffer[i] = m_Buffer[m_Head + i];
                }
            }
            else if (m_Head > m_Tail)
            {
                //memcpy(newBuffer, &m_Buffer[m_Head], m_BufferLen - m_Head);
                for (int i = 0; i < m_BufferLen - m_Head; ++i)
                {
                    newBuffer[i] = m_Buffer[m_Head + i];
                }

                //memcpy(&newBuffer[m_BufferLen - m_Head], m_Buffer, m_Tail);
                for (int i = 0; i < m_BufferLen - m_Head; ++i)
                {
                    newBuffer[m_BufferLen - m_Head + i] = m_Buffer[i];
                }
            }

            // delete[] m_Buffer;
            // 重新设置成员变量
            // 覆盖数组
            m_Buffer = newBuffer;
            // 覆盖长度
            m_BufferLen = newBufferLen;
            // 设置头尾
            m_Head = 0;
            m_Tail = len;

            return true;
        }
        public void CleanUp()
        {
            m_Head = 0;
            m_Tail = 0;
        }
        SocketInstance m_pSocket;
        Byte[] m_Buffer = null;
        Byte[] m_BufferTemp = null;     //临时缓存，解决new的GC问题

        uint m_BufferLen;
        uint m_MaxBufferLen;

        uint m_Head;
        uint m_Tail;
    }
}
