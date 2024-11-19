using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class HearingDecorator : DecoratorNode
{
    private AISensor _aiSensor;
    protected override void OnStart() 
    {
        _aiSensor = context.gameObject.GetComponent<AISensor>();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() 
    {
        if(_aiSensor.heardPlayer)
        {
            blackboard.heardPos = _aiSensor.heardPosition;
            blackboard.heardPlayer = true;
            if (child.Update() == State.Success)
            {
                _aiSensor.heardPlayer = false;
                blackboard.heardPlayer = false;
            }
        }
        return State.Running;
    }
}
