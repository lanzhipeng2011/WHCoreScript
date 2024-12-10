using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Clojure;
using System.IO;
public class GameTrigger : SGGame.Singleton<GameTrigger>
{
    public  List<SGTrigger> Triggers = new List<SGTrigger>();
    public Dictionary<string, List<SGTrigger>> Events = new Dictionary<string,List<SGTrigger>>();
     
    public Clojure.Context ClojureContext;
    public void Load() 
    { 
        ClojureContext = RT.MainContext;
        ActionFuncDefine.Define();
        DefineTriggerClojureFunc();
    }

    public void Destory() 
    {
        this.Triggers.Clear();
        this.Events.Clear();
        RT.MainContext = null;
    }

    public void AddTrigger(SGTrigger trigger) 
    {
        this.Triggers.Add(trigger);
        for (int i = 0; i < trigger.TriggerEvents.Count;i++ ) 
        {
           TriggerEvent  curEvent = trigger.TriggerEvents[i];
           if (!this.Events.ContainsKey(curEvent.EventName))
           {
               this.Events[curEvent.EventName] = new List<SGTrigger>();
           }
           this.Events[curEvent.EventName].Add(trigger);
        }
    }

    public void CallEvent(string eventName)
    {
        if (this.Events.ContainsKey(eventName)) 
        {
            List<SGTrigger> Triggers = this.Events[eventName];
            for (int i = 0; i < Triggers.Count; i++) 
            {
                Triggers[i].OnTrigger(); 
            }
        }
    }

    void DefineTriggerClojureFunc() 
    {
        ClojureContext.Define("def-trigger",new FunctionNode(_def_Trigger));
        ClojureContext.Define("def-event", new FunctionNode(_def_event));
        ClojureContext.Define("def-actions", new FunctionNode(_def_actions));
        ClojureContext.Define("action", new FunctionNode(_action));
        
    }

    public string GetTriggerConfigPath()
    {
        string localPath = Application.persistentDataPath + "/ResData/Tables/Triggers/";
        if (File.Exists(localPath))
        {
            return localPath;
        }
        #if UNITY_ANDROID && !UNITY_EDITOR
         return localPath;
        #elif UNITY_EDITOR
         return Application.dataPath + "/BundleAssets/Tables/Triggers/";
        #else
         return Application.streamingAssetsPath + "/Tables/Triggers/";
        #endif
    }
    static ASTNode _def_Trigger(List<ASTNode> args,Context context)
    {
         SGTrigger newTrigger = new SGTrigger();
         //1.描述
         newTrigger.Desc = ((StringNode)args[0]).Val;
         //2.事件
         VectorNode vecNode = (VectorNode)args[1];
         for (int i = 0; i < vecNode.Children().Count;i++)
         {
             ASTNode eventNode = RT.Evaluate(vecNode.Children()[i], context);
             newTrigger.AddEvent((TriggerEvent)((ObjectNode)eventNode).Val);  
         }
         //条件
         int actionArg = 2;
         if (args.Count == 4) 
         {
             actionArg++;
             newTrigger.ConditionNode = args[2];
         }
        //动作
         newTrigger.ActionNodes  =(List<BaseAction>)((ObjectNode)RT.Evaluate(args[actionArg],context)).Val;
         GameTrigger.Instance.AddTrigger(newTrigger);
         return new ObjectNode(newTrigger);
    }

    static ASTNode _def_event(List<ASTNode> args, Context context) 
    {
        string eventName = ((StringNode)args[0]).Val;
        TriggerEvent newEvent  =(TriggerEvent) System.Activator.CreateInstance(System.Type.GetType(eventName + "Event"));
        newEvent.OnLoad(args);
        return new ObjectNode(newEvent);
    }

    public ASTNode _def_actions(List<ASTNode> args,Context context) 
    {
        List<BaseAction> actions = new List<BaseAction>();
        for (int i = 0; i < args.Count; i++) 
        {
            ListNode node = ((ListNode)args[i]);
            ASTNode actionNode = RT.Evaluate(node, context);
            if (node != null) 
            {
               BaseAction action = (BaseAction)((ObjectNode)actionNode).Val;
               actions.Add(action);
            }
        }
        return new ObjectNode(actions);
    }

    public ASTNode _action(List<ASTNode> args, Context context) 
    {
        ListNode lstNode = new ListNode();
        lstNode.nodes = new List<ASTNode>();
        SymbolNode actionNameNode = ((SymbolNode)args[0]);
        //如果从脚本作用域中找到了这个动作那么他就是一个非阻塞动作
        ASTNode actionFuncNode = context.Find(actionNameNode.Val);
        if (actionFuncNode != null)
        {
            lstNode.nodes = args;
            NonBlockAction curNewAction = new NonBlockAction(lstNode);
            return new ObjectNode(curNewAction);
        }
        //如果没找到他就是一个阻塞动作
        else
        {
            //利用反射创建这个动作类
            BlockAction blockAction = (BlockAction)System.Activator.CreateInstance(System.Type.GetType(actionNameNode.Val+"Action"));
            blockAction.Typ = BaseAction.ActionType.Block;
            blockAction.OnLoad(args);
            if (blockAction == null)
            {
                Debug.LogError("使用了不存在的动作节点");
            }
            else
            {
                return new ObjectNode(blockAction);
            }
        }
        return new NilNode(); 
    }

    public void Update() 
    {
        CallEvent("Periodic");
        if(this.Triggers!=null)
        for (int i = 0; i < this.Triggers.Count; i++)
        {
            this.Triggers[i].Update();
        }
    }
    public void ExecTrigger(int id) 
    {
        this.Triggers[id].RunActions();
    }
}
 