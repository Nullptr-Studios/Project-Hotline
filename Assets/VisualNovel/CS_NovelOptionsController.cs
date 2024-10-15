using UnityEngine;
using UnityEngine.InputSystem;

public class NovelOptionsController : UIButtonController
{
    protected override void Awake()
    {
        base.Awake();
        MaxIndex = 6;
    }

    protected override void PerformAction(InputAction.CallbackContext context)
    {
        Debug.Log("Perform action: " + CurrentFocus);
    }
}
