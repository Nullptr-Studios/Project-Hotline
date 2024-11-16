using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using TheKiwiCoder;
using Unity.VisualScripting;

public class LookForNearestWeapon : ActionNode
{

    private bool _justStarted = true;
    
    [CanBeNull] private GameObject _weapon;
    
    protected override void OnStart()
    {
        GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");
        
        //get all
        for (int i = 0; i < weapons.Length; i++)
        {
            if(!weapons[i])
                continue;
            
            weapons[i].TryGetComponent(out IWeapon w);
            
            if(w == null)
                continue;

            if (w.isClaimed())
                weapons[i] = null;
        }
            
        /*//check for the closest gun from the w
        foreach (var h in weapons)
        {
            //null check
            if(!h)
                continue;
            h.TryGetComponent(out IWeapon w);
            
            if(w.isClaimed())
                continue;
                
            float currentDist = Vector2.Distance(h.transform.position, context.transform.position);
            if (currentDist < smallestDistance)
            {
                smallestDistance = currentDist;
                smallestIndex = j;
                j++;
            }
        }*/

        //_weapon = weapons[smallestIndex];
        
        _weapon = GetClosestPaths(context.gameObject, weapons)[0];

        context.agent.speed = blackboard.chaseSpeed;
        
        if (_weapon is not null)
            context.agent.SetDestination(_weapon.transform.position);

        context.agent.stoppingDistance = .5f;

    }
    
    private GameObject[] GetClosestPaths (GameObject go,GameObject[] weapons)
    {
        // just cache the position once
        var positionToTest = go.transform.position;
        
        // Go through the paths
        return weapons
            // Skip the "pathToTest"
            .Where(p => p != go && p)
            // Order them by distance to "pathToTest"
            .OrderBy(p => (p.transform.position - positionToTest).sqrMagnitude)
            // Take only up to 2 items
            .Take(1)
            // finally store the results in an array
            .ToArray();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() 
    {
        if (_justStarted)
        {
            _justStarted = false;
            return State.Running;
        }
        
        if (_weapon is not null) // what the fuck is this C# syntax -x
            context.agent.SetDestination(_weapon.transform.position);
        
        
        if (context.agent.remainingDistance <= context.agent.stoppingDistance)
        {
            context.transform.GetComponent<EnemyWeaponManager>()._wantsToThrowOrGet = true;

            context.agent.SetDestination(blackboard.playerPos);

            return State.Success;
        }

        return State.Running;
    }
}
