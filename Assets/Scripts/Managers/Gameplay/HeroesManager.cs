using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroesManager : BaseGameplayManager
{
    [SerializeField] private GameObject _baseHeroPrefab;


    private List<HeroBase> _currentHeroes = new List<HeroBase>();
    private List<HeroBase> _currentLivingHeroes = new List<HeroBase>();

    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();
        SpawnHeroesAtSpawnPoints();
    }

    /// <summary>
    /// Spawns all selected heroes from the selection manager
    /// Uses the spawn points from the environment manager
    /// </summary>
    void SpawnHeroesAtSpawnPoints()
    {
        List<HeroSO> heroSOs = UniversalManagers.Instance.GetSelectionManager().GetAllSelectedHeroes();
        List<GameObject> spawnLocations = GameplayManagers.Instance.GetEnvironmentManager().GetSpawnLocations();

        for (int i = 0; i < heroSOs.Count; i++)
        {
            SpawnHero(spawnLocations[i].transform,heroSOs[i]);
        }
    }

    /// <summary>
    /// Spawns a specific hero and starts their setup
    /// </summary>
    /// <param name="spawnLocation"></param>
    /// <param name="heroSO"></param>
    void SpawnHero(Transform spawnLocation, HeroSO heroSO)
    {
        GameObject newHero = Instantiate(_baseHeroPrefab, 
            spawnLocation.transform.position,spawnLocation.transform.rotation);
        HeroBase heroBase = newHero.GetComponent<HeroBase>();
        heroBase.Setup(heroSO);

        _currentHeroes.Add(heroBase);
        _currentLivingHeroes.Add(heroBase);
    }

    public void HeroDied(HeroBase deadHero)
    {
        _currentLivingHeroes.Remove(deadHero);
    }

    #region Events
    public override void SubscribeToEvents()
    {
        
    }
    #endregion

    #region Getters
    public List<HeroBase> GetCurrentHeroes() => _currentHeroes;
    public List<HeroBase> GetCurrentLivingHeroes() => _currentLivingHeroes;
    #endregion
}
