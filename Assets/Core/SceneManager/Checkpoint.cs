using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool DontSetAsLastVisitedLastScene = false;
    
    public List<SceneObject> CheckpointScenes;
    public SceneObject CheckpointActiveScene;

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerWeaponManager playerWeaponManager = other.GetComponent<PlayerWeaponManager>();
            SceneMng.AddCurrentCheckpoint(transform.position, CheckpointScenes, CheckpointActiveScene, playerWeaponManager._heldWeaponGameObject,DontSetAsLastVisitedLastScene);
            
            ScoreManager.Checkpoint();
            
            Destroy(this.gameObject);
            
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
