using FMODUnity;
using UnityEngine;

/// <summary>
/// ScriptableObject that holds data for melee weapon behavior.
/// </summary>
[CreateAssetMenu(fileName = "MD_UnnamedMelee", menuName = "ProjectHotline/Create MeleeWeaponData")]
public class MeleeWeaponData : ScriptableObject
{
    /// <summary>
    /// The range of the melee weapon.
    /// </summary>
    public float range = .5f;

    /// <summary>
    /// The width of the hitbox for the melee weapon.
    /// </summary>
    public float hitboxWith = 1;

    /// <summary>
    /// The damage dealt by the melee weapon.
    /// </summary>
    public float damage = .5f;

    /// <summary>
    /// The cooldown time between attacks.
    /// </summary>
    public float cooldownTime = .25f;

    [Header("Sprite")]
    /// <summary>
    /// The ID of the sprite used for the melee weapon.
    /// </summary>
    public int SpriteID = 0;

    [Header("Audio")]
    /// <summary>
    /// The sound event for using the melee weapon.
    /// </summary>
    public EventReference useSound;
}