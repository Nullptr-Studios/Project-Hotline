using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct SWaypoints
{
    public Vector2 waypointPos;
    public bool letItRotate;
    public float wait;
}

[CreateAssetMenu(fileName = "Enemy Behaviour Data", menuName = "ProjectHotline/Enemy Behaviour Data", order = 6)]
public class EnemyBehaviourData : ScriptableObject
{
    [Header("Idle")] 
    public bool isStatic;
    public bool returnToInitialSpot;
    
    public List<SWaypoints> waypoints;

    [Header("Movement")] 
    public float idleSpeed = 2.5f;
    public float chasingSpeed = 8.0f;
    
    [Header("Shooting")]
    public float distanceToShoot = 10;
    public float timeToStartShooting = 1;

    [Header("Melee")] 
    public float meleeDistance = 2;

    [Header("Search")]
    public int searchTimes = 3;
}
