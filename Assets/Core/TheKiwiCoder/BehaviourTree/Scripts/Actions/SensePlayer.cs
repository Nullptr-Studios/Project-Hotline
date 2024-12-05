using System.Collections.Generic;
using NavMeshPlus.Extensions;
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
    private float _rotationSpeed = 5.0f;
    private Vector3 _currentDir = Vector3.up;
    
    RotateAgentSmoothly rotateSmooth2D;
    
    private AgentOverride2d override2D;

    protected override void OnStart()
    {
        override2D = context.gameObject.GetComponent<AgentOverride2d>();

        rotateSmooth2D = new RotateAgentSmoothly(override2D.Agent, override2D, 180);
        
        _sensor = context.gameObject.GetComponent<AISensor>();
        _weaponManager = context.gameObject.GetComponent<EnemyWeaponManager>();
        _ov = context.gameObject.GetComponent<EnemyBehaviourDataOverrider>();
        _hasSensedPlayerBefore = false;
        _detectingFrames = 0;
        
        override2D.agentOverride = rotateSmooth2D;
    }

    protected override void OnStop()
    {
        context.agent.angularSpeed = 120;
        blackboard.fallBack = false;
    }

    protected override State OnUpdate()
    {
        _currentDir = context.transform.up;
        if (_ov == null || _ov.justStunned) return State.Running;

        if (_sensor != null)
        {
            if (_sensor.isDetecting)
            {
                HandleDetection();
            }
            else if(blackboard.fallBack)
            {
                HandleDetection();
            }
            else if (_hasSensedPlayerBefore)
            {
                return HandleLostDetection();
            }

            /*else if(!blackboard.isStatic/*Mathf.Approximately(context.agent.velocity.sqrMagnitude, 0))
            {
                RotateTowardsVelocity();
            }*/
            
        }

        return State.Running;
    }

    private void HandleDetection()
    {
        override2D.agentOverride = null;
        _currentDir = context.transform.up;
        
        if (_sensor.fallbackPosition != Vector3.zero && !_weaponManager.IsMelee())
        {
            StartFallback();
        }
        else if (_sensor.detectedPlayer && !blackboard.fallBack)
        {
            StartChase();
        }

        UpdateAgent();
        RotateTowardsPlayer();
        HandleShooting();
        _detectingFrames++;
    }

    private State HandleLostDetection()
    {
        override2D.agentOverride = rotateSmooth2D;

        context.agent.stoppingDistance = 2f;
        _shootTimer = 0;
        StopUsingWeapon();

        if (context.agent.remainingDistance <= context.agent.stoppingDistance)
        {
            blackboard.seePlayer = false;
            blackboard.finalizedShearch = false;
            return State.Success;
        }

        _detectingFrames = 0;
        return State.Running;
    }

    private void StartFallback()
    {
        blackboard.playerPos = _sensor.fallbackPosition;
        context.agent.stoppingDistance = 0.5f;
        context.agent.angularSpeed = 0;
        blackboard.fallBack = true;
    }

    private void StartChase()
    {
        blackboard.playerPos = _sensor.detectedPlayer.transform.position;
        context.agent.angularSpeed = 120;
    }

    private void UpdateAgent()
    {
        _currentDir = context.transform.up;
        
        blackboard.seePlayer = true;
        _sensor.heardPlayer = false;
        _hasSensedPlayerBefore = true;
        context.agent.speed = blackboard.chaseSpeed;
        context.agent.SetDestination(blackboard.playerPos);

        if (!blackboard.fallBack)
        {
            context.agent.stoppingDistance = _weaponManager.IsMelee() ? blackboard.distanceToUseMelee : blackboard.distanceToUseWeapon;
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = (_sensor.GetPlayerPositionIfInBounds() - context.transform.position).normalized;

        #if UNITY_EDITOR
        Debug.DrawRay(context.transform.position, directionToPlayer * 4, Color.red);
        Debug.DrawRay(context.transform.position, context.transform.up * 3, Color.green);
        #endif
        _currentDir = Vector3.Slerp(_currentDir, directionToPlayer, Time.deltaTime * 10f);
        float angle = Mathf.Atan2(_currentDir.y, _currentDir.x) * Mathf.Rad2Deg;
        context.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }
    
    private void RotateTowardsVelocity()
    {
        Vector3 direction = context.agent.velocity.normalized;

        _currentDir = Vector3.Slerp(_currentDir, direction, Time.deltaTime * 5f);
        float angle = Mathf.Atan2(_currentDir.y, _currentDir.x) * Mathf.Rad2Deg;
        context.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    private void HandleShooting()
    {
        float distance = Vector3.Distance(!blackboard.fallBack ? blackboard.playerPos : context.agent.pathEndPosition, context.transform.position);
        
        if (distance <= context.agent.stoppingDistance && _detectingFrames != 0)
        {
            if(blackboard.fallBack)
            {
                blackboard.fallBack = false;
                context.agent.stoppingDistance = 0;
                context.agent.angularSpeed = 120;
                blackboard.playerPos = _sensor.GetPlayerPositionIfInBounds();
                return;
            }
            /*blackboard.fallBack = false;
            context.agent.stoppingDistance = 0;
            context.agent.angularSpeed = 120;
            blackboard.playerPos = _sensor.detectedPlayer.transform.position;*/

            if (_shootTimer > blackboard.timeToStartShooting)
            {
                StartUsingWeapon();
            }
            else
            {
                _shootTimer += Time.deltaTime;
            }
        }
        else
        {
            StopUsingWeapon();
            _shootTimer = 0;
            _currentDir = context.transform.up;
        }
    }

    private void StartUsingWeapon()
    {
        if (!_usingWeapon)
        {
            _weaponManager.useWeapon(true);
            _usingWeapon = true;
        }
    }

    private void StopUsingWeapon()
    {
        if (_usingWeapon)
        {
            _weaponManager.useWeapon(false);
            _usingWeapon = false;
        }
    }
}