using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroScriptableObject", menuName = "HeroScriptableObject", order = 1)]
public class HeroSO : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private float _maxHp;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _aggro;
    [SerializeField] private float _attackRange;

    #region Getters
    public string GetHeroName() => _name;
    public float GetMaxHP() => _maxHp;
    public float GetMoveSpeed() => _moveSpeed;
    public float GetAggro() => _aggro;
    public float GetRangeAggro() => _attackRange;
    #endregion
}
