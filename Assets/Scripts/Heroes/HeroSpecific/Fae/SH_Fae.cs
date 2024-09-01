using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Fae : SpecificHeroFramework
{
    [Space]
    [SerializeField] private float _projectileSpawnDistance;
    [SerializeField] private List<Vector3> _primaryAttackEulers;

    [SerializeField] private GameObject _basicProjectile;
    


    #region Basic Abilities
    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
    }

    protected void CreateBasicAttackProjectiles()
    {
        foreach(Vector3 attackEuler in _primaryAttackEulers)
        {
            GameObject newestProjectile = Instantiate(_basicProjectile, transform.position, Quaternion.Euler(attackEuler));
            newestProjectile.transform.position = newestProjectile.transform.forward * _projectileSpawnDistance;
        }
    }

    private IEnumerator MoveProjectile(GameObject projectile, Vector3 direction)
    {

    }
    #endregion

    #region Manual Abilities

    #endregion

    #region Passive Abilities

    #endregion
}
