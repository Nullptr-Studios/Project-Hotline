using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class SetAllValuesToOverrider : ActionNode
{
    private EnemyBehaviourDataOverrider ov;
    
    protected override void OnStart()
    {
        ov = context.gameObject.GetComponent<EnemyBehaviourDataOverrider>();

        blackboard.distanceToUseWeapon = ov.behaviourData.distanceToShoot;

    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
