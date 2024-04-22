using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroScriptableObject", menuName = "HeroScriptableObject", order = 1)]
public class HeroSO : ScriptableObject
{
    [SerializeField] private string _heroName;
    [SerializeField] private float _maxHp;
    [SerializeField] private float _moveSpeed;

    #region Getters
    public string GetHeroName()
    {
        return _heroName;
    }
    #endregion
}
