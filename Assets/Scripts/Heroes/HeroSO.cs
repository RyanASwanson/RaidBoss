using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroScriptableObject", menuName = "HeroScriptableObject", order = 1)]
public class HeroSO : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string _name;

    [Header("Stats")]
    [SerializeField] private float _maxHP;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _aggro;
    [Range(.5f, 1.5f)] [SerializeField] private float _damageResistance;

    [Header("Prefabs")]
    [SerializeField] private GameObject _heroPrefab;

    [Header("Visuals")]
    [SerializeField] private Sprite _heroIcon;
    [SerializeField] private Sprite _heroBasicAbilityIcon;
    [SerializeField] private Sprite _heroManualAbilityIcon;
    [SerializeField] private Sprite _heroPassiveAbilityIcon;

    [Header("Selection")]
    [Range(0,5)][SerializeField] private int _survivalStat;
    [Range(0,5)][SerializeField] private int _damageStat;
    [Range(0,5)][SerializeField] private int _staggerStat;
    [Range(0,5)][SerializeField] private int _speedStat;
    [Range(0,5)][SerializeField] private int _utilityStat;

    [SerializeField] private Color _heroHighlightedColor;
    [SerializeField] private Color _heroPressedColor;
    [SerializeField] private Color _heroSelectedColor;

    [Header("Ability Text")]
    [SerializeField] private string _basicAbilityName;
    [TextArea(3, 10)] [SerializeField] private string _basicAbilityDescription;
    [SerializeField] private string _manualAbilityName;
    [TextArea(3, 10)] [SerializeField] private string _manualAbilityDescription;
    [SerializeField] private string _passiveAbilityName;
    [TextArea(3, 10)] [SerializeField] private string _passiveAbilityDescription;

    #region Getters
    public string GetHeroName() => _name;

    public float GetMaxHP() => _maxHP;
    public float GetMoveSpeed() => _moveSpeed;
    public float GetAggro() => _aggro;
    public float GetDamageResistance() => _damageResistance;

    public GameObject GetHeroPrefab() => _heroPrefab;

    public Sprite GetHeroIcon() => _heroIcon;
    public Sprite GetHeroBasicAbilityIcon() => _heroBasicAbilityIcon;
    public Sprite GetHeroManualAbilityIcon() => _heroManualAbilityIcon;
    public Sprite GetHeroPassiveAbilityIcon() => _heroPassiveAbilityIcon;

    public int GetSurvivalStat() => _survivalStat;
    public int GetDamageStat() => _damageStat;
    public int GetStaggerStat() => _staggerStat;
    public int GetSpeedStat() => _speedStat;
    public int GetUtilityStat() => _utilityStat;

    public Color GetHeroHighlightedColor() => _heroHighlightedColor;
    public Color GetHeroPressedColor() => _heroPressedColor;
    public Color GetHeroSelectedColor() => _heroSelectedColor;

    public string GetHeroBasicAbilityDescription() => _basicAbilityDescription;
    public string GetHeroManualAbilityDescription() => _manualAbilityDescription;
    public string GetHeroPassiveAbilityDescription() => _passiveAbilityDescription;
    #endregion
}
