using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIButtonController : MonoBehaviour
{
    [SerializeField] protected List<UIButton> buttons;

    protected int CurrentFocus;
    protected int MaxIndex;
    private bool _selectPerformed = true;

    
    private PlayerIA _input;

    protected virtual void Awake()
    {
        _input = new PlayerIA();
        _selectPerformed = false;

        for (var i = 0; i < buttons.Count; i++)
        {
            buttons[i].ID = i;
        }
        
        MaxIndex = buttons.Count;
    }

    protected virtual void PerformAction(InputAction.CallbackContext context)
    {
        buttons[CurrentFocus].Perform();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetFocus();
    }

    private void SetFocus()
    {
        for (var i = 0; i < buttons.Count; i++)
        {
            if (i == CurrentFocus)
                buttons[i].SetFocus();
            else
                buttons[i].RemoveFocus();
        }
    }

    private void SetFocusByMouse(int ID)
    {
        CurrentFocus = ID;
        SetFocus();
    }

    private void Select(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().y > 0.8)
        {
            if (_selectPerformed) return;
            
            CurrentFocus = Mathf.Clamp(CurrentFocus + 1, 0, MaxIndex - 1);
            SetFocus();
            _selectPerformed = true;
        }
        else if (context.ReadValue<Vector2>().y < -0.8)
        {
            if (_selectPerformed) return;
            
            CurrentFocus = Mathf.Clamp(CurrentFocus - 1, 0, MaxIndex - 1);
            SetFocus();
            _selectPerformed = true;
        }
        else
            _selectPerformed = false;
    }
    
    #region INPUT_SYSTEM

    private void OnEnable()
    {
        _input.UI.Select.Enable();
        _input.UI.Select.performed += Select; 
        _input.UI.Select.canceled += Select;
        _input.UI.Accept.Enable();
        _input.UI.Accept.performed += PerformAction;
        _input.UI.Cancel.Enable();
    }

    private void OnDisable()
    {
        _input.UI.Select.Disable();
        _input.UI.Accept.Disable();
        _input.UI.Cancel.Disable();
    }

    #endregion
}
