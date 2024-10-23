/*
 *  To use this class just call the BeginTimer() function and pass the time you want the progress bar to last
 * 
 *  Made by: Xein
 */

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private float duration;

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private Image background;
    [SerializeField] private Image divider;

    private bool _active;
    private bool _isInverse;
    private float _value;
    private float _current;
    private float _dividerValue;
    private int _size;
    private readonly List<char> _bar = new List<char>();
    private Animator _animator;
    private static readonly int CloseAnim = Animator.StringToHash("CloseAnim");

    /// <summary>
    /// Returns current percentage
    /// </summary>
    public float Value => _value;

    /// <summary>
    /// Returns if the current progress has surpassed the divider value
    /// </summary>
    public bool AboveDivider => _current > _dividerValue;
    
    private void Awake()
    {
        // Warnings
        if (textUI == null) Debug.LogError($"[ProgressBar] {name}: TextUI not defined");
        if (background == null) Debug.LogWarning($"[ProgressBar] {name}: Background not defined");
        if (divider == null) Debug.LogWarning($"[ProgressBar] {name}: Divider not defined");
        
        _animator = GetComponent<Animator>();
        SetValue(0);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_active) return;
        
        if (!_isInverse)
        {
            _current += Time.deltaTime / duration;
            SetValue(_current);

            if (_current >= 1)
            {
                Hide();
                _active = false;
            }
        }
        else 
        {
            _current += Time.deltaTime / duration;
            SetValue(1 - _current);

            if (1 - _current <= 0)
            {
                Hide();
                _active = false;
            }
        }

        // Color check for under divider value
        if (_current < _dividerValue)
            textUI.color = Color.red;
        else
            textUI.color = Color.white;
    }

    /// <summary>
    /// Initializes the progress bar timer
    /// </summary>
    /// <param name="time">Time to complete</param>
    /// <param name="isInverse">Go from <c>1f</c> to <c>0f</c> </param>
    /// <param name="size">Number of characters in the progress bar</param>
    /// <param name="hasDivider">Tells if bar has a division</param>
    /// <param name="dividerSize">Percentage of the bar division</param>
    public void BeginTimer(float time, bool isInverse = false, int size = 10, bool hasDivider = false, float dividerSize = 0f)
    {
        duration = time;
        _isInverse = isInverse;
        _dividerValue = dividerSize;
        _size = size;

        background.rectTransform.sizeDelta = new Vector2(size * 4, background.rectTransform.sizeDelta.y);

        if (!hasDivider)
        {
            divider.enabled = false;
        }
        else
        {
            if (dividerSize <= 0) Debug.LogWarning("[ProgressBar] Slider divider is on but Size is zero");
            divider.rectTransform.sizeDelta = new Vector2(_dividerValue * size * 4f, divider.rectTransform.sizeDelta.y);
        }

        _current = isInverse ? 1 : 0;
        SetValue(isInverse ? 1 : 0);
        Show();
        _active = true;
    }

    /// <summary>
    /// Set progress bar to given percentage and updates the UI
    /// </summary>
    /// <param name="value">Percentage from <c>0f</c> to <c>1f</c></param>
    private void SetValue(float value)
    {
        _value = Mathf.Clamp01(value);

        var currentProgress = 0f;
        _bar.Clear();

        // 100% exception
        // I know I can probably do this more cleanly but fuck this progress bar -x
        if (_value >= 1)
        {
            for (var i = 0; i < _size; i++) _bar.Add('\u2588');
            textUI.text = new string(_bar.ToArray());
            return;
        }

        while (currentProgress < _value)
        {
            if (currentProgress + 1f/_size <= _value)
            {
                _bar.Add('\u2588');
                currentProgress += 1f/_size;
            }
            else if (currentProgress + 0.75f/_size <= _value)
            {
                _bar.Add('\u2593');
                currentProgress += 0.75f/_size;
            }
            else if (currentProgress + 0.5f/_size <= _value)
            {
                _bar.Add('\u2592');
                currentProgress += 0.5f/_size;
            }
            else if (currentProgress + 0.25f/_size <= _value)
            {
                _bar.Add('\u2591');
                currentProgress += 0.25f/_size;
            }
            else 
            {
                break;
            }
        } 
        
        textUI.text = new string(_bar.ToArray());
    }

    private void Show()
    {
        SetValue(0);
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        _animator.SetTrigger(CloseAnim);
    }
}
