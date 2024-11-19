using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeControllerPrompts : MonoBehaviour
{
    [SerializeField] private TMP_SpriteAsset keyboard;
    [SerializeField] private TMP_SpriteAsset dualsense;
    [SerializeField] private TMP_SpriteAsset xbox;
    
    private TextMeshProUGUI _text;

    private float timer;
    
    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        CheckControllerPrompts();
        timer = Time.time;
    }

    // TODO: Find a way to do this that doesn't give TERMINAL CANCER
    void Update()
    {
        // At least im gonna put a timer so it isn't as badly optimized
        // I really hope this fucking shit of code doesn't get to prod
        if (!(Time.time - timer > 2)) return;
        
        timer = Time.time;
        CheckControllerPrompts();
    }
    
    private void CheckControllerPrompts()
    {
        _text.spriteAsset = PlayerMovement.Controller switch
        {
            EController.KeyboardMouse => keyboard,
            EController.Dualsense => dualsense,
            EController.Xbox => xbox,
            _ => _text.spriteAsset
        };
    }
}
