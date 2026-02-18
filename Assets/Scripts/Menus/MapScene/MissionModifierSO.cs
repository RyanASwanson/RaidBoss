using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionModifierScriptableObject", menuName = "MissionModifierScriptableObject", order = 4)]
public class MissionModifierSO : ScriptableObject
{
    [SerializeField] private string _modifierName;
    [TextArea(1, 4)][SerializeField] private string _modifierDescription;

    [Space] 
    [SerializeField] private Sprite _modifierSprite;
    
    
    #region Getters
    public string GetModifierName() => _modifierName;
    public string GetModifierDescription() => _modifierDescription;
    
    public Sprite GetModifierSprite() => _modifierSprite;
    #endregion
}
