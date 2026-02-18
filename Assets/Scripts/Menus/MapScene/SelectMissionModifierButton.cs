using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectMissionModifierButton : MonoBehaviour
{
    [SerializeField] private MissionModifierSO _associatedModifier;

    [Space]
    [SerializeField] private Image _iconVisuals;
    
    // Start is called before the first frame update
    void Start()
    {
        SetButtonModifierIconVisuals();
    }

    private void SetButtonModifierIconVisuals()
    {
        if (_associatedModifier.IsUnityNull())
        {
            return;
        }
        
        _iconVisuals.sprite = _associatedModifier.GetModifierSprite();
    }
    
    
    #region Setters
    public void SetAssociatedModifier(MissionModifierSO modifier)
    {
        _associatedModifier = modifier;

        SetButtonModifierIconVisuals();
    }

    #endregion
}
