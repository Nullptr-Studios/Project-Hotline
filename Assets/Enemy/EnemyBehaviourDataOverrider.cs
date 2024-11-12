using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Overrides the behavior data for an enemy, including handling animations and movement.
/// </summary>
public class EnemyBehaviourDataOverrider : MonoBehaviour
{
    /// <summary>
    /// The behavior data for the enemy.
    /// </summary>
    public EnemyBehaviourData behaviourData;

    /// <summary>
    /// Specifies if the enemy is initially moving or not.
    /// </summary>
    public bool isActive = true;

    /// <summary>
    /// Toggles the active state of the enemy.
    /// </summary>
    public void ToggleActive()
    {
        isActive = !isActive;
    }

    /// <summary>
    /// Indicates if the enemy was just stunned.
    /// </summary>
    public bool justStunned = false;

    /// <summary>
    /// The animator for the enemy.
    /// </summary>
    public Animator animatorEnemy;

    /// <summary>
    /// The animator for the enemy's foot.
    /// </summary>
    public Animator animatorEnemyFoot;

    /// <summary>
    /// The transform for the enemy's foot.
    /// </summary>
    public Transform footTransform;

    private NavMeshAgent _rb;

    private static readonly int IsIdle = Animator.StringToHash("IsIdle");

    /// <summary>
    /// Initializes the NavMeshAgent component.
    /// </summary>
    private void Start()
    {
        _rb = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Updates the enemy's animations based on its velocity.
    /// </summary>
    private void Update()
    {
        // Animation logic based on the enemy's velocity
        if (_rb.velocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
            footTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            animatorEnemyFoot.enabled = true;
            animatorEnemy.SetBool(IsIdle, false);
        }
        else
        {
            animatorEnemyFoot.enabled = false;
            animatorEnemy.SetBool(IsIdle, true);
        }
    }

#if UNITY_EDITOR
    [Header("Debug")]

    [SerializeField] private bool DrawGizmos = true;
    [SerializeField] private Color ColorSphere = Color.magenta;
    [SerializeField] private Color ColorSphereTurn = Color.green;
    [SerializeField] private Color Colorline = Color.yellow;

    /// <summary>
    /// Draws gizmos in the editor for debugging purposes.
    /// </summary>
    public void OnDrawGizmos()
    {
        if(DrawGizmos)
        {
            // Visual aid for waypoints
            if (behaviourData.waypoints.Count > 0)
            {
                Vector2 lastPos = transform.position;
                foreach (var waypoint in behaviourData.waypoints)
                {
                    if(waypoint.letItRotate)
                        Gizmos.color = ColorSphereTurn;
                    else
                        Gizmos.color = ColorSphere;

                    Gizmos.DrawSphere(waypoint.waypointPos, .5f);

                    Gizmos.color = Colorline;
                    Gizmos.DrawLine(lastPos, waypoint.waypointPos);

                    lastPos = waypoint.waypointPos;
                }
                // Draw the last line
                Gizmos.DrawLine(lastPos, behaviourData.waypoints[0].waypointPos);
            }
        }
    }

#endif
}