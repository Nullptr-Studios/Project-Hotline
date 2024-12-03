using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class MissionTrigger : MonoBehaviour
{
    [NonSerialized] [CanBeNull] public GameObject Objective;
    [SerializeField] private MissionType type;
    [CanBeNull] public GameObject buttonPrompt;
    [SerializeField] private UnityEvent onLevelFinished;

    private PlayerIA _input;

    private void Awake()
    {
        if (type != MissionType.Action) return;
        
        _input = new PlayerIA();
        // Yes im using a fucking lambda for this -x
        _input.UI.Accept.performed += ctx =>
        {
            onLevelFinished?.Invoke();
        };
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        
        switch (type)
        {
            case MissionType.RetrieveObjective:
                if (Objective == null) return;
                Objective.GetComponent<Collider2D>().enabled = true;
                break;
            case MissionType.Action:
                buttonPrompt!.SetActive(true);
                _input.UI.Accept.Enable();
                break;
        }

    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        
        switch (type)
        {
            case MissionType.RetrieveObjective:
                if (Objective == null) return;
                Objective.GetComponent<Collider2D>().enabled = false;
                break;
            case MissionType.Action:
                buttonPrompt!.SetActive(false);
                _input.UI.Accept.Disable();
                break;
        }

    }

    [Serializable]
    public enum MissionType
    {
        RetrieveObjective,
        Action,
        KillObjective
    }
}
