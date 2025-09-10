using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the general information for all heroes as scriptable objects
/// </summary>
[CreateAssetMenu(fileName = "HeroScriptableObject", menuName = "HeroScriptableObject", order = 1)]
public class HeroSO : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string _name;
    [SerializeField] private int _heroID;
    [SerializeField] private bool _hasUIManager;

    [Header("Stats")]
    [SerializeField] private float _maxHP;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _angularSpeed;
    [SerializeField] private float _moveAcceleration;
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

    [Space]
    [SerializeField] private EHeroRange _heroRange;
    [SerializeField] private EHeroDifficulty _heroDifficulty;

    [Space]
    [SerializeField] private Color _heroHighlightedColor;
    [SerializeField] private Color _heroPressedColor;
    [SerializeField] private Color _heroSelectedColor;

    [SerializeField] private Color _heroUIColor;

    [Header("Ability Information")]
    [SerializeField] private string _basicAbilityName;
    [TextArea(4, 10)] [SerializeField] private string _basicAbilityDescription;
    [SerializeField] private string _manualAbilityName;
    [TextArea(4, 10)] [SerializeField] private string _manualAbilityDescription;
    [SerializeField] private string _passiveAbilityName;
    [TextArea(4, 10)] [SerializeField] private string _passiveAbilityDescription;

    #region Getters
    public string GetHeroName() => _name;
    public int GetHeroID() => _heroID;
    public bool GetHasUIManager() => _hasUIManager;

    public float GetMaxHP() => _maxHP;
    public float GetMoveSpeed() => _moveSpeed;
    public float GetAngularSpeed() => _angularSpeed;
    public float GetMoveAcceleration() => _moveAcceleration;
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

    public EHeroRange GetHeroRange() => _heroRange;
    public EHeroDifficulty GetHeroDifficulty() => _heroDifficulty;

    public Color GetHeroHighlightedColor() => _heroHighlightedColor;
    public Color GetHeroPressedColor() => _heroPressedColor;
    public Color GetHeroSelectedColor() => _heroSelectedColor;
    public Color GetHeroUIColor() => _heroUIColor;

    public string GetHeroBasicAbilityName() => _basicAbilityName;
    public string GetHeroBasicAbilityDescription() => _basicAbilityDescription;
    public string GetHeroManualAbilityName() => _manualAbilityName;
    public string GetHeroManualAbilityDescription() => _manualAbilityDescription;
    public string GetHeroPassiveAbilityName() => _passiveAbilityName;
    public string GetHeroPassiveAbilityDescription() => _passiveAbilityDescription;
    #endregion
}

public enum EHeroAbilityType
{
    Basic,
    Manual,
    Passive
};

public enum EHeroRange
{
    Close,
    Medium,
    Far
};

public enum EHeroDifficulty
{
    Easy,
    Medium,
    Hard
};
