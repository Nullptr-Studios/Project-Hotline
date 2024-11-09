using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a door that can take damage and react accordingly.
/// Inherits from the Damageable class.
/// </summary>
public class DoorDamageable : Damageable
{
    private Rigidbody2D _doorRb;

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
        _doorRb.AddForce(shootDir * 50);
        base.DoDamage(amount, shootDir, hitPoint, weaponType);
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