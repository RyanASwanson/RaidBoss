using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BossScriptableObject", menuName = "BossScriptableObject", order = 3)]
public class BossSO : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string _name;

    [Header("Stats")]
    [SerializeField] private float _maxHP;
    [SerializeField] private float _baseStaggerMax;

    [SerializeField] private float _staggerDuration;

    [SerializeField] private float _rotationSpeed;

    [SerializeField] private float _damageIncrementMultiplier;

    [Range(.5f, 1)] [SerializeField] private float _damageResistanceChangeOnStagger;

    [Header("Prefabs")]
    [SerializeField] private GameObject _bossPrefab;

    [Header("Visuals")]
    [SerializeField] private Sprite _bossIcon;

    [Header("Selection")]
    [SerializeField] private Color _bossHighlightedColor;
    [SerializeField] private Color _bossPressedColor;
    [SerializeField] private Color _bossSelectedColor;

    [Header("Ability Information")]
    [SerializeField] private List<BossAbilityInformation> _bossAbilities;

    #region Getters
    public string GetBossName() => _name;
    public float GetMaxHP() => _maxHP;
    public float GetBaseStaggerMax() => _baseStaggerMax;

    public float GetStaggerDuration() => _staggerDuration;

    public float GetBossRotationSpeed() => _rotationSpeed;
    public float GetBossDamageIncrementMultiplier() => _damageIncrementMultiplier;
    public float GetDamageResistanceChangeOnStagger() => _damageResistanceChangeOnStagger;

    public GameObject GetBossPrefab() => _bossPrefab;

    public Sprite GetBossIcon() => _bossIcon;

    public Color GetBossHighlightedColor() => _bossHighlightedColor;
    public Color GetBossPressedColor() => _bossPressedColor;
    public Color GetBossSelectedColor() => _bossSelectedColor;

    public List<BossAbilityInformation> GetBossAbilityInformation() => _bossAbilities;
    #endregion
}

[System.Serializable]
public class BossAbilityInformation
{
    public string _abilityName;
    public BossAbilityType _abilityType;
    [TextArea(5, 10)] public string _abilityDescription;

    public Sprite _abilityImage;
}

public enum BossAbilityType
{
    Active,
    Passive
};
