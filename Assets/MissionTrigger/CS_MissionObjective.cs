using System;
using UnityEngine;

public class MissionObjective : Weapon
{
    private LevelManager _levelManager;
    
    private Vector3 _originalPosition;
    
    protected override void Start()
    {
        base.Start();
        
        _originalPosition = transform.position;

        _levelManager = GameObject.Find("PA_LevelManager").GetComponent<LevelManager>();
        if (_levelManager == null) Debug.LogError($"[Mission Objective] {name}: Level Manager not found");
    }

    public override void Reload()
    {
        Drop();
        transform.position = _originalPosition;
    }

    public override int GetWeaponSpriteID()
    {
        return -2;  //special objective ID
    }

    public override void Pickup(Transform weaponHolder)
    {
        base.Pickup(weaponHolder);
        
        _levelManager.CompleteMission();
    }

    public override void Drop()
    {
        _levelManager.PauseMission();
        base.Drop();
    }

    public override void Throw(Vector2 forwardVector)
    {
        _levelManager.PauseMission();
        base.Throw(forwardVector);
    }
}
