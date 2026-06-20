using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides the functionality for general management of heroes
/// Spawns heroes into the environment
/// </summary>
public class HeroesManager : MainGameplayManagerFramework
{
    public static HeroesManager Instance;
    
    [Tooltip("The base prefab that is used that all heroes are based off of")]
    [SerializeField] private GameObject _baseHeroPrefab;
    [Space]
    
    [Tooltip("The time between each hero being initially spawned")]
    [SerializeField] private float _heroSpawnInterval;
    
    [Space]
    [Header("Achievements")]
    [SerializeField] private AchievementSO _clutchAchievement;

    private List<HeroBase> _currentHeroes = new List<HeroBase>();
    private List<HeroBase> _currentLivingHeroes = new List<HeroBase>();
    
    private UnityEvent<HeroBase, float> _onHeroDamagedEvent = new UnityEvent<HeroBase, float>();
    private UnityEvent<HeroBase,float> _onHeroHealedEvent = new UnityEvent<HeroBase,float>();
    
    private UnityEvent<HeroBase> _onHeroManualUsedEvent = new UnityEvent<HeroBase>();
    
    private UnityEvent<HeroBase> _onHeroDiedEvent = new UnityEvent<HeroBase>();

    private void StartHeroSpawning()
    {
        StartCoroutine(SpawnHeroesAtSpawnPoints());
    }
    
    /// <summary>
    /// Spawns all selected heroes from the selection manager
    /// Uses the spawn points from the environment manager
    /// </summary>
    private IEnumerator SpawnHeroesAtSpawnPoints()
    {
        List<HeroSO> heroSOs = SelectionManager.Instance.GetAllSelectedHeroes();
        List<GameObject> spawnLocations = EnvironmentManager.Instance.GetHeroSpawnLocations();

        for (int i = 0; i < heroSOs.Count; i++)
        {
            SpawnHero(spawnLocations[i].transform,heroSOs[i] , i);

            yield return new WaitForSeconds(_heroSpawnInterval);
        }
    }

    /// <summary>
    /// Spawns a specific hero and starts their setup
    /// </summary>
    /// <param name="spawnLocation"></param>
    /// <param name="heroSO"></param>
    void SpawnHero(Transform spawnLocation, HeroSO heroSO, int heroID)
    {
        HeroBase heroBase = CreateHeroBase(spawnLocation, heroSO, heroID);

        _currentHeroes.Add(heroBase);
        _currentLivingHeroes.Add(heroBase);
    }

    public HeroBase CreateHeroBase(Transform spawnTransform, HeroSO heroSO, int heroID)
    {
        GameObject newHero = Instantiate(_baseHeroPrefab,
            spawnTransform.transform.position, spawnTransform.transform.rotation);
        HeroBase heroBase = newHero.GetComponent<HeroBase>();
        heroBase.SetUp(heroSO, heroID);

        return heroBase;
    }

    public HeroBase CreateHeroBase(Vector3 spawnLocation, Quaternion spawnRotation, HeroSO heroSO)
    {
        GameObject newHero = Instantiate(_baseHeroPrefab,
            spawnLocation, spawnRotation);
        HeroBase heroBase = newHero.GetComponent<HeroBase>();

        heroBase.SetUp(heroSO);

        return heroBase;
    }

    public void ToggleHeroesChargingAbilities(bool canHeroChargeAbilities)
    {
        foreach (HeroBase hero in _currentLivingHeroes)
        {
            hero.GetSpecificHeroScript().SetCanHeroChargeAbilities(canHeroChargeAbilities);
        }
    }
    public void ToggleHeroesAbleToUseAbilities(bool canHeroesUseAbilities)
    {
        foreach (HeroBase hero in _currentLivingHeroes)
        {
            hero.GetSpecificHeroScript().SetCanHeroUseAbilities(canHeroesUseAbilities);
        }
    }

    public void FullyCooldownAllHeroManualAbilities()
    {
        for (int i = 0; i < _currentLivingHeroes.Count; i++)
        {
            _currentLivingHeroes[i].GetSpecificHeroScript().ManualAbilityFullyCharged();
        }
    }

