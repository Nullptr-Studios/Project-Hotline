using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controls the muzzle light effect using a 2D light component.
/// </summary>
public class MuzzleLightController : MonoBehaviour
{
    /// <summary>
    /// Reference to the 2D light component.
    /// </summary>
    private Light2D _light2D;

    /// <summary>
    /// Initial intensity of the light.
    /// </summary>
    private float _initialIntensity;

    /// <summary>
    /// Current time elapsed since the light was activated.
    /// </summary>
    private float _time;

    /// <summary>
    /// Animation curve used to control the light intensity over time.
    /// </summary>
    private AnimationCurve _curve;

    /// <summary>
    /// Indicates whether the light effect has finished.
    /// </summary>
    private bool _finished = true;

    /// <summary>
    /// Initializes the light component and sets its initial state.
    /// </summary>
    void Start()
    {
        _light2D = GetComponent<Light2D>();
        _initialIntensity = _light2D.intensity;
        _light2D.enabled = false;
    }

    /// <summary>
    /// Activates the light with the specified animation curve.
    /// </summary>
    /// <param name="curve">The animation curve to control the light intensity.</param>
    public void ActivateLight(AnimationCurve curve)
    {
        this.enabled = true;
        _light2D.enabled = true;
        _curve = curve;
        _time = 0;
        _finished = false;
    }

    /// <summary>
    /// Updates the light intensity based on the animation curve.
    /// </summary>
    void Update()
    {
        if (_finished) return;

        if (_curve.GetDuration() < _time)
        {
            _finished = true;
            _light2D.enabled = false;
            this.enabled = false;
            return;
        }

        _light2D.intensity = _curve.Evaluate(_time) * _initialIntensity;
        _time += Time.deltaTime;
    }
}