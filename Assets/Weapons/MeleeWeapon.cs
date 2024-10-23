using System;
using UnityEngine;

public class MeleeWeapon : Weapon
{

    public MeleeWeaponData meleeWeaponData;
    public float offset;
    private bool _wantsToAttack;
    private bool _canAttack = true;
    //private Transform _player;
    
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool log;
    [SerializeField] private bool drawGizmos;
#endif

    private void Awake()
    {
        weaponType = EWeaponType.Melee;
    }

    protected override void Start()
    {
        base.Start();
        
#if UNITY_EDITOR
        //_player = GameObject.FindGameObjectWithTag("Player").transform;

        if (!meleeWeaponData)
        {
            if(log)
                Debug.LogError("MeleeWeapon Error: " + gameObject.name + "does not have MeleeWeaponData assigned");
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
        if (!_wantsToAttack) return;
        if (!_canAttack) return;
        
        _canAttack = false;
        Invoke(nameof(UpdateCanAttack), meleeWeaponData.cooldownTime);
        Attack();
    }

    private void Attack()
    {
        var hitArr = new RaycastHit2D[32];
        var cf2D = new ContactFilter2D();
        
        int hitNumber = Physics2D.CapsuleCast(transform.position + (offset * transform.right), 
            new Vector2(meleeWeaponData.hitboxWith, meleeWeaponData.range),
            CapsuleDirection2D.Horizontal,0,transform.right, cf2D, hitArr, meleeWeaponData.range);
        
#if UNITY_EDITOR
        if (log)
        {
            Debug.Log("Amount of hits:" + hitNumber);
            //Debug.Log(_player.eulerAngles.z);
        }
#endif

        for (int i = 0; i < hitNumber; i++)
        {
            if (hitArr[i].transform.TryGetComponent(out IDamageable damageableInterface)) 
            {
                //Try to do Damage
                damageableInterface.DoDamage(meleeWeaponData.damage, transform.right, hitArr[i].point, weaponType);
            }
            
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if (drawGizmos) 
            Gizmos.DrawWireCube(transform.position + (offset * transform.right), 
            new Vector3(meleeWeaponData.hitboxWith, meleeWeaponData.range*2, meleeWeaponData.range));
    }
#endif    
    
}
