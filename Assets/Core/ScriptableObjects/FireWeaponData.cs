using FMODUnity;
using UnityEngine;

public enum FireType
{
    Simple,
    Multiple
}

[CreateAssetMenu(fileName = "WD_UnnamedWeapon", menuName = "ProjectHotline/Create FireWeaponData")]
public class FireWeaponData : ScriptableObject
{
    [Header("Fire Type")] 
    public FireType fireType = FireType.Simple;
    public int fireCastsAmount = 1;
    public int penetrationAmount = 1;

    [Header("Fire Rate")] 
    public bool automatic = true;
    public bool useFireRateCurve;

    public bool overrideFireRateCurve;
    public AnimationCurve 
        fireRateCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1,1));
    
    public float timeToReachFinalFireRate = 1.0f;
    public float initialFireRate = 0.5f;
    public float finalFireRate = 0.1f;

    [Header("Ammo")]
    public int maxAmmo = 30;

    [Header("Reload")] 
    public float reloadTime = 1.0f;

    [Header("Bullet Dispersion")] 
    public bool useDispersion = true;
    //If useDispersionCurve is false, it wil default to the maximum amount of dispersion
    public bool useDispersionCurve;
    [Tooltip("If useDispersionCurve is false, it wil default to the maximum amount of dispersion")]

    public bool overrideBulletDispersionCurve;
    public AnimationCurve 
        bulletDispersionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1,1));

    public float timeToReachMaxDispersion = 1.0f;
    public float minDispersionAngle = 5;
    public float maxDispersionAngle = 25;
    
    [Header("Audio")]
    public EventReference fireSound;
    public EventReference reloadSound;
    public EventReference finishReloadSound;

    /// <summary>
    /// Generate Curves
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
