using UnityEngine;
using UnityEngine.UI;

public class PsychosisBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private float _timer;
    [SerializeField] float currentValue;
    [SerializeField] private float _coolDownTime;
    [SerializeField] private float _coolDownSpeed;
    [SerializeField] private float _barAdd;
    // Start is called before the first frame update
    void Start()
    {
        slider.value = currentValue;
        _barAdd = 1.0f;
    }
    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if (currentValue >= slider.maxValue)
        {
            NoReturnPoint();
        }
        //if the time is bigger than cooldown, it decreases the bar value and restarts the cooldown
        else if (_timer >= _coolDownTime)
        {
            currentValue -= _coolDownTime / _coolDownSpeed;
            currentValue = Mathf.Clamp(currentValue, slider.minValue, slider.maxValue);
            slider.value = currentValue;
        }
    }
    /// <summary>
    /// On a kill, it increases the bar value
    /// </summary>
    public void OnKill ()
    {
        currentValue += _barAdd;
        currentValue = Mathf.Clamp(currentValue, slider.minValue, slider.maxValue);
        slider.value = currentValue;
        _timer = 0f;
    }
/// <summary>
/// Leaves the value as maxed
/// </summary>
    private void NoReturnPoint()
    {
        currentValue = slider.maxValue;
    }
}