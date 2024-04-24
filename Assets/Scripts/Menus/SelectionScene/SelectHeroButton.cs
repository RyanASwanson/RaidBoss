using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectHeroButton : MonoBehaviour
{
    [SerializeField] private HeroSO _associatedHero;
    private bool _buttonHasBeenPressed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SelectHeroButtonPressed()
    {
        if (!_buttonHasBeenPressed)
        {
            UniversalManagers.Instance.GetSelectionManager().AddNewSelectedHero(_associatedHero);
        }
        else
        {
            UniversalManagers.Instance.GetSelectionManager().RemoveSpecificHero(_associatedHero);
        }
    }
}
