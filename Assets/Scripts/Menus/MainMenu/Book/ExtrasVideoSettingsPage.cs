using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtrasVideoSettingsPage : MonoBehaviour
{
    [SerializeField] private Image _fullScreenToggleImage;
    [SerializeField] private Sprite _fullScreenToggleOff;
    [SerializeField] private Sprite _fullScreenToggleOn;
    
    [Space]
    [SerializeField] private Toggle _fullScreenToggle;
    private bool _isDefaultToggleStateSet = false;
    
    private bool _fullScreenToggleStateSet = false;

    private void OnEnable()
    {
        SetDefaultFullScreenToggleState();
        SubscribeToToggle();
    }

    private void OnDisable()
    {
        UnsubscribeFromToggle();
    }

    private void SetDefaultFullScreenToggleState()
    {
        if (_isDefaultToggleStateSet)
        {
            return;
        }
        _fullScreenToggle.isOn = Screen.fullScreen;
        SetToggleIcon();
        _isDefaultToggleStateSet = true;
    }

    public void ToggleFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        _fullScreenToggleStateSet = isFullScreen;
        SetToggleIcon();
    }

    private void SetToggleIcon()
    {
        _fullScreenToggleImage.sprite = _fullScreenToggleStateSet ? _fullScreenToggleOn : _fullScreenToggleOff;
    }

    private void SubscribeToToggle()
    {
        _fullScreenToggle.onValueChanged.AddListener(ToggleFullScreen);
    }

    private void UnsubscribeFromToggle()
    {
        _fullScreenToggle.onValueChanged.RemoveListener(ToggleFullScreen);
    }
}
