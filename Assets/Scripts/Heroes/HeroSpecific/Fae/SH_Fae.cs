using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Fae : SpecificHeroFramework
{
    [Space]
    [SerializeField] private List<Vector3> _primaryAttackEulers;
    [SerializeField] private float _projectileSpawnDistance;
    
    [Space]
    [SerializeField] private GameObject _basicProjectile;


    [Space]
    [SerializeField] private float _passiveBasicAttackSpeedChange;
    private float _currentPassiveBasicAttackSpeed = 1;

    #region Basic Abilities

    protected override void CooldownAddToBasicAbilityCharge(float addedAmount)
    {
        base.CooldownAddToBasicAbilityCharge(addedAmount * _currentPassiveBasicAttackSpeed);
    }

    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
    }

    public override bool ConditionsToActivateBasicAbilities()
    {
        return true;
    }

    protected void CreateBasicAttackProjectiles()
    {
        foreach(Vector3 attackEuler in _primaryAttackEulers)
        {
            GameObject newestProjectile = Instantiate(_basicProjectile, transform.position, Quaternion.Euler(attackEuler));
            newestProjectile.transform.position = newestProjectile.transform.position + 
                (newestProjectile.transform.forward * _projectileSpawnDistance);

            newestProjectile.GetComponent<SHP_FaeBasicProjectile>().SetUpProjectile(_myHeroBase);

            newestProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
        }
    }


    #endregion

    #region Manual Abilities

    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);
    }

    #endregion

    #region Passive Abilities
    private void IncreaseBasicAttackSpeed()
    {
        _currentPassiveBasicAttackSpeed += _passiveBasicAttackSpeedChange;
    }

    private void DecreaseBasicAttackSpeed()
    {
        _currentPassiveBasicAttackSpeed -= _passiveBasicAttackSpeedChange;
    }
    #endregion


    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();

        _myHeroBase.GetHeroStartedMovingEvent().AddListener(IncreaseBasicAttackSpeed);
        _myHeroBase.GetHeroStoppedMovingEvent().AddListener(DecreaseBasicAttackSpeed);
    }
}
