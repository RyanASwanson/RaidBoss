using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroesManager : BaseGameplayManager
{
    [SerializeField] private GameObject _baseHeroPrefab;


    private List<HeroSO> _currentHeroes = new List<HeroSO>();
    private List<HeroSO> _currentLivingHeroes = new List<HeroSO>();

    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();
        SpawnHeroesAtSpawnPoints();
    }

    void SpawnHeroesAtSpawnPoints()
    {
        List<GameObject> spawnLocations = GameplayManagers.Instance.GetEnvironmentManager().GetSpawnLocations();
        List<HeroSO> heroSOs = UniversalManagers.Instance.GetSelectionManager().GetAllSelectedHeroes();

        /*foreach(HeroSO h in heroSOs)
        {
            Debug.Log(h.GetHeroName());
        }*/

        for (int i = 0; i < heroSOs.Count; i++)
        {
            //Debug.Log(heroSOs[i].GetHeroName());
            SpawnHero(spawnLocations[i].transform,heroSOs[i]);
        }
    }

    void SpawnHero(Transform spawnLocation, HeroSO heroSO)
    {
        Debug.Log(heroSO.GetHeroName());
        GameObject newHero = Instantiate(_baseHeroPrefab, 
            spawnLocation.transform.position,spawnLocation.transform.rotation);
        newHero.GetComponent<HeroBase>().Setup(heroSO);
        //newHero.GetComponent<HeroBase>().SetHeroSO(heroSO);
    }

    #region Events
    public override void SubscribeToEvents()
    {
        
    }
    #endregion

    #region Getters
    public List<HeroSO> GetCurrentHeroes() => _currentHeroes;
    public List<HeroSO> GetCurrentLivingHeroes() => _currentLivingHeroes;
    #endregion
}
