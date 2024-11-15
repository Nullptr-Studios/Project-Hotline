using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class GotoHearingLocation : ActionNode
{
    private bool _justStarted = true;

    protected override void OnStart() 
    {
        context.agent.speed = blackboard.idleSpeed;
        
        blackboard.heardPos.x = Random.Range(0, 2) + blackboard.heardPos.x;
        blackboard.heardPos.y = Random.Range(0, 2) + blackboard.heardPos.y;
        context.agent.SetDestination(blackboard.heardPos);
        
        _justStarted = true;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        
        if (_justStarted)
        {
            _justStarted = false;
            return State.Running;
        }

        if (context.agent.remainingDistance <= 2)
        {
            blackboard.finalizedShearch = false;
            return State.Success;
        }

        return State.Running;
    }
}
