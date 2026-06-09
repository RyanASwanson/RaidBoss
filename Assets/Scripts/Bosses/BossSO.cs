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

    [SerializeField] private float _enrageScalingDamageMultiplierIncreasePerMinute;

    [Header("Prefabs")]
    [SerializeField] private GameObject _bossPrefab;

    [Header("Visuals")]
    [SerializeField] private Sprite _bossIcon;
    
    [Space] 
    [Header("Environment")] 
    [SerializeField] private Material _floorMaterial;
    [Space]
    [SerializeField] private Material _miniFloorMaterial;
    [SerializeField] private Material _miniFloorLockedMaterial;
    [Space]
    [SerializeField] private Material _missionSelectionGlowMaterial;
    [Space]
    [SerializeField] private Material _backgroundMaterial;
    [SerializeField] private GameObject _backgroundParticles;

    [Space] 
    [SerializeField] private GameObject _bossStandard;

    [Header("Selection")]
    [SerializeField] private Color _bossHighlightedColor;
    [SerializeField] private Color _bossPressedColor;
    [SerializeField] private Color _bossSelectedColor;
    
    [SerializeField] private Color _bossUIColor;
    [SerializeField] private Color _bossAbilityTextUIColor;

    [SerializeField] private Sprite _bossSelectionIcon;

    [Header("Ability Information")]
    [SerializeField] private List<BossAbilityInformation> _bossAbilities;
    
    [Header("Achievements")]
    [SerializeField] protected AchievementSO[] _associatedAchievements;

    private const int _bossSpecialistAchievementID = 0;

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
    public float GetEnrageScalingDamageMultiplierIncreasePerMinute() => _enrageScalingDamageMultiplierIncreasePerMinute;

    public float GetEnrageScalingDamageMultiplierIncreasePerSecond() =>
        _enrageScalingDamageMultiplierIncreasePerMinute / 60;

    public GameObject GetBossPrefab() => _bossPrefab;

    public Sprite GetBossIcon() => _bossIcon;
    
    public Material GetFloorMaterial() => _floorMaterial;
    public Material GetMiniFloorMaterial() => _miniFloorMaterial;
    public Material GetMiniFloorLockedMaterial() => _miniFloorLockedMaterial;
    public Material GetMissionSelectionGlowMaterial() => _missionSelectionGlowMaterial;
    public Material GetBackgroundMaterial() => _backgroundMaterial;
    public GameObject GetBackgroundParticles() => _backgroundParticles;
    public GameObject GetBossStandard() => _bossStandard;

    public Color GetBossHighlightedColor() => _bossHighlightedColor;
    public Color GetBossPressedColor() => _bossPressedColor;
    public Color GetBossSelectedColor() => _bossSelectedColor;
    public Color GetBossUIColor() => _bossUIColor;
    public Color GetBossAbilityTextUIColor() => _bossAbilityTextUIColor;
    
    public Sprite GetBossSelectionIcon() => _bossSelectionIcon;

    public List<BossAbilityInformation> GetBossAbilityInformation() => _bossAbilities;

    public AchievementSO[] GetAssociatedAchievements() => _associatedAchievements;

    public AchievementSO GetAssociatedSpecialistAchievement()
    {
        if (_associatedAchievements.Length < _bossSpecialistAchievementID + 1)
        {
            return null;
        }
        
        return _associatedAchievements[_bossSpecialistAchievementID];
    }
    
    public Sprite GetAbilityIconFromID(int id) => _bossAbilities[id]._abilityImage;
    public string GetAbilityNameFromID(int id) => _bossAbilities[id]._abilityName;
    public EBossAbilityType GetAbilityTypeFromID(int id) => _bossAbilities[id]._abilityType;
    public string GetAbilityDescriptionFromID(int id) => _bossAbilities[id]._abilityDescription;
    public string GetAbilityWideDescriptionFromID(int id) => _bossAbilities[id]._abilityWideDescription;
    #endregion
}

[System.Serializable]
public class BossAbilityInformation
{
    [TextArea(1, 2)] public string _abilityName;
    public EBossAbilityType _abilityType;
    [TextArea(5, 10)] public string _abilityDescription;
    [TextArea(5, 10)] public string _abilityWideDescription;

    public Sprite _abilityImage;
}

public enum EBossAbilityType
{
    Active,
    Passive,
    Hybrid
};
