using UnityEngine;

/// <summary>
/// Base Damage Interface
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Applies damage to the object.
    /// </summary>
    /// <param name="amount">Amount to damage</param>
    public void DoDamage(float amount);

    /// <summary>
    /// Applies damage to the object with additional parameters.
    /// </summary>
    /// <param name="amount">Amount to damage</param>
    /// <param name="shootDir">The normalized shooting direction</param>
    /// <param name="hitPoint">The hit point in world coordinates</param>
    public void DoDamage(float amount, Vector3 shootDir, Vector3 hitPoint);

    /// <summary>
    /// Applies damage to the object with additional parameters including weapon type.
    /// </summary>
    /// <param name="amount">Amount to damage</param>
    /// <param name="shootDir">The normalized shooting direction</param>
    /// <param name="hitPoint">The hit point in world coordinates</param>
    /// <param name="weaponType">The type of weapon used</param>
    public void DoDamage(float amount, Vector3 shootDir, Vector3 hitPoint, EWeaponType weaponType);

    /// <summary>
    /// Stuns the object in a specified direction.
    /// </summary>
    /// <param name="dir">The direction of the stun</param>
    public void Stun(Vector3 dir);

    /// <summary>
    /// Gets the current health of the object.
    /// </summary>
    /// <returns>The current amount of health</returns>
    public float GetCurrentHealth();
}

//@TODO: Maybe add FX option?
public class Damageable : MonoBehaviour, IDamageable
{
    [Header("Base Damageable")]
    public float maxHealth = 1;

    protected float _currentHealth;

    /// <summary>
    /// Sets the max health as the current health.
    /// </summary>
    public virtual void Start()
    {
        _currentHealth = maxHealth;
    }

    /// <summary>
    /// Stuns the object in a specified direction.
    /// </summary>
    /// <param name="dir">The direction of the stun</param>
    public virtual void Stun(Vector3 dir)
    {

    }

    /// <summary>
    /// Applies damage to the object with additional parameters including weapon type.
    /// </summary>
    /// <param name="amount">Amount to damage</param>
    /// <param name="shootDir">The normalized shooting direction</param>
    /// <param name="hitPoint">The hit point in world coordinates</param>
    /// <param name="weaponType">The type of weapon used</param>
    public virtual void DoDamage(float amount, Vector3 shootDir, Vector3 hitPoint, EWeaponType weaponType)
    {
        DoDamage(amount);
    }

    /// <summary>
    /// Applies damage to the object with additional parameters.
    /// </summary>
    /// <param name="amount">Amount to damage</param>
    /// <param name="shootDir">The normalized shooting direction</param>
    /// <param name="hitPoint">The hit point in world coordinates</param>
    public virtual void DoDamage(float amount, Vector3 shootDir, Vector3 hitPoint)
    {
        DoDamage(amount);
    }

    /// <summary>
    /// Base Damage function with basic functionality.
    /// </summary>
    /// <param name="amount">Amount to damage</param>
    public virtual void DoDamage(float amount)
    {
        SceneMng.CivilianPanicDelegate?.Invoke();

        // Subtract health
        _currentHealth -= amount;

        // Dead check
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            OnDead();
        }
    }

    /// <summary>
    /// On Dead function with basic functionality.
    /// </summary>
    public virtual void OnDead()
    {
        // Default Action
        Destroy(gameObject);
    }

    /// <summary>
    /// Gets the current health of the object.
    /// </summary>
    /// <returns>The current amount of health</returns>
    public virtual float GetCurrentHealth()
    {
        return _currentHealth;
    }
}