using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Represents a door that can take damage and react accordingly.
/// Inherits from the Damageable class.
/// </summary>
public class DoorDamageable : Damageable
{
    public float hitForce = 100;
    private Rigidbody2D _doorRb;

    [Header("Sound")] 
    public EventReference doorSound;

    public float soundWait = 1;

    private bool delay = false;
    private float _timer = 0.0f;

    /// <summary>
    /// Initializes the DoorDamageable instance.
    /// </summary>
    public override void Start()
    {
        base.Start();
        _doorRb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Applies damage to the door and adds force to simulate the door opening.
    /// </summary>
    /// <param name="amount">The amount of damage to apply.</param>
    /// <param name="shootDir">The direction of the shot.</param>
    /// <param name="hitPoint">The point where the shot hit.</param>
    /// <param name="weaponType">The type of weapon used.</param>
    public override void DoDamage(float amount, Vector3 shootDir, Vector3 hitPoint, EWeaponType weaponType)
    {
        // This is done so the door opens when hit
        _doorRb.AddForce(shootDir * hitForce);
        base.DoDamage(amount, shootDir, hitPoint, weaponType);
    }

    public void Update()
    {
        if(_doorRb.velocity.magnitude > 0.5f && !delay)
        {
            delay = true;
            FMODUnity.RuntimeManager.PlayOneShot(doorSound, transform.position);
        }
        
        if(delay)
        {
            if(_timer >= soundWait)
            {
                if (Mathf.Approximately(_doorRb.velocity.magnitude, 0))
                {
                    delay = false; 
                    _timer = 0.0f;
                }
            }
            else
            {            
                _timer += Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Handles the logic when the door is destroyed.
    /// </summary>
    public override void OnDead()
    {
        // @TODO: Add animation and effects
        Destroy(transform.parent.gameObject);
    }
}