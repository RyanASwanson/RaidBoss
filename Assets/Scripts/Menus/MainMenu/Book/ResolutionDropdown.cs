using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;

    private List<TMP_Dropdown.OptionData> dropDownOptionData;
    // Start is called before the first frame update
    void Start()
    {
        SetStartingDropdownVisuals();
        SetStartingDropdownValue();
        SubscribeToDropdown();
    }

    private void SetStartingDropdownVisuals()
    {
        Vector2Int[] gameResolutions = EngineSettingsManager.Instance.GetGameResolutions();
        
        string[] diffNames = new string[gameResolutions.Length];
        for (int i = 0; i < gameResolutions.Length; i++)
        {
            diffNames[i] = gameResolutions[i].x + " X " + gameResolutions[i].y;
        }
        
        dropDownOptionData = _dropdown.options;

        for(int i = 0; i< gameResolutions.Length; i++)
        {
            var iconOptions = new TMP_Dropdown.OptionData(diffNames[i],null);
            dropDownOptionData.Add(iconOptions);
        }
    }

    private void SetStartingDropdownValue()
    {
        int resolutionIndex = EngineSettingsManager.Instance.GetResolutionIDFromCurrentResolution();
        if (resolutionIndex < 0)
        {
            resolutionIndex = 0;
        }
        
        if (resolutionIndex != -1)
        {
            _dropdown.value = resolutionIndex;
            _dropdown.RefreshShownValue();
        }
    }

    private void SubscribeToDropdown()
    {
        _dropdown.onValueChanged.AddListener(ResolutionChanged);
    }

    public void ResolutionChanged(int resolutionIndex)
    {
        ResolutionChanged();
    }

    public void ResolutionChanged()
    {
        EngineSettingsManager.Instance.SetScreenResolutionFromArray(_dropdown.value);
    }
}
