using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using STOP_MODE = FMOD.Studio.STOP_MODE;

/// <summary>
/// Contains the functionality for audio in the game
/// </summary>
public class AudioManager : MainUniversalManagerFramework
{
    public static AudioManager Instance;

    [SerializeField] private bool _doesShowAudioDebug;

    [Space]
    [Header("VCA Paths")]
    [SerializeField] private string _generalStartingVCAPath;
    [SerializeField] private string _masterVCAPath;
    [SerializeField] private string _musicVCAPath;
    [SerializeField] private string _soundEffectVCAPath;
    
    private VCA _activeVca;
    
    [Space]
    [Header("Bus Paths")]
    [SerializeField] private string _generalStartingBusPath;
    [SerializeField] private string _pausableBusPath;
    [SerializeField] private string _unpausableBusPath;
    
    [Space] 
    [Header("Bus Settings")] 
    [SerializeField] private float _defaultPausableBusFadeTime;
    
    private Bus _pausableAudioBus;

    private float _pausableBusCurrentVolume = 1;
    private Coroutine _pausableAudioVolumeCoroutine;
    
    #region InitialValues

    private void SetInitialAudioVolumeValues()
    {
        _activeVca = GetVCAFromAudioVCAType(EAudioVCAType.Master);
        _activeVca.setVolume(SaveManager.Instance.GetMasterVolume());
        
        _activeVca = GetVCAFromAudioVCAType(EAudioVCAType.Music);
        _activeVca.setVolume(SaveManager.Instance.GetMusicVolume());
        
        _activeVca = GetVCAFromAudioVCAType(EAudioVCAType.SoundEffect);
        _activeVca.setVolume(SaveManager.Instance.GetSFXVolume());
    }
    #endregion
    
    #region AudioReferences
    
    [Space]
    [Header("Boss Audio")]
    public GeneralBossAudio GeneralBossAudio;
    public SpecificBossAudio[] AllSpecificBossAudio;
    
    [Space]
    [Header("Hero Audio")]
    public GeneralHeroAudio GeneralHeroAudio;
    public SpecificHeroAudio[] AllSpecificHeroAudio;
    
    [Space]
    [Header("User Interface")]
    public UserInterfaceAudio UserInterfaceAudio;

    [Space] 
    [Header("Music")] 
    public SpecificAudio[] AllMusic;
    
    public const int MAIN_MENU_MUSIC_ID = 0;
    public const int SELECTION_SCENE_MUSIC_ID = 1;

    private int _currentMusicID = -1;
    private EventInstance _currentMusicInstance;
    private Coroutine _musicChangeCoroutine;
    private Coroutine _musicVolumeChangeCoroutine;
    private SpecificAudio _currentMusic;
    
    private Dictionary<EventInstance, Coroutine> _changeInstanceVolumeDictionary = new Dictionary<EventInstance, Coroutine>();
    
    private Dictionary<SpecificAudio, EventInstance> _referenceInstanceDictionary = new Dictionary<SpecificAudio, EventInstance>();
    
    #endregion
    
    #region CreateInstance
    /// <summary>
    /// Creates an event instance from a specific audio using the default track choice.
    /// </summary>
    /// <param name="specificAudio"></param>
    /// <param name="eventInstance"></param>
    /// <returns> If the Event Instance was successfully created. </returns>
    public bool CreateInstanceFromSpecificAudio(SpecificAudio specificAudio, out EventInstance eventInstance)
    {
        return CreateInstanceFromReference(specificAudio.GetAudioTrackFromDefault(), out eventInstance);
    }
    
    /// <summary>
    /// Creates an event instance from a specific audio using a specific track choice.
    /// </summary>
    /// <param name="specificAudio"></param>
    /// <param name="trackChoice"></param>
    /// <param name="eventInstance"></param>
    /// <returns> If the Event Instance was successfully created. </returns>
    public bool CreateInstanceFromSpecificAudio(SpecificAudio specificAudio, ESpecificAudioTrackChoice trackChoice, out EventInstance eventInstance)
    {
        return CreateInstanceFromReference(specificAudio.GetAudioTrackFromTrackChoice(trackChoice), out eventInstance);
    }
    
