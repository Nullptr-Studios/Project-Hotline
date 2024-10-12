using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "WD_UnnamedFireWeaponData", menuName = "ProjectHotline/Create FireWeaponData")]
public class FireWeaponData : ScriptableObject
{
    [Header("Fire Rate")]
    public AnimationCurve 
        fireRateCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1,1));

    public float timeToReachMaxFireRate = 1.0f;
    public float maxFireRate;
    
    //@TODO: Add Cooldown

    [Header("Ammo")]
    public int maxAmmo;

    [Header("Reload")] 
    public float reloadTime = 1.0f;
    
    [Header("Bullet Dispersion")]
    public AnimationCurve 
        bulletDispersionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1,1));

    public float timeToReachMaxDispersion = 1.0f;
    public float maxDispersion;
}
