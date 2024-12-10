/********************************************************************
	created:	2014/07/11
	created:	11:7:2014   11:10
	filename: 	RecycleList.cs
	author:		王迪
	
	purpose:	可以回收new的类，增加内存消耗
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RecycleList<T> where T : new() {
    private List<T> m_recycleList = new List<T>();
    private List<T> m_usingList = new List<T>();

    public List<T> UsingList() { return m_usingList; }

    public T GetNewItem()
    {
        if (m_recycleList.Count > 0)
        {
            T retItem = m_recycleList[0];
            m_recycleList.RemoveAt(0);
            return retItem;
        }
        else
        {
            return new T();
        }
    }

    public void Add(T item)
    {
        m_usingList.Add(item);
    }

    public void RemoveAt(int index)
    {
        if (m_usingList.Count > index)
        {
            m_recycleList.Add(m_usingList[index]);
            m_usingList.RemoveAt(index);
        }
    }

    public void Clear()
    {
        for (int i = 0; i < m_usingList.Count; i++)
        {
            m_recycleList.Add(m_usingList[i]);
        }

        m_usingList.Clear();
    }
}


public class RecycleQueue<T> where T : new()
{
    private Queue<T> m_recycleQueue = new Queue<T>();
    private Queue<T> m_usingQueue = new Queue<T>();

    public Queue<T> UsingQueue() { return m_usingQueue; }

    public T GetNewItem()
    {
        if (m_recycleQueue.Count > 0)
        {
            T retItem = m_recycleQueue.Peek();
            m_recycleQueue.Dequeue();
            return retItem;
        }
        else
        {
            return new T();
        }
    }

    public void Enqueue(T item)
    {
        m_usingQueue.Enqueue(item);
    }

    public void Dequeue()
    {
        if (m_usingQueue.Count > 0)
        {
            T retItem = m_usingQueue.Peek();
            m_usingQueue.Dequeue();

            m_recycleQueue.Enqueue(retItem);
        }
    }

    public T Peek()
    {
        return m_usingQueue.Peek();
    }

    public void Clear()
    {
        while (m_usingQueue.Count > 0)
        {
            m_recycleQueue.Enqueue(m_usingQueue.Peek());
            m_usingQueue.Dequeue();
        }

        m_usingQueue.Clear();
    }
}