    /// <summary>
    /// Creates an event reference
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="eventInstance"></param>
    /// <returns> If the Event Instance was successfully created. </returns>
    public bool CreateInstanceFromReference(EventReference reference, out EventInstance eventInstance)
    {
        if (reference.IsUnityNull())
        {
            AudioDebug("No reference found when creating instance from reference: " + reference.Guid);
            eventInstance = new EventInstance();
            return false;
        }
        eventInstance = RuntimeManager.CreateInstance(reference);
        return true;
    }
    
    #endregion CreateInstance

    #region OneShot
    public bool PlayOneShotFromSpecificAudio(SpecificAudio specificAudio)
    {
        return PlaySpecificAudioAsOneShot(specificAudio.GetAudioTrackFromDefault());
    }
    
    public bool PlayOneShotFromSpecificAudio(SpecificAudio specificAudio, ESpecificAudioTrackChoice trackChoice)
    {
        return PlaySpecificAudioAsOneShot(specificAudio.GetAudioTrackFromTrackChoice(trackChoice));
    }
    
    public bool PlaySpecificAudioAsOneShot(EventReference eventReference)
    {
        if (eventReference.IsUnityNull())
        {
            AudioDebug("No reference found when playing one shot: " + eventReference.Guid);
            return false;
        }
        
        RuntimeManager.PlayOneShot(eventReference);
        return true;
    }
    
    #endregion OneShot

    #region PlayAudio
    public bool PlaySpecificAudio(EventReference eventReference, out EventInstance eventInstance)
    {
        if (eventReference.IsUnityNull())
        {
            AudioDebug("No reference found when playing specific audio: " + eventReference.Guid);
            eventInstance = new EventInstance();
            return false;
        }

        if (CreateInstanceFromReference(eventReference, out eventInstance))
        {
            eventInstance.start();
        }

        return true;
    }

    public bool PlaySpecificAudio(SpecificAudio specificAudio)
    {
        return PlaySpecificAudio(specificAudio, out EventInstance eventInstance);
    }

    public bool PlaySpecificAudio(SpecificAudio specificAudio, out EventInstance eventInstance)
    {
        return PlaySpecificAudio(specificAudio,specificAudio.DefaultPlayType,specificAudio.DefaultAudioChoice, true, out eventInstance);
    }

    public bool PlaySpecificAudio(SpecificAudio specificAudio, ESpecificAudioPlayType playType,
        ESpecificAudioTrackChoice trackChoice, bool doesAddToVolumeAdjustmentDictionary, out EventInstance eventInstance)
    {
        if (!specificAudio.HasAudioTracks())
        {
            AudioDebug("No audio tracks found when playing specific audio");
            eventInstance = new EventInstance();
            return false;
        }
        
        switch (playType)
        {
            case ESpecificAudioPlayType.OneShot:
                eventInstance = new EventInstance();
                return PlayOneShotFromSpecificAudio(specificAudio, trackChoice);
            case ESpecificAudioPlayType.Instance:
                bool succeeded = StartSpecificAudioInstance(specificAudio, trackChoice,doesAddToVolumeAdjustmentDictionary,out eventInstance);
                
                // If this audio cancels previous instances of itself on play
                if (specificAudio.DoesCancelPreviousInstancesOfSpecificAudioOnPlay)
                {
                    // If the previous instance exists in the dictionary
                    if (_referenceInstanceDictionary.TryGetValue(specificAudio, out EventInstance previousInstance))
                    {
                        StopAndReleaseAudioInstance(previousInstance);
                    }
                    // Add the new value into the dictionary
                    _referenceInstanceDictionary[specificAudio] = eventInstance;
                }

                return succeeded;
        }
        eventInstance = new EventInstance();
        return false;
    }
    #endregion PlayAudio

