using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class RaveLights : MonoBehaviour
{
    private Light2D _light2D;
    public bool onBeat = true;
    // Start is called before the first frame update
    void Start()
    {
        _light2D = GetComponent<Light2D>();
        
        if (onBeat)
        {
            MusicManager.OnBeat += Beat;
        }
        else
        {
            MusicManager.OnBar += Beat;
        }
            
    }

    private void OnDisable()
    {
        if (onBeat)
        {
            MusicManager.OnBeat -= Beat;
        }
        else
        {
            MusicManager.OnBar -= Beat;
        }
    }

    private void Beat()
    {
        _light2D.color = Random.ColorHSV(0,1,1,1,1,1,1,1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
