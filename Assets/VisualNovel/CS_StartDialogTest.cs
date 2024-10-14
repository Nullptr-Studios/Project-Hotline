using CC.DialogueSystem;
using UnityEngine;

public class CS_NovelStartTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DialogueController.Instance.StartConversation("VNC_TestConversation");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
