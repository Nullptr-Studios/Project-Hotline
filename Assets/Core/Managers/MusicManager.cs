using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class MusicManager : MonoBehaviour
{
    
    //@TODO: Implement a way to change the music dynamically, for now this works
    public EventReference sceneMusic;
    
    private FMOD.Studio.EventInstance _gameMusicInstance;

    
    // Start is called before the first frame update
    void Start()
    {
        _gameMusicInstance = FMODUnity.RuntimeManager.CreateInstance(sceneMusic);
        _gameMusicInstance.start();
    }

    private void OnDisable()
    {
        _gameMusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
