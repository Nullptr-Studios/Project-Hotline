using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MC_UnnamedCurve", menuName = "Create MotionCurve")]
public class MotionCurve : ScriptableObject
{
    // Animation curve variables
    public AnimationCurve 
        accelerationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1,1));

    public AnimationCurve 
        decelerationCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1,0));

    [SerializeField] private AnimationCurve timeline;

    // Updates _timeline when acceleration and deceleration curves are changed
    // This DOESN'T run at runtime, only when values are changed in a MotionCurve at the editor
    private void OnValidate()
    {
        timeline = new AnimationCurve();

        // Acceleration
        foreach (var k in accelerationCurve.keys)
            timeline.AddKey(k);

        // Deceleration
        float decelerationStartTime = timeline.GetDuration() + 0.5f;
        Keyframe[] decelerationKeys = decelerationCurve.keys;
        for (var i = 0; i < decelerationKeys.Length; i++)
        {
            decelerationKeys[i].time += decelerationStartTime;
            timeline.AddKey(decelerationKeys[i]);
        }
        
        // Fix _timeline curve going >1 after accelerationCurve
        AnimationUtility.SetKeyRightTangentMode(
            timeline, 
            accelerationCurve.length - 1, 
            AnimationUtility.TangentMode.Constant
        );
    }
}


/// <summary>
/// Handles the output from MotionCurve and makes the logic on when should the accelerationCurve and decelerationCurve be played.
/// </summary>
[System.Serializable] public struct MotionController
{
    [FormerlySerializedAs("_curve")] [SerializeField] private MotionCurve curve;
    [FormerlySerializedAs("_speedMax")] [SerializeField] private float speedMax;
    private float _speedPercentage;
    public float Speed => speedMax * _speedPercentage;
    private float _time;
    private bool _wasMoving;

    /// <summary>
    /// Acceleration and deceleration logic based on if the GameObject is moving or not.
    /// </summary>
    /// <param name="isMoving">Specify if the player is inputting something on the controller</param>
    /// <param name="isMoving">Specify if the character velocity is not 0</param>
    public void Update(bool isMoving)
    {
        var currentCurve = isMoving ? curve.accelerationCurve : curve.decelerationCurve;
        // Make so transition between acceleration and deceleration is smooth even if
        // movement is stopped before accelerationCurve is finished (value != 1){

        if (isMoving != _wasMoving)
        {
            _time = currentCurve.GetTime(_speedPercentage);
        }

        _time += Time.deltaTime;
        // Clamp is needed so we don't go further than m_Timeline's length
        _time = Mathf.Clamp(_time, 0f, currentCurve.GetDuration());
        _speedPercentage = currentCurve.Evaluate(_time);
        
        _wasMoving = isMoving;
    }
}

