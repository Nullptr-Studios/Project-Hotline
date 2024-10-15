using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    public Color textColor = Color.blue;
    [NonSerialized] public int ID;

    private bool _disableMouse;

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image background;
    
    // Start is called before the first frame update
    private void Start()
    {
        RemoveFocus();
    }

    private void OnMouseOver()
    {
        if (_disableMouse) return;
        
        transform.parent.SendMessage("SetFocusByMouse", ID);
        _disableMouse = true;
    }

    private void OnMouseExit()
    {
        _disableMouse = false;
    }

    public virtual void Perform() { }

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
