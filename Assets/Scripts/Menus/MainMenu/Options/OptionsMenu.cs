using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    void OnEnable()
    {
        _saveManager = UniversalManagers.Instance.GetSaveManager();

        SetSliderValuesOnOpen();
    }

    void SetSliderValuesOnOpen()
    {
        
        _screenShakeSlider.value = _saveManager.GSD._screenShakeStrength;
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
