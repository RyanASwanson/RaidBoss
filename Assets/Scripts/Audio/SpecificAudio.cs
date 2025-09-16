using System.Collections;
using System.Collections.Generic;
using FMODUnity;
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
    [Header("Delays")] 
    public float DefaultStartDelay;
    
    [Space]
    [Header("Fade Times")]
    public float DefaultInstanceFadeInTime;
    public float DefaultInstanceFadeOutTime;
    
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
    public bool HasDefaultDelay() => DefaultStartDelay > 0;
    public bool ShouldUseAudioDelay() => HasDefaultDelay() && DefaultPlayType == ESpecificAudioPlayType.OneShot;

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
