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

    /// <summary>
    /// Returns the amount of uses (bullets left), -1 if this weapon has no Uses functionality 
    /// </summary>
    public int UsesLeft();

    public float ReloadTime();

    public int MaxUses();

    public void Pickup(Transform weaponHolder);

    public void Throw(Vector2 forwardVector);

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
    
    public float throwForce = 15;
    
    public float rotationForce = 1000;

    public Collider2D gfxCollider2D;

    private bool _held;
    private Rigidbody2D _rb;
    
    public virtual int UsesLeft()
    {
        return -1;
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
        
        
        transform.parent = null;
        
        gfxCollider2D.enabled = true;
        
        _held = false;
        
        //reset Use
        Use(false);
    }


    /// <summary>
    /// Use Virtual Void
    /// </summary>
    public virtual void Use(bool pressed)
    {
        
    }
}
