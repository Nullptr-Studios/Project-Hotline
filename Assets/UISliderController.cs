using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class UISliderController : MonoBehaviour
{
    
    private PlayerIA _input;
    private bool _selectPerformed;
    
    private List<UISlider> Sliders;
    private int MaxIndex;
    
    private int CurrentFocus;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    
    
    protected virtual void Awake()
    {
        _input = new PlayerIA();
        _selectPerformed = false;

        Sliders = GetComponentsInChildren<UISlider>().ToList();

        for (var i = 0; i < Sliders.Count; i++)
        {
            Sliders[i].ID = i;
        }
        
        MaxIndex = Sliders.Count;
        CurrentFocus = -1;
    }
    
    protected void SetFocus()
    {
        for (var i = 0; i < MaxIndex; i++)
        {
            if (i == CurrentFocus)
                Sliders[i].SetFocus();
            else
                Sliders[i].RemoveFocus();
        }
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
    
    protected virtual void PerformAction(InputAction.CallbackContext context)
    {
        
    }
    
    #region INPUT_SYSTEM

    protected virtual void OnEnable()
    {
        _input.UI.Select.Enable();
        _input.UI.Select.performed += Select; 
        _input.UI.Select.canceled += Select;
        _input.UI.Accept.Enable();
        _input.UI.Accept.performed += PerformAction;
        _input.UI.Cancel.Enable();

        CurrentFocus = -1;
    }

    protected virtual void OnDisable()
    {
        _input.UI.Select.Disable();
        _input.UI.Accept.Disable();
        _input.UI.Cancel.Disable();
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }
}
