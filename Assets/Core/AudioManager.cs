using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    private static EventInstance _gameMusicInstance;

    // Start is called before the first frame update
    void Start()
    {
        _gameMusicInstance = FMODUnity.RuntimeManager.CreateInstance(Sounds[""]);
        _gameMusicInstance.start();
    }

    // Update is called once per frame
    void Update()
    {

    }
    static void StopMusic()
    {
        _gameMusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

}
