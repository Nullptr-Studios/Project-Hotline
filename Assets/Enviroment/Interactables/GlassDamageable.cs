using FMODUnity;
using UnityEngine;

//@TODO: Maybe add FX option?
public class GlassDamageable : Damageable
{
    [Header("Sound")]
    public EventReference glassBreakSound;
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
        //Play glass break sound
        FMODUnity.RuntimeManager.PlayOneShot(glassBreakSound, transform.position);
        base.OnDead();
    }
    
}
