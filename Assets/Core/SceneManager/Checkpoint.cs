using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    
    public List<string> CheckpointScenes;
    public string CheckpointActiveScene;

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneMng.AddCurrentCheckpoint(transform.position, CheckpointScenes, CheckpointActiveScene);
            this.enabled = false;
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Color boxCol = Color.green;
        boxCol.a = .25f;
        Gizmos.color = boxCol;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
    
#endif
}
