using UnityEngine;
using UnityEngine.UI;

public class PsychosisBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private float _timer;
    [SerializeField] private float currentValue;
    [SerializeField] private float coolDownTime;
    [SerializeField] private float coolDownSpeed;
    [SerializeField] private float barAdd;
    // Start is called before the first frame update
    void Start()
    {
        slider.value = currentValue;
    }
    // Update is called once per frame
    void Update()
    {
        slider.value = currentValue;
        _timer += Time.deltaTime;
        if (currentValue >= 1.0f)
        {
            NoReturnPoint();
        }
        //if the time is bigger than cooldown, it decreases the bar value and restarts the cooldown
        else if (_timer > coolDownTime)
        {
            currentValue -= coolDownSpeed * Time.deltaTime; 
            slider.value = currentValue;
            currentValue = Mathf.Clamp(currentValue, 0.0f, 1.0f);
        }
    }
    /// <summary>
    /// On a kill, it increases the bar value
    /// </summary>
    public void OnKill ()
    {
        currentValue += barAdd;
        currentValue = Mathf.Clamp(currentValue, 0f, 1f);
        slider.value = currentValue;
        _timer = 0f;
    }
/// <summary>
/// Leaves the value as maxed
/// </summary>
    private void NoReturnPoint()
    {
        currentValue = 1f;
        slider.value = currentValue;
    }
}