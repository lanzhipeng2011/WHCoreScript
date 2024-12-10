using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Clojure;
public class TriggerEvent
{

    public string EventName;

    public virtual void OnLoad(List<ASTNode> args)
    {
        this.EventName = ((StringNode)args[0]).Val;

    }
    public virtual bool EventCondition() 
    {
        return true;
    }
}
