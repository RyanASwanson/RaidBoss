using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DifficultyDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateDifficulty()
    {
        UniversalManagers.Instance.GetSelectionManager().SetSelectedDifficulty((GameDifficulty)_dropdown.value);
    }
}
