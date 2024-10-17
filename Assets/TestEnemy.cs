using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestEnemy : MonoBehaviour
{
    NavMeshAgent nva;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        nva = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        nva.SetDestination(player.transform.position);
    }
}
