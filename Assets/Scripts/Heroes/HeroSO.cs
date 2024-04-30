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

    [Header("Visuals")]
    [SerializeField] private Sprite _heroIcon;

    [Header("Prefabs")]
    [SerializeField] private GameObject _heroPrefab;

    #region Getters
    public string GetHeroName() => _name;
    public float GetMaxHP() => _maxHP;
    public float GetMoveSpeed() => _moveSpeed;
    public float GetAggro() => _aggro;
    public float GetDamageResistance() => _damageResistance;

    public Sprite GetHeroIcon() => _heroIcon;
    public GameObject GetHeroPrefab() => _heroPrefab;
    #endregion
}
