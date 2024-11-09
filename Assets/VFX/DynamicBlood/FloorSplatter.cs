using System;
using System.Collections;
using System.Collections.Generic;
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
        _trans.localScale = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Updates the scale of the blood splatter over time.
    /// </summary>
    private void Update()
    {
        Vector3 scale = _trans.localScale;

        if (scale.x > .95f)
        {
            Destroy(this);
            return;
        }

        float finalLerpMagnitudeCached = _finalLerpMagnitude * Time.deltaTime;

        // Since scale is linear, we can do this optimization and save 1 lerp
        float lerpVal = math.lerp(scale.x, 1, finalLerpMagnitudeCached);

        scale = new Vector3(lerpVal, lerpVal, 1);

        _trans.localScale = scale;
    }
}