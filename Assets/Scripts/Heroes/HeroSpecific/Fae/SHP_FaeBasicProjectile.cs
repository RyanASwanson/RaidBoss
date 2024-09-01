using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_FaeBasicProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileSpeed;

    private IEnumerator MoveProjectile( Vector3 direction)
    {
        while (gameObject != null)
        {
            transform.position += direction * _projectileSpeed * Time.deltaTime;
            yield return null;
        }
    }

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
        StartCoroutine(MoveProjectile(transform.forward));
    }
}
