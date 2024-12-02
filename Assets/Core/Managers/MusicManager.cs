using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class MusicManager : MonoBehaviour
{
    
    class TimelineInfo
    {
        public int currentMusicBar = 0;
        public int currentMusicBeat = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }

    TimelineInfo timelineInfo;
    GCHandle timelineHandle;
    
    public EventReference sceneMusic;
    
    private FMOD.Studio.EventInstance _gameMusicInstance;
    
    private FMOD.Studio.EVENT_CALLBACK beatCallback;
    
    public bool hasDialogMusic = false;
    
    public int lastbar = 0;
    public int lastBeat;

    public delegate void Beat();
    public delegate void Bar();
    public static Bar OnBar;
    public static Beat OnBeat;

    
    // Start is called before the first frame update
    void Start()
    {
        timelineInfo = new TimelineInfo();
        
        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        if (_gameMusicInstance.isValid())
        {
            _gameMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _gameMusicInstance.release();
        }

        _gameMusicInstance = FMODUnity.RuntimeManager.CreateInstance(sceneMusic);
        
        // Pin the class that will store the data modified during the callback
        timelineHandle = GCHandle.Alloc(timelineInfo);
        // Pass the object through the userdata of the instance
        _gameMusicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

        _gameMusicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        
        NovelUIController.OnStartGame += Startscene;
        LoadingScreen.OnFinalizedLoading += endedLoading;
        _gameMusicInstance.setParameterByName("DialogueLoop", 1);
        _gameMusicInstance.setParameterByName("LowPass",0);
        
        PauseMenu.OnPause += Pause;
    }

    private void Pause(bool pause)
    {
        if(pause)
            _gameMusicInstance.setParameterByName("LowPass",1);
        else
            _gameMusicInstance.setParameterByName("LowPass",0);
    }

    private void endedLoading()
    {
        if(hasDialogMusic)
            _gameMusicInstance.start();
    }
    private void OnDisable()
    {
        NovelUIController.OnStartGame -= Startscene;
        _gameMusicInstance.setUserData(IntPtr.Zero);
        _gameMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        timelineHandle.Free();
        hasDialogMusic = false;
        Destroy(this);
    }

    private void Startscene()
    {
        //_gameMusicInstance.start();
        _gameMusicInstance.setParameterByName("DialogueLoop", 0);
        if(!hasDialogMusic)
            _gameMusicInstance.start();
    }
    
    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                {
                    var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                    timelineInfo.currentMusicBar = parameter.bar;
                    timelineInfo.currentMusicBeat = parameter.beat;
                }
                    break;
            }
        }
        return FMOD.RESULT.OK;
    }

    // Update is called once per frame
    void Update()
    {
        if(lastBeat != timelineInfo.currentMusicBeat)
        {
            lastBeat = timelineInfo.currentMusicBeat;
            //Debug.Log("Beat!");
            OnBeat?.Invoke();
            return;
        }
        
        if(lastbar != timelineInfo.currentMusicBar)
        {
            lastbar = timelineInfo.currentMusicBar;
            //Debug.Log("Bar!");
            OnBar?.Invoke();
            return;
        }
        
        
    }
}
