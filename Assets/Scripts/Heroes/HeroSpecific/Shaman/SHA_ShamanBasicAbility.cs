using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHA_ShamanBasicAbility : HeroProjectileFramework
{
    private Collider _projCollider;
    private float _attackDamageCooldown;

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
        _projCollider = GetComponentInChildren<Collider>();
    }

    public void AdditionalSetup(float lifeTime, float damageCooldown)
    {
        _attackDamageCooldown = damageCooldown;
        Destroy(gameObject, lifeTime);
    }

    private void StartDamageCooldown()
    {
        StartCoroutine(DamageCooldown());
    }

    private IEnumerator DamageCooldown()
    {
        _projCollider.enabled = false;
        yield return new WaitForSeconds(_attackDamageCooldown);
        _projCollider.enabled = true;
    }

    protected void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag(TagStringData.GetBossHitboxTagName()))
        {
            StartDamageCooldown();

            _ownerHeroBase.GetSpecificHeroScript().DamageBoss(_mySpecificHero.GetBasicAbilityStrength());
            _ownerHeroBase.GetSpecificHeroScript().StaggerBoss(_mySpecificHero.GetBasicAbilityStagger());
        }
    }

}
