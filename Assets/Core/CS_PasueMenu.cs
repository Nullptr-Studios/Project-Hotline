using UnityEngine;

public class PasueMenu : MonoBehaviour
{
    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    private void OnFinishAnimation()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    public void OnContinue()
    { }

    public void OnExit()
    {
        LevelManager.EndLevel();
    }

    public void OnRestart()
    {
        LevelManager.RestartLevel();
    }
}
