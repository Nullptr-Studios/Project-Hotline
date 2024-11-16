/*
 *  This class has to be paired with UIButtonController. You must have all the buttons as child gameObjects.
 *  If you need custom behaviour, you can inherit from this class and overwrite the Awake() and PerformAction()
 *  functions.
 *
 *  Made by: Xein
 */

using System.Collections.Generic;
using System.Linq; // .ToList() function
using UnityEngine;
using UnityEngine.InputSystem;

public class UIButtonController : MonoBehaviour
{
    protected int CurrentFocus;
    protected int MaxIndex;
    protected List<UIButton> Buttons;
    private bool _selectPerformed = true;
    
    private PlayerIA _input;

    protected virtual void Awake()
    {
        _input = new PlayerIA();
        _selectPerformed = false;

        Buttons = GetComponentsInChildren<UIButton>().ToList();

        for (var i = 0; i < Buttons.Count; i++)
        {
            Buttons[i].ID = i;
        }
        
        MaxIndex = Buttons.Count;
        CurrentFocus = -1;
    }

    protected virtual void PerformAction(InputAction.CallbackContext context)
    {
        if (CurrentFocus < 0 || CurrentFocus >= MaxIndex) return;
        Buttons[CurrentFocus].Perform();
    }

    private void SetFocus()
    {
        for (var i = 0; i < MaxIndex; i++)
        {
            if (i == CurrentFocus)
                Buttons[i].SetFocus();
            else
                Buttons[i].RemoveFocus();
        }
    }

    public void SetFocusByMouse(int ID)
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
