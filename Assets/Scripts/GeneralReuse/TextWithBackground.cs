using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextWithBackground : MonoBehaviour
{
    [SerializeField] private bool _hasDefaultText;
    [TextArea(1, 10)][SerializeField] private string _defaultText;
    
    [Space]
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Text _backgroundText;
    
    private RectTransform _rectTransform;

    internal string CurrentString;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        if (_hasDefaultText)
        {
            UpdateText(_defaultText);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log(_text.text);
            Debug.Log(_text.GetRenderedValues(true));
            Debug.Log(_text.textBounds);
        }
    }

    public void UpdateText(string newString)
    {
        CurrentString = newString;
        _text.text = newString;
        _backgroundText.text = newString;
    }

    public void UpdateTextColor(Color color)
    {
        _text.color = color;
    }

    #region Getters

    public RectTransform GetRectTransform() => _rectTransform;

    #endregion
}
