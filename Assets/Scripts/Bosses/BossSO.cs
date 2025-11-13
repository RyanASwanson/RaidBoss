using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Stores the general information for all bosses as scriptable objects
/// </summary>
[CreateAssetMenu(fileName = "BossScriptableObject", menuName = "BossScriptableObject", order = 3)]
public class BossSO : CharacterSO
{
    [Header("General")]
    [SerializeField] private string _name;
    [TextArea(2, 4)][SerializeField] private string _selectionScreenName;
    [SerializeField] private int _bossID;
    [SerializeField] private int _bossMusicID;

    [Header("Stats")]
    [SerializeField] private float _maxHP;
    [SerializeField] private float _baseStaggerMax;

    [SerializeField] private float _staggerDuration;

    [SerializeField] private float _rotationSpeed;

    [SerializeField] private float _damageIncrementMultiplier;

    [Range(0,.5f)] [SerializeField] private float _damageResistanceChangeOnStagger;

    [SerializeField] private float _fightStartDelay;
    
    [SerializeField] private float _enrageTime;
    [SerializeField] private float _enrageDamageMultiplier;

    [Header("Prefabs")]
    [SerializeField] private GameObject _bossPrefab;

    [Header("Visuals")]
    [SerializeField] private Sprite _bossIcon;
    
    [Space] 
    [Header("Environment")] 
    [SerializeField] private Material _floorMaterial;
    [SerializeField] private Material _backgroundMaterial;
    [SerializeField] private GameObject _backgroundParticles;

    [Header("Selection")]
    [SerializeField] private Color _bossHighlightedColor;
    [SerializeField] private Color _bossPressedColor;
    [SerializeField] private Color _bossSelectedColor;

    [Header("Ability Information")]
    [SerializeField] private List<BossAbilityInformation> _bossAbilities;
    

    #region Getters
    public string GetBossName() => _name;
    public string GetBossSelectionScreenName() => _selectionScreenName;
    public int GetBossID() => _bossID;
    public int GetBossMusicID() => _bossMusicID;
    public float GetMaxHP() => _maxHP;
    public float GetBaseStaggerMax() => _baseStaggerMax;

    public float GetStaggerDuration() => _staggerDuration;

    public float GetBossRotationSpeed() => _rotationSpeed;
    public float GetBossDamageIncrementMultiplier() => _damageIncrementMultiplier;
    public float GetDamageResistanceChangeOnStagger() => _damageResistanceChangeOnStagger;

    public float GetFightStartDelay() => _fightStartDelay;
    
    public float GetEnrageTime() => _enrageTime;
    public float GetEnrageDamageMultiplier() => _enrageDamageMultiplier;

    public GameObject GetBossPrefab() => _bossPrefab;

    public Sprite GetBossIcon() => _bossIcon;
    
    public Material GetFloorMaterial() => _floorMaterial;
    public Material GetBackgroundMaterial() => _backgroundMaterial;
    public GameObject GetBackgroundParticles() => _backgroundParticles;

    public Color GetBossHighlightedColor() => _bossHighlightedColor;
    public Color GetBossPressedColor() => _bossPressedColor;
    public Color GetBossSelectedColor() => _bossSelectedColor;

    public List<BossAbilityInformation> GetBossAbilityInformation() => _bossAbilities;
    #endregion
}

[System.Serializable]
public class BossAbilityInformation
{
    [TextArea(1, 2)] public string _abilityName;
    public BossAbilityType _abilityType;
    [TextArea(5, 10)] public string _abilityDescription;

    public Sprite _abilityImage;
}

public enum BossAbilityType
{
    Active,
    Passive
};
