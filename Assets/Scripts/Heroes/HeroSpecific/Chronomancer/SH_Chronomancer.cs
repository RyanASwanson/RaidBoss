using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Chronomancer : SpecificHeroFramework
{
    [SerializeField] private GameObject _damageProjectile;
    [SerializeField] private Vector3 _attackRotationIncrease;
    [SerializeField] protected float _basicProjectileLifetime;
    [SerializeField] protected float _basicProjectileSpeed;

    private Vector3 _currentAttackDirection = new Vector3(0, 0, 1);

    #region Basic Abilities

    public override bool ConditionsToActivateBasicAbilities()
    {
        return !myHeroBase.GetPathfinding().IsHeroMoving();
    }

    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();
        Debug.Log("Activate Chrono Basic Abilities");

        CreateBasicAttackProjectiles();
        IncreaseCurrentAttackRotation();
    }

    protected void CreateBasicAttackProjectiles()
    {
        GameObject spawnedProjectile = Instantiate(_damageProjectile, myHeroBase.transform.position, Quaternion.identity);
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(myHeroBase);

        spawnedProjectile.GetComponent<SHA_ChronoDamageProjectile>().AdditionalSetup
            (_basicProjectileLifetime, _currentAttackDirection, _basicProjectileSpeed);
    }

    private void IncreaseCurrentAttackRotation()
    {
        _currentAttackDirection = Quaternion.Euler(_attackRotationIncrease) * _currentAttackDirection;
    }


    #endregion
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);
    }

    public override void ActivatePassiveAbilities()
    {
        throw new System.NotImplementedException();
    }

    

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