using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectHeroButton : MonoBehaviour
{
    [SerializeField] private HeroSO _associatedHero;
    private bool _buttonHasBeenPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        SetButtonHeroIconVisuals();
    }

    private void SetButtonHeroIconVisuals()
    {
        GetComponent<Image>().sprite = _associatedHero.GetHeroIcon();
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

    private void HeroSelect()
    {
        UniversalManagers.Instance.GetSelectionManager().AddNewSelectedHero(_associatedHero);
    }

    private void HeroDeselect()
    {
        UniversalManagers.Instance.GetSelectionManager().RemoveSpecificHero(_associatedHero);
    }
}
