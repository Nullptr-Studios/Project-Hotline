using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using Unity.Mathematics;
using UnityEditor.Timeline.Actions;

public class SlerpToRotation : ActionNode
{

    public float timeout = 1;

    private float _timer = 0.0f;
    protected override void OnStart()
    {
        _timer = 0.0f;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() 
    {
        if (_timer >= timeout)
        {
            return State.Success;
        }
        else
        {
            _timer += Time.deltaTime;
        }
        
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
