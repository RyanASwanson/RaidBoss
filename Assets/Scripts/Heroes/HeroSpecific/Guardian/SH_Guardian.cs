using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Guardian : SpecificHeroFramework
{
    [Space]
    [SerializeField] private float _heroBasicAbilityDamage;
    [SerializeField] private float _heroBasicAbilityStagger;
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
        return !myHeroBase.GetPathfinding().IsHeroMoving();  
    }

    /// <summary>
    /// Spawns the projectile that deals damage
    /// </summary>
    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        Instantiate(_basicProjectile, myHeroBase.transform.position, Quaternion.identity);
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        GameplayManagers.Instance.GetBossManager().GetBossBase().GetSpecificBossScript()
            .HeroOverrideAggro(myHeroBase, _heroManualAbilityDuration);
        base.ActivateManualAbilities(attackLocation);
    }
    #endregion

    #region Passive Abilities
    public void ActivatePassiveAbilities(float damageTaken)
    {
        if (_passiveCoroutine != null)
        {
            myHeroBase.GetHeroStats().ChangeCurrentHeroDamageResistance(-_heroPassiveDamageResistance);
            StopCoroutine(_passiveCoroutine);
        }

        _passiveCoroutine = StartCoroutine(PassiveAbilityProcess());
    }

    private IEnumerator PassiveAbilityProcess()
    {
        myHeroBase.GetHeroStats().ChangeCurrentHeroDamageResistance(_heroPassiveDamageResistance);
        ActivatePassiveVFX();
        yield return new WaitForSeconds(_heroPassiveAbilityDuration);
        myHeroBase.GetHeroStats().ChangeCurrentHeroDamageResistance(-_heroPassiveDamageResistance);

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

    public override void ActivateHeroSpecificActivity()
    {
        base.ActivateHeroSpecificActivity();
    }

    public override void DeactivateHeroSpecificActivity()
    {
        base.DeactivateHeroSpecificActivity();
    }

    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();

        myHeroBase.GetHeroDamagedEvent().AddListener(ActivatePassiveAbilities);
    }

    
}
