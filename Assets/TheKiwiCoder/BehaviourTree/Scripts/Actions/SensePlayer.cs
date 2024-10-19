using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class SensePlayer : ActionNode
{
    private AISensor sensor;
    
    private bool hasSensedPlayerBefore = false;
    protected override void OnStart() 
    {
        sensor = context.gameObject.GetComponent<AISensor>();
        hasSensedPlayerBefore = false;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (sensor.isDetecting)
        {
            blackboard.playerPos = sensor.detectedObjects[0].transform.position;
            blackboard.seePlayer = true;
            context.agent.SetDestination(blackboard.playerPos);
            hasSensedPlayerBefore = true;
        }
        else if (hasSensedPlayerBefore)
        {
            if (context.agent.remainingDistance <= context.agent.stoppingDistance)
            {
                blackboard.seePlayer = false;
                blackboard.finalizedShearch = false;
                return State.Success;
            }
        }
        return State.Running;
    }
}
