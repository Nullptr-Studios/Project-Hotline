using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{

    public MeleeWeaponData meleeWeaponData;

    private bool _wantsToAttack = false;

    private bool _canAttack = true;
    
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool log = false;
    [SerializeField] private bool drawGyzmos = false;
#endif
    
    public override void Start()
    {
        base.Start();
        
#if UNITY_EDITOR
        if (!meleeWeaponData)
        {
            if(log)
                Debug.LogError("MeleeWeapon Error: " + gameObject.name + "does not have MeleeWeaponData assigned!!!!!!!!");
        }
#endif
        
    }

    public override void Use(bool pressed)
    {
        _wantsToAttack = pressed;
    }

    private void UpdateCanAttack()
    {
        _canAttack = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (_wantsToAttack)
        {
            if (_canAttack)
            {
                _canAttack = false;
                Invoke("UpdateCanAttack", meleeWeaponData.cooldownTime);
                Attack();
            }
        }
    }

    private void Attack()
    {
        RaycastHit2D[] hitArr = new RaycastHit2D[32];
        ContactFilter2D cf2D = new ContactFilter2D();
        
        int hitNumber = Physics2D.CapsuleCast(transform.position, new Vector2(0.5f, 0.5f),
            CapsuleDirection2D.Horizontal,0,transform.right, cf2D, hitArr,meleeWeaponData.range);
        
#if UNITY_EDITOR
        if(log)
            Debug.Log("Amount of hits:" + hitNumber);
#endif

        for (int i = 0; i < hitNumber; i++)
        {
            if (hitArr[i].transform.TryGetComponent(out IDamageable damageableInterface)) 
            {
                damageableInterface.DoDamage(meleeWeaponData.damage);
            }
            
        }
    }
    
}
