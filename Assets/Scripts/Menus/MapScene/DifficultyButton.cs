using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    [SerializeField] private Image _difficultyImage;
    private EGameDifficulty _associatedDifficulty;

    public void UpdateDifficulty(EGameDifficulty associatedDifficulty)
    {
        _associatedDifficulty = associatedDifficulty;
        _difficultyImage.sprite = SelectionManager.Instance.GetDifficultyIconFromDifficulty(_associatedDifficulty);
    }
    
    public void DifficultyHoveredOver()
    {
        SelectionManager.Instance.InvokeDifficultyHoveredOverEvent(_associatedDifficulty);
    }

    public void DifficultyNotHoveredOver()
    {
        SelectionManager.Instance.InvokeDifficultyNotHoveredOverEvent(_associatedDifficulty);
    }
}
