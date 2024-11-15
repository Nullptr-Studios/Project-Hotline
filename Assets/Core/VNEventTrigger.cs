using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class VNEventTrigger : MonoBehaviour
{
    /// <summary>
    /// The UnityEvent to invoke when the trigger is activated.
    /// </summary>
    public UnityEvent TriggerActions;

    private PlayerIA _playerIA;

    private void Awake()
    {
        _playerIA = new PlayerIA();

        _playerIA.UI.Accept.performed += ActionPerformed;

    }

    private void OnDisable()
    {
        _playerIA.UI.Accept.performed -= ActionPerformed;
        _playerIA.UI.Accept.Disable();
    }

    private void ActionPerformed(InputAction.CallbackContext context)
    {
        TriggerActions.Invoke();
        this.enabled = false;
    }

    /// <summary>
    /// Called when another collider enters the trigger collider attached to this GameObject.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>s
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to a GameObject tagged "Player"
        if (other.CompareTag("Player"))
            _playerIA.UI.Accept.Enable();
            
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the collider belongs to a GameObject tagged "Player"
        if (other.CompareTag("Player"))
            _playerIA.UI.Accept.Disable();

    }
}
