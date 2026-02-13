using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecificTutorialPageButton : MonoBehaviour
{
    [SerializeField] private Button _associatedButton;
    [SerializeField] private TMP_Text _numberText;
    
    private int _associatedPageID;
    private MissionTutorialVisuals _missionTutorialVisuals;
    private RectTransform _associatedRectTransform;
    
    public void SetUpSpecificPageButton(MissionTutorialVisuals missionTutorialVisuals,int pageID)
    {
        _missionTutorialVisuals = missionTutorialVisuals;
        _associatedPageID = pageID;
        _numberText.text = (_associatedPageID+1).ToString();
        _associatedRectTransform = GetComponent<RectTransform>();

        if (pageID == 0)
        {
            ToggleButton(false);
        }
        
        _associatedButton.onClick.AddListener(ButtonPressed);
    }

    public void SetTutorialPageTransform(Vector2 position)
    {
        _associatedRectTransform.anchoredPosition = position;
    }

    private void ButtonPressed()
    {
        _missionTutorialVisuals.PageButtonPressed(_associatedPageID);
        ToggleButton(false);
    }

    public void ButtonNoLongerPressed()
    {
        ToggleButton(true);
    }

    public void ToggleButton(bool toggle)
    {
        _associatedButton.interactable = toggle;
    }
    
    
    #region Getter
    public float GetButtonTransformAnchorX() => _associatedRectTransform.anchoredPosition.x;
    #endregion
}
