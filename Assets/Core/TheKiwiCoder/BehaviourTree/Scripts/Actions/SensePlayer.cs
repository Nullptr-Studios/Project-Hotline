using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using Unity.Mathematics;

public class SensePlayer : ActionNode
{

    private EnemyBehaviourDataOverrider _ov;
    
    private AISensor _sensor;
    private EnemyWeaponManager _weaponManager;

    private bool _usingWeapon = false;
    
    private bool _hasSensedPlayerBefore = false;

    private long _detectingFrames = 0;

    private float _shootTimer = 0.0f;
    
    protected override void OnStart() 
    {
        _sensor = context.gameObject.GetComponent<AISensor>();
        _weaponManager = context.gameObject.GetComponent<EnemyWeaponManager>();

        _ov = context.gameObject.GetComponent<EnemyBehaviourDataOverrider>();
        
        _hasSensedPlayerBefore = false;
        _detectingFrames = 0;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (!_ov.justStunned)
        {
            if (_sensor.isDetecting)
            {

                if (_sensor.detectedPlayer)
                    blackboard.playerPos = _sensor.detectedPlayer.transform.position;

                blackboard.seePlayer = true;
                _hasSensedPlayerBefore = true;

                context.agent.speed = blackboard.chaseSpeed;

                context.agent.SetDestination(blackboard.playerPos);

                
                context.agent.stoppingDistance = _weaponManager.IsMelee() ? blackboard.distanceToUseMelee : blackboard.distanceToUseWeapon;


                if (context.agent.remainingDistance <= context.agent.stoppingDistance && _detectingFrames != 0)
                {

                    Vector3 targetPos = blackboard.playerPos;
                    Vector3 thisPos = context.transform.position;
                    targetPos.x -= thisPos.x;
                    targetPos.y -= thisPos.y;
                    float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
                    Quaternion look = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                    context.transform.rotation = Quaternion.Slerp(context.transform.rotation, look, 3 * Time.deltaTime);

                    if (_shootTimer > blackboard.timeToStartShooting)
                    {
                        if (!_usingWeapon)
                        {
                            _weaponManager.useWeapon(true);
                            _usingWeapon = true;
                        }
                    }
                    else
                    {
                        _shootTimer += Time.deltaTime;
                    }


                }
                else
                {
                    if (_usingWeapon)
                    {
                        _weaponManager.useWeapon(false);
                        _usingWeapon = false;
                    }

                    _shootTimer = 0;
                }

                _detectingFrames++;
            }
            else if (_hasSensedPlayerBefore)
            {
                context.agent.stoppingDistance = 2f;

                _shootTimer = 0;

                if (_usingWeapon)
                {
                    _weaponManager.useWeapon(false);
                    _usingWeapon = false;
                }

                if (context.agent.remainingDistance <= context.agent.stoppingDistance)
                {
                    blackboard.seePlayer = false;
                    blackboard.finalizedShearch = false;
                    return State.Success;
                }

                _detectingFrames = 0;
            }
        }

        return State.Running;
    }
}
