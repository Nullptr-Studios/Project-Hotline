using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Represents a waypoint with position, rotation, and wait time.
/// </summary>
[Serializable]
public struct SWaypoints
{
    /// <summary>
    /// The position of the waypoint.
    /// </summary>
    public Vector2 waypointPos;

    /// <summary>
    /// Indicates if the entity should rotate at the waypoint.
    /// </summary>
    public bool letItRotate;

    /// <summary>
    /// The wait time at the waypoint.
    /// </summary>
    public float wait;
}

/// <summary>
/// ScriptableObject that holds data for enemy behavior.
/// </summary>
[CreateAssetMenu(fileName = "Enemy Behaviour Data", menuName = "ProjectHotline/Enemy Behaviour Data", order = 6)]
public class EnemyBehaviourData : ScriptableObject
{
    [Header("Idle")]
    /// <summary>
    /// Indicates if the enemy is static.
    /// </summary>
    public bool isStatic;

    /// <summary>
    /// Indicates if the enemy should return to the initial spot.
    /// </summary>
    public bool returnToInitialSpot;

    /// <summary>
    /// List of waypoints for the enemy to follow.
    /// </summary>
    public List<SWaypoints> waypoints;

    [Header("Movement")]
    /// <summary>
    /// The speed of the enemy when idle.
    /// </summary>
    public float idleSpeed = 2.5f;

    /// <summary>
    /// The speed of the enemy when chasing.
    /// </summary>
    public float chasingSpeed = 8.0f;

    [Header("Shooting")]
    /// <summary>
    /// The distance at which the enemy starts shooting.
    /// </summary>
    public float distanceToShoot = 10;

    /// <summary>
    /// The time it takes for the enemy to start shooting.
    /// </summary>
    public float timeToStartShooting = 1;

    [Header("Melee")]
    /// <summary>
    /// The distance at which the enemy performs melee attacks.
    /// </summary>
    public float meleeDistance = 3;

    [Header("Search")]
    /// <summary>
    /// The number of times the enemy will search for the player.
    /// </summary>
    public int searchTimes = 3;
}