using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementScriptableObject", menuName = "AchievementScriptableObject", order = 5)]
public class AchievementSO : ScriptableObject
{
    public string AchievmentName;
    public string AchievementUnlockName;
    public int AchievmentID;
    [TextArea(1, 2)] public string AchievementDescription;
}
