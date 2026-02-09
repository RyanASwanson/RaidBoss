using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Contains an array of audio tracks with getters
/// Allows for a single thing
/// </summary>
[System.Serializable]
public class SpecificAudio
{
    public string AudioName;
    
    [Space]
    [Header("Fade Times")]
    public float DefaultInstanceFadeInTime;
    public float DefaultInstanceFadeOutTime;

    private WaitForSeconds _defaultInstanceFadeInWait;
    private WaitForSeconds _defaultInstanceFadeOutWait;
    
    [Space]
    [Header("Duplicate Protection")]
    public bool DoesCancelPreviousInstancesOfSpecificAudioOnPlay;
    
    [Space]
    [Header("PlayRulesAndChoices")]
    public ESpecificAudioPlayType DefaultPlayType;
    
    public ESpecificAudioTrackChoice DefaultAudioChoice;
    
    [Space]
    public EventReference[] AudioTracks;

    #region Getters

    public WaitForSeconds GetDefaultInstanceFadeInWait()
    {
        if (_defaultInstanceFadeInWait.IsUnityNull())
        {
            _defaultInstanceFadeInWait = new WaitForSeconds(DefaultInstanceFadeInTime);
        }

        return _defaultInstanceFadeInWait;
    }
    
    public WaitForSeconds GetDefaultInstanceFadeOutWait()
    {
        if (_defaultInstanceFadeOutWait.IsUnityNull())
        {
            _defaultInstanceFadeOutWait = new WaitForSeconds(DefaultInstanceFadeOutTime);
        }

        return _defaultInstanceFadeOutWait;
    }
    
    /// <summary>
    /// Returns an audio track based on what the default audio choice is for this specific audio
    /// </summary>
    /// <returns></returns>
    public EventReference GetAudioTrackFromDefault()
    {
        return GetAudioTrackFromTrackChoice(DefaultAudioChoice);
    }
    
    /// <summary>
    /// Returns an audio track based on an inputted 
    /// </summary>
    /// <param name="choice"></param>
    /// <returns></returns>
    public EventReference GetAudioTrackFromTrackChoice(ESpecificAudioTrackChoice choice)
    {
        switch (choice)
        {
            case ESpecificAudioTrackChoice.First:
                return GetFirstAudioTrack();
            case ESpecificAudioTrackChoice.Random:
                return GetRandomAudioTrack();
            default:
                return new EventReference();
        }
    }
    
    /// <summary>
    /// Gets the first audio in the track
    /// </summary>
    /// <returns></returns>
    public EventReference GetFirstAudioTrack()
    {
        return AudioTracks[0];
    }
    
    /// <summary>
    /// Gets a random audio in the track
    /// </summary>
    /// <returns></returns>
    public EventReference GetRandomAudioTrack()
    {
        return AudioTracks[Random.Range(0, AudioTracks.Length)];
    }
    
    public bool HasAudioTracks() => AudioTracks.Length > 0;

    #endregion
}

public enum ESpecificAudioPlayType
{
    OneShot,
    Instance
}

public enum ESpecificAudioTrackChoice
{
    First,
    Random
};
