using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    private BoxCollider2D _collider;
    [SerializeField] private GameObject elevatorMenu;
    
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        elevatorMenu.SetActive(true);
    }
}
