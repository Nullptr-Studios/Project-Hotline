using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceManager : MonoBehaviour
{
    
    public int maxBlood = 1000;
    public int bloodThreshold = 10;

    public float startingFPS = 60.0f;
    
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool log;
#endif

    public void CheckBlood()
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
                Debug.Log("PerformanceManager: Vanishing " + diference + " blood instances");
#endif
            
        }
    }

    public void CheckFPS()
    {
        if (Time.deltaTime > (double)(1 / startingFPS))
        {
            
#if UNITY_EDITOR
            if(log)
                Debug.LogError("PerformanceManager: Low FPS detected!!!");
#endif
            
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CheckBlood", 5, 5);
        InvokeRepeating("CheckFPS", 5, 5);
    }
    
}
