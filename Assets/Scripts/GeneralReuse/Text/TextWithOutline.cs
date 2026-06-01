using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class TextWithOutline : MonoBehaviour
{
    [SerializeField] private bool _hasDefaultText;
    [TextArea(1, 10)][SerializeField] private string _defaultText;

    [Space] 
    [SerializeField] private bool _doesRemoveColorFromBackgroundText = true;
    [SerializeField] private bool _doesRemoveLineBreaks = false;
    
    [Space]
    [SerializeField] private Text _text;
    
    private RectTransform _rectTransform;

    internal string CurrentString;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        if (_hasDefaultText)
        {
            UpdateText(_defaultText);
        }
    }

    public void UpdateText(string newString)
    {
        if (_doesRemoveLineBreaks)
        {
            newString = newString.Replace("\n", " ");
        }
        
        CurrentString = newString;
        
        if (_doesRemoveColorFromBackgroundText)
        {
            newString = Regex.Replace(newString, "<color=.*?>|</color>", string.Empty);
        }
        
        _text.text = newString;
    }

    public void UpdateTextColor(Color color)
    {
        _text.color = color;
    }

    public void UpdateLocation(Vector2 location)
    {
        _rectTransform.anchoredPosition = location;
    }

    #region Getters

    public RectTransform GetRectTransform() => _rectTransform;

    #endregion
}
