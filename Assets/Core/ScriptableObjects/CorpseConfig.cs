using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Corpse Config", menuName = "ProjectHotline/Corpse Config", order = 7)]
public class CorpseConfig : ScriptableObject
{
    public Material Material;
    public List<Sprite> Sprites;

    [Header("Corpse Behaviour")] 
    public float Force = 5;
    public float Drag = 5;
}
