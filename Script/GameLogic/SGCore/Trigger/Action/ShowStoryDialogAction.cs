using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Clojure;
public class ShowStoryDialogAction : BlockAction
{
    int stroyId;
    bool isSet = false;
    public override void OnLoad(List<ASTNode> args)
    {
        stroyId  =(int) ((NumberNode)args[1]).NumberVal;
    }
    public override void OnExec()
    {
        StoryDialogLogic.ShowStory(stroyId);
    }

    public override void Update()
    {
        if (isSet==false&&StoryDialogLogic.Instance() != null) 
        {
            isSet = true;
            StoryDialogLogic.Instance().DialogShowOver = () =>
            {
                IsFinish = true;
            };
        }
    }
}
