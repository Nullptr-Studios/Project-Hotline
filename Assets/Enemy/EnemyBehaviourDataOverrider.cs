using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviourDataOverrider : MonoBehaviour
{
    public EnemyBehaviourData behaviourData;

    public bool justStunned = false;

    public Animator animatorEnemy;
    public Animator animatorEnemyFoot;
    public Transform footTransform;

    private NavMeshAgent _rb;
    
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");

    private void Start()
    {
        _rb = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Animation stuff

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
    
    public void OnDrawGizmos()
    {
        if(DrawGizmos)
            //Added visual help for waypoints
            if (behaviourData.waypoints.Count > 0)
            {

                Vector2 lastPos = transform.position;
                foreach (var waypoint in behaviourData.waypoints)
                {
                    if(waypoint.letItRotate)
                        Gizmos.color = ColorSphereTurn;
                    else
                        Gizmos.color = ColorSphere;
                    
                    Gizmos.DrawSphere(waypoint.waypointPos,.5f);
                    
                    Gizmos.color = Colorline;
                    Gizmos.DrawLine(lastPos, waypoint.waypointPos);
                    
                    lastPos = waypoint.waypointPos;
                }
                //last line
                Gizmos.DrawLine(lastPos, behaviourData.waypoints[0].waypointPos);
            }
    }
    
#endif
}
