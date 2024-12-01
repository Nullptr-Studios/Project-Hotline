using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A component that triggers UnityEvents when a GameObject with the "Player" tag enters its collider.
/// </summary>
public class TriggerEvent : MonoBehaviour
{
    /// <summary>
    /// The UnityEvent to invoke when the trigger is activated.
    /// </summary>
    public UnityEvent TriggerActions;
    public bool OnlyOnce;
    private bool _triggered = false;

    /// <summary>
    /// Called when another collider enters the trigger collider attached to this GameObject.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>s
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (OnlyOnce && _triggered) return;
        _triggered = true;
        
        // Check if the collider belongs to a GameObject tagged "Player"
        if(other.CompareTag("Player"))
            // Invoke the assigned UnityEvent
            TriggerActions?.Invoke();
    }
}