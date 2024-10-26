using UnityEngine;

/// <summary>
/// Base Damage Interface
/// </summary>
public interface IDamageable
{
    public void DoDamage(float amount);
    
    /// <param name="amount">Amount to damage</param>
    /// <param name="shootDir">The normalized shooting direction</param>
    /// <param name="hitPoint">The hit point in world coordinates</param>
    public void DoDamage(float amount, Vector3 shootDir, Vector3 hitPoint);
    
    public void DoDamage(float amount, Vector3 shootDir, Vector3 hitPoint, EWeaponType weaponType);

    public void Stun(Vector3 dir);

    //Pretty self-explanatory
    public float GetCurrentHealth();
}

//@TODO: Maybe add FX option?
public class Damageable : MonoBehaviour, IDamageable
{
    [Header("Base Damageable")] 
    public float maxHealth = 1;
    
    private float _currentHealth;

    //Sets the max health as the current health
    public virtual void Start()
    {
        _currentHealth = maxHealth;
    }

    public virtual void Stun(Vector3 dir)
    {
        
    }

    public virtual void DoDamage(float amount, Vector3 shootDir, Vector3 hitPoint, EWeaponType weaponType)
    {
        DoDamage(amount);
    }
    
    public virtual void DoDamage(float amount, Vector3 shootDir, Vector3 hitPoint)
    {
        DoDamage(amount);
    }

    /// <summary>
    /// Base Damage function with basic functionality 
    /// </summary>
    public virtual void DoDamage(float amount)
    {
        //Subtract health
        _currentHealth -= amount;

        //Dead check
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            OnDead();
        }
    }

    /// <summary>
    /// On Dead function with basic functionality 
    /// </summary>
    public virtual void OnDead()
    {
        //Default Action
        Destroy(gameObject);
    }
    
    /// <returns>The current amount of health</returns>
    public virtual float GetCurrentHealth()
    {
        return _currentHealth;
    }
}
