using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Blood Config", menuName = "ProjectHotline/Blood Config", order = 5)]
public class BloodConfig : ScriptableObject
{
    public Material Material;
    public List<Sprite> Sprites;
    public Color Color;
}
