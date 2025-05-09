using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Shaman hero
/// </summary>
public class SH_Shaman : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;

    [SerializeField] private GameObject _manualProjectile;

    [SerializeField] private GameObject _totem;
    private GameObject _currentTotem;

    #region Basic Abilities
    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
    }

    protected void CreateBasicAttackProjectiles()
    {
        //Spawns the projectile at the hero location
        GameObject spawnedProjectile = Instantiate(_basicProjectile, _myHeroBase.transform.position, Quaternion.identity);

        //Performs the setup for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);

        CreateManualAttackProjectiles();
    }

    protected void CreateManualAttackProjectiles()
    {
        GameObject spawnedProjectile = Instantiate(_manualProjectile, 
            _myHeroBase.transform.position, Quaternion.identity);
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(_myHeroBase);

        spawnedProjectile.GetComponent<SHP_ShamanManualProjectile>().AdditionalSetup(_currentTotem);

        //Performs the setup for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
    }

    #endregion

    #region Passive Abilities
    public override void ActivatePassiveAbilities()
    {
        if (_currentTotem != null)
            Destroy(_currentTotem);

        _currentTotem = Instantiate(_totem, transform.position, Quaternion.identity);
    }
    #endregion



    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
    }
}
