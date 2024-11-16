using System;
using JetBrains.Annotations;
using UnityEngine;

public class MissionTrigger : MonoBehaviour
{
    [NonSerialized] [CanBeNull] public GameObject Objective;

    private void Start()
    {
        // This might need to change depending on the collider type -x
        // I want a function that searches for all collider types -x
        //_objectiveCollider = Objective.GetComponent<CircleCollider2D>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (Objective == null) return;
        
        if (collision.gameObject.CompareTag("Player"))
            Objective.GetComponent<Collider2D>().enabled = true;
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (Objective == null) return;
        
        if (collision.gameObject.CompareTag("Player"))
            Objective.GetComponent<Collider2D>().enabled = false;
    }
}
