using System.Collections.Generic;
using ToolBox.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MissionEnd : MonoBehaviour
{
    private BoxCollider2D _endCollider;
    [SerializeField] private UnityEvent onLevelFinished;

    private void Start()
    {
        _endCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        // TODO: Go to score screen
        Debug.Log($"[MissionEnd] {this.name}: Level Ended");
        var _ls = DataSerializer.Load<List<bool>>(SaveKeywords.LevelPassed);
        if (_ls[SceneManager.GetActiveScene().buildIndex - 1] == false)
        {
            _ls[SceneManager.GetActiveScene().buildIndex - 1] = true;
            DataSerializer.Save(SaveKeywords.LevelScore, _ls);
        }
        onLevelFinished.Invoke();
    }
}
