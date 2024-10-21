using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using Unity.Mathematics;

public class SlerpToRotation : ActionNode
{
    protected override void OnStart() 
    {
        
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() 
    {
        if (blackboard.doSlerp)
        {
            if (Mathf.Approximately(Mathf.Floor(context.transform.rotation.eulerAngles.z),
                    Mathf.Floor(blackboard.moveToRotation.eulerAngles.z)))
            {
                blackboard.doSlerp = false;
                return State.Success;
            }

            context.transform.rotation =
                Quaternion.Slerp(context.transform.rotation, blackboard.moveToRotation, 3 * Time.deltaTime);
            return State.Running;
        }
        else
        {
            return State.Success;
        }
    }
}
