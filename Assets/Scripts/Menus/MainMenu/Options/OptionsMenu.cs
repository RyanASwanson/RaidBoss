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
    private bool _tempClickDragMovement;

    private float _tempMasterAudioValue;
    private float _tempMusicAudioValue;
    private float _tempSFXAudioValue;


    public void OptionsMenuOpened()
    {
        _saveManager = UniversalManagers.Instance.GetSaveManager();
        SetValuesOnOpen();
    }

    /// <summary>
    /// Sets up the settings options and sliders
    /// </summary>
    void SetValuesOnOpen()
    {
        //Sets the temporary setting value
        _tempScreenShakeValue = _saveManager.GetScreenShakeIntensity();
        //Sets the slider to be at the saved amount
        _screenShakeSlider.value = _tempScreenShakeValue ;

        _tempClickDragMovement = _saveManager.GetClickAndDragEnabled();

        _tempMasterAudioValue = _saveManager.GetMasterVolume();
        _masterAudioSlider.value = _tempMasterAudioValue;

        _tempMusicAudioValue = _saveManager.GetMusicVolume();
        _musicAudioSlider.value = _tempMusicAudioValue;

        _tempSFXAudioValue = _saveManager.GetSFXVolume();
        _sfxAudioSlider.value = _tempSFXAudioValue ;
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


    public void ApplySettingsPressed()
    {
        _saveManager.SaveSettingsOptions(_tempScreenShakeValue, _tempClickDragMovement,
            _tempMasterAudioValue, _tempMusicAudioValue, _tempSFXAudioValue);
    }
}
