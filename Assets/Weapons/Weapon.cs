using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Classify different weapon types.
/// </summary>
public enum EWeaponType
{
    Fire,
    Melee,
    Default
}

/// <summary>
/// This interface will implement all the functions weapons will make use of.
/// </summary>
public interface IWeapon
{
    /// <summary>
    /// Gets the weapon type.
    /// </summary>
    /// <returns>The weapon type.</returns>
    public EWeaponType GetWeaponType();

    /// <summary>
    /// Gets the hearing range of the weapon.
    /// </summary>
    /// <returns>The hearing range.</returns>
    public float GetHearingRange();

    /// <summary>
    /// Sets whether the weapon is held by the player.
    /// </summary>
    /// <param name="isPlayer">True if held by the player, false otherwise.</param>
    public void SetIsPlayer(bool isPlayer);

    /// <summary>
    /// Gets the weapon sprite ID.
    /// </summary>
    /// <returns>The weapon sprite ID.</returns>
    public int GetWeaponSpriteID();

    /// <summary>
    /// Checks if the weapon is claimed.
    /// </summary>
    /// <returns>True if claimed, false otherwise.</returns>
    public bool isClaimed();

    /// <summary>
    /// Sets the claimed status of the weapon.
    /// </summary>
    /// <param name="claimed">True to claim the weapon, false to unclaim.</param>
    public void setClaimed(bool claimed);

    /// <summary>
    /// Gets the number of uses left for the weapon.
    /// </summary>
    /// <returns>The number of uses left.</returns>
    public int UsesLeft();

    /// <summary>
    /// Checks if the weapon is automatic.
    /// </summary>
    /// <returns>True if automatic, false otherwise.</returns>
    public bool IsAutomatic();

    /// <summary>
    /// Gets the reload time of the weapon.
    /// </summary>
    /// <returns>The reload time.</returns>
    public float ReloadTime();

    /// <summary>
    /// Gets the maximum number of uses for the weapon.
    /// </summary>
    /// <returns>The maximum number of uses.</returns>
    public int MaxUses();

    /// <summary>
    /// Picks up the weapon.
    /// </summary>
    /// <param name="weaponHolder">The transform of the weapon holder.</param>
    public void Pickup(Transform weaponHolder);

    /// <summary>
    /// Throws the weapon.
    /// </summary>
    /// <param name="forwardVector">The forward vector for the throw.</param>
    public void Throw(Vector2 forwardVector);

    /// <summary>
    /// Drops the weapon.
    /// </summary>
    public void Drop();

    /// <summary>
    /// Uses the weapon.
    /// </summary>
    /// <param name="pressed">True if the use action is pressed, false otherwise.</param>
    public void Use(bool pressed);

    /// <summary>
    /// Gets the time between uses of the weapon.
    /// </summary>
    /// <returns>The time between uses.</returns>
    public float TimeBetweenUses();
}

/// <summary>
/// Base weapon class, basic functionality.
/// </summary>
public class Weapon : MonoBehaviour, IWeapon
{
    [Header("Base")]
    public EWeaponType weaponType = EWeaponType.Default;

    private bool _isClaimed = false;
    private bool _stun;
    private bool _held;
    private Rigidbody2D _rb;
    private ContactFilter2D _contactFilter;
    private RaycastHit2D[] _hitArr = new RaycastHit2D[10];

    public float throwForce = 15;
    public float rotationForce = 1000;
    public Collider2D gfxCollider2D;
    public bool canStun = true;
    public LayerMask maskToStun;
    public bool isPlayer;

