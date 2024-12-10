using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Clojure;
public class SGTrigger 
{

    public bool TriggerEnable = true; 
   public string Desc;
   protected bool IsRuning  = false;
   protected int  CurActioni = 0;
   protected bool IsWaitAction = false;

   public List<TriggerEvent> TriggerEvents =new List<TriggerEvent>();   
   public ASTNode ConditionNode; 
   public List<BaseAction> ActionNodes;

   public virtual void OnTrigger() 
   {
       if (TriggerEnable&&IsEvent() && IsCondition()) 
       {
           RunActions();
       }
   }

    //终止并关闭触发器
   public void CloseTrigger() 
   {
       IsRuning = false;
   }
   public bool IsEvent() 
   {
       for (int i = 0; i < TriggerEvents.Count; i++)
       {
           if (TriggerEvents[i].EventCondition())
           {
               return true;
           }
       }
       return false;
   }

   public bool IsCondition() 
   {
       if (ConditionNode == null) 
       {
           return true;
       }
      return ((BoolNode)RT.Evaluate(ConditionNode, GameTrigger.Instance.ClojureContext)).Val;
   }
   public void AddEvent(TriggerEvent triggerEvent)  
   {
       this.TriggerEvents.Add(triggerEvent);
   }

   public void RunActions() 
   {
       if (this.IsRuning == false) 
       {
           this.IsRuning = true;
           this.CurActioni = 0;
           this.IsWaitAction =false;
       }
       
   }

   public virtual void Update() 
   {

       if (CurActioni >= this.ActionNodes.Count)
       {
           this.IsRuning = false;
       }
       if (IsRuning == false) 
       {
           return;
       }
      BaseAction action = this.ActionNodes[CurActioni];
      while (CurActioni<this.ActionNodes.Count&&!IsWaitAction) 
      {
         
          if (action.Typ == BaseAction.ActionType.Block)
          {
              IsWaitAction = true;
              action.OnExec();
          }
          else 
          {
              IsWaitAction = false;
              action.OnExec();
              CurActioni++;
              if (CurActioni == this.ActionNodes.Count)
              {
                  this.IsRuning = false;
                  return;
              }
              action = this.ActionNodes[CurActioni];
          }
          
      }
     
      if (IsWaitAction && action != null) 
      {
          BlockAction blockAction = (BlockAction)action;
          blockAction.Update();
          if (blockAction.IsFinish) 
          {
              IsWaitAction = false;
              CurActioni++;
              blockAction.OnExit();
          }
      }
      

   
   }
   
}
