using System.Collections;
using System.Collections.Generic;
using CC.DialogueSystem;
using UnityEngine.InputSystem;

public class NovelOptionsController : UIButtonController
{
    private NovelUIController _novelController;
    
    protected override void Awake()
    {
        base.Awake();
        MaxIndex = 3;
        
        _novelController = transform.parent.GetComponent<NovelUIController>();
    }

    public IEnumerator ShowOptions(List<Option> options)
    {
        MaxIndex = options.Count;
        
        for (var i = 0; i < Buttons.Count; i++)
        {
            if (i < options.Count)
            {
                StartCoroutine(Buttons[i].SetText(options[i].Text));
            }
            else
            {
                Destroy(Buttons[i].gameObject);
            }
        }

        yield return null;
    }

    protected override void PerformAction(InputAction.CallbackContext context)
    {
        _novelController.OptionButtonClicked(CurrentFocus);
        Destroy(gameObject);
    }
}
