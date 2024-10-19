using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class SensePlayer : ActionNode
{
    
    private AISensor sensor;
    private EnemyWeaponManager WeaponManager;

    private bool usingWeapon = false;
    
    private bool hasSensedPlayerBefore = false;

    private long detectingFrames = 0;
    
    protected override void OnStart() 
    {
        sensor = context.gameObject.GetComponent<AISensor>();
        WeaponManager = context.gameObject.GetComponent<EnemyWeaponManager>();
        
        hasSensedPlayerBefore = false;
        detectingFrames = 0;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (sensor.isDetecting)
        {
            if(sensor.detectedPlayer)
                blackboard.playerPos = sensor.detectedPlayer.transform.position;
            
            blackboard.seePlayer = true;
            hasSensedPlayerBefore = true;
            
            context.agent.stoppingDistance = blackboard.distanceToUseWeapon;
            
            context.agent.SetDestination(blackboard.playerPos);
            
            
            if (context.agent.remainingDistance <= context.agent.stoppingDistance && detectingFrames != 0)
            {
                //@TODO: Do this with slerp
                Vector3 targetPos = blackboard.playerPos;
                Vector3 thisPos = context.transform.position;
                targetPos.x = targetPos.x - thisPos.x;
                targetPos.y = targetPos.y - thisPos.y;
                float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
                context.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                
                if (!usingWeapon)
                {
                    WeaponManager.useWeapon(true);
                    usingWeapon = true;
                }
            }
            else
            {
                if (usingWeapon)
                {
                    WeaponManager.useWeapon(false);
                    usingWeapon = false;
                }
            }
            
            detectingFrames++;
        }
        else if (hasSensedPlayerBefore)
        {
            context.agent.stoppingDistance = 2f;
            
            if (usingWeapon)
            {
                WeaponManager.useWeapon(false);
                usingWeapon = false;
            }
            
            if (context.agent.remainingDistance <= context.agent.stoppingDistance)
            {
                blackboard.seePlayer = false;
                blackboard.finalizedShearch = false;
                return State.Success;
            }

            detectingFrames = 0;
        }
        return State.Running;
    }
}
