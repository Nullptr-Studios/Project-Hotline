using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWeapon : Weapon
{
    /// <summary>
    /// Use Functionality
    /// </summary>
    public override void Use()
    {
        Debug.Log("Cac");
    }

    // Start is called before the first frame update
    public override void Start()
    {
        //Implement Base class functionality
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
