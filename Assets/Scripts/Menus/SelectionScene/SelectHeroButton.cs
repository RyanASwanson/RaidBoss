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
        Debug.Log("HoverEnd");
    }

    private void UpdateHeroIconColor(Color newColor)
    {
        _iconVisuals.color = newColor;
    }

    private void HeroSelect()
    {
        UniversalManagers.Instance.GetSelectionManager().AddNewSelectedHero(_associatedHero);
    }

    private void HeroDeselect()
    {
        UniversalManagers.Instance.GetSelectionManager().RemoveSpecificHero(_associatedHero);
    }
}
