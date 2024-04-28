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

    [Header("Prefabs")]
    [SerializeField] private GameObject _bossPrefab;

    #region Getters
    public string GetHeroName() => _name;
    public float GetMaxHP() => _maxHP;
    public float GetBaseStaggerMax() => _baseStaggerMax;

    public GameObject GetBossPrefab() => _bossPrefab;
    #endregion
}
