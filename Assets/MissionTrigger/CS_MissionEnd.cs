using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionEnd : MonoBehaviour
{
    private BoxCollider2D _endCollider;

    private void Start()
    {
        _endCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        // TODO: Go to score screen
        Debug.Log($"[MissionEnd] {this.name}: Level Ended");
        SceneManager.LoadScene("MainMenu");
    }
}
