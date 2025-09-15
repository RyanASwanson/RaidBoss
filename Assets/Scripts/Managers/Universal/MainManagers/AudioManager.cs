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

    [SerializeField] private string _masterVCAPath;
    [SerializeField] private string _musicVCAPath;
    [SerializeField] private string _soundEffectVCAPath;
    
    private const string DEFAULT_VCA_PATH = "vca:/";

    #region AudioReferences
    
    [Space]
    [Header("Boss Audio")]
    public SpecificBossAudio[] AllSpecificBossAudio;
    
    [Space]
    [Header("Hero Audio")]
    public SpecificHeroAudio[] AllSpecificHeroAudio;

    [Space] 
    [Header("Music")] 
    public SpecificAudio[] AllMusic;

    private int _currentMusicID = -1;
    private EventInstance _currentMusicInstance;
    private Coroutine _musicChangeCoroutine;
    private SpecificAudio _currentMusic;
    
    private Dictionary<EventInstance, Coroutine> _changeInstanceVolumeDictionary = new Dictionary<EventInstance, Coroutine>();
    
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

    public bool PlaySpecificAudio(SpecificAudio specificAudio, out EventInstance eventInstance)
    {
        if (!specificAudio.HasAudioTracks())
        {
            AudioDebug("No audio tracks found when playing specific audio");
            eventInstance = new EventInstance();
            return false;
        }
        return PlaySpecificAudio(specificAudio,specificAudio.DefaultPlayType,specificAudio.DefaultAudioChoice, true, out eventInstance);
    }

    public bool PlaySpecificAudio(SpecificAudio specificAudio, ESpecificAudioPlayType playType,
        ESpecificAudioTrackChoice trackChoice, bool doesAddToVolumeAdjustmentDictionary, out EventInstance eventInstance)
    {
        switch (playType)
        {
            case ESpecificAudioPlayType.OneShot:
                eventInstance = new EventInstance();
                return PlayOneShotFromSpecificAudio(specificAudio, trackChoice);
            case ESpecificAudioPlayType.Instance:
                return StartSpecificAudioInstance(specificAudio, trackChoice,doesAddToVolumeAdjustmentDictionary,out eventInstance);
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
                StartAdjustInstanceVolumeOverTime(eventInstance, false, doesAddToVolumeAdjustmentDictionary, 1,
                    specificAudio.DefaultInstanceFadeInTime);
            }

            return true;
        }

        return false;
    }

    public bool StopSpecificAudioInstance(EventInstance eventInstance, bool releaseAfter)
    {
        if (eventInstance.IsUnityNull())
        {
            AudioDebug("No instance found when stopping specific audio");
        }

        eventInstance.stop(STOP_MODE.IMMEDIATE);
        if (releaseAfter)
        {
            eventInstance.release();
        }

        return true;
    }

    
    public void StartFadeOutStopInstance(EventInstance eventInstance, SpecificAudio specificAudio)
    {
        StartCoroutine(FadeOutStopInstance(eventInstance,true,false,0,specificAudio.DefaultInstanceFadeOutTime));
    }
    
    public void StartFadeOutStopInstance(EventInstance eventInstance, float adjustTime)
    {
        StartCoroutine(FadeOutStopInstance(eventInstance,true,false,0,adjustTime));
    }

    private IEnumerator FadeOutStopInstance(EventInstance eventInstance, 
        bool doesStopPreviousVolumeAdjustment, bool doesAddToVolumeAdjustmentDictionary, float endVolume, float adjustTime)
    {
        StartAdjustInstanceVolumeOverTime(eventInstance, doesStopPreviousVolumeAdjustment, doesAddToVolumeAdjustmentDictionary, endVolume, adjustTime);
        yield return adjustTime;
        StopSpecificAudioInstance(eventInstance, doesAddToVolumeAdjustmentDictionary);
    }

    public void StartAdjustInstanceVolumeOverTime(EventInstance eventInstance, 
        bool doesStopPreviousVolumeAdjustment, bool doesAddToVolumeAdjustmentDictionary, float endVolume, float adjustTime)
    {
        if (doesStopPreviousVolumeAdjustment && _changeInstanceVolumeDictionary.TryGetValue(eventInstance, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
        }
        
        Coroutine adjustInstanceVolumeCoroutine = StartCoroutine(AdjustInstanceVolumeOverTime(eventInstance,endVolume,adjustTime));
        if (doesAddToVolumeAdjustmentDictionary)
        {
            _changeInstanceVolumeDictionary[eventInstance] = adjustInstanceVolumeCoroutine;
        }
    }

    /// <summary>
    /// Modifies the volume of an instnace
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
    
    public void PlayMusic(int musicID, float fadeOutTime, float fadeInTime, bool allowSameSong)
    {
        if (ShouldPreventSameSong(musicID,allowSameSong))
        {
            AudioDebug("Attempted to play the same music. Music ID: " + musicID);
            return;
        }

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
            Debug.Log("Fading out " + newMusicID + " with a fade out of " + fadeOutTime);
            // Fade out that track
            StartFadeOutStopInstance(_currentMusicInstance, fadeOutTime);
            yield return fadeOutTime;
        }
        else
        {
            Debug.Log("Failed " + _currentMusicID);
        }

        Debug.Log("Attempting to play specific audio");
        if (PlaySpecificAudio(specificAudio,out EventInstance eventInstance))
        {
            Debug.Log("Setting new music ID: " + newMusicID);
            _currentMusicID = newMusicID;
            _currentMusic = specificAudio;
            _currentMusicInstance = eventInstance;
            
            yield return fadeInTime;
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
    #endregion

    #region Getters

    public string GetVCAPathFromAudioVCAType(EAudioVCAType audioType)
    {
        switch (audioType)
        {
            case(EAudioVCAType.Master):
                return DEFAULT_VCA_PATH + _masterVCAPath;
            case(EAudioVCAType.Music):
                return DEFAULT_VCA_PATH + _musicVCAPath;
            case(EAudioVCAType.SoundEffect):
                return DEFAULT_VCA_PATH + _soundEffectVCAPath;
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
