using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextWithBackground : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Text _backgroundText;

    internal string CurrentString;
    
    public void UpdateText(string newString)
    {
        CurrentString = newString;
        _text.text = newString;
        _backgroundText.text = newString;
    }
}
