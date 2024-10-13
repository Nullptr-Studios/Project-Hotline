using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WD_UnnamedMeleeWeaponData", menuName = "ProjectHotline/Create MeleeWeaponData")]
public class MeleeWeaponData : ScriptableObject
{
    public float range = .5f;

    public float damage = .5f;

    public float cooldownTime = .25f;

}
