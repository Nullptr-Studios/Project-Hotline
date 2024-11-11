using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class AfterStun : DecoratorNode
{
    private EnemyBehaviourDataOverrider Ov;
    protected override void OnStart()
    {
        Ov = context.transform.GetComponent<EnemyBehaviourDataOverrider>();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {

        if (Ov.justStunned)
        {
            if (child.Update() == State.Success)
            {
                Ov.justStunned = false;
            }
        }
        
        return State.Running;
    }
}
