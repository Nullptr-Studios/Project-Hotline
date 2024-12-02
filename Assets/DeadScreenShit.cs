using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadScreenShit : MonoBehaviour
{
    public delegate void DeadScreen(bool dead);
    public static DeadScreen OnDeadScreen;

    private void OnEnable()
    {
        OnDeadScreen?.Invoke(true);
    }
    
    private void OnDisable()
    {
        OnDeadScreen?.Invoke(false);
    }
}
