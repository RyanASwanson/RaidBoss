using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// The button that is pressed in order to selected which boss
/// that you are going to fight
/// </summary>
public class SelectBossLevelButton : MonoBehaviour
{
    [SerializeField] private LevelSO _associatedLevel;
    [SerializeField] private bool _bossEnabled;
    [Space]

    [SerializeField] private Image _iconVisuals;
    [SerializeField] private Button _levelBossButton;
    private bool _buttonHasBeenPressed = false;

    private Color _defaultColor;

    [SerializeField] private GameObject _lockVisuals;

    // Start is called before the first frame update
    void Start()
    {
        UpdateButtonInteractability();
        SetButtonBossIconVisuals();
    }

    private void UpdateButtonInteractability()
    {
        bool heroUnlocked = SaveManager.Instance.
            GSD._bossesUnlocked[_associatedLevel.GetLevelBoss().GetBossName()];

        _levelBossButton.interactable = heroUnlocked;
        _lockVisuals.SetActive(!heroUnlocked);
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
        {
            BossLevelSelect();
        }
        else
        {
            BossLevelDeselect();
        }
        
        EventSystem.current.SetSelectedGameObject(null);

        _buttonHasBeenPressed = !_buttonHasBeenPressed;
    }

    public void SelectBossButtonHoverBegin()
    {
        SelectionManager.Instance.BossHoveredOver(_associatedLevel.GetLevelBoss());
    }

    public void SelectBossButtonHoverEnd()
    {
        SelectionManager.Instance.BossNotHoveredOver(_associatedLevel.GetLevelBoss());
    }

    private void UpdateBossIconColor(Color newColor)
    {
        _iconVisuals.color = newColor;
    }

    private void BossLevelSelect()
    {
        SelectionManager.Instance.SetSelectedBoss(_associatedLevel.GetLevelBoss());
        SelectionManager.Instance.SetSelectedLevel(_associatedLevel);

        UpdateBossIconColor(_associatedLevel.GetLevelBoss().GetBossSelectedColor());
    }

    private void BossLevelDeselect()
    {
        SelectionManager.Instance.RemoveSelectedBoss();
        SelectionManager.Instance.RemoveSelectedLevel();

        UpdateBossIconColor(_defaultColor);
    }

    #region Getters
    public LevelSO GetAssociatedLevel() => _associatedLevel;
    public BossSO GetAssociatedBoss() => _associatedLevel.GetLevelBoss();
    #endregion
}
