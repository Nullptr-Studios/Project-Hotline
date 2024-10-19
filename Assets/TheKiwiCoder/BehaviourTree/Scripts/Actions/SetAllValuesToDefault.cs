using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class SetAllValuesToDefault : DecoratorNode
{
    private bool done = false;
    protected override void OnStart()
    {
        if (!done)
        {
            blackboard.finalizedShearch = true;
            blackboard.seePlayer = false;

            blackboard.playerPos = new Vector3();
            blackboard.moveToPosition = new Vector3();
            done = true;
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
