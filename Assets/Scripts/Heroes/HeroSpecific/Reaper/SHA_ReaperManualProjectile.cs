using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHA_ReaperManualProjectile : HeroProjectileFramework
{
    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
    }

    public void AdditionalSetup(float lifeTime, float projectileSpeed)
    {
        StartCoroutine(MoveProjectile(projectileSpeed));
        Destroy(gameObject, lifeTime);
    }

    private IEnumerator MoveProjectile(float projectileSpeed)
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                _ownerHeroBase.gameObject.transform.position, projectileSpeed * Time.deltaTime);
            //transform.position +=  * Time.deltaTime;
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
