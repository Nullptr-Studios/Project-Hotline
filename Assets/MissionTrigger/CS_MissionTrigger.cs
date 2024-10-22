using System;
using UnityEngine;

public class MissionTrigger : MonoBehaviour
{
    [NonSerialized] public GameObject Objective;

    private void Start()
    {
        // This might need to change depending on the collider type -x
        // I want a function that searches for all collider types -x
        //_objectiveCollider = Objective.GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            Objective.GetComponent<Collider2D>().enabled = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            Objective.GetComponent<Collider2D>().enabled = false;
    }
}
