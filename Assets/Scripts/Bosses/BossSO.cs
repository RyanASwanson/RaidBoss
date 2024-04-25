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
    [SerializeField] private float _baseStaggerCounter;
}
