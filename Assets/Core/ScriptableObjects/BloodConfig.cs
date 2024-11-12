using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Configuration for blood effects, including material, sprites, and color.
/// </summary>
[CreateAssetMenu(fileName = "Blood Config", menuName = "ProjectHotline/Blood Config", order = 5)]
public class BloodConfig : ScriptableObject
{
    /// <summary>
    /// The material used for the blood effect.
    /// </summary>
    public Material Material;

    /// <summary>
    /// List of sprites used for the blood effect.
    /// </summary>
    public List<Sprite> Sprites;

    /// <summary>
    /// The color of the blood effect.
    /// </summary>
    public Color Color;
}