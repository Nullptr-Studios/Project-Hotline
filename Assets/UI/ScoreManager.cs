using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int playerKills = 0;
    public static int playerDeaths = 0;
    public static int scoreAdder;
    public static int scoreSubstractor;
    public static int score = 0;
    private static float _startTime;
    [SerializeField] float _finalTime;
    [SerializeField] 
    
    
    public static void AddKill()
    {
        playerKills++;
        score += scoreAdder;
    }

    public static void AddDeath()
    {
        playerDeaths++;
        score -= scoreSubstractor;
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _startTime = Time.time;
    }
    public float CalcScore()
    { 
        _finalTime = Time.time - _startTime;
    }
}
