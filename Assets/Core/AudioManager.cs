using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public EventReference MusicReference;

    private static EventInstance _gameMusicInstance;

    // Start is called before the first frame update
    void Start()
    {
        _gameMusicInstance = FMODUnity.RuntimeManager.CreateInstance(MusicReference);
        _gameMusicInstance.start();
    }

    // Update is called once per frame
    void Update()
    {

    }
    static void StopMusic()
    {
        _gameMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

}
