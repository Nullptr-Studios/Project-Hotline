using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Classify different weapon types
/// </summary>
public enum EWeaponType
{
    Fire,
    Melee,
    Default
}

/// <summary>
/// This interface will implement all the functions weapons will make use of
/// </summary>
public interface IWeapon
{
    /// <summary>
    /// Returns the type of the weapon.
    /// </summary>
    public EWeaponType GetWeaponType();

    /// <summary>
    /// Returns the sprite ID of the weapon.
    /// </summary>
    public int GetWeaponSpriteID();

    /// <summary>
    /// Checks if the weapon is claimed.
    /// </summary>
    public bool isClaimed();

    /// <summary>
    /// Sets the claimed status of the weapon.
    /// </summary>
    /// <param name="claimed">The claimed status to set.</param>
    public void setClaimed(bool claimed);

    /// <summary>
    /// Returns the amount of uses (bullets left), -1 if this weapon has no Uses functionality.
    /// </summary>
    public int UsesLeft();

    /// <summary>
    /// Checks if the weapon is automatic.
    /// </summary>
    public bool IsAutomatic();

    /// <summary>
    /// Returns the reload time of the weapon.
    /// </summary>
    public float ReloadTime();

    /// <summary>
    /// Returns the maximum number of uses for the weapon.
    /// </summary>
    public int MaxUses();

    /// <summary>
    /// Handles the pickup functionality of the weapon.
    /// </summary>
    /// <param name="weaponHolder">The transform of the weapon holder.</param>
    public void Pickup(Transform weaponHolder);

    /// <summary>
    /// Handles the throw functionality of the weapon.
    /// </summary>
    /// <param name="forwardVector">The forward vector for the throw.</param>
    public void Throw(Vector2 forwardVector);

    /// <summary>
    /// Handles the drop functionality of the weapon.
    /// </summary>
    public void Drop();

    /// <summary>
    /// Handles the use functionality of the weapon.
    /// </summary>
    /// <param name="pressed">Indicates if the use button is pressed.</param>
    public void Use(bool pressed);

    /// <summary>
    /// Returns the time between uses of the weapon.
    /// </summary>
    public float TimeBetweenUses();
}

/// <summary>
/// Base weapon class, basic functionality
/// </summary>
public class Weapon : MonoBehaviour, IWeapon
{
    [Header("Base")]
    public EWeaponType weaponType = EWeaponType.Default;

    //for AI
    private bool _isClaimed = false;

    /// <summary>
    /// Returns the sprite ID of the weapon.
    /// </summary>
    public virtual int GetWeaponSpriteID()
    {
        return -1;
    }

    /// <summary>
    /// Checks if the weapon is claimed.
    /// </summary>
    public bool isClaimed()
    {
        return _isClaimed;
    }

    /// <summary>
    /// Sets the claimed status of the weapon.
    /// </summary>
    /// <param name="claimed">The claimed status to set.</param>
    public void setClaimed(bool claimed)
    {
        _isClaimed = claimed;
    }

    public float throwForce = 15;

    public float rotationForce = 1000;

    public Collider2D gfxCollider2D;

    public bool canStun = true;
    public LayerMask maskToStun;

    private bool _stun;

    private bool _held;
    private Rigidbody2D _rb;

    /// <summary>
    /// Returns the amount of uses (bullets left), -1 if this weapon has no Uses functionality.
    /// </summary>
    public virtual int UsesLeft()
    {
        return -1;
    }

    /// <summary>
    /// Checks if the weapon is automatic.
    /// </summary>
    public virtual bool IsAutomatic()
    {
        return false;
    }

    /// <summary>
    /// Returns the maximum number of uses for the weapon.
    /// </summary>
    public virtual int MaxUses()
    {
        return -1;
    }

    /// <summary>
    /// Returns the reload time of the weapon.
    /// </summary>
    public virtual float ReloadTime()
    {
        return -1;
    }

    /// <summary>
    /// Returns the time between uses of the weapon.
    /// </summary>
    public virtual float TimeBetweenUses()
    {
        return -1;
    }

    /// <summary>
    /// Returns the type of the weapon.
    /// </summary>
    public virtual EWeaponType GetWeaponType()
    {
        return weaponType;
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    protected virtual void Start()
    {
        if(!_held)
            AddRb();
    }

    /// <summary>
    /// Internally called to add RigidBody2D with defined params.
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
    /// Function is called whenever the weapon has been picked up.
    /// </summary>
    /// <param name="weaponHolder">Needs the parent for attaching.</param>
    public virtual void Pickup(Transform weaponHolder)
    {
        if(_held)
            return;

        //Destroys RigidBody2D to avoid funky things
        Destroy(_rb);

        transform.parent = weaponHolder;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        gfxCollider2D.enabled = false;

        _held = true;

        _isClaimed = true;
    }

    /// <summary>
    /// Function is called whenever the weapon is thrown.
    /// </summary>
    /// <param name="forwardVector">The forward vector of the player.</param>
    public virtual void Throw(Vector2 forwardVector)
    {
        if (!_held)
            return;

        AddRb();

        _rb.velocity = forwardVector * throwForce;
        _rb.angularVelocity = Random.Range(-rotationForce,rotationForce);

        _stun = true;

        transform.parent = null;

        gfxCollider2D.enabled = true;

        _held = false;

        _isClaimed = false;

        //reset Use
        Use(false);
    }

    /// <summary>
    /// Function is called whenever the weapon is dropped.
    /// </summary>
    public virtual void Drop()
    {
        if (!_held)
            return;

        AddRb();

        _rb.angularVelocity = Random.Range(-(rotationForce / 2), rotationForce / 2);

        transform.parent = null;

        gfxCollider2D.enabled = true;

        _held = false;

        _isClaimed = false;

        //reset Use
        Use(false);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    public virtual void Update()
    {
        if (_stun && canStun)
        {
            if(!_rb)
                return;

            if (_rb.velocity.magnitude >= 5)
            {
                ContactFilter2D cf2D = new ContactFilter2D();
                RaycastHit2D[] hitArr = new RaycastHit2D[10];

                cf2D.SetLayerMask(maskToStun);
                cf2D.useLayerMask = true;

                int hitNumber = Physics2D.CapsuleCast(transform.position, new Vector2(.6f, .6f),
                    CapsuleDirection2D.Horizontal,0,new Vector2(0,0),cf2D, hitArr);

                if (hitNumber > 0)
                {
                    for (int i = 0; i < hitNumber; i++)
                    {
                        IDamageable d = hitArr[i].transform.GetComponent<IDamageable>();

                        if (d == null)
                            return;

                        d.Stun((hitArr[i].transform.position - transform.position).normalized);
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
    /// Handles the use functionality of the weapon.
    /// </summary>
    /// <param name="pressed">Indicates if the use button is pressed.</param>
    public virtual void Use(bool pressed)
    {

    }
}