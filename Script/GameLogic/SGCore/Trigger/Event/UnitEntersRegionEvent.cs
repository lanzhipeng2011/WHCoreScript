using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Clojure;
public class UnitEntersRegionEvent : TriggerEvent
{
    Vector3 CenterPos;
    Vector3 Size;
    Quaternion Rotation;
    public override void OnLoad(List<ASTNode> args)
    {
        this.CenterPos = (Vector3)((ObjectNode)args[1]).Val;
        this.Size = (Vector3)((ObjectNode)args[2]).Val;
        this.Rotation = Quaternion.Euler( (Vector3)((ObjectNode)args[1]).Val );
        base.OnLoad(args);
    }

    public override bool EventCondition()
    {
        return true;
    }
}
