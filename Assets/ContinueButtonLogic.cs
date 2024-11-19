using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueButtonLogic : MonoBehaviour
{
    public Sprite keyboard;
    public Sprite dualsense;
    public Sprite xbox;
    
    
    // Start is called before the first frame update
    void OnEnable()
    {
        Image _image = GetComponent<Image>();
        _image.sprite = PlayerMovement.Controller switch
        {
            EController.KeyboardMouse => keyboard,
            EController.Dualsense => dualsense,
            EController.Xbox => xbox,
            _ => xbox
        };
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
