using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Astromancer : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;

    [Space]
    [SerializeField] private GameObject _manualProjectiles;

    [Space]
    [SerializeField] private float _passiveRechargeManualAmount;

    #region Basic Abilities
    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
    }

    protected void CreateBasicAttackProjectiles()
    {

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
    }

    #endregion

    #region Passive Abilities
    public override void ActivatePassiveAbilities()
    {

    }

    #endregion

}

