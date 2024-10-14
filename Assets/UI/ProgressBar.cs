using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    private TextMeshProUGUI _textUI;
    private float _value;
    private readonly List<char> _bar = new List<char>();

    /// <summary>
    /// Returns current percentage
    /// </summary>
    public float Value => _value;
    
    private void Awake()
    {
        _textUI = GetComponentInChildren<TextMeshProUGUI>();
        SetValue(0);
    }

    /// <summary>
    /// Set progress bar to given percentage and updates the UI
    /// </summary>
    /// <param name="value">Percentage from <c>0f</c> to <c>1f</c></param>
    public void SetValue(float value)
    {
        _value = Mathf.Clamp01(value);

        var currentProgress = 0f;
        _bar.Clear();

        // 100% exception
        // I know I can probably do this more cleanly but fuck this progress bar -x
        if (_value >= 1)
        {
            for (var i = 0; i < 10; i++) _bar.Add('\u2588');
            _textUI.text = new string(_bar.ToArray());
            return;
        }

        while (currentProgress < _value)
        {
            if (currentProgress + 10/100f <= _value)
            {
                _bar.Add('\u2588');
                currentProgress += 1/10f;
            }
            else if (currentProgress + 7.5/100f <= _value)
            {
                _bar.Add('\u2593');
                currentProgress += 75/1000f;
            }
            else if (currentProgress + 5/100f <= _value)
            {
                _bar.Add('\u2592');
                currentProgress += 5 / 100f;
            }
            else if (currentProgress + 2.5/100f <= _value)
            {
                _bar.Add('\u2591');
                currentProgress += 25/1000f;
            }
            else 
            {
                break;
            }
        } 
        
        _textUI.text = new string(_bar.ToArray());
    }
}
