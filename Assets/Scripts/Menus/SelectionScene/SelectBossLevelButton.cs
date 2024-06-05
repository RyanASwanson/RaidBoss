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
    [SerializeField] private Button _levelBossButton;
    private bool _buttonHasBeenPressed = false;

    private Color _defaultColor;

    // Start is called before the first frame update
    void Start()
    {
        SetButtonBossIconVisuals();
    }

    private void SetButtonBossIconVisuals()
    {
        _defaultColor = _iconVisuals.color;
        _iconVisuals.sprite = _associatedLevel.GetLevelBoss().GetBossIcon();

        //Get the colorblock for the button
        ColorBlock colorVar = _levelBossButton.colors;
        //Set the highlighted color for the button to be the boss highlighted color
        colorVar.highlightedColor = _associatedLevel.GetLevelBoss().GetBossHighlightedColor();
        //Set the pressed color for the button to be the boss highlighted color
        colorVar.pressedColor = _associatedLevel.GetLevelBoss().GetBossPressedColor();
        //Sets the colorblock for the button
        _levelBossButton.colors = colorVar;
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

    public void SelectBossButtonHoverBegin()
    {
        UniversalManagers.Instance.GetSelectionManager().BossHoveredOver(_associatedLevel.GetLevelBoss());
    }

    public void SelectBossButtonHoverEnd()
    {
        UniversalManagers.Instance.GetSelectionManager().BossNotHoveredOver(_associatedLevel.GetLevelBoss());
    }

    private void UpdateHeroIconColor(Color newColor)
    {
        _iconVisuals.color = newColor;
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