    public bool StartSpecificAudioInstance(SpecificAudio specificAudio,ESpecificAudioTrackChoice trackChoice,
        bool doesAddToVolumeAdjustmentDictionary, out EventInstance eventInstance)
    {
        if (CreateInstanceFromReference(specificAudio.GetAudioTrackFromTrackChoice(trackChoice), out eventInstance))
        {
            eventInstance.start();
            if (specificAudio.DefaultInstanceFadeInTime > 0)
            {
                eventInstance.setVolume(0);
                StartAdjustInstanceVolumeOverTime(eventInstance, false, doesAddToVolumeAdjustmentDictionary, 1,
                    specificAudio.DefaultInstanceFadeInTime);
            }

            return true;
        }

        return false;
    }

    public bool StopSpecificAudioInstance(EventInstance eventInstance, bool releaseAfter)
    {
        if (eventInstance.IsUnityNull() || !eventInstance.isValid())
        {
            AudioDebug("No instance found when stopping specific audio");
            return false;
        }
        

        eventInstance.stop(STOP_MODE.IMMEDIATE);
        if (releaseAfter)
        {
            eventInstance.release();
        }

        return true;
    }

    public void StopAndReleaseAudioInstance(EventInstance eventInstance)
    {
        if (!eventInstance.isValid())
        {
            AudioDebug("No instance found when stopping and releasing specific audio");
            return;
        }
        
        eventInstance.stop(STOP_MODE.IMMEDIATE);
        eventInstance.release();
    }
    
    public void StartFadeOutStopInstance(EventInstance eventInstance, SpecificAudio specificAudio)
    {
        StartCoroutine(FadeOutStopInstance(eventInstance,true,false,0,specificAudio.DefaultInstanceFadeOutTime));
    }
    
    public void StartFadeOutStopInstance(EventInstance eventInstance, float adjustTime)
    {
        StartCoroutine(FadeOutStopInstance(eventInstance,true,false,0,adjustTime));
    }

    public void StartFadeOutStopInstance(EventInstance eventInstance,
        bool doesStopPreviousVolumeAdjustment, bool doesAddToVolumeAdjustmentDictionary, float adjustTime)
    {
        StartCoroutine(FadeOutStopInstance(eventInstance, doesStopPreviousVolumeAdjustment,
            doesAddToVolumeAdjustmentDictionary, 0, adjustTime));
    }

    private IEnumerator FadeOutStopInstance(EventInstance eventInstance, 
        bool doesStopPreviousVolumeAdjustment, bool doesAddToVolumeAdjustmentDictionary, float endVolume, float adjustTime)
    {
        StartAdjustInstanceVolumeOverTime(eventInstance, doesStopPreviousVolumeAdjustment, doesAddToVolumeAdjustmentDictionary, endVolume, adjustTime);
        yield return new WaitForSeconds(adjustTime);
        StopSpecificAudioInstance(eventInstance, doesAddToVolumeAdjustmentDictionary);
    }

    public Coroutine StartAdjustInstanceVolumeOverTime(EventInstance eventInstance, 
        bool doesStopPreviousVolumeAdjustment, bool doesAddToVolumeAdjustmentDictionary, float endVolume, float adjustTime)
    {
        if (eventInstance.IsUnityNull())
        {
            return null;
        }
        
        if (doesStopPreviousVolumeAdjustment && _changeInstanceVolumeDictionary.TryGetValue(eventInstance, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
        }
        
        Coroutine adjustInstanceVolumeCoroutine = StartCoroutine(AdjustInstanceVolumeOverTime(eventInstance,endVolume,adjustTime));
        if (doesAddToVolumeAdjustmentDictionary)
        {
            _changeInstanceVolumeDictionary[eventInstance] = adjustInstanceVolumeCoroutine;
        }
        return adjustInstanceVolumeCoroutine;
    }

    /// <summary>
    /// Modifies the volume of an instance
    /// </summary>
    /// <param name="eventInstance"></param>
    /// <param name="endVolume"></param>
    /// <param name="adjustTime"></param>
    /// <returns></returns>
    private IEnumerator AdjustInstanceVolumeOverTime(EventInstance eventInstance, float endVolume,float adjustTime)
    {
        eventInstance.getVolume(out float startVolume);
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime/ adjustTime;
            
            eventInstance.setVolume(Mathf.Lerp(startVolume, endVolume, timer));
            yield return null;
        }
        eventInstance.setVolume(endVolume);
    }

