using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHA_ChronoDamageProjectile : HeroProjectileFramework
{
    public override void SetUpProjectile(HeroBase heroBase)
    {
        _ownerHeroBase = heroBase;
        Debug.Log("ProjectileSetup");
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
        Debug.Log("Trigger");
        if (collision.gameObject.CompareTag(TagStringData.GetBossHitboxTagName()))
        {
            Debug.Log("Debug");
            _ownerHeroBase.GetSpecificHeroScript().DamageBoss(_ownerHeroBase.GetHeroSO().GetBasicAbilityStrength());
            _ownerHeroBase.GetSpecificHeroScript().StaggerBoss(_ownerHeroBase.GetHeroSO().GetBasicAbilityStagger());
        }
    }
}
