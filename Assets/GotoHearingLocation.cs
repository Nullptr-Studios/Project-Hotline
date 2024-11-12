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

        if (context.agent.remainingDistance <= context.agent.stoppingDistance)
        {
            blackboard.finalizedShearch = false;
            return State.Success;
        }

        return State.Running;
    }
}
