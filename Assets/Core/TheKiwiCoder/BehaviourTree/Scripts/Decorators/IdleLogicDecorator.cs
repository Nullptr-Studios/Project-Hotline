using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class IdleLogicDecorator : DecoratorNode
{
    private bool abortOnce = true;

    private EnemyBehaviourDataOverrider overrider;
    protected override void OnStart()
    {
        overrider = context.gameObject.GetComponent<EnemyBehaviourDataOverrider>();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() 
    {
        if (overrider.isActive)
        {
            if (!blackboard.seePlayer && blackboard.finalizedShearch)
            {
                child.Update();
                abortOnce = false;
            }
            else if (!abortOnce)
            {
                child.Abort();
                abortOnce = true;
            }
        }

        return State.Running;
    }
}
