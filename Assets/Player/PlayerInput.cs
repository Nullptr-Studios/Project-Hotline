using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerIA _input;

    private void Awake()
    {
        _input = new PlayerIA();
    }

    // NOTE: All Actions MUST be enabled AND disabled or code will explode (not joking) -x
    private void OnEnable()
    {
        _input.Gameplay.Debug.performed += OnDebug;
        _input.Gameplay.Debug.Enable();
    }
    
    private void OnDisable()
    {
        _input.Gameplay.Debug.Disable();   
    }
    
    /// <summary>
    /// Logic for the Debug Button
    /// </summary>
    /// <param name="context">Input Context</param>
    private void OnDebug(InputAction.CallbackContext context)
    {
        
#if UNITY_EDITOR
        Debug.Log("Debug Pressed " + context.duration);
#endif
        
    }
}
