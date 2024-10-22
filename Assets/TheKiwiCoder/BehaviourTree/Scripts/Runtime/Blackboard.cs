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

        public Vector3 initialPos;
        public Quaternion initialRotation;

        public bool seePlayer;
        public bool finalizedShearch = true;

        public bool doSlerp = false;

        [Header("Enemy behaviour")]
        public float distanceToUseWeapon = 10;
        public float timeToStartShooting = 1;

        public bool returnToInitialPos = true;
        public bool isStatic = true;

        public List<Vector2> waypoints;

        public int searchTimes = 3;
    }
}