using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The button that is pressed in order to selected a specific hero
///     to add them to your team
/// </summary>
public class SelectHeroButton : MonoBehaviour
{
    [SerializeField] private HeroSO _associatedHero;
    [Space]

    [SerializeField] private Image _iconVisuals;
    [SerializeField] private Button _heroButton;
    private bool _buttonHasBeenPressed = false;

    private Color _defaultColor;

    // Start is called before the first frame update
    void Start()
    {
        SetButtonHeroIconVisuals();
    }

    private void SetButtonHeroIconVisuals()
    {
        _defaultColor = _iconVisuals.color;
        _iconVisuals.sprite = _associatedHero.GetHeroIcon();

        //Get the colorblock for the button
        ColorBlock colorVar = _heroButton.colors;
        //Set the highlighted color for the button to be the hero highlighted color
        colorVar.highlightedColor = _associatedHero.GetHeroHighlightedColor();
        //Set the pressed color for the button to be the hero highlighted color
        colorVar.pressedColor = _associatedHero.GetHeroPressedColor();
        //Sets the colorblock for the button
        _heroButton.colors = colorVar;
    }
    
    /// <summary>
    /// The button to select and deselect heroes is pressed
    /// </summary>
    public void SelectHeroButtonPressed()
    {
        if (!_buttonHasBeenPressed)
            HeroSelect();
        else
            HeroDeselect();

        _buttonHasBeenPressed = !_buttonHasBeenPressed;
    }

    public void SelectHeroButtonHoverBegin()
    {
        UniversalManagers.Instance.GetSelectionManager().HeroHoveredOver(_associatedHero);
    }

    public void SelectHeroButtonHoverEnd()
    {
        UniversalManagers.Instance.GetSelectionManager().HeroNotHoveredOver(_associatedHero);
    }

    private void UpdateHeroIconColor(Color newColor)
    {
        _iconVisuals.color = newColor;
    }

    private void HeroSelect()
    {
        UniversalManagers.Instance.GetSelectionManager().AddNewSelectedHero(_associatedHero);
        UpdateHeroIconColor(_associatedHero.GetHeroSelectedColor());
    }

    private void HeroDeselect()
    {
        UniversalManagers.Instance.GetSelectionManager().RemoveSpecificHero(_associatedHero);
        UpdateHeroIconColor(_defaultColor);
    }
}
