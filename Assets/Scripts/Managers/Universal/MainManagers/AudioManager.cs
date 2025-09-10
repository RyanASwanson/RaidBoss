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

    #region AudioReferences
    [Header("Test Audio")]
    public SpecificAudio TestAudio;
    
    [Space]
    [Header("Boss Audio")]
    public SpecificBossAudio[] AllBossAudio;
    
    [Space]
    [Header("Hero Audio")]
    public SpecificHeroAudio[] AllHeroAudio;
    
    private Dictionary<EventInstance, Coroutine> _changeInstanceVolumeDictionary = new Dictionary<EventInstance, Coroutine>();
    
    #endregion

    
    #region CreateInstance
    public bool CreateInstanceFromSpecificAudio(SpecificAudio specificAudio, out EventInstance eventInstance, ESpecificAudioTrackChoice trackChoice)
    {
        return CreateInstanceFromReference(specificAudio.GetAudioTrackFromTrackChoice(trackChoice), out eventInstance);
    }
    
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
    
    
    public void PlaySpecificAudioAsOneShot(EventReference eventReference)
    {
        if (eventReference.IsUnityNull())
        {
            AudioDebug("No reference found when playing one shot: " + eventReference.Guid);
            return;
        }
        
        RuntimeManager.PlayOneShot(eventReference);
    }
    #endregion OneShot

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
}
