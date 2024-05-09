using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHA_ChronoDamageProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileLifetime;
    [SerializeField] private float _projectileSpeed;

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
    }

    public void AdditionalSetup(Vector3 direction)
    {
        StartCoroutine(MoveProjectile(direction));
        Destroy(gameObject, _projectileLifetime);
    }

    private IEnumerator MoveProjectile(Vector3 direction)
    {
        while(true)
        {
            transform.position += direction * _projectileSpeed * Time.deltaTime;
            yield return null;
        }
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag(TagStringData.GetBossHitboxTagName()))
        {
            _ownerHeroBase.GetSpecificHeroScript().DamageBoss(_mySpecificHero.GetBasicAbilityStrength());
            _ownerHeroBase.GetSpecificHeroScript().StaggerBoss(_mySpecificHero.GetBasicAbilityStagger());
        }
    }
}
