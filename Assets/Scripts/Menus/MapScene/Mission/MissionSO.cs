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
    [Header("Tutorials")]
    [SerializeField] private TutorialPage[] _tutorialPages;

    [Space] 
    [Header("Unlocks")] 
    [SerializeField] private CharacterSO _characterUnlock;

    [Space] 
    [SerializeField] private MissionSO[] _missionUnlocks;

    [Space] 
    [Header("Modifiers")]
    [SerializeField] private MissionStatModifiers _missionStatModifiers;
    
    #region Getters
    public int GetMissionID() =>_missionID;
    public string GetMissionName() =>_missionName;
    
    public EGameDifficulty GetAssociatedDifficulty() =>_associatedDifficulty;
    public LevelSO GetAssociatedLevel() =>_associatedLevel;
    public HeroSO[] GetAssociatedHeroes() =>_associatedHeroes;

    public TutorialPage[] GetTutorialPages() => _tutorialPages;
    
    public CharacterSO GetCharacterUnlock() =>_characterUnlock;
    public MissionSO[] GetMissionUnlocks() =>_missionUnlocks;
    
    public MissionStatModifiers GetMissionStatModifiers() =>_missionStatModifiers;
    #endregion
}

[System.Serializable]
public class MissionStatModifiers
{
    [SerializeField] private float _bossHealthMultiplier = 1;
    [SerializeField] private float _bossStaggerMultiplier = 1;
    [SerializeField] private float _bossAttackSpeedMultiplier = 1;
    [SerializeField] private float _bossDamageResistanceChangeOnStaggerMultiplier = 1;
    [SerializeField] private float _bossEnrageTimeMultiplier = 1;
    [SerializeField] private float _bossEnrageDamageMultiplier = 1;
    
    [SerializeField] private bool[] _bossAbilitiesUsable = { true, true, true, true, true };

    [Space] 
    [SerializeField] private float _heroHealthMultiplier = 1;
    [SerializeField] private float _heroDamageMultiplier = 1;
    [SerializeField] private float _heroStaggerMultiplier = 1;
    [SerializeField] private float _heroHealingReceivedMultiplier = 1;
    [SerializeField] private float _heroManualCooldownTimeMultiplier = 1;
    
    #region Getters
    public float GetBossHealthMultiplier() =>_bossHealthMultiplier;
    public float GetBossStaggerMultiplier() =>_bossStaggerMultiplier;
    public float GetBossAttackSpeedMultiplier() => _bossAttackSpeedMultiplier;
    public float GetBossDamageResistanceChangeOnStaggerMultiplier() => _bossDamageResistanceChangeOnStaggerMultiplier;
    public float GetBossEnrageTimeMultiplier() => _bossEnrageTimeMultiplier;
    public float GetBossEnrageDamageMultiplier() => _bossEnrageDamageMultiplier;
    public bool[] GetBossAbilitiesUsable() =>_bossAbilitiesUsable;
    
    public float GetHeroHealthMultiplier() =>_heroHealthMultiplier;
    public float GetHeroDamageMultiplier() =>_heroDamageMultiplier;
    public float GetHeroStaggerMultiplier() =>_heroStaggerMultiplier;
    public float GetHeroHealingReceivedMultiplier() =>_heroHealingReceivedMultiplier;
    public float GetHeroManualCooldownTimeMultiplier() =>_heroManualCooldownTimeMultiplier;
    #endregion
}

[System.Serializable]
public class TutorialPage
{
    [TextArea(1, 2)] public string TutorialPageTitle;
    public float TutorialPageTitleWidth;
    public GameObject TutorialPageObject;
}