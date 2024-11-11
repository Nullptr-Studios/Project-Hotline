using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages performance by checking and cleaning up blood, corpses, and monitoring FPS.
/// </summary>
public class PerformanceManager : MonoBehaviour
{
    [Header("Blood")]
    /// <summary>
    /// The maximum number of blood instances allowed.
    /// </summary>
    public int maxBlood = 1000;

    /// <summary>
    /// The threshold for blood instances before cleanup is triggered.
    /// </summary>
    public int bloodThreshold = 10;

    [Header("Corpses")]
    /// <summary>
    /// The maximum number of corpses allowed.
    /// </summary>
    public int maxCorpses = 100;

    /// <summary>
    /// The threshold for corpses before cleanup is triggered.
    /// </summary>
    public int corpseThreshold = 10;

    /// <summary>
    /// The starting frames per second (FPS) value.
    /// </summary>
    public float startingFPS = 60.0f;

#if UNITY_EDITOR
    [Header("Debug")]
    /// <summary>
    /// Indicates if debug logging is enabled.
    /// </summary>
    [SerializeField] private bool log;
#endif

    /// <summary>
    /// Checks the number of blood instances and triggers cleanup if necessary.
    /// </summary>
    /// <returns>True if cleanup was performed, otherwise false.</returns>
    public bool CheckBlood()
    {
        GameObject[] arr = GameObject.FindGameObjectsWithTag("Blood");

        if (arr.Length > maxBlood + bloodThreshold)
        {
            int diference = arr.Length - maxBlood; // threshold
            for (int i = 0; i < diference - 1; i++)
            {
                arr[i].AddComponent<FloorSplatterDisapear>();
            }
#if UNITY_EDITOR
            if (log)
                Debug.LogWarning("PerformanceManager: Vanishing " + diference + " blood instances");
#endif
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks the number of corpses and triggers cleanup if necessary.
    /// </summary>
    /// <returns>True if cleanup was performed, otherwise false.</returns>
    public bool CheckCorpses()
    {
        GameObject[] arr = GameObject.FindGameObjectsWithTag("Corpse");

        if (arr.Length > maxCorpses + corpseThreshold)
        {
            int diference = arr.Length - maxCorpses; // threshold
            for (int i = 0; i < diference - 1; i++)
            {
                ResourceManager.GetCorpsePool().Release(arr[i]);
            }
#if UNITY_EDITOR
            if (log)
                Debug.LogWarning("PerformanceManager: Vanishing " + diference + " blood instances");
#endif
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks the current FPS and logs a warning if it drops below the starting FPS.
    /// </summary>
    /// <returns>True if low FPS was detected, otherwise false.</returns>
    public bool CheckFPS()
    {
        if (Time.deltaTime > (double)(1 / startingFPS))
        {
#if UNITY_EDITOR
            if (log)
                Debug.LogWarning("PerformanceManager: Low FPS detected!!!");
#endif
            return true;
        }

        return false;
    }

    /// <summary>
    /// Performs cleanup by checking blood, corpses, and FPS.
    /// </summary>
    public void Cleanup()
    {
        bool blood = CheckBlood();
        bool corpses = CheckCorpses();

        CheckFPS();
    }

    /// <summary>
    /// Initializes the PerformanceManager and starts the cleanup process.
    /// </summary>
    void Start()
    {
        InvokeRepeating("Cleanup", 5, 5);
    }
}