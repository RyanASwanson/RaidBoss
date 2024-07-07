using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DifficultyDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;

    [Space]
    [SerializeField] private Image _currentDifficultyIcon;


    // Start is called before the first frame update
    void Start()
    {
       /* List<Sprite> diffIcons = UniversalManagers.Instance.GetSelectionManager().GetDifficultyIcons();

        _dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> difficultyIcons = new List<TMP_Dropdown.OptionData>();
        //List<Dropdown.OptionData> difficultyIcons = new List<Dropdown.OptionData>();

        foreach (var icon in diffIcons)
        {
            var iconOptions = new TMP_Dropdown.OptionData(icon.name, icon);
            difficultyIcons.Add(iconOptions);
        }
        _dropdown.AddOptions(difficultyIcons);*/
    }

    public void UpdateDifficulty()
    {
        UniversalManagers.Instance.GetSelectionManager().SetSelectedDifficulty((GameDifficulty)_dropdown.value+1);
    }
}
