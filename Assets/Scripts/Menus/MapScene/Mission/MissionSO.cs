using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionScriptableObject", menuName = "MissionScriptableObject", order = 3)]
public class MissionSO : ScriptableObject
{
    [SerializeField] private int _missionID;
    [TextArea(1, 2)] [SerializeField] private string _missionName;

    [Space] 
    [Header("Selection")]
    [SerializeField] private EGameDifficulty _associatedDifficulty;
    [SerializeField] private LevelSO _associatedLevel;
    [SerializeField] private HeroSO[] _associatedHeroes;

    [Space] 
    [Header("Unlocks")] 
    [SerializeField] private CharacterSO _characterUnlock;

    [Space] 
    [SerializeField] private MissionSO[] _missionUnlocks;

    [Space] 
    [Header("Modifiers")]
    [SerializeField] private float _bossHealthMultiplier = 1;
    [SerializeField] private float _bossStaggerMultiplier = 1;
    
    [SerializeField] private bool[] _bossAbilitiesUsable = { true, true, true, true, true };

    [Space] 
    [SerializeField] private float _heroHealthMultiplier = 1;
    [SerializeField] private float _heroDamageMultiplier = 1;
    [SerializeField] private float _heroStaggerMultiplier = 1;
    [SerializeField] private float _heroHealingMultiplier = 1;
    [SerializeField] private float _heroManualCooldownMultiplier = 1;
    
    #region Getters
    public int GetMissionID() =>_missionID;
    public string GetMissionName() =>_missionName;
    
    public EGameDifficulty GetAssociatedDifficulty() =>_associatedDifficulty;
    public LevelSO GetAssociatedLevel() =>_associatedLevel;
    public HeroSO[] GetAssociatedHeroes() =>_associatedHeroes;
    
    public CharacterSO GetCharacterUnlock() =>_characterUnlock;
    public MissionSO[] GetMissionUnlocks() =>_missionUnlocks;
    #endregion
}
