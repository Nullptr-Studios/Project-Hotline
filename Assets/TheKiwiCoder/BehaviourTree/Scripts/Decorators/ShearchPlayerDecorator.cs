using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class ConditionalSeenPlayer : DecoratorNode
{

    public int repeatTimes = 2;
    
    [SerializeField] private int currentRepeatTimes = 0;
    protected override void OnStart() 
    {
        currentRepeatTimes = repeatTimes;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (!blackboard.seePlayer && currentRepeatTimes > 0 && !blackboard.finalizedShearch)
        {
            switch (child.Update())
            {
                case State.Running:
                    break;
                case State.Failure:
                    return State.Failure;
                case State.Success:
                    currentRepeatTimes--;
                    return State.Running;
            }

        }
        else if(!blackboard.finalizedShearch)
        {
            child.Abort();
            blackboard.finalizedShearch = true;
            currentRepeatTimes = repeatTimes;
        }
        return State.Running;
    }
}
