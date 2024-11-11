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

    /// <summary>
    /// Called when another collider enters the trigger collider attached to this GameObject.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to a GameObject tagged "Player"
        if(other.CompareTag("Player"))
            // Invoke the assigned UnityEvent
            TriggerActions?.Invoke();
    }

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool drawGizmos = false;

    /// <summary>
    /// Draws debug gizmos in the editor.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
#endif
}