using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class IdleLogic : DecoratorNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() 
    {
        if (!blackboard.seePlayer && blackboard.finalizedShearch)
        {
            child.Update();
        }
        else
        {
            child.Abort();
        }
        
        return State.Running;
    }
}
