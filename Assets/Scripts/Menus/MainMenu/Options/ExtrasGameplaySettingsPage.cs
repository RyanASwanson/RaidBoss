using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the functionality on the options menu on the main menu
/// </summary>
public class ExtrasGameplaySettingsPage : MonoBehaviour
{
    [Header("HeroControlUI")]
    [SerializeField] private Image _heroControlInputImage;
    [SerializeField] private Image _heroManualInputImage;
    
    [Space]
    [SerializeField] private Toggle _heroControlInputUIToggle;
    [SerializeField] private Toggle _heroManualInputUIToggle;
    
    [Header("Camera")]
    [SerializeField] private Slider _screenShakeSlider;
    
    [Header("Outlines")]
    [SerializeField] private Slider _bossTargetZoneOutlineStrength;

    private float _tempScreenShakeValue;
    private float _tempBossTargetZoneOutlineStrength;
    private bool _tempClickDragMovement;

    private void Start()
    {
        SetValuesOnOpen();

        ListenForSettings();
    }

    private void OnDestroy()
    {
        StopListeningForSettings();
    }

    /// <summary>
    /// Sets up the settings options and sliders
    /// </summary>
    void SetValuesOnOpen()
    {
        SetHeroControlInputToggle(SaveManager.Instance.GetDoesShowHeroControlInputUI());
        SetHeroManualInputToggle(SaveManager.Instance.GetDoesShowHeroManualInputUI());
        
        _heroControlInputUIToggle.isOn = SaveManager.Instance.GetDoesShowHeroControlInputUI();
        _heroManualInputUIToggle.isOn = SaveManager.Instance.GetDoesShowHeroManualInputUI();
        
        //Sets the temporary setting value
        _tempScreenShakeValue = SaveManager.Instance.GetScreenShakeIntensity();
        //Sets the slider to be at the saved amount
        _screenShakeSlider.value = _tempScreenShakeValue ;
        
        _tempBossTargetZoneOutlineStrength = SaveManager.Instance.GetBossTargetZoneOutlineStrength();
        _bossTargetZoneOutlineStrength.value = _tempBossTargetZoneOutlineStrength;

        _tempClickDragMovement = SaveManager.Instance.GetClickAndDragEnabled();
    }

    private void ToggleHeroControlInputUI(bool doesShow)
    {
        SaveManager.Instance.SetDoesShowHeroControlInputUI(doesShow);
        SetHeroControlInputToggle(doesShow);
    }

    private void SetHeroControlInputToggle(bool doesShow)
    {
        _heroControlInputImage.sprite = ExtrasUIFunctionality.Instance.GetOptionsToggleBool(doesShow);
    }

    private void ToggleHeroManualInputUI(bool doesShow)
    {
        SaveManager.Instance.SetDoesShowHeroManualInputUI(doesShow);
        SetHeroManualInputToggle(doesShow);
    }

    private void SetHeroManualInputToggle(bool doesShow)
    {
        _heroManualInputImage.sprite = ExtrasUIFunctionality.Instance.GetOptionsToggleBool(doesShow);
    }

    public void ScreenShakeSliderUpdated(float val)
    {
        _tempScreenShakeValue = val;
        
        SaveManager.Instance.SetScreenShakeStrength(_tempScreenShakeValue);
    }

    public void BossTargetZoneOutlineStrengthSliderUpdated(float val)
    {
        _tempBossTargetZoneOutlineStrength = val;
        
        SaveManager.Instance.SetBossTargetZoneOutlineStrength(val);
    }

    private void ListenForSettings()
    {
        _heroControlInputUIToggle.onValueChanged.AddListener(ToggleHeroControlInputUI);
        _heroManualInputUIToggle.onValueChanged.AddListener(ToggleHeroManualInputUI);
        
        _screenShakeSlider.onValueChanged.AddListener(ScreenShakeSliderUpdated);
        
        _bossTargetZoneOutlineStrength.onValueChanged.AddListener(BossTargetZoneOutlineStrengthSliderUpdated);
    }

    private void StopListeningForSettings()
    {
        _heroControlInputUIToggle.onValueChanged.RemoveListener(ToggleHeroControlInputUI);
        _heroManualInputUIToggle.onValueChanged.RemoveListener(ToggleHeroManualInputUI);
        
        _screenShakeSlider.onValueChanged.RemoveListener(ScreenShakeSliderUpdated);
        
        _bossTargetZoneOutlineStrength.onValueChanged.RemoveListener(BossTargetZoneOutlineStrengthSliderUpdated);
    }

    public void ResetSaveData()
    {
        SaveManager.Instance.ResetGameplaySaveData();
    }
}
