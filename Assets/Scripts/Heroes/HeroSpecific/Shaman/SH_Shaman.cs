using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //Shaman Projectile Script Not Currently in use
        //GameObject spawnedProjectile = 
        GameObject spawnedProjectile = Instantiate(_basicProjectile, _myHeroBase.transform.position, Quaternion.identity);

        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
        //spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(myHeroBase);

        //spawnedProjectile.GetComponent<SHP_ShamanBasicAbility>().AdditionalSetup();
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
