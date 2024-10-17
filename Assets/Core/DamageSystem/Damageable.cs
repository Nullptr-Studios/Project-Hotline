using UnityEngine;

public interface IDamageable
{
    public void DoDamage(float amount, Vector3 shootDir, Vector3 hitPoint);

    public float GetCurrentHealth();
}

//@TODO: Maybe add FX option?
public class Damageable : MonoBehaviour, IDamageable
{
    [Header("Base Damageable")] 
    public float maxHealth = 1;
    
    private float _currentHealth;

    public virtual void Start()
    {
        _currentHealth = maxHealth;
    }

    public virtual void DoDamage(float amount, Vector3 shootDir, Vector3 hitPoint)
    {
        _currentHealth -= amount;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            OnDead();
        }
    }

    public virtual void OnDead()
    {
        //Default Action
        Destroy(gameObject);
    }

    public virtual float GetCurrentHealth()
    {
        return _currentHealth;
    }
}
