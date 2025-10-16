using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        //Performs the set up for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities()
    {
        base.ActivateManualAbilities();

        CreateManualAttackProjectiles();
    }

    protected void CreateManualAttackProjectiles()
    {
        GameObject spawnedProjectile = Instantiate(_manualProjectile, 
            _myHeroBase.transform.position, Quaternion.identity);
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(_myHeroBase, EHeroAbilityType.Manual);

        spawnedProjectile.GetComponent<SHP_ShamanManualProjectile>().AdditionalSetUp(_currentTotem);

        //Performs the setup for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
    }

    #endregion

    #region Passive Abilities
    /// <summary>
    /// Activates the Shaman passive and spawns a totem
    /// </summary>
    public override void ActivatePassiveAbilities()
    {
        if (!_currentTotem.IsUnityNull())
        {
            // TODO replace with totem have a vanish animation
            Destroy(_currentTotem);
        }

        _currentTotem = Instantiate(_totem, transform.position, Quaternion.identity);
    }
    #endregion
}