    #region Music

    public void PlayMusic(int musicID, bool allowSameSong)
    {
        if (ShouldPreventSameSong(musicID,allowSameSong))
        {
            AudioDebug("Attempted to play the same music. Music ID: " + musicID);
            return;
        }
        
        float fadeOutTime = 0;
        if (IsMusicAlreadyPlaying())
        {
            fadeOutTime = _currentMusic.DefaultInstanceFadeOutTime;
        }
        PlayMusic(musicID, fadeOutTime,AllMusic[musicID].DefaultInstanceFadeInTime, allowSameSong);
    }

    public void PlayMusic(EMusicTracks musicTrack, bool allowSameSong)
    {
        switch (musicTrack)
        {
            case EMusicTracks.MainMenu:
                PlayMusic(MAIN_MENU_MUSIC_ID, allowSameSong);
                break;
            case EMusicTracks.Map:
                PlayMusic(SELECTION_SCENE_MUSIC_ID, allowSameSong);
                break;
            case EMusicTracks.Selection:
                PlayMusic(SELECTION_SCENE_MUSIC_ID, allowSameSong);
                break;
        }
    }
    
    public void PlayMusic(int musicID, float fadeOutTime, float fadeInTime, bool allowSameSong)
    {
        if (ShouldPreventSameSong(musicID,allowSameSong))
        {
            AudioDebug("Attempted to play the same music. Music ID: " + musicID);
            return;
        }

        StopChangeCurrentMusicVolume();

        if (!_musicChangeCoroutine.IsUnityNull())
        {
            StopCoroutine(_musicChangeCoroutine);
        }
        
        _musicChangeCoroutine = StartCoroutine(MusicChangingProcess(AllMusic[musicID],musicID,fadeOutTime,fadeInTime));
    }

    private IEnumerator MusicChangingProcess(SpecificAudio specificAudio, int newMusicID, float fadeOutTime, float fadeInTime)
    {
        // If there is a music track already playing
        if (IsMusicAlreadyPlaying())
        {
            // Fade out that track
            StartFadeOutStopInstance(_currentMusicInstance, fadeOutTime);
            yield return fadeOutTime;
        }
        else
        {
            AudioDebug("Failed to fade out current track");
        }
        
        if (PlaySpecificAudio(specificAudio,out EventInstance eventInstance))
        {
            _currentMusicID = newMusicID;
            _currentMusic = specificAudio;
            _currentMusicInstance = eventInstance;
            
            yield return fadeInTime;
        }

        _musicChangeCoroutine = null;
    }

    public void StartChangeCurrentMusicVolume(float endVolume, float adjustTime)
    {
        // If the music is currently changing
        // Note that _musicChangeCoroutine is being used not _musicVolumeChangeCoroutine
        if (!_musicChangeCoroutine.IsUnityNull())
        {
            return;
        }
        
        StopChangeCurrentMusicVolume();
        
        _musicVolumeChangeCoroutine = StartAdjustInstanceVolumeOverTime(_currentMusicInstance,
            false,false,endVolume,adjustTime);
    }

    public void StopChangeCurrentMusicVolume()
    {
        if (!_musicVolumeChangeCoroutine.IsUnityNull())
        {
            StopCoroutine(_musicVolumeChangeCoroutine);
            _musicVolumeChangeCoroutine = null;
        }
    }

    private bool IsMusicAlreadyPlaying()
    {
        return _currentMusicID != -1;
    }

