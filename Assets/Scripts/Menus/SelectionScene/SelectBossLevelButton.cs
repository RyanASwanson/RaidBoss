using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The button that is pressed in order to selected which boss
///     that you are going to fight
/// </summary>
public class SelectBossLevelButton : MonoBehaviour
{
    [SerializeField] private LevelSO _associatedLevel;
    [Space]

    [SerializeField] private Image _iconVisuals;
    private bool _buttonHasBeenPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        SetButtonBossIconVisuals();
    }

    private void SetButtonBossIconVisuals()
    {
        _iconVisuals.sprite = _associatedLevel.GetLevelBoss().GetBossIcon();
    }

    /// <summary>
    /// The button to select and deselect heroes is pressed
    /// </summary>
    public void SelectBossLevelButtonPressed()
    {
        if (!_buttonHasBeenPressed)
            BossLevelSelect();
        else
            BossLevelDeselect();

        _buttonHasBeenPressed = !_buttonHasBeenPressed;
    }

    private void BossLevelSelect()
    {
        UniversalManagers.Instance.GetSelectionManager().SetSelectedLevel(_associatedLevel);
        UniversalManagers.Instance.GetSelectionManager().SetSelectedBoss(_associatedLevel.GetLevelBoss());
        //UniversalManagers.Instance.GetSelectionManager().AddNewSelectedHero(_associatedHero);
    }

    private void BossLevelDeselect()
    {
        //UniversalManagers.Instance.GetSelectionManager().RemoveSpecificHero(_associatedHero);
    }
}
