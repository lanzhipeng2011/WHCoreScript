using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Clojure;
public class WaitAction : BlockAction
{
    public float waitTime;
    public float addTime;
    public override void OnLoad(List<ASTNode> args)
    {
        waitTime = (float)((NumberNode)args[1]).NumberVal;
      
       
    }

    public override void OnExec()
    {
        addTime = waitTime;
        this.IsFinish = false;
    }

    public override void Update()
    {
        addTime -= Time.deltaTime;
        if (addTime <= 0) 
        {
            this.IsFinish = true;
        }
    }
}
