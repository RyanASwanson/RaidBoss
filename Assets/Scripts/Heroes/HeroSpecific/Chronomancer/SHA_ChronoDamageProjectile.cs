using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHA_ChronoDamageProjectile : HeroProjectileFramework
{
    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
    }

    public void AdditionalSetup(float lifeTime, Vector3 direction)
    {
        StartCoroutine(MoveProjectile(direction));
        Destroy(gameObject, lifeTime);
    }

    private IEnumerator MoveProjectile(Vector3 direction)
    {
        while(true)
        {
            transform.position += direction * Time.deltaTime;
            yield return null;
        }
            

    }

    /*private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("ProjectileHit");
    }*/

    protected override void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag(TagStringData.GetBossHitboxTagName()))
        {
            if (mySpecificHero == null)
                Debug.Log("NoSpecificHero");
            _ownerHeroBase.GetSpecificHeroScript().DamageBoss(mySpecificHero.GetAbilityStrength());
            _ownerHeroBase.GetSpecificHeroScript().StaggerBoss(mySpecificHero.GetAbilityStagger());
        }
    }
}
