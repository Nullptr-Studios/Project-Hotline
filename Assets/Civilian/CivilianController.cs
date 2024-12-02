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
    
    public SpriteRenderer SpriteRenderer;
    
    public List<Sprite> CivilianSprites;

    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        SceneMng.CivilianPanicDelegate += CivilianPanic;
        
        Sprite sprite = CivilianSprites[UnityEngine.Random.Range(0, CivilianSprites.Count - 1)];
        SpriteRenderer.sprite = sprite;
    }

    private void OnDisable()
    {
    }

    private void OnEnable()
    {
        if (_navMeshAgent != null && _navMeshAgent.enabled)
            _navMeshAgent.SetDestination(_exitNodePos);
    }

    void CivilianPanic()
    {
        _panic = true;
        _exitNodePos = SceneMng.ExitNodes[UnityEngine.Random.Range(0, SceneMng.ExitNodes.Count - 1)].transform.position;
        if (_navMeshAgent != null && _navMeshAgent.enabled)
            _navMeshAgent.SetDestination(_exitNodePos);
        _navMeshAgent.stoppingDistance = 0;
    }

    private void OnDestroy()
    {
        if (SceneMng.CivilianPanicDelegate != null)
            SceneMng.CivilianPanicDelegate -= CivilianPanic;
    }

    private void Update()
    {
        if (_panic)
        {
            float distance = Vector3.Distance(transform.position, _exitNodePos);
            if(distance < 1)
            {
                Destroy(gameObject);
            }
        }
    }
}
