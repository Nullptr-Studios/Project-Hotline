using UnityEngine;

public class MissionObjective : Weapon
{
    private LevelManager _levelManager;
    
    protected override void Start()
    {
        base.Start();

        _levelManager = GameObject.Find("PA_LevelManager").GetComponent<LevelManager>();
        if (_levelManager == null) Debug.LogError($"[Mission Objective] {name}: Level Manager not found");
    }

    public override void Pickup(Transform weaponHolder)
    {
        base.Pickup(weaponHolder);
        
        _levelManager.CompleteMission();
    }
    
    public override void Throw(Vector2 forwardVector)
    {
        base.Throw(forwardVector);

        _levelManager.PauseMission();
    }
}
