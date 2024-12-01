using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private static readonly int CloseAnim = Animator.StringToHash("CloseAnim");
    
    private PlayerMovement _playerinput;
    private PlayerIA _input;
    private Animator _animator;

    public delegate void Pause(bool pause);
    public static Pause OnPause;
    
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _playerinput = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        _input = new PlayerIA();
        _input.UI.Cancel.performed += OnContinueButton;
    }

    private void OnContinueButton(InputAction.CallbackContext obj)
    {
        OnContinue();
    }

    private void OnDisable() => Time.timeScale = 1f;

    private void OnEnable()
    {
        _playerinput.OnDisable();
        OnPause?.Invoke(true);
    }

    // This happens when the open animations finish
    public void OnOpen() 
    {
        Time.timeScale = 0f;
        _input.UI.Cancel.Enable();
        transform.localScale = Vector3.one;
    }

    // Continue button
    public void OnContinue()
    {
        Time.timeScale = 1f;
        _playerinput.OnEnable();
        _input.UI.Cancel.Disable();
        _animator.SetTrigger(CloseAnim);
        OnPause?.Invoke(false);
    }

    // Exit to main menu button
    public void OnExit() => GameObject.Find("PA_LevelManager").SendMessage("EndLevelMessage");

    // Restart level button
    public void OnRestart()
    {
        SceneMng.Reload();
        OnContinue();
    }
}
