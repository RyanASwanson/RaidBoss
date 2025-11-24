using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyDropdownTab : MonoBehaviour
{
    [SerializeField] private Image _insideImage;
    [SerializeField] private TMP_Text _insideText;
    [Space]
    [SerializeField] private DifficultyDropdown _difficultyDropdown;
    
    // Start is called before the first frame update
    void Start()
    {
        SetDifficultyColor();
    }

    private void SetDifficultyColor()
    {
        List<string> difficultyNames = SelectionManager.Instance.GetDifficultyNames();

        for (int i = 0; i < difficultyNames.Count; i++)
        {
            if (difficultyNames[i] == _insideText.text)
            {
                _insideImage.color = _difficultyDropdown.GetDropdownColors()[i];
                return;
            }
            
        }
        
    }
}