    /// <summary>
    /// Called by a hero when they die
    /// </summary>
    /// <param name="deadHero"> The hero that called this function </param> 
    public void HeroDied(HeroBase deadHero)
    {
        //Removes the hero from the list of living heroes
        _currentLivingHeroes.Remove(deadHero);
        
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.GeneralHeroAudio.HealthAudio.HeroDied);
        
        //Checks if the game should be declared a loss
        if (CheckIfAllHeroesDead())
        {
            TimeManager.Instance.BattleLostTimeSlow();
            AudioManager.Instance.PlaySpecificAudio(
                AudioManager.Instance.GeneralHeroAudio.HealthAudio.LastHeroDied);
        }
        else
        {
            TimeManager.Instance.HeroDiedTimeSlow();
        }

        BossBase.Instance.GetSpecificBossScript().HeroDied(deadHero);
        
        InvokeOnHeroDiedEvent(deadHero);
    }

    /// <summary>
    /// Performs a check for if all heroes are dead
    /// If they are declare the battle a loss
    /// </summary>
    private bool CheckIfAllHeroesDead()
    {
        if (_currentLivingHeroes.Count == 0)
        {
            GameStateManager.Instance.SetGameplayState(EGameplayStates.PostBattleLost);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Kills all heroes that are currently alive.
    /// Ignores all heroes death overrides.
    /// </summary>
    public void ForceKillAllHeroes()
    {
        while (_currentLivingHeroes.Count > 0)
        {
            _currentLivingHeroes[0].GetHeroStats().KillHero();
        }
    }

    private void BattleWon()
    {
        if (SelectionManager.Instance.GetSelectedDifficulty() < EGameDifficulty.Mythic)
        {
            return;
        }

        if (_currentLivingHeroes.Count <= 1)
        {
            AchievementManager.Instance.UnlockAchievement(_clutchAchievement);
        }
    }

    #region BaseManager
    /// <summary>
    /// Establishes the Instance for the HeroesManager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        GameStateManager.Instance.GetStartOfCharacterSpawningEvent().AddListener(StartHeroSpawning);
        
        GameStateManager.Instance.GetBattleWonEvent().AddListener(BattleWon);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _onHeroDamagedEvent.RemoveAllListeners();
        _onHeroHealedEvent.RemoveAllListeners();
        _onHeroManualUsedEvent.RemoveAllListeners();
        _onHeroDiedEvent.RemoveAllListeners();
    }

    #endregion
    
    #region Events
    public void InvokeOnHeroDamagedEvent(HeroBase heroBase, float damageAmount)
    {
        _onHeroDamagedEvent?.Invoke(heroBase,damageAmount);
    }
    
    public void InvokeOnHeroHealedEvent(HeroBase heroBase, float healAmount)
    {
        _onHeroHealedEvent?.Invoke(heroBase,healAmount);
    }

    public void InvokeOnHeroManualAbilityUsed(HeroBase heroBase)
    {
        _onHeroManualUsedEvent?.Invoke(heroBase);
    }
    
    public void InvokeOnHeroDiedEvent(HeroBase heroBase)
    {
        _onHeroDiedEvent?.Invoke(heroBase);
    }
    #endregion Events

    #region Getters
    public GameObject GetBaseHeroPrefab() => _baseHeroPrefab;
    public List<HeroBase> GetCurrentHeroes() => _currentHeroes;
    public List<HeroBase> GetCurrentLivingHeroes() => _currentLivingHeroes;
    public HeroBase GetRandomCurrentLivingHero() => _currentLivingHeroes[Random.Range(0, _currentLivingHeroes.Count)];
    public int GetAmountOfLivingHeroes() => _currentLivingHeroes.Count;
    public int GetAmountOfDeadHeroes() => _currentHeroes.Count - _currentLivingHeroes.Count;

    public UnityEvent<HeroBase,float> GetOnHeroDamagedEvent() => _onHeroDamagedEvent;
    public UnityEvent<HeroBase,float> GetOnHeroHealedEvent() => _onHeroHealedEvent;
    public UnityEvent<HeroBase> GetOnHeroManualAbilityUsedEvent() => _onHeroManualUsedEvent;
    public UnityEvent<HeroBase> GetOnHeroDiedEvent() => _onHeroDiedEvent;

    #endregion
}
