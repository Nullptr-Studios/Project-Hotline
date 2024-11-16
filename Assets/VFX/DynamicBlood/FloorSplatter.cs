using System;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Handles the behavior of blood splatter on the floor, including its scaling over time.
/// </summary>
public class FloorSplatter : MonoBehaviour
{
    /// <summary>
    /// The maximum magnitude for the lerp function.
    /// </summary>
    public float lerpMagnitudeMax = 2;

    /// <summary>
    /// The minimum magnitude for the lerp function.
    /// </summary>
    public float lerpMagnitudeMin = 4;

    /// <summary>
    /// The final magnitude used for lerping.
    /// </summary>
    private float _finalLerpMagnitude;

    /// <summary>
    /// The transform component of the GameObject.
    /// </summary>
    private Transform _trans;

    /// <summary>
    /// Initializes the FloorSplatter instance.
    /// </summary>
    private void Start()
    {
        _finalLerpMagnitude = UnityEngine.Random.Range(lerpMagnitudeMin, lerpMagnitudeMax);
        _trans = transform;
        _trans.localScale = Vector3.zero;
    }

    /// <summary>
    /// Updates the scale of the blood splatter over time.
    /// </summary>
    private void Update()
    {
        Vector3 scale = _trans.localScale;

        if (scale.x > 0.95f)
        {
            Destroy(this);
            return;
        }

        float finalLerpMagnitudeCached = _finalLerpMagnitude * Time.deltaTime;
        float lerpVal = math.lerp(scale.x, 1, finalLerpMagnitudeCached);

        _trans.localScale = new Vector3(lerpVal, lerpVal, 1);
    }
}