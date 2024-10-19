using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class IdleLogic : DecoratorNode
{
    private bool abortOnce = true;
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() 
    {
        if (!blackboard.seePlayer && blackboard.finalizedShearch)
        {
            child.Update();
            abortOnce = false;
        }
        else if(!abortOnce)
        {
            child.Abort();
            abortOnce = true;
        }
        
        return State.Running;
    }
}
