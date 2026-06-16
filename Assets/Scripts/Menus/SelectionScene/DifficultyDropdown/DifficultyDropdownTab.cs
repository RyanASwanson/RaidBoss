using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyDropdownTab : MonoBehaviour
{
    [SerializeField] private Image _insideImage;
    [SerializeField] private Image _lockCover;
    [SerializeField] private TMP_Text _insideText;
    [Space]
    [SerializeField] private Toggle _difficultyToggle;
    [SerializeField] private DifficultyDropdown _difficultyDropdown;
    
    // Start is called before the first frame update
    private void Start()
    {
        SetUpDifficultyTab();
    }

    private void SetUpDifficultyTab()
    {
        List<string> difficultyNames = SelectionManager.Instance.GetDifficultyNames();

        for (int i = 0; i < difficultyNames.Count; i++)
        {
            if (difficultyNames[i] == _insideText.text)
            {
                _insideImage.color = _difficultyDropdown.GetDropdownColors()[i];
                DetermineLockState((EGameDifficulty)i+1);
                return;
            }
            
        }
    }

    private void DetermineLockState(EGameDifficulty difficulty)
    {
        SetDifficultyLock(difficulty > SaveManager.Instance.GetHighestDifficultyUnlocked());
    }

    private void SetDifficultyLock(bool isLocked)
    {
        _difficultyToggle.interactable = !isLocked;
        _lockCover.enabled = isLocked;
    }
}
