using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the functionality for the reapers manual attack
/// After the projectile is created it will move towards the reaper
///     dealing damage along its path
///     The initial location is done by the reaper script
/// </summary>
public class SHP_ReaperManualProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileSpeed;


    private IEnumerator MoveProjectile()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                _myHeroBase.gameObject.transform.position, _projectileSpeed * Time.deltaTime);

            transform.LookAt(_myHeroBase.gameObject.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            yield return null;
        }
    }


    #region Base Ability
    public override void SetUpProjectile(HeroBase heroBase)
    {
        StartCoroutine(MoveProjectile());
        base.SetUpProjectile(heroBase);
    }
    #endregion
}
