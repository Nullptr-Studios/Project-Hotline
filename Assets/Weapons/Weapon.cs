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
    public EWeaponType GetWeaponType();

    public int GetWeaponSpriteID();

    public bool isClaimed();

    public void setClaimed(bool claimed);

    /// <summary>
    /// Returns the amount of uses (bullets left), -1 if this weapon has no Uses functionality 
    /// </summary>
    public int UsesLeft();

    public bool IsAutomatic();

    public float ReloadTime();

    public int MaxUses();

    public void Pickup(Transform weaponHolder);

    public void Throw(Vector2 forwardVector);

    public void Drop();

    public void Use(bool pressed);

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

    public virtual int GetWeaponSpriteID()
    {
        return -1;
    }

    public bool isClaimed()
    {
        return _isClaimed;
    }

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
    
    public virtual int UsesLeft()
    {
        return -1;
    }

    public virtual bool IsAutomatic()
    {
        return false;
    }

    public virtual int MaxUses()
    {
        return -1;
    }

    public virtual float ReloadTime()
    {
        return -1;
    }

    public virtual float TimeBetweenUses()
    {
        return -1;
    }

    /// <returns>Returns this weapon type</returns>
    public virtual EWeaponType GetWeaponType()
    {
        return weaponType;
    }

    protected virtual void Start()
    {
        if(!_held)
            AddRb();
    }

    /// <summary>
    /// Internally called to add RigidBody2D with defined params
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
    /// Function is called whenever the weapon has been picked up
    /// </summary>
    /// <param name="weaponHolder">Needs the parent for attaching</param>
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
    /// Function is called whenever the weapon is thrown
    /// </summary>
    /// <param name="forwardVector">The forward vector of the player</param>
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
    /// Use Virtual Void
    /// </summary>
    public virtual void Use(bool pressed)
    {
        
    }
}
