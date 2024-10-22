using UnityEngine;

[CreateAssetMenu(fileName = "Bullet Trail Config", menuName = "ProjectHotline/Bullet Trail Config", order = 4)]
public class BulletTrailConfig : ScriptableObject
{
    public Material Material;
    public AnimationCurve WidthCurve;
    public float Duration = 0.5f;
    public float MinVertexDistance = 0.1f;
    public Gradient Color;

    public float MissDistance = 100f;
    public float SimulationSpeed = 100f;
}