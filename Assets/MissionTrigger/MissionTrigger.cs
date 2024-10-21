using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class MissionTrigger : MonoBehaviour
{
    private PlayerIA _input;
    private void Start()
    {
        _input = new PlayerIA();
        _input.Gameplay.Interact.performed += GetEndMission;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _input.Gameplay.Interact.Enable();
        }
    }

    private void GetEndMission(InputAction.CallbackContext context)
    {
        UnityEngine.Debug.Log("Mission Complete");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _input.Gameplay.Interact.Disable();

    }
}
