using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static int _playerKills;
    private static int _playerCivilianKills;
    private static int _playerDeaths;

    public static float _killXP = 100f;
    public static float _killCivilianXP = 100f;
    private static float _deathXP = 50f;
    private static float _minTime = 120f;
    private static float _maxTime = 240f;
    private static MathFormula _minFormula;
    private static MathFormula _maxFormula;
    // Don't change this variable, it's only to show text on inspector
    [TextArea] public string text = "Power formula is calculated sign * (x * factor/100) ^ pow. " + 
                                           "Log formula is calculated Log{100pow} (sign * x + 100pow)";
    
    private static int _playerKillsInCheckpoint = 0;

    [Header("Values")]
    [SerializeField] private float killXP;
    [SerializeField] private float killCivilianXP;
    [SerializeField] private float deathXP;
    
    [Header("Timer")] 
    [SerializeField] private Vector2 timeThreshold = new Vector2(120f, 240f);
    [SerializeField] private MathFormula minFormula;
    [SerializeField] private MathFormula maxFormula;

    // Time
    private static float _startTime;

    public void Awake()
    {
        _playerKills = 0;
        _playerCivilianKills = 0;
        _playerDeaths = 0;
        // Sets timer when scene begins
        _startTime = Time.time;
    }

    public void Start()
    {
        // Set up variables because they're static
        _killCivilianXP = killCivilianXP;
        _killXP = killXP;
        _deathXP = deathXP;
        _minTime = timeThreshold.x;
        _maxTime = timeThreshold.y;
        _minFormula = minFormula;
        _maxFormula = maxFormula;
        
        // Setup formulas
        _minFormula.xOffset = _minTime;
        _maxFormula.xOffset = _maxTime;
        
        _playerKills = 0;
    }

    public static void AddCivilianKill()
    {
        _playerCivilianKills++;
    }

    public static void Checkpoint()
    {
        _playerKillsInCheckpoint = _playerKills;
    }
    
    public static void Restart()
    {
        _playerKills = _playerKillsInCheckpoint;
    }

    public static void AddKill()
    {
        _playerKills++;
    }

    public static void AddDeath()
    {
        _playerDeaths++; 
    }

    public static Score CalculateScore()
    {
        var finalScore = new Score
        {
            Time = Time.time - _startTime,
            Kills = _playerKills,
            Deaths = _playerDeaths,
            
            Value = (_playerKills * _killXP + _playerCivilianKills * _killCivilianXP) - _playerDeaths * _deathXP
        };

        if (finalScore.Time < _minTime)
        {
            finalScore.Value = _minFormula.Calculate(finalScore.Value);
        }
        else if (finalScore.Time > _maxTime)
        {
            finalScore.Value = _maxFormula.Calculate(finalScore.Value);
        }

        return finalScore;
    }

    /// <summary>
    /// Struct in charge of calculating the formula
    /// </summary>
    [Serializable] private struct MathFormula
    {
        public FormulaType type;
        public int sign;
        [Tooltip("This value is multiplied by 100 on power formulas")] public float factor;
        [Tooltip("This value is divided by 100 on Log formulas")] public float pow;
        [NonSerialized] public float xOffset;
        
        public float Calculate(float x)
        {
            return type switch
            {
                FormulaType.Power => (sign * Mathf.Pow(((x - xOffset) * factor/100), pow)) + 1,
                FormulaType.Log => Mathf.Log((sign * x + pow*100 + xOffset), pow*100),
                _ => -1
            };
        }
    }

    private enum FormulaType
    {
        Power, Log
    }

}

/// <summary>
/// Score data
/// </summary>
public struct Score
{
    /// <summary>
    /// Actual score value
    /// </summary>
    public float Value;
    /// <summary>
    /// Time for level completion
    /// </summary>
    public float Time;
    /// <summary>
    /// Enemies killed
    /// </summary>
    public int Kills;
    /// <summary>
    /// Innocents killed
    /// </summary>
    public int InnocentKills; //TODO: Implement this
    /// <summary>
    /// Times the player has died
    /// </summary>
    public int Deaths;
}
