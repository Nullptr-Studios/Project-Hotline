using UnityEngine;
using UnityEngine.InputSystem;

public class MissionTrigger : MonoBehaviour
{
    private PlayerIA _input;
    [SerializeField] private GameObject objective;
    
    private void Start()
    {
        _input = new PlayerIA();
        _input.Gameplay.Interact.performed += GetEndMission;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _input.Gameplay.Interact.Enable(); 
        }
    }

    private void GetEndMission(InputAction.CallbackContext context)
    {
        Debug.Log("Mission Complete");
        // TODO: Equip objective logic
        // TODO: Send message to update mission clear
        Destroy(objective); // Placeholder
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _input.Gameplay.Interact.Disable();
    }
}
