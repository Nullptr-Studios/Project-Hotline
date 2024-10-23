using UnityEngine;

//@TODO: Maybe add FX option?
public class GlassDamageable : Damageable
{
    //Sets the max health as the current health
    public override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// On Dead function with basic functionality 
    /// </summary>
    public override void OnDead()
    {
        
    }
    
}