    private bool ShouldPreventSameSong(int musicID, bool allowSameSong)
    {
        return musicID == _currentMusicID && !allowSameSong;
    }
    
    #endregion
    
    #region Pausing

    public void PausePausableAudio()
    {
        _pausableAudioBus.setPaused(true);
    }

    public void UnpausePausableAudio()
    {
        _pausableAudioBus.setPaused(false);
    }

    public void FadeOutPausableAudio()
    {
        FadeOutPausableAudio(_defaultPausableBusFadeTime);
    }

    public void FadeOutPausableAudio(float fadeTime)
    {
        StopPausableAudioVolumeProcess();
        _pausableAudioVolumeCoroutine = StartCoroutine(FadeBusAudioVolume(_pausableAudioBus, 0, fadeTime, true));
    }

    public void FadeInPausableAudio()
    {
        FadeInPausableAudio(_defaultPausableBusFadeTime);
    }
    
    public void FadeInPausableAudio(float fadeTime)
    {
        StopPausableAudioVolumeProcess();
        _pausableAudioVolumeCoroutine = StartCoroutine(FadeBusAudioVolume(_pausableAudioBus, 1, fadeTime, false));
    }

    private void StopPausableAudioVolumeProcess()
    {
        if (!_pausableAudioVolumeCoroutine.IsUnityNull())
        {
            StopCoroutine(_pausableAudioVolumeCoroutine);
        }
    }


    private IEnumerator FadeBusAudioVolume(Bus targetBus, float endVolume, float fadeTime, bool doesCancelEventsOnEnd)
    {
        targetBus.getVolume(out float startVolume);
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime/ fadeTime;
            
            targetBus.setVolume(Mathf.Lerp(startVolume, endVolume, timer));
            yield return null;
        }
        targetBus.setVolume(endVolume);
        
        if (doesCancelEventsOnEnd)
        {
            targetBus.stopAllEvents(STOP_MODE.IMMEDIATE);
        }
    }
    #endregion Pausing

    /// <summary>
    /// Provides debug statements to better fix bugs
    /// </summary>
    /// <param name="text"></param>
    private void AudioDebug(string text)
    {
        #if UNITY_EDITOR
        if (!_doesShowAudioDebug)
        {
            return;
        }
        
        Debug.Log("Audio warning: " + text);
        #endif
    }
    
    #region BaseManager
    /// <summary>
    /// Establishes the Instance of the Audio Manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        FMODUnity.RuntimeManager.StudioSystem.getBus(_generalStartingBusPath + _pausableBusPath, out _pausableAudioBus);
        SetInitialAudioVolumeValues();
    }

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        TimeManager.Instance.GetGamePausedEvent().AddListener(PausePausableAudio);
        TimeManager.Instance.GetGameUnpausedEvent().AddListener(UnpausePausableAudio);
        SceneLoadManager.Instance.GetOnEndOfSceneLoad().AddListener(UnpausePausableAudio);
        
        SceneLoadManager.Instance.GetOnStartOfSceneLoad().AddListener(FadeOutPausableAudio);
        SceneLoadManager.Instance.GetOnEndOfSceneLoad().AddListener(FadeInPausableAudio);
    }

    #endregion

    #region Getters

    public string GetVCAPathFromAudioVCAType(EAudioVCAType audioType)
    {
        switch (audioType)
        {
            case(EAudioVCAType.Master):
                return _generalStartingVCAPath + _masterVCAPath;
            case(EAudioVCAType.Music):
                return _generalStartingVCAPath + _musicVCAPath;
            case(EAudioVCAType.SoundEffect):
                return _generalStartingVCAPath + _soundEffectVCAPath;
            default:
                return string.Empty;
        }
    }

    public VCA GetVCAFromAudioVCAType(EAudioVCAType audioType)
    {
        return FMODUnity.RuntimeManager.GetVCA(GetVCAPathFromAudioVCAType(audioType));
    }

    #endregion
}

public enum EMusicTracks
{
    MainMenu,
    Map,
    Selection,
    
}