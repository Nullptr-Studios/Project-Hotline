using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsRoll : MonoBehaviour
{
    
    public MusicManager musicManager;
    
    int endingLocation = 0;

    private bool doOnce = true;
    
    private bool start = false;
    
    // Start is called before the first frame update
    void Start()
    {
        start = false;
        doOnce = true;
        musicManager.PlayCreditsMusic();
        
        
        endingLocation = (int)transform.position.y;
        transform.position = Vector3.zero;
        
        Invoke(nameof(starting), 2f);
    }

    private void starting()
    {
        start = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y > endingLocation && doOnce)
        {
            musicManager.StopCreditsMusic();
            doOnce = false;
            Invoke(nameof(LoadPC), 2f);
        }
        else if(start)
        {
            transform.position = new Vector3(0, transform.position.y + 0.5f * Time.deltaTime, 0);
        }
    }

    private void LoadPC()
    {
        SceneManager.LoadScene("TheBestFuckingMenu");
    }
}
