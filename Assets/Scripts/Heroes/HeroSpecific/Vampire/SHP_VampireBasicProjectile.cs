using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality to the Vampire basic ability projectile
/// </summary>
public class SHP_VampireBasicProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileSpeed;
    [Space]

    [SerializeField] private bool _canSplit;
    [SerializeField] private float _splitProjectileCount;
    [SerializeField] private float _splitAngle;

    [Space]
    [SerializeField] private GameObject _splitProjectile;

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);

        StartCoroutine(MoveProjectile());
    }

    private IEnumerator MoveProjectile()
    {
        while (true)
        {
            transform.position += transform.forward * _projectileSpeed * Time.deltaTime;
            yield return null;
        }
    }

    public void SplitProjectile()
    {

        for(int i = 0; i <= _splitProjectileCount; i++)
        {
            float tempRot = Mathf.Lerp(-_splitAngle, _splitAngle, i / _splitProjectileCount);
            Vector3 a = new Vector3(0,tempRot,0);

            GameObject newestProjectile = Instantiate(_splitProjectile, transform.position, Quaternion.Euler(a));

            newestProjectile.GetComponent<SHP_VampireBasicProjectile>().SetUpProjectile(_myHeroBase);
        }
    }
}
