using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This is the main AI enemy class
/// </summary>
///
/// 1º Check for player in sight
///     if Player is in sight act accordingly with the type of weapon equiped
/// if player not seen wander or patrol
/// repeat
/// 
public class EnemyLogic : MonoBehaviour
{
    
    private AISensor _sensor;
    private NavMeshAgent _agent;
    
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _sensor = GetComponent<AISensor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_sensor || !_agent)
            return;
            
        if (_sensor.isDetecting)
        { 
            _agent.destination = _sensor.detectedObjects[0].transform.position;
        }
        else
        {
            
        }
    }


    private void SearchLastPlayerPos(Vector2 lastPlayerPos)
    {
        
    }
    
    
    
    //################## Utilities ####################//
    private Vector2 GetRandomPointOnNavMesh(float maxDistance)
    {
        return new Vector2(Random.Range(-maxDistance, maxDistance), Random.Range(-maxDistance, maxDistance));
    }
}
