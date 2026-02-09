using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// The button that is pressed in order to selected which boss
/// that you are going to fight
/// </summary>
public class SelectBossLevelButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private LevelSO _associatedLevel;
    [SerializeField] private bool _bossEnabled;
    [Space]

    [SerializeField] private Image _iconVisuals;
    [SerializeField] private Button _levelBossButton;
    private bool _buttonHasBeenPressed = false;

    private Color _defaultColor;

    [SerializeField] private GameObject _lockVisuals;

    [Space] 
    [SerializeField] private CurveProgression _buttonSizeCurve;
    
    private bool _isInteractable = false;

    // Start is called before the first frame update
    void Start()
    {
        if (SelectionManager.Instance.IsPlayingMissionsMode())
        {
            SetDefaultIconColor();
        }
        else
        {
            UpdateFreePlayButtonInteractability();
            SetButtonBossIconVisuals();
        }
    }

    private void UpdateFreePlayButtonInteractability()
    {
        bool bossUnlocked = SaveManager.Instance.IsBossUnlocked(_associatedLevel.GetLevelBoss());
        
        SetButtonInteractability(bossUnlocked);
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
    
    private void SetDefaultIconColor()
    {
        _defaultColor = _iconVisuals.color;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isInteractable)
        {
            return;
        }
        
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                SelectBossLevelLeftClicked();
                return;
            case PointerEventData.InputButton.Middle:
                return;
            case PointerEventData.InputButton.Right:
                SelectBossLevelRightClicked();
                return;
        }
    }

    /// <summary>
    /// The button to select and deselect heroes is pressed
    /// </summary>
    public void SelectBossLevelLeftClicked()
    {
        if (!_buttonHasBeenPressed)
        {
            BossLevelSelect();
            _buttonSizeCurve.StartMovingUpOnCurve();
        }
        else
        {
            BossLevelDeselect();
        }
        
        EventSystem.current.SetSelectedGameObject(null);

        _buttonHasBeenPressed = !_buttonHasBeenPressed;
    }
    
    private void SelectBossLevelRightClicked()
    {
        SelectionManager.Instance.LockUnlockBossInformation(_associatedLevel.GetLevelBoss());
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
    
    #region Setters
    private void SetButtonInteractability(bool interactable)
    {
        if (!_levelBossButton.IsUnityNull())
        {
            _levelBossButton.interactable = interactable;
            _isInteractable = interactable;
        }
        
        _lockVisuals.SetActive(!interactable);
    }
    #endregion
}
