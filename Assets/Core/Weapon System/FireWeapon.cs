using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWeapon : Weapon
{

    [Header("Fire Weapon")] 
    public FireWeaponData fireWeaponData;

    public int currentAmmo = 0;
    
    private float _fireRateTimer = 0.0f;
    
    public override int UsesLeft()
    {
        return currentAmmo;
    }
    
    /// <summary>
    /// Use Functionality
    /// </summary>
    public override void Use(bool pressed)
    {
        Debug.Log("Fire: " + pressed);
    }

    // Start is called before the first frame update
    public override void Start()
    {
        //Implement Base class functionality
        base.Start();

        if (fireWeaponData)
        {
            currentAmmo = fireWeaponData.maxAmmo;
        }
        else
        {
            Debug.LogError("FireWeapon Error: " + gameObject.name + "does not have fireWeaponData!!!!!!!!");
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
