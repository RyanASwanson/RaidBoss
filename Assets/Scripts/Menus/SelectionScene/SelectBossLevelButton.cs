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
public class SelectBossLevelButton : MonoBehaviour
{
    [SerializeField] private LevelSO _associatedLevel;
    [SerializeField] private bool _bossEnabled;
    [Space]

    [SerializeField] private Image _iconVisuals;
    [SerializeField] private Button _levelBossButton;
    private bool _buttonHasBeenPressed = false;

    private Color _defaultColor;

    [Space]
    [SerializeField] private GameObject _lockVisuals;
    
    [Space]
    [SerializeField] private CurveProgression _informationLockCurve;
    [SerializeField] private Image _informationLockVisuals;
    [SerializeField] private Sprite _informationLockIcon;
    [SerializeField] private Sprite _informationUnlockIcon;
    [SerializeField] private Color _informationLockUnlockedColor;
    private Color _informationUnlockLockedColor;

    [Space] 
    [SerializeField] private CurveProgression _buttonSizeCurve;
    [SerializeField] private CurveProgression _buttonVisualsHolderSizeCurve;
    
    [Space] 
    [SerializeField] private ResetEventSystemSelectedObject _resetEventSystemObject;
    [SerializeField] private ResetEventSystemSelectedObject _lockResetEventSystemObject;
    
    private bool _isInteractable = false;
    private bool _isBossButtonHoveredOver = false;
    private bool _isInformationLocked = false;

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
            
            _informationUnlockLockedColor = _informationLockVisuals.color;
            SetInformationLockIcon();
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

        if (!_levelBossButton.IsUnityNull())
        {
            //Get the colorblock for the button
            ColorBlock colorVar = _levelBossButton.colors;
            //Set the highlighted color for the button to be the boss highlighted color
            colorVar.highlightedColor = _associatedLevel.GetLevelBoss().GetBossHighlightedColor();
            //Set the pressed color for the button to be the boss highlighted color
            colorVar.pressedColor = _associatedLevel.GetLevelBoss().GetBossPressedColor();
            //Sets the colorblock for the button
            _levelBossButton.colors = colorVar;
        }
    }
    
    private void SetDefaultIconColor()
    {
        _defaultColor = _iconVisuals.color;
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
        
        _resetEventSystemObject.ResetSelectedEventSystemObject();
        
        ToggleButtonPressed();
    }

    public void BossLevelSwapped()
    {
        SelectionManager.Instance.RemoveSelectedBoss();
        UpdateBossIconColor(_defaultColor);
        //BossLevelDeselect();
        _resetEventSystemObject.ResetSelectedEventSystemObject();
        ToggleButtonPressed();
    }

    public void ToggleButtonPressed()
    {
        _buttonHasBeenPressed = !_buttonHasBeenPressed;
    }
    
    #region Information Lock
    public void SelectBossLevelRightClicked()
    {
        _lockResetEventSystemObject.ResetSelectedEventSystemObject();
        if (!_isInformationLocked)
        {
            InformationLockBossSelection();
        }
        else
        {
            SelectionManager.Instance.UnlockBossInformation();
        }
    }
    
    private void InformationLockBossSelection()
    {
        _isInformationLocked = true;
        
        SetInformationLockIcon();
        
        SelectionManager.Instance.LockUnlockBossInformation(_associatedLevel.GetLevelBoss());
        
        SelectionManager.Instance.GetInformationUnlockedEvent().AddListener(InformationUnlockBossSelection);
    }
    
    private void InformationUnlockBossSelection()
    {
        SelectionManager.Instance.GetInformationUnlockedEvent().RemoveListener(InformationUnlockBossSelection);

        _isInformationLocked = false;
        SetInformationLockIcon();

        if (!_isBossButtonHoveredOver)
        {
            HideInformationLock();
        }
    }

    private void SetInformationLockIcon()
    {
        if (_informationLockVisuals.IsUnityNull())
        {
            return;
        }
        
        _informationLockVisuals.sprite = _isInformationLocked ? _informationLockIcon : _informationUnlockIcon;
        _informationLockVisuals.color = _isInformationLocked ? _informationUnlockLockedColor  : _informationLockUnlockedColor;
    }
    
    private void ShowInformationLock()
    {
        if (_informationLockCurve.IsUnityNull())
        {
            return;
        }
        
        _informationLockCurve.StartMovingUpOnCurve();
    }

    private void HideInformationLock()
    {
        if (_isInformationLocked)
        {
            return;
        }
        
        if (_informationLockCurve.IsUnityNull())
        {
            return;
        }
        
        _informationLockCurve.StartMovingDownOnCurve();
    }
    #endregion

    public void SelectBossButtonHoverBegin()
    {
        _isBossButtonHoveredOver = true;
        
        if (!_buttonVisualsHolderSizeCurve.IsUnityNull())
        {
            _buttonVisualsHolderSizeCurve.StartMovingUpOnCurve();
        }
        
        ShowInformationLock();

        SelectionManager.Instance.BossHoveredOver(_associatedLevel.GetLevelBoss());
    }

    public void SelectBossButtonHoverEnd()
    {
        _isBossButtonHoveredOver = false;
        
        if (!_buttonVisualsHolderSizeCurve.IsUnityNull())
        {
            _buttonVisualsHolderSizeCurve.StartMovingDownOnCurve();
        }

        HideInformationLock();
        
        SelectionManager.Instance.BossNotHoveredOver(_associatedLevel.GetLevelBoss());
    }

    private void UpdateBossIconColor(Color newColor)
    {
        _iconVisuals.color = newColor;
    }

    private void BossLevelSelect()
    {
        SelectionManager.Instance.SetSelectedLevelAndBoss(_associatedLevel);

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

    public void SetAssociatedLevel(LevelSO level)
    {
        _associatedLevel = level;
        SetButtonBossIconVisuals();
    }
    #endregion
}
