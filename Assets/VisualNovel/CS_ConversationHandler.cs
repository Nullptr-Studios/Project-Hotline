/*
 *  Start conversation logic. Send a message to NovelManager to start a conversation. StartVNConversation() uses an
 *  int ID from the ConversationRepo component. StartVNConversationByName lets you start a loaded conversation by
 *  inputting the name instead.
 *
 *  Made by: Xein
 */

using System.Collections;
using System.Collections.Generic;
using CC.DialogueSystem;
using UnityEngine;

public class ConversationHandler : MonoBehaviour
{
    private readonly List<string> _conversations = new List<string>();
    private int ID;
    private ConversationRepo _conversationRepo;

    private void Start()
    {
        _conversationRepo = GetComponent<ConversationRepo>();
        
        foreach (var cnv in _conversationRepo.conversationsToLoad)
        {
            _conversations.Add(cnv.name);
        }
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.O))
    //     {
    //         StartVNConversation(0);
    //     }
    // }

    /// <summary>
    /// Send a message to this script to start a conversation
    /// </summary>
    /// <param name="id">Conversation ID to load</param>
    public void StartVNConversation(int id) => DialogueController.Instance.StartConversation(_conversations[id]);
    
    /// <summary>
    /// Send a message to this script to start a conversation
    /// </summary>
    /// <param name="id">Conversation ID to load</param>
    /// <param name="time">Time before conversation starts</param>
    /// <returns></returns>
    public IEnumerator StartVNConversation(int id, float time)
    {
        yield return new WaitForSeconds(time);
        DialogueController.Instance.StartConversation(_conversations[id]);
    }

    /// <summary>
    /// Send a message to this script to start a conversation using its name
    /// </summary>
    /// <param name="name">Name of the conversation to load</param>
    public void StartVNConversation(string name) => DialogueController.Instance.StartConversation(name);
}
