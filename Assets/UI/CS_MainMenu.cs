using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI version;
    
    public GameObject mainMenuCanvas;
    private PlayerIA input;
    
    // Start is called before the first frame update
    void Awake()
    {
        version.text = Application.version;
        input = new PlayerIA();
        input.UI.Accept.performed += Action;
        input.UI.Accept.Enable();
    }

    private void OnDisable()
    {
        input.UI.Accept.Disable();
        input.UI.Accept.performed -= Action;
    }

    private void Action(InputAction.CallbackContext obj)
    {
        mainMenuCanvas.SetActive(true);
        gameObject.SetActive(false);
    }
}
