using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageNavigationButton : MonoBehaviour
{
    [SerializeField] private Button _associatedButton;
    [SerializeField] private TMP_Text _numberText;
    
    private int _associatedPageID;
    private PageNavigationCreator _pageNavigationCreator;
    private RectTransform _associatedRectTransform;
    
    public void SetUpSpecificPageButton(PageNavigationCreator pageNavigationCreator,int pageID)
    {
        _pageNavigationCreator = pageNavigationCreator;
        _associatedPageID = pageID;
        _numberText.text = (_associatedPageID+1).ToString();
        _associatedRectTransform = GetComponent<RectTransform>();

        if (pageID == 0)
        {
            ToggleButton(false);
        }
        
        _associatedButton.onClick.AddListener(ButtonPressed);
    }

    public void SetPressedColor(Color defaultColor)
    {
        ColorBlock colorBlock = _associatedButton.colors;
        colorBlock.disabledColor = defaultColor;    
        _associatedButton.colors = colorBlock;
    }

    public void SetColorBlock(ColorBlock colorBlock)
    {
        _associatedButton.colors = colorBlock;
    }

    public void SetTutorialPageTransform(Vector2 position)
    {
        _associatedRectTransform.anchoredPosition = position;
    }

    private void ButtonPressed()
    {
        _pageNavigationCreator.PageButtonPressed(_associatedPageID);
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
    public Button GetAssociatedButton() => _associatedButton;
    public float GetButtonTransformAnchorX() => _associatedRectTransform.anchoredPosition.x;
    #endregion
}
