using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroScriptableObject", menuName = "HeroScriptableObject", order = 1)]
public class HeroSO : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string _name;

    [Header("Stats")]
    [SerializeField] private float _maxHp;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _aggro;

    [Header("Visuals")]
    [SerializeField] private Sprite _heroIcon;
    [SerializeField] private MeshFilter _mesh;

    #region Getters
    public string GetHeroName() => _name;
    public float GetMaxHP() => _maxHp;
    public float GetMoveSpeed() => _moveSpeed;
    public float GetAggro() => _aggro;
    public Sprite GetHeroIcon() => _heroIcon;
    #endregion
}
