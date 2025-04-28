using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectionManager : MainUniversalManagerFramework
{
    [Header("Difficulty")]
    [Range(1, 5)] [SerializeField] private float _normalDamageMultiplier;
    [Range(1, 3)] [SerializeField] private float _normalSpeedMultiplier;
    [Range(1, 3)] [SerializeField] private float _normalHealthMultiplier;
    [Range(1, 3)] [SerializeField] private float _normalStaggerMultiplier;
    [Space]
    [Range(1, 5)] [SerializeField] private int _normalHeroLimit;
    [Space]
    [Range(1, 5)] [SerializeField] private float _heroicDamageMultiplier;
    [Range(1, 3)] [SerializeField] private float _heroicSpeedMultiplier;
    [Range(1, 3)] [SerializeField] private float _heroicHealthMultiplier;
    [Range(1, 3)] [SerializeField] private float _heroicStaggerMultiplier;
    [Space]
    [Range(1, 5)] [SerializeField] private int _heroicHeroLimit;
    [Space]
    [Range(1, 5)] [SerializeField] private float _mythicDamageMultiplier;
    [Range(1, 3)] [SerializeField] private float _mythicSpeedMultiplier;
    [Range(1, 3)] [SerializeField] private float _mythicHealthMultiplier;
    [Range(1, 3)] [SerializeField] private float _mythicStaggerMultiplier;
    [Space]
    [Range(1, 5)] [SerializeField] private int _mythicHeroLimit;
    [Space]
    [Range(1, 5)] [SerializeField] private float _mythicPlusDamageMultiplier;
    [Range(1, 3)] [SerializeField] private float _mythicPlusSpeedMultiplier;
    [Range(1, 3)] [SerializeField] private float _mythicPlusHealthMultiplier;
    [Range(1, 3)] [SerializeField] private float _mythicPlusStaggerMultiplier;
    [Space]
    [Range(1, 5)] [SerializeField] private int _mythicPlusHeroLimit;

    [Space]
    [SerializeField] private List<string> _difficultyNames;
    [SerializeField] private List<Sprite> _difficultyIcons;

    private Dictionary<GameDifficulty, float> _difficultyDamageMultiplierDictionary = new();
    private Dictionary<GameDifficulty, float> _difficultyAttackSpeedMultiplierDictionary = new();
    private Dictionary<GameDifficulty, float> _difficultyHealthMultiplierDictionary = new();
    private Dictionary<GameDifficulty, float> _difficultyStaggerMultiplierDictionary = new();

    private Dictionary<GameDifficulty, int> _difficultyHeroLimit = new();

    private LevelSO _selectedLevel;
    private BossSO _selectedBoss;

    private List<HeroSO> _selectedHeroes = new List<HeroSO>();
    private int _previousMaxHeroes = 3;
    private const int _maxHeroes = 5;
    private int _indexOfLastRemovedHero;

    private GameDifficulty _currentGameDifficulty = GameDifficulty.Normal;


    private UnityEvent<BossSO> _bossSelectionEvent = new UnityEvent<BossSO>();
    private UnityEvent<BossSO> _bossDeselectionEvent = new UnityEvent<BossSO>();
    private UnityEvent<BossSO> _bossSwapEvent = new UnityEvent<BossSO>();
    private UnityEvent<BossSO> _bossHoveredOverEvent = new UnityEvent<BossSO>();
    private UnityEvent<BossSO> _bossNotHoveredOverEvent = new UnityEvent<BossSO>();

    private UnityEvent<GameDifficulty> _difficultySelectionEvent = new UnityEvent<GameDifficulty>();

    private UnityEvent<HeroSO> _heroSelectionEvent = new UnityEvent<HeroSO>();
    private UnityEvent<HeroSO> _heroDeselectionEvent = new UnityEvent<HeroSO>();
    private UnityEvent<HeroSO> _heroSwapEvent = new UnityEvent<HeroSO>();
    private UnityEvent<HeroSO> _heroHoveredOverEvent = new UnityEvent<HeroSO>();
    private UnityEvent<HeroSO> _heroNotHoveredOverEvent = new UnityEvent<HeroSO>();


    private void SetupDifficultyDictionaries()
    {
        _difficultyDamageMultiplierDictionary.Add(GameDifficulty.Normal, _normalDamageMultiplier);
        _difficultyAttackSpeedMultiplierDictionary.Add(GameDifficulty.Normal, _normalSpeedMultiplier);
        _difficultyHealthMultiplierDictionary.Add(GameDifficulty.Normal, _normalHealthMultiplier);
        _difficultyStaggerMultiplierDictionary.Add(GameDifficulty.Normal, _normalStaggerMultiplier);

        _difficultyHeroLimit.Add(GameDifficulty.Normal, _normalHeroLimit);

        _difficultyDamageMultiplierDictionary.Add(GameDifficulty.Heroic, _heroicDamageMultiplier);
        _difficultyAttackSpeedMultiplierDictionary.Add(GameDifficulty.Heroic, _heroicSpeedMultiplier);
        _difficultyHealthMultiplierDictionary.Add(GameDifficulty.Heroic, _heroicHealthMultiplier);
        _difficultyStaggerMultiplierDictionary.Add(GameDifficulty.Heroic, _heroicStaggerMultiplier);

        _difficultyHeroLimit.Add(GameDifficulty.Heroic, _heroicHeroLimit);

        _difficultyDamageMultiplierDictionary.Add(GameDifficulty.Mythic, _mythicDamageMultiplier);
        _difficultyAttackSpeedMultiplierDictionary.Add(GameDifficulty.Mythic, _mythicSpeedMultiplier);
        _difficultyHealthMultiplierDictionary.Add(GameDifficulty.Mythic, _mythicHealthMultiplier);
        _difficultyStaggerMultiplierDictionary.Add(GameDifficulty.Mythic, _mythicStaggerMultiplier);

        _difficultyHeroLimit.Add(GameDifficulty.Mythic, _mythicHeroLimit);

        _difficultyDamageMultiplierDictionary.Add(GameDifficulty.MythicPlus, _mythicPlusDamageMultiplier);
        _difficultyAttackSpeedMultiplierDictionary.Add(GameDifficulty.MythicPlus, _mythicPlusSpeedMultiplier);
        _difficultyHealthMultiplierDictionary.Add(GameDifficulty.MythicPlus, _mythicPlusHealthMultiplier);
        _difficultyStaggerMultiplierDictionary.Add(GameDifficulty.MythicPlus, _mythicPlusStaggerMultiplier);

        _difficultyHeroLimit.Add(GameDifficulty.MythicPlus, _mythicPlusHeroLimit);
    }

    public void RemoveSelectedLevel()
    {
        _selectedLevel = null;
    }

    public void RemoveSelectedBoss()
    {
        InvokeBossDeselectionEvent(_selectedBoss);
        _selectedBoss = null;
    }

    /// <summary>
    /// Adds a new hero to the list of selected heroes
    /// To be used for the selection scene
    /// </summary>
    /// <param name="newHeroSO"></param>
    public void AddNewSelectedHero(HeroSO newHeroSO)
    {
        if (AtMaxHeroesSelected())
        {
            InvokeHeroSwapEvent(_selectedHeroes[GetMaxHeroesCount()-1]);
        }

        _selectedHeroes.Add(newHeroSO);
        InvokeHeroSelectionEvent(newHeroSO);
    }


    /// <summary>
    /// Removes a hero from the list of selected heroes
    /// To be used for the selection scene
    /// </summary>
    /// <param name="removingHero"></param>
    public void RemoveSpecificHero(HeroSO removingHero)
    {
        if (!_selectedHeroes.Contains(removingHero))
            return;

        _indexOfLastRemovedHero = _selectedHeroes.IndexOf(removingHero);

        _selectedHeroes.Remove(removingHero);
        InvokeHeroDeselectionEvent(removingHero);
    }

    public void BossHoveredOver(BossSO bossSO)
    {
        InvokeBossHoveredOverEvent(bossSO);
    }
    public void BossNotHoveredOver(BossSO bossSO)
    {
        InvokeBossNotHoveredOverEvent(bossSO);
    }
    public void HeroHoveredOver(HeroSO heroSO)
    {
        InvokeHeroHoveredOverEvent(heroSO);
    }

    public void HeroNotHoveredOver(HeroSO heroSO)
    {
        InvokeHeroNotHoveredOverEvent(heroSO);
    }

    /// <summary>
    /// Removes the currently selected heroes, boss, level
    /// Difficulty is not reset
    /// </summary>
    public void ResetSelectionData()
    {
        _selectedHeroes = new();
        _selectedBoss = null;
        _selectedLevel = null;
    }

    #region BaseManager
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        SetupDifficultyDictionaries();
    }
    #endregion

    #region Events
    public void InvokeBossSelectionEvent(BossSO bossSO)
    {
        _bossSelectionEvent?.Invoke(bossSO);
    }
    public void InvokeBossDeselectionEvent(BossSO bossSO)
    {
        _bossDeselectionEvent?.Invoke(bossSO);
    }
    public void InvokeBossSwapEvent(BossSO previousBoss)
    {
        _bossSwapEvent?.Invoke(previousBoss);
    }

    public void InvokeBossHoveredOverEvent(BossSO bossSO)
    {
        _bossHoveredOverEvent?.Invoke(bossSO);
    }
    public void InvokeBossNotHoveredOverEvent(BossSO bossSO)
    {
        _bossNotHoveredOverEvent?.Invoke(bossSO);
    }

    public void InvokeDifficultySelectionEvent(GameDifficulty gameDifficulty)
    {
        _difficultySelectionEvent?.Invoke(gameDifficulty);
    }

    public void InvokeHeroSelectionEvent(HeroSO heroSO)
    {
        _heroSelectionEvent?.Invoke(heroSO);
    }

    public void InvokeHeroDeselectionEvent(HeroSO heroSO)
    {
        _heroDeselectionEvent?.Invoke(heroSO);
    }
    public void InvokeHeroSwapEvent(HeroSO previousHero)
    {
        _heroSwapEvent?.Invoke(previousHero);
    }

    public void InvokeHeroHoveredOverEvent(HeroSO heroSO)
    {
        _heroHoveredOverEvent?.Invoke(heroSO);
    }
    public void InvokeHeroNotHoveredOverEvent(HeroSO heroSO)
    {
        _heroNotHoveredOverEvent?.Invoke(heroSO);
    }
    #endregion

    #region Getters
    public bool AtMaxBossSelected() => _selectedBoss != null;

    public float GetDamageMultiplierFromDifficulty() => _difficultyDamageMultiplierDictionary[_currentGameDifficulty];
    public float GetSpeedMultiplierFromDifficulty() => _difficultyAttackSpeedMultiplierDictionary[_currentGameDifficulty];
    public float GetHealthMultiplierFromDifficulty() => _difficultyHealthMultiplierDictionary[_currentGameDifficulty];
    public float GetStaggerMultiplierFromDifficulty() => _difficultyHealthMultiplierDictionary[_currentGameDifficulty];

    public int GetHeroLimitFromDifficulty() => _difficultyHeroLimit[_currentGameDifficulty];

    public List<string> GetDifficultyNames() => _difficultyNames;
    public List<Sprite> GetDifficultyIcons() => _difficultyIcons;

    public List<HeroSO> GetAllSelectedHeroes() => _selectedHeroes;
    public HeroSO GetHeroAtValue(int val) => _selectedHeroes[val];
    public HeroSO GetHeroAtLastPostion() => GetHeroAtValue(GetSelectedHeroesCount() - 1);
    public int GetSelectedHeroesCount() => _selectedHeroes.Count;
    //public int GetMaxHeroesCount() => _maxHeroes;
    public int GetMaxHeroesCount() => GetHeroLimitFromDifficulty();
    public bool AtMaxHeroesSelected() => _selectedHeroes.Count >= GetMaxHeroesCount();
    public int GetIndexOfLastHeroRemoved() => _indexOfLastRemovedHero;
    

    public BossSO GetSelectedBoss() => _selectedBoss;
    public LevelSO GetSelectedLevel() => _selectedLevel;

    public UnityEvent<BossSO> GetBossSelectionEvent() => _bossSelectionEvent;
    public UnityEvent<BossSO> GetBossDeselectionEvent() => _bossDeselectionEvent;
    public UnityEvent<BossSO> GetBossSwapEvent() => _bossSwapEvent;
    public UnityEvent<BossSO> GetBossHoveredOverEvent() => _bossHoveredOverEvent;
    public UnityEvent<BossSO> GetBossNotHoveredOverEvent() => _bossNotHoveredOverEvent;

    public GameDifficulty GetSelectedDifficulty() => _currentGameDifficulty;
    public UnityEvent<GameDifficulty> GetDifficultySelectionEvent() => _difficultySelectionEvent;

    public UnityEvent<HeroSO> GetHeroSelectionEvent() => _heroSelectionEvent;
    public UnityEvent<HeroSO> GetHeroDeselectionEvent() => _heroDeselectionEvent;
    public UnityEvent<HeroSO> GetHeroSwapEvent() => _heroSwapEvent;
    public UnityEvent<HeroSO> GetHeroHoveredOverEvent() => _heroHoveredOverEvent;
    public UnityEvent<HeroSO> GetHeroNotHoveredOverEvent() => _heroNotHoveredOverEvent;
    #endregion

    #region Setters
    public void SetSelectedBoss(BossSO bossSO)
    {
        if (AtMaxBossSelected())
        {
            InvokeBossSwapEvent(_selectedBoss);
        }
        

        _selectedBoss = bossSO;

        InvokeBossSelectionEvent(bossSO);
    }

    public void SetSelectedLevel(LevelSO levelSO)
    {
        _selectedLevel = levelSO;
    }
    public void SetSelectedDifficulty(GameDifficulty gameDifficulty)
    {
        _currentGameDifficulty = gameDifficulty;
        InvokeDifficultySelectionEvent(gameDifficulty);
    }
    #endregion
}

public enum GameDifficulty
{
    Empty,
    Normal,
    Heroic,
    Mythic,
    MythicPlus
};
