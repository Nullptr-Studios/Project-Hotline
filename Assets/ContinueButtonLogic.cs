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
        var image = GetComponent<Image>();
        image.sprite = PlayerMovement.Controller switch
        {
            EController.KeyboardMouse => keyboard,
            EController.Dualsense => dualsense,
            _ => xbox
        };
    }
}
