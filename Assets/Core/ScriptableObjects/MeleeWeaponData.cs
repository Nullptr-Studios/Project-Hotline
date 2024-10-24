using UnityEngine;

[CreateAssetMenu(fileName = "MD_UnnamedMelee", menuName = "ProjectHotline/Create MeleeWeaponData")]
public class MeleeWeaponData : ScriptableObject
{
    public float range = .5f;
    public float hitboxWith = 1;
    public float damage = .5f;

    public float cooldownTime = .25f;
}
