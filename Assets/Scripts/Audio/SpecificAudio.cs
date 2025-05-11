using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Contains an array of audio tracks with getters
/// Allows for a single thing
/// </summary>
[System.Serializable]
public class SpecificAudio
{
    public EventReference[] AudioTracks;

    #region Getters
    public EventReference GetFirstAudioTrack()
    {
        return AudioTracks[0];
    }
    public EventReference GetRandomAudioTrack()
    {
        return AudioTracks[Random.Range(0, AudioTracks.Length)];
    }
    #endregion
}
