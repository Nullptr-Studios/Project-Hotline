using FMODUnity;
using UnityEngine;

/// <summary>
/// Enum representing the type of fire for the weapon.
/// </summary>
public enum FireType
{
    Simple,
    Multiple
}

[CreateAssetMenu(fileName = "WD_UnnamedWeapon", menuName = "ProjectHotline/Create FireWeaponData")]
public class FireWeaponData : ScriptableObject
{
    [Header("Fire Type")] 
    /// <summary>
    /// The type of fire for the weapon.
    /// </summary>
    public FireType fireType = FireType.Simple;

    /// <summary>
    /// The amount of fire casts.
    /// </summary>
    public int fireCastsAmount = 1;

    /// <summary>
    /// The amount of penetration.
    /// </summary>
    public int penetrationAmount = 1;

    [Header("Fire Rate")] 
    /// <summary>
    /// Indicates if the weapon is automatic.
    /// </summary>
    public bool automatic = true;

    /// <summary>
    /// Indicates if the fire rate curve should be used.
    /// </summary>
    public bool useFireRateCurve;

    /// <summary>
    /// Indicates if the fire rate curve should be overridden.
    /// </summary>
    public bool overrideFireRateCurve;

    /// <summary>
    /// The animation curve for the fire rate.
    /// </summary>
    public AnimationCurve fireRateCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1,1));
    
    /// <summary>
    /// The time to reach the final fire rate.
    /// </summary>
    public float timeToReachFinalFireRate = 1.0f;

    /// <summary>
    /// The initial fire rate.
    /// </summary>
    public float initialFireRate = 0.5f;

    /// <summary>
    /// The final fire rate.
    /// </summary>
    public float finalFireRate = 0.1f;

    [Header("Ammo")]
    /// <summary>
    /// The maximum ammo capacity.
    /// </summary>
    public int maxAmmo = 30;

    [Header("Reload")] 
    /// <summary>
    /// The time required to reload the weapon.
    /// </summary>
    public float reloadTime = 1.0f;

    [Header("Bullet Dispersion")] 
    /// <summary>
    /// Indicates if bullet dispersion should be used.
    /// </summary>
    public bool useDispersion = true;

    /// <summary>
    /// Indicates if the dispersion curve should be used.
    /// </summary>
    public bool useDispersionCurve;

    /// <summary>
    /// Indicates if the bullet dispersion curve should be overridden.
    /// </summary>
    public bool overrideBulletDispersionCurve;

    /// <summary>
    /// The animation curve for bullet dispersion.
    /// </summary>
    public AnimationCurve bulletDispersionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1,1));

    /// <summary>
    /// The time to reach maximum dispersion.
    /// </summary>
    public float timeToReachMaxDispersion = 1.0f;

    /// <summary>
    /// The minimum dispersion angle.
    /// </summary>
    public float minDispersionAngle = 5;

    /// <summary>
    /// The maximum dispersion angle.
    /// </summary>
    public float maxDispersionAngle = 25;

    [Header("Sprite")]
    /// <summary>
    /// The sprite animation ID.
    /// 0 -> pistol
    /// 1 -> SMG
    /// 2 -> rifle
    /// 3 -> Shotgun
    /// </summary>
    public int SpriteAnimID = 0; //default is pistol
    
    [Header("Audio")]
    /// <summary>
    /// The sound event for firing the weapon.
    /// </summary>
    public EventReference fireSound;

    /// <summary>
    /// The sound event for reloading the weapon.
    /// </summary>
    public EventReference reloadSound;

    /// <summary>
    /// The sound event for finishing the reload.
    /// </summary>
    public EventReference finishReloadSound;
    
    [Header("SFX")]
    public AnimationCurve sfxMuzzleFlashCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(.25f,0));
    
    /// <summary>
    /// Generates the curves for fire rate and bullet dispersion.
    /// </summary>
    public void OnValidate()
    {
        if(!overrideFireRateCurve){
            fireRateCurve = new AnimationCurve(new Keyframe(0, initialFireRate),
                new Keyframe(timeToReachFinalFireRate, finalFireRate));
        }

        if (!overrideBulletDispersionCurve)
        {
            bulletDispersionCurve = new AnimationCurve(new Keyframe(0, minDispersionAngle),
                new Keyframe(timeToReachMaxDispersion, maxDispersionAngle));
        }
    }
}