using System;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private BoxCollider2D missionTrigger;
    [SerializeField] private BoxCollider2D endTrigger;
    [SerializeField] private MissionObjective objective;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool logMissionEnd;
#endif
    
    // Start is called before the first frame update
    void Start()
    {
        // Log errors
        if (missionTrigger == null || endTrigger == null)
        {
            Debug.LogError($"[LevelManager] {name}: Error with colliders. Level unwinnable.");
            this.enabled = false;
        }
        
        if (objective == null)
        {
            Debug.LogError($"[LevelManager] {name}: Objective mot found. Level unwinnable.");
            this.enabled = false;
        }
        
        missionTrigger.GetComponent<MissionTrigger>().Objective = objective.gameObject;
    }

    public void CompleteMission()
    {
        endTrigger.enabled = true;
        missionTrigger.enabled = false;

#if UNITY_EDITOR
        if (logMissionEnd) Debug.Log($"[LevelManager] {name}: Mission completed");
#endif
        
    }

    public void PauseMission()
    {
        endTrigger.enabled = false;

#if UNITY_EDITOR
        if (logMissionEnd) Debug.Log($"[LevelManager] {name}: Mission paused");
#endif
        
    }
}
