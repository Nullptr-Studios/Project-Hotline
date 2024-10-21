using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class IdleLogic : ActionNode
{
    public int currentWaypointIndex = 0;
    
    protected override void OnStart() 
    {
        if (blackboard.returnToInitialPos)
        {
            blackboard.moveToPosition = blackboard.initialPos;
            blackboard.moveToRotation = blackboard.initialRotation;

            blackboard.doSlerp = blackboard.isStatic;
        }

        if (!blackboard.isStatic)
        {
            if (currentWaypointIndex > blackboard.waypoints.Count - 1)
                currentWaypointIndex = 0;
            
            blackboard.moveToPosition = blackboard.waypoints[currentWaypointIndex++];
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
