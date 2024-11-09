using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Represents a melee weapon with functionalities such as attacking and applying effects.
/// Inherits from the Weapon class.
/// </summary>
public class MeleeWeapon : Weapon
{
    public MeleeWeaponData meleeWeaponData;
    public float offset;
    private bool _wantsToAttack;
    private bool _canAttack = true;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool log;
    [SerializeField] private bool drawGizmos;
#endif

    /// <summary>
    /// Initializes the weapon type.
    /// </summary>
    private void Awake()
    {
        weaponType = EWeaponType.Melee;
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    protected override void Start()
    {
        base.Start();

#if UNITY_EDITOR
        if (!meleeWeaponData)
        {
            if(log)
                Debug.LogError("MeleeWeapon Error: " + gameObject.name + " does not have MeleeWeaponData assigned");
        }
#endif
    }

    /// <summary>
    /// Gets the weapon sprite ID.
    /// </summary>
    /// <returns>The sprite ID of the weapon.</returns>
    public override int GetWeaponSpriteID()
    {
        return meleeWeaponData.SpriteID;
    }

    /// <summary>
    /// Handles the use functionality of the weapon.
    /// </summary>
    /// <param name="pressed">Indicates if the use button is pressed.</param>
    public override void Use(bool pressed)
    {
        _wantsToAttack = pressed;
    }

    /// <summary>
    /// Updates the can attack state. Called via Invoke.
    /// </summary>
    private void UpdateCanAttack()
    {
        _canAttack = true;
    }

    /// <summary>
    /// Performs the attack action.
    /// </summary>
    private void Attack()
    {
        var hitArr = new RaycastHit2D[32];
        var cf2D = new ContactFilter2D();

        int hitNumber = Physics2D.CapsuleCast(transform.position + (offset * transform.right),
            new Vector2(meleeWeaponData.hitboxWith, meleeWeaponData.range),
            CapsuleDirection2D.Horizontal, 0, transform.right, cf2D, hitArr, meleeWeaponData.range);

#if UNITY_EDITOR
        if (log)
        {
            Debug.Log("Amount of hits:" + hitNumber);
        }
#endif

        for (int i = 0; i < hitNumber; i++)
        {
            if (hitArr[i].transform.TryGetComponent(out IDamageable damageableInterface))
            {
                // Try to do damage
                damageableInterface.DoDamage(meleeWeaponData.damage, transform.right, hitArr[i].point, weaponType);
            }
        }

        // Play sound
        FMODUnity.RuntimeManager.PlayOneShot(meleeWeaponData.useSound, transform.position);
    }

    /// <summary>
    /// Handles the pickup functionality of the weapon.
    /// </summary>
    /// <param name="weaponHolder">The transform of the weapon holder.</param>
    public override void Pickup(Transform weaponHolder)
    {
        GetComponent<SpriteRenderer>().enabled = false;
        base.Pickup(weaponHolder);
    }

    /// <summary>
    /// Handles the throw functionality of the weapon.
    /// </summary>
    /// <param name="forwardVector">The forward vector for the throw.</param>
    public override void Throw(Vector2 forwardVector)
    {
        GetComponent<SpriteRenderer>().enabled = true;
        base.Throw(forwardVector);
    }

    /// <summary>
    /// Handles the drop functionality of the weapon.
    /// </summary>
    public override void Drop()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        base.Drop();
    }

    /// <summary>
    /// Updates the weapon state.
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (!_wantsToAttack) return;
        if (!_canAttack) return;

        _canAttack = false;
        Invoke(nameof(UpdateCanAttack), meleeWeaponData.cooldownTime);
        Attack();
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draws gizmos for debugging purposes.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if (drawGizmos)
            Gizmos.DrawWireCube(transform.position + (offset * transform.right),
            new Vector3(meleeWeaponData.hitboxWith, meleeWeaponData.range * 2, meleeWeaponData.range));
    }
#endif
}