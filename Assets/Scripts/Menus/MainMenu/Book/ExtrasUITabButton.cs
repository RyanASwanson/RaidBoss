using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtrasUITabButton : MonoBehaviour
{
    [SerializeField] private TextWithBackground _associatedText;
    [SerializeField] private Image _associatedImage;
    [SerializeField] private Image _associatedGlow;
    
    private int _associatedPage;

    public void SetUpPage(int page, ExtrasUITabData tabData)
    {
        SetAssociatedPage(page);
        
        _associatedText.UpdateText(tabData._buttonText);
    }

    /// <summary>
    /// Changes what tab is currently being displayed
    /// Called by button
    /// </summary>
    public void ChangeAssociatedTab()
    {
        ExtrasUIFunctionality.Instance.ChangeTabBasedOnCurrentPage(_associatedPage, this);
    }

    public void SetAssociatedPage(int newPage)
    {
        _associatedPage = newPage;
    }

    public void SetButtonColor(Color newColor)
    {
        _associatedText.UpdateTextColor(newColor);
        _associatedGlow.color = newColor;
    }
}

[System.Serializable]
public class ExtrasUITabData
{
    public string _buttonText;
    public Sprite _buttonIcon;
}