using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Guardian hero
/// </summary>
public class SH_Guardian : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;
    
    [Space]
    [SerializeField] private float _heroManualAbilityDuration;

    [Space]
    [SerializeField] private float _heroPassiveAbilityDuration;
    [SerializeField] private float _heroPassiveDamageResistance;

    [SerializeField] private List<ParticleSystem> _passiveEyeVFX;
    private Coroutine _passiveCoroutine;

    #region Basic Abilities
    public override bool ConditionsToActivateBasicAbilities()
    {
        return !_myHeroBase.GetPathfinding().IsHeroMoving();  
    }

    /// <summary>
    /// Spawns the projectile that deals damage
    /// </summary>
    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        GameObject spawnedProjectile = Instantiate(_basicProjectile, _myHeroBase.transform.position, Quaternion.identity);

        //Performs the setup for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetSpecificBossScript()
            .StartHeroOverrideAggro(_myHeroBase, _heroManualAbilityDuration);

        base.ActivateManualAbilities(attackLocation);
    }
    #endregion

    #region Passive Abilities
    public void ActivatePassiveAbilities(float damageTaken)
    {
        if (_passiveCoroutine != null)
        {
            _myHeroBase.GetHeroStats().ChangeCurrentHeroDamageResistance(-_heroPassiveDamageResistance);
            StopCoroutine(_passiveCoroutine);
        }

        _passiveCoroutine = StartCoroutine(PassiveAbilityProcess());
    }

    private IEnumerator PassiveAbilityProcess()
    {
        _myHeroBase.GetHeroStats().ChangeCurrentHeroDamageResistance(_heroPassiveDamageResistance);
        ActivatePassiveVFX();
        yield return new WaitForSeconds(_heroPassiveAbilityDuration);
        _myHeroBase.GetHeroStats().ChangeCurrentHeroDamageResistance(-_heroPassiveDamageResistance);

        _passiveCoroutine = null;
    }

    private void ActivatePassiveVFX()
    {
        foreach (ParticleSystem particleSystem in _passiveEyeVFX)
        {
            particleSystem.Play();
        }
    }
    #endregion


    #region Base Hero
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();

        _myHeroBase.GetHeroDamagedEvent().AddListener(ActivatePassiveAbilities);
    }

    #endregion

}
