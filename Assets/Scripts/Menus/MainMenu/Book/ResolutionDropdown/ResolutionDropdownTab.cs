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
    void Start()
    {
        DetermineLockState();
    }

    private void DetermineLockState()
    {
        _resolutionIndex = (int)Char.GetNumericValue(name[5]);

        SetResolutionLock(!EngineSettingsManager.Instance.GetIsResolutionCompatibleWithCurrentScreenResolution(_resolutionIndex));
    }

    private void SetResolutionLock(bool isLocked)
    {
        _resolutionToggle.interactable = !isLocked;
        _lockCover.enabled = isLocked;
    }
}
