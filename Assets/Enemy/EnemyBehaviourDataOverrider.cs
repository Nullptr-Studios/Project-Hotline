using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviourDataOverrider : MonoBehaviour
{
    public EnemyBehaviourData behaviourData;

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
