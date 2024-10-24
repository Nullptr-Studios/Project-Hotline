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

        if (ov && ov.behaviourData != null)
        {
            blackboard.distanceToUseWeapon = ov.behaviourData.distanceToShoot;
            blackboard.timeToStartShooting = ov.behaviourData.timeToStartShooting;

            blackboard.returnToInitialPos = ov.behaviourData.returnToInitialSpot;
            blackboard.isStatic = ov.behaviourData.isStatic;
            blackboard.waypoints = ov.behaviourData.waypoints;

            blackboard.searchTimes = ov.behaviourData.searchTimes;

            blackboard.idleSpeed = ov.behaviourData.idleSpeed;
            blackboard.chaseSpeed = ov.behaviourData.chasingSpeed;
        }
        else
        {
            Debug.LogWarning("EnemyBehaviour: " + context.gameObject.name + " does not have a data overrider, it will have default behaviour values");
        }

        blackboard.initialPos = context.transform.position;
        blackboard.initialRotation = context.transform.rotation;

    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
