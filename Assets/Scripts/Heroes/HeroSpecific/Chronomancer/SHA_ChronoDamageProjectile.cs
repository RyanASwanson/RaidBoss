using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHA_ChronoDamageProjectile : HeroProjectileFramework
{
    
    protected override void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag(TagStringData.GetBossHitboxTagName()))
        {
            _ownerHeroBase.GetSpecificHeroScript().DamageBoss(_ownerHeroBase.GetHeroSO().GetBasicAbilityStrength());
        }
    }
}
