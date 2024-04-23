using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelScriptableObject", menuName = "LevelScriptableObject", order = 2)]
public class LevelSO : ScriptableObject
{
    [SerializeField] private int _levelNumber;
    [SerializeField] private int _levelBuildID;

    #region Getters
    public int GetLevelNumber() => _levelNumber;
    public int GetLevelBuildID() => _levelBuildID;
    #endregion

    #region Setters

    #endregion
}
