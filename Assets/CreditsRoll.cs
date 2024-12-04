using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class CreditsRoll : MonoBehaviour
{
    public EventReference endcreditsmusic;

    private EventInstance _music;
    
    int endingLocation = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        if (_music.isValid())
        {
            _music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _music.release();
        }

        _music = FMODUnity.RuntimeManager.CreateInstance(endcreditsmusic);
        _music.start();
        
        endingLocation = (int)transform.position.y;
        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y > endingLocation)
        {
            _music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            
            
        }
        else
        {
            transform.position = new Vector3(0, transform.position.y + 0.5f * Time.deltaTime, 0);
        }
    }
}
