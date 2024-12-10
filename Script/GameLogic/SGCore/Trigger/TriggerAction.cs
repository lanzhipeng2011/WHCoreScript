using UnityEngine;
using System.Collections;
using Clojure;
using System.Collections.Generic;
public class BaseAction 
{
    public enum ActionType 
    {
        //非阻塞.
        NonBlock,   
        //阻塞
        Block,
    }
    public ActionType Typ; 
    public virtual void OnExec(){}

    public virtual void Update() { }
}

public class NonBlockAction : BaseAction
{
    protected ASTNode ActionNode;

    public NonBlockAction(ASTNode node) 
    {
        this.Typ = ActionType.NonBlock;
        this.ActionNode = node;
    }

    public override void OnExec()
    {
        RT.Evaluate(ActionNode,GameTrigger.Instance.ClojureContext);
    }
}

public class BlockAction : BaseAction 
{
    public bool IsFinish = false;
    public BlockAction() 
    {
        this.Typ = ActionType.Block;
    }
    public virtual void OnLoad(List<ASTNode> args) 
    {
    
    }
   
    public virtual void OnExit() 
    { 
    
    }
}
