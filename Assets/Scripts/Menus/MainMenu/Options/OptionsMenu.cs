using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the functionality on the options menu on the main menu
/// </summary>
public class OptionsMenu : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Slider _screenShakeSlider;

    [Space]
    [Header("Audio")]
    [SerializeField] private Slider _masterAudioSlider;
    [SerializeField] private Slider _musicAudioSlider;
    [SerializeField] private Slider _sfxAudioSlider;

    private SaveManager _saveManager;

    private float _tempScreenShakeValue;

    private float _tempMasterAudioValue;
    private float _tempMusicAudioValue;
    private float _tempSFXAudioValue;

    public void OptionsMenuOpened()
    {
        _saveManager = UniversalManagers.Instance.GetSaveManager();
        SetSliderValuesOnOpen();
    }

    void SetSliderValuesOnOpen()
    {
        
        _screenShakeSlider.value = _saveManager.GSD._screenShakeStrength;

        _masterAudioSlider.value = _saveManager.GSD._masterVolume;
        _musicAudioSlider.value = _saveManager.GSD._musicVolume;
        _sfxAudioSlider.value = _saveManager.GSD._sfxVolume;
    }

    public void ScreenShakeSliderUpdated(float val)
    {
        _tempScreenShakeValue = val;
    }

    public void MasterVolumeSliderUpdated(float val)
    {
        _tempMasterAudioValue = val;
    }
    public void MusicVolumeSliderUpdated(float val)
    {
        _tempMusicAudioValue = val;
    }
    public void SFXVolumeSliderUpdated(float val)
    {
        _tempSFXAudioValue = val;
    }


    public void ApplySettings()
    {
        _saveManager.SetScreenShakeIntensity(_tempScreenShakeValue);

        _saveManager.SetMasterAudioVolume(_tempMasterAudioValue);
        _saveManager.SetMusicAudioVolume(_tempMusicAudioValue);
        _saveManager.SetSFXAudioVolume(_tempSFXAudioValue);

        _saveManager.SaveText();
    }
}
