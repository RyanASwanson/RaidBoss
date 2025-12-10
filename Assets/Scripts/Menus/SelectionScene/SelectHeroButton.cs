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
public class SelectHeroButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private HeroSO _associatedHero;

    [Space]
    [SerializeField] private Image _iconVisuals;
    [SerializeField] private Button _heroButton;
    private bool _buttonHasBeenPressed = false;

    private Color _defaultColor;

    [SerializeField] private Image _bestDifficultyBeatenIcon;

    [SerializeField] private GameObject _lockVisuals;

    [Space] 
    [SerializeField] private CurveProgression _buttonSizeCurve;


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
        }
    }

    private void UpdateFreePlayButtonInteractability()
    {
        bool heroUnlocked = SaveManager.Instance.
            GSD._heroesUnlocked[_associatedHero.GetHeroName()];
        
        SetButtonInteractability(heroUnlocked);
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

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                SelectHeroButtonLeftClicked();
                return;
            case PointerEventData.InputButton.Middle:
                return;
            case PointerEventData.InputButton.Right:
                SelectHeroButtonRightClicked();
                return;
        }
    }
    
    /// <summary>
    /// The button to select and deselect heroes is pressed
    /// </summary>
    public void SelectHeroButtonLeftClicked()
    {
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

        EventSystem.current.SetSelectedGameObject(null);

        _buttonHasBeenPressed = !_buttonHasBeenPressed;
    }

    private void SelectHeroButtonRightClicked()
    {
        SelectionManager.Instance.LockUnlockHeroInformation(_associatedHero);
    }
    
    
    public void SelectHeroButtonHoverBegin()
    {
        SelectionManager.Instance.HeroHoveredOver(_associatedHero);
    }

    public void SelectHeroButtonHoverEnd()
    {
        SelectionManager.Instance.HeroNotHoveredOver(_associatedHero);
    }

    private void UpdateHeroIconColor(Color newColor)
    {
        _iconVisuals.color = newColor;
    }

    public void SetBestDifficultyBeatenIcon(BossSO hoveredBoss)
    {
        EGameDifficulty eGameDifficulty = SaveManager.Instance.
            GetBestDifficultyBeatenOnHeroForBoss(hoveredBoss, _associatedHero);
        
        if ((int)eGameDifficulty > 0)
        {
            _bestDifficultyBeatenIcon.sprite = SelectionManager.Instance.GetDifficultyIcons()[(int)eGameDifficulty - 1];
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

    #region Getters
    public HeroSO GetAssociatedHero() => _associatedHero;
    #endregion
    
    #region Setters
    public void SetButtonInteractability(bool interactable)
    {
        if (!_heroButton.IsUnityNull())
        {
            _heroButton.interactable = interactable;
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
