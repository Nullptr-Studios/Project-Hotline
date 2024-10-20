using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class IdleLogic : ActionNode
{
    protected override void OnStart() 
    {
        if (blackboard.returnToInitialPos && blackboard.isStatic)
        {
            blackboard.moveToPosition = blackboard.initialPos;
            blackboard.moveToRotation = blackboard.initialRotation;
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
