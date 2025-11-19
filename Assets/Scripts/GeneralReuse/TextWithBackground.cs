using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextWithBackground : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Text _backgroundText;
    
    private RectTransform _rectTransform;

    internal string CurrentString;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
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
