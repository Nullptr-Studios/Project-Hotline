using UnityEngine;

public static class AnimationCurveExtensions
{   
    /// <summary>
    /// Gets duration of given Animation curve
    /// </summary>
    /// <param name="curve"></param>
    /// <returns>Duration (in seconds) of Animation Curve</returns>
    public static float GetDuration(this AnimationCurve curve) => curve[curve.length - 1].time;

    /// <summary>
    /// Returns time of AnimationCurve in given value
    /// </summary>
    /// <param name="curve"></param>
    /// <param name="value">Value of curve to find a time</param>
    /// <returns>Time of Animation Curve</returns>
    public static float GetTime(this AnimationCurve curve, float value)
    {
        // Threshold when finding distance
        const float MAX_DISTANCE = 0.05f;
        // Checks if we are accelerating or decelerating
        // Useful to correct in case we have gone beyond in the path finding
        bool accelerating = curve[0].value < curve[curve.length - 1].value;
        float start = 0f, end = curve.GetDuration();
        // Pathfinding logic for a way to find time (linear interpolation)
        while (start <= end)
        { 
            float time = (start + end) / 2f;
            float currentValue = curve.Evaluate(time);
            if (Mathf.Abs(value - currentValue) <= MAX_DISTANCE)
                return time;
            else if (value > currentValue)
            {
                if (accelerating)
                    start = time;
                else
                    end = time;
            }
            else if (value < currentValue)
            {
                if (accelerating)
                    end = time;
                else
                    start = time;
            }
        }
        
        // In case finding time is not possible
        Debug.LogError("AnimationCurveExtensions.GetDuration() ERROR: Time not found for value.");
        return -1;
    }
}