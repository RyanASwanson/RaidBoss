using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_Shatter : BossProjectileFramework
{
    [SerializeField] private float _projectileSpeed;

    public override void SetUpProjectile(BossBase bossBase)
    {
        base.SetUpProjectile(bossBase);
        StartCoroutine(MoveProjectile());
    }


    protected IEnumerator MoveProjectile()
    {

        while (true)
        {
            transform.position += transform.forward * _projectileSpeed * Time.deltaTime;
            yield return null;
        }
    }
}
