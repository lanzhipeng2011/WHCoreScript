using UnityEngine;
using System.Collections;

public class EventManager {
	
	// 单例实例
	private static EventManager _instance;
	
	// 设置
	public bool allowSingleton = true; // 场景scene变换时EventManager会相应传递.
	public bool allowWarningOutputs = false;
	public bool allowDebugOutputs = false;

	private Hashtable _listeners = new Hashtable();	

	// 退出时清空监听器
	public void OnApplicationQuit() {
		_listeners.Clear();
	}
	//
	private EventManager(){

	}

	public static EventManager instance {
		get {
			if(_instance == null){
				_instance = new EventManager();
			}
			return _instance;
		}
	}

	// PUBLIC 公共函数*******************************
	
	// 添加事件监听器
	public bool addEventListener(string eventType, GameObject listener, string function) {
		if (listener == null || eventType == null) {
			if (allowWarningOutputs) {
				Debug.LogWarning("Event Manager: AddListener failed due to no listener or event name specified.");
			}
			return false;
		}
		recordEvent(eventType);
		return recordListener(eventType, listener, function);
	}
	// 添加事件监听器
	public bool insertEventListener(string eventType, GameObject listener, string function) {
		if (listener == null || eventType == null) {
			if (allowWarningOutputs) {
				Debug.LogWarning("Event Manager: AddListener failed due to no listener or event name specified.");
			}
			return false;
		}
		recordEvent(eventType);
		return insertListener(eventType, listener, function);
	}
	// 移除事件监听器
	public bool removeEventListener(string eventType, GameObject listener) {
		if (!checkForEvent(eventType)) 
			return false;
		
		ArrayList listenerList = _listeners[eventType] as ArrayList;
		foreach (EventListener callback in listenerList) {
			if (callback.name == listener.GetInstanceID().ToString()) {
				listenerList.Remove(callback);
				return true;
			}
		}
		return false;
	}
	
	// 移除所有事件监听器
	public void removeAllEventListeners(GameObject listener) {
		foreach (EventListener callback in _listeners) {
			if (callback.listener.GetInstanceID().ToString() == listener.GetInstanceID().ToString()) {
				_listeners.Remove(callback);
			}
		}
	}
	public bool dispatchEvent(Hashtable args,string eventKey) {
		CustomEvent ev = new CustomEvent (args,eventKey);
		return dispatchEvent (ev);
	}
	// 派发事件
	public bool dispatchEvent(CustomEvent evt) {
		string eventType = evt.type;
		if (!checkForEvent(eventType)) {
			if (allowWarningOutputs) {
				Debug.LogWarning("Event Manager: Event \"" + eventType + "\" triggered has no listeners!");
			}
			return false;
		}
		
		ArrayList listenerList = _listeners[eventType] as ArrayList;
		if (allowDebugOutputs) {
			Debug.Log("Event Manager: Event " + eventType + " dispatched to " + listenerList.Count + ((listenerList.Count == 1) ? " listener." : " listeners."));
		}
		foreach (EventListener callback in listenerList) {
			if (callback.listener && callback.listener.activeSelf) {
				callback.listener.SendMessage(callback.function, evt, SendMessageOptions.DontRequireReceiver);
			}
		}
		return false;
	}
	
	// PRIVATE 私有函数*******************************
	
	private void Setup() {
		// 所要处理的: 创建GameObject，若已存在就无视本函数吧
	}
	
	// 检查事件是否已经存在
	private bool checkForEvent(string eventType) {
		if (_listeners.ContainsKey(eventType)) 
			return true;
		return false;
	}
	
	// 记录事件，已记录的就不再重复了
	private bool recordEvent(string eventType) {
		if (!checkForEvent(eventType)) {
			_listeners.Add(eventType, new ArrayList());
			return true;
		}
		return false;
	}
	
	// 删除事件
	private bool deleteEvent(string eventType) {
		if (!checkForEvent(eventType)) 
			return false;
		_listeners.Remove(eventType);
		return true;
	}
	
	// 检查监听器是否存在
	private bool checkForListener(string eventType, GameObject listener) {
		if (!checkForEvent(eventType)) {
			recordEvent(eventType);
		}
		
		ArrayList listenerList = _listeners[eventType] as ArrayList;
		foreach (EventListener callback in listenerList) {
			if (callback.name == listener.GetInstanceID().ToString()) 
				return true;
		}
		return false;
	}
	
	// 记录监听器, 已存在的就不记录了
	private bool recordListener(string eventType, GameObject listener, string function) {
		if (!checkForListener(eventType, listener)) {
			ArrayList listenerList = _listeners[eventType] as ArrayList;
			EventListener callback = new EventListener();
			callback.name = listener.GetInstanceID().ToString();
			callback.listener = listener;
			callback.function = function;
			listenerList.Add(callback);
			return true;
		} else {
			if (allowWarningOutputs) {
				Debug.LogWarning("Event Manager: Listener: " + listener.name + " is already in list for event: " + eventType);
			}
			return false;
		}
	}
	// 记录监听器, 已存在的就不记录了
	private bool insertListener(string eventType, GameObject listener, string function) {
		if (!checkForListener(eventType, listener)) {
			ArrayList listenerList = _listeners[eventType] as ArrayList;
			EventListener callback = new EventListener();
			callback.name = listener.GetInstanceID().ToString();
			callback.listener = listener;
			callback.function = function;
			listenerList.Insert(0,callback);
			return true;
		} else {
			if (allowWarningOutputs) {
				Debug.LogWarning("Event Manager: Listener: " + listener.name + " is already in list for event: " + eventType);
			}
			return false;
		}
	}
}