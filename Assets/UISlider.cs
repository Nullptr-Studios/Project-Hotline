using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    public ScrollRect scrollRect;
    
    float duration = 0.25f;
    
    public int ID;
    
    public Image background;
    
    private bool focus = false;
    
    private PlayerIA _input;
    private bool _selectPerformed;
    public int CurrentFocus;
    public int MaxIndex;

    public int startingfocus = 0;

    protected List<UIButton> Buttons;
    private Coroutine _scrollCoroutine;

    private float lastPos;

    private void ScrollTo(int index)
    {
        float targetHorizontalPosition;
        if(Buttons[index].transform.localPosition.x > lastPos && index == 0 || index == MaxIndex - 1)
            targetHorizontalPosition = (Buttons[index].transform.localPosition.x + Buttons[index].GetComponent<RectTransform>().rect.width/2) / scrollRect.content.rect.width;
        else
            targetHorizontalPosition = (Buttons[index].transform.localPosition.x - Buttons[index].GetComponent<RectTransform>().rect.width/2) / scrollRect.content.rect.width;
        //float targetHorizontalPosition = (Buttons[index].transform.localPosition.x + Buttons[index].GetComponent<RectTransform>().rect.width/2) / scrollRect.content.rect.width;
        
        
        lastPos = Buttons[index].transform.localPosition.x;
            
        if (_scrollCoroutine != null)
            StopCoroutine(_scrollCoroutine);
            
        _scrollCoroutine = StartCoroutine(LerpToPos(targetHorizontalPosition));
        
    }
    
    private IEnumerator LerpToPos(float targetHorizontalPosition)
    {  
        float elapsedTime = 0f;
        float initialPos = scrollRect.horizontalNormalizedPosition;
        float newPosition = initialPos;

        if (duration > 0)
        {
            while (elapsedTime <= duration)
            {
                newPosition = Mathf.Lerp(newPosition, targetHorizontalPosition, .1f);

                scrollRect.horizontalNormalizedPosition = newPosition;

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        
        scrollRect.horizontalNormalizedPosition = targetHorizontalPosition;
    }
    
    public void SetFocus()
    {
        focus = true;
        background.enabled = true;
    }

    /// <summary>
    /// UI changes that indicate the response lost focus
    /// </summary>
    public void RemoveFocus()
    {
        focus = false;
        background.enabled = true;
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        
        _selectPerformed = false;

        Buttons = GetComponentsInChildren<UIButton>().ToList();
        
        for (var i = 0; i < Buttons.Count; i++)
        {
            Buttons[i].ID = i;
        }
        
        MaxIndex = Buttons.Count;
        CurrentFocus = startingfocus;
        
        SetFocusmine();
        
        _input = new PlayerIA();
        _input.UI.Select.performed += Select;
        _input.UI.Select.Enable();
    }
    
    public int GetCurrentFocus()
    {

        for (int i = 0; i < MaxIndex; i++)
        {
            if(Buttons[i].Focus)
                return i;
        }

        return 0;
    }
    
    protected void SetFocusmine()
    {
        for (var i = 0; i < MaxIndex; i++)
        {
            if (i == CurrentFocus)
                Buttons[i].SetFocus();
            else
                Buttons[i].RemoveFocus();
        }
    }

    public void removeAllFocus()
    {
        for (var i = 0; i < MaxIndex; i++)
        {
            Buttons[i].RemoveFocus();
        }
    }

    private void Select(InputAction.CallbackContext obj)
    {
        if (!focus) 
            return;
        
        if (obj.ReadValue<Vector2>().x > 0.8)
        {
            if (_selectPerformed) return;
            
            CurrentFocus = Mathf.Clamp(CurrentFocus + 1, 0, MaxIndex - 1);
            ScrollTo(CurrentFocus);
            SetFocusmine();
            _selectPerformed = true;
        }
        else if (obj.ReadValue<Vector2>().x < -0.8)
        {
            if (_selectPerformed) return;
            
            CurrentFocus = Mathf.Clamp(CurrentFocus - 1, 0, MaxIndex - 1);
            ScrollTo(CurrentFocus);
            SetFocusmine();
            _selectPerformed = true;
        }
        else
            _selectPerformed = false;
    }
    
    private void OnEnable()
    {
        _input.UI.Select.Enable();
    }
    
    private void OnDisable()
    {
        _input.UI.Select.Disable();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
