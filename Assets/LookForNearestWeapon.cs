using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class LookForNearestWeapon : ActionNode
{

    private bool _justStarted = true;
    
    private GameObject _weapon;
    
    protected override void OnStart()
    {
        GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");
        
        //get all
        for (int i = 0; i < weapons.Length - 1; i++)
        {
            if(weapons[i] == null)
                continue;
            
            weapons[i].TryGetComponent(out IWeapon w);
            
            if(w == null)
                continue;

            if (w.isClaimed())
                weapons[i] = null;
        }
        
        float smallestDistance = float.MaxValue;
        int smallestIndex = 0;

        int j = 0;
            
        //check for the closest gun from the w
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
        }

        _weapon = weapons[smallestIndex];
        
        context.agent.SetDestination(_weapon.transform.position);

        context.agent.stoppingDistance = .3f;

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
