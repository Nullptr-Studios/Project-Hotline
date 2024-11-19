using System;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Handles the behavior of blood splatter on the floor, including its disappearance over time.
/// </summary>
public class FloorSplatterDisapear : MonoBehaviour
{
    /// <summary>
    /// The maximum magnitude for the lerp function.
    /// </summary>
    public float lerpMagnitudeMax = 2;

    /// <summary>
    /// The minimum magnitude for the lerp function.
    /// </summary>
    public float lerpMagnitudeMin = 1;

    /// <summary>
    /// The final magnitude used for lerping.
    /// </summary>
    private float _finalLerpMagnitude;

    /// <summary>
    /// The transform component of the GameObject.
    /// </summary>
    private Transform _trans;

    /// <summary>
    /// Initializes the FloorSplatterDisapear instance.
    /// </summary>
    private void Start()
    {
        _finalLerpMagnitude = UnityEngine.Random.Range(lerpMagnitudeMin, lerpMagnitudeMax);
        _trans = transform;
    }

    /// <summary>
    /// Updates the scale of the blood splatter over time, causing it to disappear.
    /// </summary>
    private void Update()
    {
        Vector3 scale = _trans.localScale;

        if (scale.x < 0.05f)
        {
            gameObject.SetActive(false);
            ResourceManager.GetBloodPool().Release(gameObject);
            Destroy(this);
            return;
        }

        float finalLerpMagnitudeCached = _finalLerpMagnitude * Time.deltaTime;
        float lerpVal = math.lerp(scale.x, 0, finalLerpMagnitudeCached);

        _trans.localScale = new Vector3(lerpVal, lerpVal, 1);
    }
}