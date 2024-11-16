using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NavMeshPlus.Extensions;
using UnityEngine;
using TheKiwiCoder;
using Unity.VisualScripting;

public class LookForNearestWeapon : ActionNode
{

    private bool _justStarted = true;
    
    private EnemyWeaponManager _enemyWeaponManager;
    
    [CanBeNull] private GameObject _weapon;
    
    private bool _sucess = false;
    private bool _waitForDelegate = true;
    private bool _getOnce = true;
    
    private bool _chaseWeapon = false;
    
    private IWeapon _iWeapon;
    
        
    RotateAgentSmoothly rotateSmooth2D;
    
    private AgentOverride2d override2D;
    
    protected override void OnStart()
    {
        override2D = context.gameObject.GetComponent<AgentOverride2d>();

        rotateSmooth2D = new RotateAgentSmoothly(override2D.Agent, override2D, 180);
        
        override2D.agentOverride = rotateSmooth2D;
        
        _enemyWeaponManager = context.transform.GetComponent<EnemyWeaponManager>();

        _enemyWeaponManager.GotWeaponDelegate += GotWeaponMine;
        
        _getOnce = true;
        
        context.agent.angularSpeed = 120;
        blackboard.fallBack = false;
        
        GetNearest();
        
    }

    private void GetNearest()
    {
        _chaseWeapon = false;
        
        GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");
        
        weapons = GetClosestPaths(context.gameObject, weapons);
        
        if (weapons is null)
            return;
        
        _weapon = weapons[0];
        
        context.agent.speed = blackboard.chaseSpeed;
        
        if (_weapon is not null)
            context.agent.SetDestination(_weapon.transform.position);

        context.agent.stoppingDistance = .5f;
        
        _iWeapon = _weapon.GetComponent<IWeapon>();

        _getOnce = true;
        _chaseWeapon = true;
    }

    private void GotWeaponMine()
    {
        if (_enemyWeaponManager._isWeaponHeld)
        {
            _sucess = true;
        }
        else
        {
            _sucess = false;
        }
        
        _waitForDelegate = false;
            
    }

    private GameObject[] GetClosestPaths(GameObject go, GameObject[] weapons)
    {
        // just cache the position once
        var positionToTest = go.transform.position;

        int nullcount = 0;
        // Filter out invalid weapons
        for (int i = 0; i < weapons.Length; i++)
        {
            if (!weapons[i])
                continue;

            weapons[i].TryGetComponent(out IWeapon w);

            if (w == null || w.isClaimed())
            {
                weapons[i] = null;
                nullcount++;
            }
        }
        
        if(nullcount == weapons.Length)
            return null;

        // Go through the paths
        return weapons
            // Skip the "pathToTest" and null weapons
            .Where(p => p != go && p != null)
            // Order them by distance to "pathToTest"
            .OrderBy(p => (p.transform.position - positionToTest).sqrMagnitude)
            // Take only up to 1 item
            .Take(1)
            // finally store the results in an array
            .ToArray();
    }

    protected override void OnStop() 
    {
        
    }

    protected override State OnUpdate() 
    {
        
        if (_justStarted)
        {
            _justStarted = false;
            return State.Running;
        }

        if (_chaseWeapon)
        {
            context.agent.speed = blackboard.chaseSpeed;

            if (!_iWeapon.isClaimed())
            {
                // what the fuck is this C# syntax -x
                if (_weapon is not null)
                {
                    
                    context.agent.SetDestination(_weapon.transform.position);
                }
                else
                {
                    GetNearest();
                }
            }
            else
            {
                GetNearest();
            }
        }


        if (context.agent.remainingDistance <= context.agent.stoppingDistance)
        {
            if (_getOnce)
            {
                _enemyWeaponManager.OnlyGetWeapon();
                _getOnce = false;
            }

            if(!_waitForDelegate)
            {
                if (_sucess)
                {
                    context.agent.SetDestination(context.gameObject.GetComponent<AISensor>().GetPlayerPositionIfInBounds());
                    return State.Success;
                }
                else
                {
                    GetNearest();
                }

                _getOnce = true;
                _waitForDelegate = true;
            }
            

        }

        return State.Running;
    }
}
