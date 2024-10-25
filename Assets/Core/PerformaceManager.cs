using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceManager : MonoBehaviour
{
    [Header("Blood")]
    public int maxBlood = 1000;
    public int bloodThreshold = 10;

    [Header("Corpses")]
    public int maxCorpses = 100;
    public int corpseThreshold = 10;

    public float startingFPS = 60.0f;
    
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool log;
#endif

    public bool CheckBlood()
    {
        GameObject[] arr = GameObject.FindGameObjectsWithTag("Blood");

        if (arr.Length > maxBlood+bloodThreshold)
        {
            int diference = arr.Length - (maxBlood); //threshold
            for (int i = 0; i < diference - 1; i++)
            {
                arr[i].AddComponent<FloorSplatterDisapear>();
            }
#if UNITY_EDITOR
            if(log)
                Debug.LogWarning("PerformanceManager: Vanishing " + diference + " blood instances");
#endif
            return true;
        }
        return false;
    }
    public bool CheckCorpses()
    {
        GameObject[] arr = GameObject.FindGameObjectsWithTag("Corpse");

        if (arr.Length > maxCorpses + corpseThreshold)
        {
            int diference = arr.Length - (maxCorpses); //threshold
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

    public bool CheckFPS()
    {
        if (Time.deltaTime > (double)(1 / startingFPS))
        {
            
#if UNITY_EDITOR
            if(log)
                Debug.LogWarning("PerformanceManager: Low FPS detected!!!");
#endif
            return true;
        }

        return false;
    }

    public void Cleanup()
    {
        bool blood = CheckBlood();
        bool corpses = CheckCorpses();

        CheckFPS();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Cleanup", 5, 5);
    }
    
}
