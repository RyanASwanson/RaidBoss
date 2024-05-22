using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Samurai : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;

    [Space]
    [SerializeField] int _manualAbilityDuration;

    private bool _isParrying;

    private Coroutine _parryCoroutine;

    #region Basic Abilities
    /// <summary>
    /// Holds the conditions needed for a successful basic attack
    /// Requires the samurai to be stationary
    /// </summary>
    /// <returns></returns>
    public override bool ConditionsToActivateBasicAbilities()
    {
        return !myHeroBase.GetPathfinding().IsHeroMoving();
    }

    /// <summary>
    /// Performs a horizontal slash
    /// </summary>
    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
    }

    /// <summary>
    /// Creates the projectile of the horizontal slash
    /// </summary>
    protected void CreateBasicAttackProjectiles()
    {
        GameObject spawnedProjectile = Instantiate(_basicProjectile, myHeroBase.transform.position, Quaternion.identity);
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(myHeroBase);

        spawnedProjectile.GetComponent<SHP_SamuraiBasicProjectile>().AdditionalSetup(1);
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);

        _parryCoroutine = StartCoroutine(ParryCoroutine());
    }

    private IEnumerator ParryCoroutine()
    {
        _isParrying = true;
        yield return new WaitForSeconds(_manualAbilityDuration);
        _isParrying = false;

        _parryCoroutine = null;

        //JUST TO REMOVE WARNING, DELETE LATER
        if (_isParrying)
            Debug.Log("Test");
    }


    #endregion

    #region Passive Abilities
    /// <summary>
    /// On hitting the boss with an attack the samurai charges his manual ability
    /// </summary>
    /// <param name="cooldownAmount"></param>
    public void ActivatePassiveAbilities(float cooldownAmount)
    {
        AddToManualAbilityChargeTime(cooldownAmount);
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
    }

    
}
