using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum FireType
{
    Simple,
    Multiple
}

[CreateAssetMenu(fileName = "WD_UnnamedFireWeaponData", menuName = "ProjectHotline/Create FireWeaponData")]
public class FireWeaponData : ScriptableObject
{
    [Header("Fire Type")] 
    public FireType fireType = FireType.Simple;

    public int fireCastsAmount = 1;

    public int penetrationAmount = 1;


    [Header("Fire Rate")] 
    public bool automatic = true;
    public bool useCurve = false;

    public AnimationCurve 
        fireRateCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1,1));
    
    public float timeToReachFinalFireRate = 1.0f;
    public float initialFireRate = 0.5f;
    public float finalFireRate = 0.1f;
    
    

    [Header("Ammo")]
    public int maxAmmo;

    [Header("Reload")] 
    public float reloadTime = 1.0f;
    
    [Header("Bullet Dispersion")]
    public AnimationCurve 
        bulletDispersionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1,1));

    public float timeToReachMaxDispersion = 1.0f;
    public float minDispersionAngle = 5;
    public float maxDispersionAngle = 25;

    public void OnValidate()
    {
        fireRateCurve = new AnimationCurve(new Keyframe(0, initialFireRate),
            new Keyframe(timeToReachFinalFireRate, finalFireRate));

        bulletDispersionCurve = new AnimationCurve(new Keyframe(0, minDispersionAngle),
            new Keyframe(timeToReachMaxDispersion, maxDispersionAngle));
    }
}
