using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectionManager : BaseUniversalManager
{
    private List<HeroSO> _selectedHeroes = new List<HeroSO>();
    private const int _maxHeroes = 5;

    private LevelSO _selectedLevel;
    private BossSO _selectedBoss;

    private GameDifficulty _currentGameDifficulty;


    private UnityEvent<BossSO> _bossSelectionEvent = new UnityEvent<BossSO>();
    private UnityEvent<GameDifficulty> _difficultySelectionEvent = new UnityEvent<GameDifficulty>();
    private UnityEvent<HeroSO> _heroSelectionEvent = new UnityEvent<HeroSO>();
    private UnityEvent<HeroSO> _heroDeselectionEvent = new UnityEvent<HeroSO>();

    /// <summary>
    /// Adds a new hero to the list of selected heroes
    /// To be used for the selection scene
    /// </summary>
    /// <param name="newHeroSO"></param>
    public void AddNewSelectedHero(HeroSO newHeroSO)
    {
        if (_selectedHeroes.Count < _maxHeroes)
            _selectedHeroes.Add(newHeroSO);
    }


    /// <summary>
    /// Removes a hero from the list of selected heroes
    /// To be used for the selection scene
    /// </summary>
    /// <param name="removingHero"></param>
    public void RemoveSpecificHero(HeroSO removingHero)
    {
        if(_selectedHeroes.Contains(removingHero))
            _selectedHeroes.Remove(removingHero);    
    }


    #region Events
    public void InvokeBossSelectionEvent(BossSO bossSO)
    {
        _bossSelectionEvent?.Invoke(bossSO);
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
    #endregion

    #region Getters
    public List<HeroSO> GetAllSelectedHeroes() => _selectedHeroes;
    public HeroSO GetHeroAtValue(int val) => _selectedHeroes[val];
    public int GetSelectedHeroesCount() => _selectedHeroes.Count;
    public int GetMaxHeroesCount() => _maxHeroes;

    public BossSO GetSelectedBoss() => _selectedBoss;
    public LevelSO GetSelectedLevel() => _selectedLevel;

    public UnityEvent<BossSO> GetBossSelectionEvent() => _bossSelectionEvent;
    public UnityEvent<GameDifficulty> GetDifficultySelectionEvent() => _difficultySelectionEvent;
    public UnityEvent<HeroSO> GetHeroSelectionEvent() => _heroSelectionEvent;
    public UnityEvent<HeroSO> GetHeroDeselectionEvent() => _heroDeselectionEvent;
    #endregion

    #region Setters
    public void SetSelectedBoss(BossSO bossSO)
    {
        _selectedBoss = bossSO;
    }

    public void SetSelectedLevel(LevelSO levelSO)
    {
        _selectedLevel = levelSO;
    }
    public void SetSelectedDifficulty(GameDifficulty gameDifficulty)
    {
        _currentGameDifficulty = gameDifficulty;
    }
    #endregion
}

public enum GameDifficulty
{
    Easy,
    Medium,
    Hard
};
