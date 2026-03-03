using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtrasVideoSettingsPage : MonoBehaviour
{
    [SerializeField] private Toggle _fullScreenToggle;
    private bool _isDefaultToggleStateSet = false;

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
        _isDefaultToggleStateSet = true;
    }

    public void ToggleFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
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
