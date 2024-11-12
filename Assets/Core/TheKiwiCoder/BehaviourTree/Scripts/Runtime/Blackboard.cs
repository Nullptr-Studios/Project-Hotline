using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {

    // This is the blackboard container shared between all nodes.
    // Use this to store temporary data that multiple nodes need read and write access to.
    // Add other properties here that make sense for your specific use case.
    [System.Serializable]
    public class Blackboard
    {
        public Vector3 moveToPosition;
        public Quaternion moveToRotation;
        public Vector3 playerPos;

        public bool heardPlayer;
        public Vector3 heardPos;

        public Vector3 initialPos;
        public Quaternion initialRotation;

        public bool seePlayer;
        public bool finalizedShearch = true;

        public bool doSlerp = false;

        public float waitTime = .5f;

        [Header("Enemy behaviour")]
        public float distanceToUseWeapon = 10;
        public float distanceToUseMelee = 1.5f;
        public float timeToStartShooting = 1;

        public bool returnToInitialPos = true;
        public bool isStatic = true;

        public float chaseSpeed = 8;
        public float idleSpeed = 2.5f;

        public List<SWaypoints> waypoints;

        public int searchTimes = 3;
    }
}