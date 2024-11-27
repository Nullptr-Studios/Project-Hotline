using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class IdleLogic : ActionNode
{
    public int currentWaypointIndex = 0;
    
    protected override void OnStart() 
    {
        blackboard.fallBack = false;
        
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
            
            blackboard.moveToPosition = blackboard.waypoints[currentWaypointIndex].waypointPos;

            if (blackboard.waypoints[currentWaypointIndex].letItRotate)
            {
                blackboard.doSlerp = true;

                int nextIndex = currentWaypointIndex + 1;
                if (nextIndex > blackboard.waypoints.Count - 1)
                    nextIndex = 0;
                
                Vector3 dir = blackboard.waypoints[nextIndex].waypointPos -
                              blackboard.waypoints[currentWaypointIndex].waypointPos;

                Quaternion rotateTo = Quaternion.FromToRotation(Vector2.up, dir.normalized);

                blackboard.moveToRotation = rotateTo;
            }

            blackboard.waitTime = blackboard.waypoints[currentWaypointIndex].wait;

            currentWaypointIndex++;
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
