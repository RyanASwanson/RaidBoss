using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBA_DespairHex : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private float _despairDamage;
    
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _despairHex;
    
    private BossTargetZoneParent _newestTargetZone;
    private SBP_DespairHex _newestHex;
    
    private List<HeroBase> _hexedHeroes = new List<HeroBase>();

    public void AddHexedHero(HeroBase hero)
    {
        if (hero.IsUnityNull())
        {
            return;
        }
        
        if (_hexedHeroes.Contains(hero))
        {
            Debug.LogError("Attempted to apply hex to hexed hero");
            return;
        }

        _hexedHeroes.Add(hero);
    }

    public void RemoveHexedHero(HeroBase hero)
    {
        if (hero.IsUnityNull())
        {
            return;
        }
        
        _hexedHeroes?.Remove(hero);
    }

    public bool AttemptHexDamage(HeroBase hero)
    {
        if (!GetIsHeroHexed(hero))
        {
            return false;
        }

        _myBossBase.GetSpecificBossScript().DamageHero(hero,_despairDamage);
        
        return true;
    }
    
    #region Base Ability

    public override void AbilitySetUp(BossBase bossBase)
    {
        base.AbilitySetUp(bossBase);
    }

    protected override void AbilityPrep()
    {
        if (!_newestHex.IsUnityNull())
        {
            _newestHex.ForceEndDuration();
        }

        base.AbilityPrep();
    }

    protected override void StartShowTargetZone()
    {
        //Spawns the target area
        _newestTargetZone = Instantiate(_targetZone, _storedTargetLocation, Quaternion.identity).GetComponent<BossTargetZoneParent>();
        //Adds the target area to the list of target areas
        _currentTargetZones.Add(_newestTargetZone);

        _newestTargetZone.GetComponent<FollowObject>().StartFollowingObject(_storedTarget.gameObject);
        
        base.StartShowTargetZone();
    }


    protected override void AbilityStart()
    {
        if (_storedTarget.IsUnityNull())
        {
            return;
        }
        
        _newestHex = Instantiate(_despairHex, _storedTargetLocation, Quaternion.identity).GetComponent<SBP_DespairHex>();
        _newestHex.SetUpProjectile(_myBossBase, _abilityID,_wasBossEnragedOnAbilityActivation);
        _newestHex.AdditionalSetUp(this, _storedTarget);
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
        float lowestHeroHealth = float.MaxValue;
        int lowestHeroHealthIndex = 0;

        for (int i = 0; i < heroes.Count; i++)
        {
            if (heroes[i].GetHeroStats().GetCurrentHealth() < lowestHeroHealth)
            {
                lowestHeroHealth = heroes[i].GetHeroStats().GetCurrentHealth();
                lowestHeroHealthIndex = i;
            }
        }

        return heroes[lowestHeroHealthIndex];
    }

    public override bool GetCanAbilityBeUsed()
    {
        if (_newestHex.IsUnityNull())
        {
            return true;
        }

        Debug.Log("Change");
        // Has a chance to not allow the ability to be used if there is already a hex in use
        return Random.Range(0,2) == 0;
    }
    #endregion

    #region Getters

    public float GetDespairDamage() => _despairDamage;
    
    public bool GetIsHeroHexed(HeroBase hero) => _hexedHeroes.Contains(hero);

    #endregion
}
