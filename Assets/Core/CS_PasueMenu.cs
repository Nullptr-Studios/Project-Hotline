using UnityEngine;

public class PasueMenu : MonoBehaviour
{
    private Canvas _canvas;
    private PlayerMovement _input;
    private Animator _animator;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _animator = GetComponentInChildren<Animator>();
        _input = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    /*private void OnFinishAnimation()
    {
        Time.timeScale = 0f;
    }*/

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        _input.OnDisable();
    }

    public void OnContinue()
    { 
        _input.OnEnable();
        _animator.SetTrigger("CloseAnim");
    }

    public void OnExit()
    {
        LevelManager.EndLevel();
    }

    public void OnRestart()
    {
        LevelManager.RestartLevel();
    }
}
