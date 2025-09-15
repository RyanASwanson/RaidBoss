using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private EAudioVCAType _audioVCAType;
    private VCA _activeVca;
    
    [Space]
    [SerializeField] private Slider _audioSlider;

    /// <summary>
    /// Performs initial set up
    /// </summary>
    private void Awake()
    {
        SetVCA();
        
        _audioSlider.value = SaveManager.Instance.GetVolumeFromAudioVCAType(_audioVCAType);
        Debug.Log(SaveManager.Instance.GetVolumeFromAudioVCAType(_audioVCAType));
    }

    /// <summary>
    /// Sets the associated VCA to the slider
    /// </summary>
    private void SetVCA()
    {
        _activeVca = AudioManager.Instance.GetVCAFromAudioVCAType(_audioVCAType);
    }

    /// <summary>
    /// Updates the volume of the slider
    /// </summary>
    public void UpdateVolume()
    {
        _activeVca.setVolume(_audioSlider.value);
        SaveManager.Instance.SetVolumeFromAudioVCAType(_audioVCAType, _audioSlider.value);
    }
}

public enum EAudioVCAType
{
    Master,
    Music,
    SoundEffect,
}