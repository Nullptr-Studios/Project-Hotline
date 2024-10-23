using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int playerKills = 0;
    public static int playerDeaths = 0;

    public static int score = 0;
    
    public static void AddKill()
    {
        playerKills++;
    }

    public static void AddDeath()
    {
        playerDeaths++; 
    }

}
