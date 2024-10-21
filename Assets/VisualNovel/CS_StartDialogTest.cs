using CC.DialogueSystem;
using UnityEngine;

public class NovelStartTest : MonoBehaviour
{
    [SerializeField] private string dialogueName = "VNC_TestConversation";
    
    // Start is called before the first frame update
    private void Start()
    {
        DialogueController.Instance.StartConversation(dialogueName);
    }
}
