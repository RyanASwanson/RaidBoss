using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the general information for all levels as scriptable objects
/// </summary>
[CreateAssetMenu(fileName = "LevelScriptableObject", menuName = "LevelScriptableObject", order = 2)]
public class LevelSO : ScriptableObject
{
    [SerializeField] private int _levelNumber;
    [SerializeField] private int _levelBuildID;
    [SerializeField] private BossSO _associatedBoss;

    #region Getters
    public int GetLevelNumber() => _levelNumber;
    public int GetLevelBuildID() => _levelBuildID;
    public BossSO GetLevelBoss() => _associatedBoss;
    #endregion

    #region Setters

    #endregion
}
