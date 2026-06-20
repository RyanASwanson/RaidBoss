using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionDropdownTab : MonoBehaviour
{
    [SerializeField] private Image _lockCover;
    //[SerializeField] private TMP_Text _insideText;
    [Space]
    [SerializeField] private Toggle _resolutionToggle;
    
    private int _resolutionIndex;
    
    // Start is called before the first frame update
    private void Start()
    {
        DetermineLockState();
    }

    private void DetermineLockState()
    {
        // Gets the number listed in the name
        _resolutionIndex = (int)Char.GetNumericValue(name[5]);
        
        // Gets the second number listed in the name
        int tempInt = (int)Char.GetNumericValue(name[6]);
        // If the second number exists
        if (tempInt >= 0)
        {
            // Put the first value in the 10s place
            _resolutionIndex *= 10;
            // Put the second value in the 1s place
            _resolutionIndex += tempInt;
        }

        SetResolutionLock(!EngineSettingsManager.Instance.GetIsResolutionCompatibleWithCurrentScreenResolution(_resolutionIndex));
    }

    private void SetResolutionLock(bool isLocked)
    {
        _resolutionToggle.interactable = !isLocked;
        _lockCover.enabled = isLocked;
    }
}
