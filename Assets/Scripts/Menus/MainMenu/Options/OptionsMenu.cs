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

    private float _tempScreenShakeValue;
    private bool _tempClickDragMovement;

    private float _tempMasterAudioValue;
    private float _tempMusicAudioValue;
    private float _tempSFXAudioValue;


    public void OptionsMenuOpened()
    {
        SetValuesOnOpen();
    }

    /// <summary>
    /// Sets up the settings options and sliders
    /// </summary>
    void SetValuesOnOpen()
    {
        //Sets the temporary setting value
        _tempScreenShakeValue = SaveManager.Instance.GetScreenShakeIntensity();
        //Sets the slider to be at the saved amount
        _screenShakeSlider.value = _tempScreenShakeValue ;

        _tempClickDragMovement = SaveManager.Instance.GetClickAndDragEnabled();

        _tempMasterAudioValue = SaveManager.Instance.GetMasterVolume();
        _masterAudioSlider.value = _tempMasterAudioValue;

        _tempMusicAudioValue = SaveManager.Instance.GetMusicVolume();
        _musicAudioSlider.value = _tempMusicAudioValue;

        _tempSFXAudioValue = SaveManager.Instance.GetSFXVolume();
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

    /// <summary>
    /// Called when the apply settings button is pressed
    /// </summary>
    public void ApplySettingsPressed()
    {
        SaveManager.Instance.SaveSettingsOptions(_tempScreenShakeValue, _tempClickDragMovement,
            _tempMasterAudioValue, _tempMusicAudioValue, _tempSFXAudioValue);
    }
}
