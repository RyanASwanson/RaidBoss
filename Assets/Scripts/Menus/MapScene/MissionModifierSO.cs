using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionModifierScriptableObject", menuName = "MissionModifierScriptableObject", order = 4)]
public class MissionModifierSO : ScriptableObject
{
    [TextArea(1, 2)][SerializeField] private string _modifierName;
    [SerializeField] private int _modifierID;
    [TextArea(1, 4)][SerializeField] private string _modifierDescription;

    [Space] 
    [SerializeField] private Sprite _modifierSprite;

    [SerializeField] private Color _modifierHighlightedColor;
    [SerializeField] private Color _modifierSelectedColor;

    [Space]
    [SerializeField] private GameObject _missionModifierObject;
    
    
    #region Getters
    public string GetModifierName() => _modifierName;
    public int GetModifierID() => _modifierID;
    public string GetModifierDescription() => _modifierDescription;
    
    public Sprite GetModifierSprite() => _modifierSprite;
    public Color GetModifierHighlightedColor() => _modifierHighlightedColor;
    public Color GetModifierSelectedColor() => _modifierSelectedColor;
    
    public GameObject GetMissionModifierObject() => _missionModifierObject;
    #endregion
}
