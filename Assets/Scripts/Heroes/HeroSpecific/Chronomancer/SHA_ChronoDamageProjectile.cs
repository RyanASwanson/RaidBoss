using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHA_ChronoDamageProjectile : HeroProjectileFramework
{
    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
    }

    public void AdditionalSetup(float lifeTime, Vector3 direction, float projectileSpeed)
    {
        StartCoroutine(MoveProjectile(direction,projectileSpeed));
        Destroy(gameObject, lifeTime);
    }

    private IEnumerator MoveProjectile(Vector3 direction, float projectileSpeed)
    {
        while(true)
        {
            transform.position += direction * projectileSpeed* Time.deltaTime;
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