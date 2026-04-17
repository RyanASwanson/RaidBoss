using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the functionality on the options menu on the main menu
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Slider _screenShakeSlider;
    [SerializeField] private Slider _bossTargetZoneOutlineStrength;

    private float _tempScreenShakeValue;
    private float _tempBossTargetZoneOutlineStrength;
    private bool _tempClickDragMovement;


    private void Start()
    {
        SetValuesOnOpen();

        ListenForSliders();
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
        
        _tempBossTargetZoneOutlineStrength = SaveManager.Instance.GetBossTargetZoneOutlineStrength();
        _bossTargetZoneOutlineStrength.value = _tempBossTargetZoneOutlineStrength;

        _tempClickDragMovement = SaveManager.Instance.GetClickAndDragEnabled();
        
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

    private void ListenForSliders()
    {
        _screenShakeSlider.onValueChanged.AddListener(ScreenShakeSliderUpdated);
        _bossTargetZoneOutlineStrength.onValueChanged.AddListener(BossTargetZoneOutlineStrengthSliderUpdated);
    }

    public void ResetSaveData()
    {
        SaveManager.Instance.ResetGameplaySaveData();
    }
}
