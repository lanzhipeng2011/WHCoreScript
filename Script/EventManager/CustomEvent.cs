using UnityEngine;
using System.Collections;
// internal 事件监听器模型
internal class EventListener {
	
    public string name;
	
    public GameObject listener;
	
    public string function; 
	
}


// 自定义事件类, 在创建自定义事件的时候继承
public class CustomEvent{

	private string _type;
	
    private Hashtable _arguments = new Hashtable();

	// 构造函数
			
	public CustomEvent(Hashtable args, string eventType = "") {
		
        _type = eventType;
		if(args != null && args.Count > 0){
			_arguments = args;
		}
    }
	
    
		
	// 事件类型type
			
    public string type {
		
        get { return _type; }
		
        set { _type = value; }
		
    }

	// 事件传递的变量
			
   public Hashtable arguments {
		
        get { return _arguments; }
		
        set { _arguments = value; }
		
    }

}
