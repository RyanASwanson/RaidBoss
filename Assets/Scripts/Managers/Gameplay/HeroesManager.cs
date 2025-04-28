using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for general management of heroes
/// Spawns heroes into the environment
/// 
/// </summary>
public class HeroesManager : MainGameplayManagerFramework
{
    [SerializeField] private GameObject _baseHeroPrefab;
    [Space]

    [SerializeField] private float _heroSpawnInterval;

    private List<HeroBase> _currentHeroes = new List<HeroBase>();
    private List<HeroBase> _currentLivingHeroes = new List<HeroBase>();


    /// <summary>
    /// Spawns all selected heroes from the selection manager
    /// Uses the spawn points from the environment manager
    /// </summary>
    private IEnumerator SpawnHeroesAtSpawnPoints()
    {
        List<HeroSO> heroSOs = UniversalManagers.Instance.GetSelectionManager().GetAllSelectedHeroes();
        List<GameObject> spawnLocations = GameplayManagers.Instance.GetEnvironmentManager().GetSpawnLocations();

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
        heroBase.Setup(heroSO, heroID);

        return heroBase;
    }

    public HeroBase CreateHeroBase(Vector3 spawnLocation, Quaternion spawnRotation, HeroSO heroSO)
    {
        GameObject newHero = Instantiate(_baseHeroPrefab,
            spawnLocation, spawnRotation);
        HeroBase heroBase = newHero.GetComponent<HeroBase>();

        heroBase.Setup(heroSO);

        return heroBase;
    }

    /// <summary>
    /// Called by a hero when they die
    /// </summary>
    /// <param name="deadHero"></param> The hero that called this function
    public void HeroDied(HeroBase deadHero)
    {
        //Removes the hero from the list of living heroes
        _currentLivingHeroes.Remove(deadHero);
        //Checks if the game should be declared a loss
        CheckIfAllHeroesDead();

        GameplayManagers.Instance.GetBossManager().GetBossBase().GetSpecificBossScript().HeroDied(deadHero);
        UniversalManagers.Instance.GetTimeManager().HeroDiedTimeSlow();
    }


    /// <summary>
    /// Performs a check for if all heroes are dead
    /// If they are declare the battle a loss
    /// </summary>
    private void CheckIfAllHeroesDead()
    {
        if (_currentLivingHeroes.Count == 0)
            GameplayManagers.Instance.GetGameStateManager().SetGameplayState(GameplayStates.PostBattleLost);
    }

    /// <summary>
    /// Kills all heroes that are currently alive.
    /// Ignores all heroes death overrides.
    /// </summary>
    public void KillAllHeroes()
    {
        while (_currentLivingHeroes.Count > 0)
        {
            _currentLivingHeroes[0].GetHeroStats().KillHero();
        }
    }

    #region BaseManager
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        StartCoroutine(SpawnHeroesAtSpawnPoints());
    }
    #endregion

    #region Getters
    public GameObject GetBaseHeroPrefab() => _baseHeroPrefab;
    public List<HeroBase> GetCurrentHeroes() => _currentHeroes;
    public List<HeroBase> GetCurrentLivingHeroes() => _currentLivingHeroes;
    #endregion
}
