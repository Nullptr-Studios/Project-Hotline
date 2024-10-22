using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Behaviour Data", menuName = "ProjectHotline/Enemy Behaviour Data", order = 6)]
public class EnemyBehaviourData : ScriptableObject
{
    [Header("Idle")] 
    public bool isStatic;
    public bool returnToInitialSpot;
    
    public List<Vector2> waypoints;
    
    [Header("Shooting")]
    public float distanceToShoot = 10;
    public float timeToStartShooting = 1;

    [Header("Search")]
    public int searchTimes = 3;
}
