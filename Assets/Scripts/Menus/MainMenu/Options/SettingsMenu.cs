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

    private float _tempScreenShakeValue;
    private bool _tempClickDragMovement;


    private void Start()
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
        
    }

    public void ScreenShakeSliderUpdated(float val)
    {
        _tempScreenShakeValue = val;
        
        SaveManager.Instance.SetScreenShakeStrength(_tempScreenShakeValue);
    }
}
