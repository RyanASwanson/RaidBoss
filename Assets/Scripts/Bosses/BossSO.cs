using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Prefabs")]
    [SerializeField] private GameObject _bossPrefab;

    [Header("Visuals")]
    [SerializeField] private Sprite _bossIcon;

    [Header("Selection")]
    [SerializeField] private Color _bossHighlightedColor;
    [SerializeField] private Color _bossPressedColor;
    [SerializeField] private Color _bossSelectedColor;

    #region Getters
    public string GetBossName() => _name;
    public float GetMaxHP() => _maxHP;
    public float GetBaseStaggerMax() => _baseStaggerMax;

    public float GetStaggerDuration() => _staggerDuration;

    public float GetBossRotationSpeed() => _rotationSpeed;
    public float GetBossDamageIncrementMultiplier() => _damageIncrementMultiplier;

    public GameObject GetBossPrefab() => _bossPrefab;

    public Sprite GetBossIcon() => _bossIcon;

    public Color GetBossHighlightedColor() => _bossHighlightedColor;
    public Color GetBossPressedColor() => _bossPressedColor;
    public Color GetBossSelectedColor() => _bossSelectedColor;
    #endregion
}
