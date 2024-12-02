using System;
using System.Collections;
using System.Collections.Generic;
using ToolBox.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartCanvas : MonoBehaviour
{
    public UISlider sliderLevels;
    public UISlider sliderDifficulties;
    
    private PlayerIA _input;
    
    // xein if youre reading this, please dont kill me, im tired and i need to sleep :( i have not slept in 2 days and i need to sleep for my health and sanity :( so yeah, please dont kill me :( and sorry for the mess :( i will fix it tomorrow :( i promise :( sincerely, a tired and sleepy person :( im so tired this comment was written by copilot :(
    
    // Start is called before the first frame update
    void Awake()
    {
        _input = new PlayerIA();
    }

    private void OnEnable()
    {
        _input.UI.Accept.performed += Accept;
        _input.UI.Accept.Enable();
    }

    private void Accept(InputAction.CallbackContext obj)
    {
        EDifficulty difficulty = (EDifficulty) sliderDifficulties.CurrentFocus;
        DataSerializer.Save(SaveKeywords.Difficulty, difficulty);
        
        switch (sliderLevels.CurrentFocus+1)
        {
            case 1:
                SceneManager.LoadScene("Tutorial__Main");
                break;
            case 2:
                SceneManager.LoadScene("Alleyways__Main");
                break;
            case 3:
                SceneManager.LoadScene("Metro__Main");
                break;
            case 4:
                SceneManager.LoadScene("Skyscraper__Main");
                break;
            case 5:
                SceneManager.LoadScene("Club__Main");
                break;
            case 6:
                SceneManager.LoadScene("Containers__Main");
                break;
            case 7:
                SceneManager.LoadScene("Mansion__Main");
                break;
            case 8:
                SceneManager.LoadScene("Motel__Main");
                break;
            case 9:
                SceneManager.LoadScene("Hideout__Main");
                break;
            default:
                SceneManager.LoadScene("Tutorial__Main");
                break;
        }
    }

    private void OnDisable()
    {
        _input.UI.Accept.Disable();
    }

}
