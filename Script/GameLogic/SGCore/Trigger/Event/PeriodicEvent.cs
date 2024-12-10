using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Clojure;
public class PeriodicEvent : TriggerEvent
{
    float PeriodicTime;
    float addTime;
    public override void OnLoad(List<ASTNode> args)
    {
        PeriodicTime = (float)((NumberNode)args[1]).NumberVal;
        addTime = PeriodicTime;
        base.OnLoad(args);
    }

    public override bool EventCondition()
    {
        addTime -= Time.deltaTime;
        if (addTime <= 0) 
        {
            addTime = PeriodicTime;
            return true;
        }
        return false;
    }
}
