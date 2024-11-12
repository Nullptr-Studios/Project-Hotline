using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Configuration for corpse behavior, including material, sprites, force, and drag.
/// </summary>
[CreateAssetMenu(fileName = "Corpse Config", menuName = "ProjectHotline/Corpse Config", order = 7)]
public class CorpseConfig : ScriptableObject
{
    /// <summary>
    /// The material used for the corpse.
    /// </summary>
    public Material Material;

    /// <summary>
    /// List of sprites used for the corpse.
    /// </summary>
    public List<Sprite> Sprites;

    /// <summary>
    /// List of sprites used for civilian corpses.
    /// </summary>
    public List<Sprite> CivilianSprites;

    [Header("Corpse Behaviour")]
    /// <summary>
    /// The force applied to the corpse.
    /// </summary>
    public float Force = 5;

    /// <summary>
    /// The drag applied to the corpse.
    /// </summary>
    public float Drag = 5;
}