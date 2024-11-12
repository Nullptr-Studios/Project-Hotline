using UnityEngine;

/// <summary>
/// Configuration for bullet trails, including material, width curve, duration, and other properties.
/// </summary>
[CreateAssetMenu(fileName = "Bullet Trail Config", menuName = "ProjectHotline/Bullet Trail Config", order = 4)]
public class BulletTrailConfig : ScriptableObject
{
    /// <summary>
    /// The material used for the bullet trail.
    /// </summary>
    public Material Material;

    /// <summary>
    /// The animation curve that defines the width of the bullet trail over its lifetime.
    /// </summary>
    public AnimationCurve WidthCurve;

    /// <summary>
    /// The duration of the bullet trail in seconds.
    /// </summary>
    public float Duration = 0.5f;

    /// <summary>
    /// The minimum distance between vertices in the bullet trail.
    /// </summary>
    public float MinVertexDistance = 0.1f;

    /// <summary>
    /// The gradient that defines the color of the bullet trail over its lifetime.
    /// </summary>
    public Gradient Color;

    /// <summary>
    /// The distance at which the bullet trail will be considered a miss.
    /// </summary>
    public float MissDistance = 100f;

    /// <summary>
    /// The speed at which the bullet trail simulation runs.
    /// </summary>
    public float SimulationSpeed = 100f;
}