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

    void SpawnHeroesAtSpawnPoints()
    {
        List<GameObject> spawnLocations = GameplayManagers.Instance.GetEnvironmentManager().GetSpawnLocations();
        List<HeroSO> heroSOs = UniversalManagers.Instance.GetSelectionManager().GetAllSelectedHeroes();

        for (int i = 0; i < heroSOs.Count; i++)
        {
            SpawnHero(spawnLocations[i].transform,heroSOs[i]);
        }
    }

    void SpawnHero(Transform spawnLocation, HeroSO heroSO)
    {
        GameObject newHero = Instantiate(_baseHeroPrefab, 
            spawnLocation.transform.position,spawnLocation.transform.rotation);
        newHero.GetComponent<HeroBase>().Setup(heroSO);
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
