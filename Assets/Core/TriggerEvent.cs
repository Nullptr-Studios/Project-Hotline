using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// A component that triggers UnityEvents when a GameObject with the "Player" tag enters its collider.
/// </summary>
public class TriggerEvent : MonoBehaviour
{
    /// <summary>
    /// The UnityEvent to invoke when the trigger is activated.
    /// </summary>
    [FormerlySerializedAs("TriggerActions")] public UnityEvent TriggerEnterAction;
    public UnityEvent TriggerExitAction;
    [FormerlySerializedAs("OnlyOnce")]  public bool onlyOnce;
    private bool _triggered = false;

    private void Start()
    {
        // i hate unity apparently if you have the same game object in two scenes it is not destroyed for optimization -x
        // hours wasted on this bug: 5h -x
        _triggered = false;
    }

    private void OnDisable()
    {
        _triggered = false;
    }

    /// <summary>
    /// Called when another collider enters the trigger collider attached to this GameObject.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (onlyOnce && _triggered) 
            return;
        
        
        // Check if the collider belongs to a GameObject tagged "Player"
        if (!other.CompareTag("Player")) return;
        _triggered = true;
        // Invoke the assigned UnityEvent
        TriggerEnterAction?.Invoke();
        
        if(onlyOnce)
            Destroy(gameObject);
    }
    // This is not a joke i will FUCKING kill you if i see AI generated comments on my project 😊🔪 -x
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (onlyOnce && _triggered) 
            return;
        
        // Check if the collider belongs to a GameObject tagged "Player"
        if (!other.CompareTag("Player")) return;
        _triggered = true;

        
        // Invoke the assigned UnityEvent
        TriggerExitAction?.Invoke();
    }
}