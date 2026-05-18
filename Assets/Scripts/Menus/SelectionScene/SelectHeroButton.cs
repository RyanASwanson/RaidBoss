using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// The button that is pressed in order to selected a specific hero
///     to add them to your team
/// </summary>
public class SelectHeroButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private HeroSO _associatedHero;

    [Space]
    [SerializeField] private Image _iconVisuals;
    [SerializeField] private Button _heroButton;
    private bool _buttonHasBeenPressed = false;

    private Color _defaultColor;

    [SerializeField] private Image _bestDifficultyBeatenIcon;
    [SerializeField] private TextWithBackground _mythicPlusLevelText;

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
    private bool _isHeroButtonHoveredOver = false;
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
            SetButtonHeroIconVisuals();
            _informationUnlockLockedColor = _informationLockVisuals.color;
            SetInformationLockIcon();
        }
    }

    private void UpdateFreePlayButtonInteractability()
    {
        SetButtonInteractability(SaveManager.Instance.IsHeroUnlocked(_associatedHero));
    }

    private void SetButtonHeroIconVisuals()
    {
        SetDefaultIconColor();
        //Sets the image to be the hero icon
        _iconVisuals.sprite = _associatedHero.GetHeroIcon();

        if (!_heroButton.IsUnityNull())
        {
            //Get the colorblock for the button
            ColorBlock colorVar = _heroButton.colors;
            //Set the highlighted color for the button to be the hero highlighted color
            colorVar.highlightedColor = _associatedHero.GetHeroHighlightedColor();
            //Set the pressed color for the button to be the hero highlighted color
            colorVar.pressedColor = _associatedHero.GetHeroPressedColor();
            //Sets the colorblock for the button
            _heroButton.colors = colorVar;
        }
    }

    private void SetDefaultIconColor()
    {
        _defaultColor = _iconVisuals.color;
    }

    public void ClearButtonHeroIconVisuals()
    {
        _iconVisuals.sprite = null;
        _iconVisuals.color = _defaultColor;
    }
    
    /// <summary>
    /// The button to select and deselect heroes is pressed
    /// </summary>
    public void SelectHeroButtonLeftClicked()
    {
        if (!_isInteractable)
        {
            return;
        }
        
        if (!_buttonHasBeenPressed)
        {
            //Prevent button press at max heroes just in case
            //if (UniversalManagers.Instance.GetSelectionManager().AtMaxHeroesSelected()) return;
            HeroSelect();
            _buttonSizeCurve.StartMovingUpOnCurve();
        }
        else
        {
            HeroDeselect();
        }

        _resetEventSystemObject.ResetSelectedEventSystemObject();

        _buttonHasBeenPressed = !_buttonHasBeenPressed;
    }

    #region Information Lock
    public void SelectHeroButtonRightClicked()
    {
        _lockResetEventSystemObject.ResetSelectedEventSystemObject();
        if (!_isInformationLocked)
        {
            InformationLockHeroSelection();
        }
        else
        {
            SelectionManager.Instance.UnlockHeroInformation();
        }
    }

    private void InformationLockHeroSelection()
    {
        _isInformationLocked = true;
        
        SetInformationLockIcon();
        
        SelectionManager.Instance.LockUnlockHeroInformation(_associatedHero);
        
        SelectionManager.Instance.GetInformationUnlockedEvent().AddListener(InformationUnlockHeroSelection);
    }

    private void InformationUnlockHeroSelection()
    {
        SelectionManager.Instance.GetInformationUnlockedEvent().RemoveListener(InformationUnlockHeroSelection);

        _isInformationLocked = false;
        SetInformationLockIcon();

        if (!_isHeroButtonHoveredOver)
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
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        SelectHeroButtonHoverBegin();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SelectHeroButtonHoverEnd();
    }
    
    
    public void SelectHeroButtonHoverBegin()
    {
        _isHeroButtonHoveredOver = true;
        
        if (!_buttonVisualsHolderSizeCurve.IsUnityNull())
        {
            _buttonVisualsHolderSizeCurve.StartMovingUpOnCurve();
        }

        ShowInformationLock();

        if (_associatedHero.IsUnityNull())
        {
            return;
        }

        SelectionManager.Instance.HeroHoveredOver(_associatedHero);
    }

    public void SelectHeroButtonHoverEnd()
    {
        _isHeroButtonHoveredOver = false;
        
        if (!_buttonVisualsHolderSizeCurve.IsUnityNull())
        {
            _buttonVisualsHolderSizeCurve.StartMovingDownOnCurve();
        }
        
        HideInformationLock();
        
        if (_associatedHero.IsUnityNull())
        {
            return;
        }
        
        SelectionManager.Instance.HeroNotHoveredOver(_associatedHero);
    }

    private void UpdateHeroIconColor(Color newColor)
    {
        _iconVisuals.color = newColor;
    }

    public void SetBestDifficultyBeatenIcon(BossSO hoveredBoss)
    {
        int gameDifficulty = SaveManager.Instance.
            GetBestDifficultyIntBeatenOnHeroForBoss(hoveredBoss, _associatedHero);

        if (gameDifficulty > (int)EGameDifficulty.MythicPlus)
        {
            _mythicPlusLevelText.gameObject.SetActive(true);
            _mythicPlusLevelText.UpdateText((gameDifficulty - (int) EGameDifficulty.MythicPlus).ToString());
            gameDifficulty = (int)EGameDifficulty.MythicPlus;
        }
        else
        {
            _mythicPlusLevelText.gameObject.SetActive(false);
        }
        
        if (gameDifficulty > 0)
        {
            _bestDifficultyBeatenIcon.sprite = SelectionManager.Instance.GetDifficultyIcons()[gameDifficulty - 1];
            UpdateBestDifficultyBeatenIconAlpha(1);
        }
        else
        {
            UpdateBestDifficultyBeatenIconAlpha(0);
        }
    }

    /// <summary>
    /// Sets the opacity of the difficulty icon
    /// </summary>
    /// <param name="newAlpha"></param>
    private void UpdateBestDifficultyBeatenIconAlpha(float newAlpha)
    {
        Color tempColor = _bestDifficultyBeatenIcon.color;

        tempColor.a = newAlpha;

        _bestDifficultyBeatenIcon.color = tempColor;
    }

    private void HeroSelect()
    {
        SelectionManager.Instance.AddNewSelectedHero(_associatedHero);
        UpdateHeroIconColor(_associatedHero.GetHeroSelectedColor());
    }

    private void HeroDeselect()
    {
        SelectionManager.Instance.RemoveSpecificHero(_associatedHero);
        UpdateHeroIconColor(_defaultColor);
    }

    public void ClearAssociatedHero()
    {
        ClearButtonHeroIconVisuals();
        _associatedHero = null;
    }

    #region Getters
    public HeroSO GetAssociatedHero() => _associatedHero;
    #endregion
    
    #region Setters
    public void SetButtonInteractability(bool interactable)
    {
        if (!_heroButton.IsUnityNull())
        {
            _heroButton.interactable = interactable;
            _isInteractable = interactable;
        }
        
        _lockVisuals.SetActive(!interactable);
    }

    public void SetAssociatedHero(HeroSO hero)
    {
        _associatedHero = hero;

        SetButtonHeroIconVisuals();
    }
    
    #endregion
}
