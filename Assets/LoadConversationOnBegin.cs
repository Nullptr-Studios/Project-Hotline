using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadConversationOnBegin : MonoBehaviour
{
    private void Start()
    {
        GetComponent<ConversationHandler>().StartVNConversation(0);
    }
}
