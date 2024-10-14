using UnityEngine;
using UnityEngine.UI;

public class PsychosisBar : MonoBehaviour
{
    public Slider slider;
    private float _timer;
    public float currentValue;
    public float coolDownTime;
    // Start is called before the first frame update
    void Start()
    {
        slider.value = currentValue;
    }
    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= coolDownTime)
        {
            currentValue -= 1f;
            currentValue = Mathf.Clamp(currentValue, slider.minValue, slider.maxValue);
            slider.value = currentValue;
            _timer = 0f;
        }
    }

    public void OnKill()
    {
        currentValue += 1f;
        currentValue = Mathf.Clamp(currentValue, slider.minValue, slider.maxValue);
        slider.value = currentValue;
        _timer = 0f;
    }
 
}