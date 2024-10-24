using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int PlayerKills;
    public static int PlayerDeaths;
    public static float ScoreAdder;
    public static float ScoreSubstractor;
    public static float Score;
    private float _startTime;
    private string _scoreText;
    [SerializeField] float finalTime;
    [SerializeField] float normalTime;
    [SerializeField] float slowTime;
    
    
    public static void AddKill()
    {
        PlayerKills++;
        Score += ScoreAdder;
    }

    public static void AddDeath()
    {
        PlayerDeaths++;
        Score -= ScoreSubstractor;
    }
    private void Awake()
    {
        Score = 0;
        DontDestroyOnLoad(gameObject);
        _startTime = Time.time;
    }
    public float CalcScore()
    { 
        finalTime = Time.time - _startTime;
        if (normalTime <= finalTime)
        {
            Score = Score * finalTime;
            return Score;
        }
        if (finalTime <= slowTime)
        {
            Score = Score / finalTime;
            return Score;
        }
        else
        {
            return Score;
        }
    }
    
}

