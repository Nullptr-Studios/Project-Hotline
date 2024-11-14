using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CivilianController : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Vector2 _exitNodePos;
    
    private bool _panic = false;
    private int frames = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        SceneMng.CivilianPanicDelegate += CivilianPanic;
    }

    void CivilianPanic()
    {
        _panic = true;
        _exitNodePos = SceneMng.ExitNodes[UnityEngine.Random.Range(0, SceneMng.ExitNodes.Count - 1)].transform.position;
        _navMeshAgent.SetDestination(_exitNodePos);
    }

    private void OnDestroy()
    {
        SceneMng.CivilianPanicDelegate -= CivilianPanic;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_panic && frames < 2)
        {
            frames++;
            return;
        }
        
        if (_panic && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance) {
            Destroy(gameObject);
        }
    }
}