    /// <summary>
    /// Initializes the contact filter.
    /// </summary>
    private void Awake()
    {
        _contactFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = maskToStun
        };
    }

    /// <summary>
    /// Gets the weapon sprite ID.
    /// </summary>
    /// <returns>The weapon sprite ID.</returns>
    public virtual int GetWeaponSpriteID() => -1;

    /// <summary>
    /// Checks if the weapon is claimed.
    /// </summary>
    /// <returns>True if claimed, false otherwise.</returns>
    public bool isClaimed() => _isClaimed;

    /// <summary>
    /// Gets the hearing range of the weapon.
    /// </summary>
    /// <returns>The hearing range.</returns>
    public virtual float GetHearingRange() => 0;

    /// <summary>
    /// Sets the claimed status of the weapon.
    /// </summary>
    /// <param name="claimed">True to claim the weapon, false to unclaim.</param>
    public void setClaimed(bool claimed) => _isClaimed = claimed;

    /// <summary>
    /// Gets the number of uses left for the weapon.
    /// </summary>
    /// <returns>The number of uses left.</returns>
    public virtual int UsesLeft() => -1;

    /// <summary>
    /// Checks if the weapon is automatic.
    /// </summary>
    /// <returns>True if automatic, false otherwise.</returns>
    public virtual bool IsAutomatic() => false;

    /// <summary>
    /// Gets the maximum number of uses for the weapon.
    /// </summary>
    /// <returns>The maximum number of uses.</returns>
    public virtual int MaxUses() => -1;

    /// <summary>
    /// Gets the reload time of the weapon.
    /// </summary>
    /// <returns>The reload time.</returns>
    public virtual float ReloadTime() => -1;

    /// <summary>
    /// Gets the time between uses of the weapon.
    /// </summary>
    /// <returns>The time between uses.</returns>
    public virtual float TimeBetweenUses() => -1;

    /// <summary>
    /// Gets the weapon type.
    /// </summary>
    /// <returns>The weapon type.</returns>
    public virtual EWeaponType GetWeaponType() => weaponType;

    /// <summary>
    /// Sets whether the weapon is held by the player.
    /// </summary>
    /// <param name="isPlayer">True if held by the player, false otherwise.</param>
    public virtual void SetIsPlayer(bool isPlayer) => this.isPlayer = isPlayer;

    /// <summary>
    /// Initializes the weapon.
    /// </summary>
    protected virtual void Start()
    {
        if (!_held)
            AddRb();
    }

    /// <summary>
    /// Adds a Rigidbody2D component to the weapon.
    /// </summary>
    private void AddRb()
    {
        _rb = gameObject.AddComponent<Rigidbody2D>();
        _rb.mass = 0.1f;
        _rb.gravityScale = 0;
        _rb.drag = 1;
        _rb.angularDrag = 1;
    }

    /// <summary>
    /// Picks up the weapon.
    /// </summary>
    /// <param name="weaponHolder">The transform of the weapon holder.</param>
    public virtual void Pickup(Transform weaponHolder)
    {
        if (_held) return;

        Destroy(_rb);
        transform.SetParent(weaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        gfxCollider2D.enabled = false;
        _held = true;
        _isClaimed = true;
    }

    /// <summary>
    /// Throws the weapon.
    /// </summary>
    /// <param name="forwardVector">The forward vector for the throw.</param>
    public virtual void Throw(Vector2 forwardVector)
    {
        if (!_held) return;

        AddRb();
        _rb.velocity = forwardVector * throwForce;
        _rb.angularVelocity = Random.Range(-rotationForce, rotationForce);
        _stun = true;
        transform.SetParent(null);
        gfxCollider2D.enabled = true;
        _held = false;
        _isClaimed = false;
        Use(false);
    }

    /// <summary>
    /// Drops the weapon.
    /// </summary>
    public virtual void Drop()
    {
        if (!_held) return;

        AddRb();
        _rb.angularVelocity = Random.Range(-(rotationForce / 2), rotationForce / 2);
        transform.SetParent(null);
        gfxCollider2D.enabled = true;
        _held = false;
        _isClaimed = false;
        Use(false);
    }

    /// <summary>
    /// Updates the weapon state.
    /// </summary>
    public virtual void Update()
    {
        if (_stun && canStun && _rb != null)
        {
            if (_rb.velocity.magnitude >= 5)
            {
                int hitNumber = Physics2D.CapsuleCast(transform.position, new Vector2(.6f, .6f), CapsuleDirection2D.Horizontal, 0, Vector2.zero, _contactFilter, _hitArr);
                if (hitNumber > 0)
                {
                    for (int i = 0; i < hitNumber; i++)
                    {
                        if (_hitArr[i].transform.TryGetComponent(out IDamageable damageable))
                        {
                            damageable.Stun((_hitArr[i].transform.position - transform.position).normalized);
                        }
                    }
                }
            }
            else
            {
                _stun = false;
            }
        }
    }

    /// <summary>
    /// Uses the weapon.
    /// </summary>
    /// <param name="pressed">True if the use action is pressed, false otherwise.</param>
    public virtual void Use(bool pressed) { }
}