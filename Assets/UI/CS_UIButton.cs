/*
 *  This class has to be paired with UIButtonController.
 *  To use this class, inherit a child from it and override the Perform() function, this is what the 
 *  UIButtonController will call when an action is performed.
 *
 *  Made by: Xein
 */

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    public Color textColor = Color.blue;
    [NonSerialized] public int ID;
    [SerializeField] private int maxLabelLength = 12;
    [SerializeField] private UnityEvent perform;

    private bool _disableMouse;

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image background;
    
    // Start is called before the first frame update
    private void Start()
    {
        RemoveFocus();
    }

    public void OnMouseOver()
    {
        if (_disableMouse) return;
        
        transform.parent.GetComponent<UIButtonController>().SetFocusByMouse(ID);
        _disableMouse = true;
    }

    public void OnMouseExit()
    {
        _disableMouse = false;
    }

    public virtual void Perform()
    {
        perform?.Invoke();
    }

    /// <summary>
    /// Set button label to given text
    /// </summary>
    /// <param name="value">Text</param>
    public IEnumerator SetText(string value)
    {
        text.text = value;
        if (text.text.Length > maxLabelLength) Debug.LogWarning($"[UIButton] {name}: Text is too long");
        yield return null;
    }

    /// <summary>
    /// UI changes that indicate the response has focus
    /// </summary>
    public void SetFocus()
    {
        text.color = textColor;
        background.enabled = true;
    }

    /// <summary>
    /// UI changes that indicate the response lost focus
    /// </summary>
    public void RemoveFocus()
    {
        text.color = Color.white;
        background.enabled = false;
    }
}
