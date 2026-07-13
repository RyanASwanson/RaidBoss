using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBA_Isolation : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _targetSafeZone;
    [SerializeField] private GameObject _isolation;

    private BossSharedSafeAndTargetZone _currentTargetSafeZone;
    
    #region Base Ability

    public override void AbilitySetUp(BossBase bossBase)
    {
        base.AbilitySetUp(bossBase);
    }

    protected override void StartShowTargetZone()
    {
        base.StartShowTargetZone();
        
        _currentTargetSafeZone = Instantiate(_targetSafeZone, _storedTarget.transform.position, Quaternion.identity)
            .GetComponent<BossSharedSafeAndTargetZone>();
        
        _currentTargetSafeZone.GetComponent<FollowObject>().StartFollowingObject(_storedTarget.gameObject);
        /*//Spawns the target area
        _newestTargetZone = Instantiate(_targetZone, _storedTargetLocation, Quaternion.identity).GetComponent<BossTargetZoneParent>();
        //Adds the target area to the list of target areas
        _currentTargetZones.Add(_newestTargetZone);*/
    }

    protected override void RemoveTargetZones()
    {
        Debug.Log("RemoveTargetZones");
        _currentTargetSafeZone.RemoveAllZones();
        base.RemoveTargetZones();
    }

    protected override void AbilityStart()
    {
        if (_currentTargetSafeZone.GetIsHeroInSafeZone())
        {
            return;
        }

        Instantiate(_isolation, _storedTarget.transform.position, Quaternion.identity);
        base.AbilityStart();
    }
    
    protected override void AbilityDurationEnded()
    {
        base.AbilityDurationEnded();
    }

    public override void StopBossAbility()
    {
        base.StopBossAbility();
    }
    
    public override HeroBase GetSpecificHeroTarget()
    {
        List<HeroBase> heroes = HeroesManager.Instance.GetCurrentLivingHeroes();
        
        float currentDistance = 0;
        float currentMinimumDistance;
        float minimumDistance = 0;
        
        HeroBase currentHero = null;
        HeroBase currentFurthestHero = heroes[0];
        HeroBase furthestHero = null;
        

        if (heroes.Count == 1)
        {
            return heroes[0];
        }
        
        switch (heroes.Count)
        {
            case 1:
                return heroes[0];
            case 2:
                return heroes[Random.Range(0, heroes.Count)];
        }

        for (int i = 0; i < heroes.Count; i++)
        {
            currentMinimumDistance = float.MaxValue;

            for (int j = 0; j < heroes.Count; j++)
            {
                if (heroes[i] == heroes[j])
                {
                    continue;
                }
                
                currentDistance = Vector3.Distance(heroes[i].transform.position, heroes[j].transform.position);

                if (currentDistance < currentMinimumDistance)
                {
                    currentMinimumDistance = currentDistance;
                    currentFurthestHero = heroes[i];
                }
            }

            if (currentMinimumDistance > minimumDistance)
            {
                minimumDistance = currentMinimumDistance;
                furthestHero = currentFurthestHero;
            }
        }
        
        return furthestHero;
    }
    #endregion
}
