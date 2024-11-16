using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// ScriptableObject that holds motion curves for acceleration and deceleration.
/// </summary>
[CreateAssetMenu(fileName = "MC_UnnamedCurve", menuName = "ProjectHotline/Create MotionCurve")]
public class MotionCurve : ScriptableObject
{
    /// <summary>
    /// The animation curve for acceleration.
    /// </summary>
    public AnimationCurve accelerationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    /// <summary>
    /// The animation curve for deceleration.
    /// </summary>
    public AnimationCurve decelerationCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));

    [SerializeField] private AnimationCurve timeline;

#if UNITY_EDITOR

    /// <summary>
    /// Updates the timeline when acceleration and deceleration curves are changed.
    /// This does not run at runtime, only when values are changed in a MotionCurve in the editor.
    /// </summary>
    private void OnValidate()
    {
        timeline = new AnimationCurve();

        // Acceleration
        foreach (Keyframe k in accelerationCurve.keys)
            timeline.AddKey(k);

        // Deceleration
        float decelerationStartTime = timeline.GetDuration() + 0.5f;
        Keyframe[] decelerationKeys = decelerationCurve.keys;
        for (var i = 0; i < decelerationKeys.Length; i++)
        {
            decelerationKeys[i].time += decelerationStartTime;
            timeline.AddKey(decelerationKeys[i]);
        }

        // Fix timeline curve going >1 after accelerationCurve
        AnimationUtility.SetKeyRightTangentMode(
            timeline,
            accelerationCurve.length - 1,
            AnimationUtility.TangentMode.Constant
        );
    }

#endif
}

/// <summary>
/// Handles the output from MotionCurve and makes the logic on when the accelerationCurve and decelerationCurve should be played.
/// </summary>
[System.Serializable]
public struct MotionController
{
    [FormerlySerializedAs("_curve")] [SerializeField] private MotionCurve curve;
    [FormerlySerializedAs("_speedMax")] [SerializeField] private float speedMax;
    private float _speedPercentage;
    public float Speed => speedMax * _speedPercentage;
    private float _time;
    private bool _wasMoving;

    /// <summary>
    /// Acceleration and deceleration logic based on whether the GameObject is moving or not.
    /// </summary>
    /// <param name="isMoving">Specifies if the player is inputting something on the controller.</param>
    public void Update(bool isMoving)
    {
        AnimationCurve currentCurve = isMoving ? curve.accelerationCurve : curve.decelerationCurve;
        // Make the transition between acceleration and deceleration smooth even if
        // movement is stopped before accelerationCurve is finished (value != 1)

        if (isMoving != _wasMoving)
        {
            _time = currentCurve.GetTime(_speedPercentage);
        }

        _time += Time.deltaTime;
        // Clamp is needed so we don't go further than the timeline's length
        _time = Mathf.Clamp(_time, 0f, currentCurve.GetDuration());
        _speedPercentage = currentCurve.Evaluate(_time);

        _wasMoving = isMoving;
    }

    /// <summary>
    /// Resets variables when disabled.
    /// </summary>
    public void OnDisable()
    {
        _speedPercentage = 0f;
        _time = 0f;
        _wasMoving = false;
    }
}