using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Configuration for blood effects, including material, sprites, and color.
/// </summary>
[CreateAssetMenu(fileName = "Weapon Dictionary", menuName = "ProjectHotline/Weapon Dictionary")]
public class WeaponDictionary : ScriptableObject
{
    public List<GameObject> weapons;
}