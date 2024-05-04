using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHA_ReaperManualProjectile : HeroProjectileFramework
{
    private Collider _projCollider;
    private float _attackDamageCooldown;

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
        _projCollider = GetComponentInChildren<Collider>();
    }

    public void AdditionalSetup(float lifeTime, float projectileSpeed, float damageCooldown)
    {
        StartCoroutine(MoveProjectile(projectileSpeed));
        _attackDamageCooldown = damageCooldown;
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

    private void StartDamageCooldown()
    {
        StartCoroutine(DamageCooldown());
    }

    private IEnumerator DamageCooldown()
    {
        _projCollider.enabled = false;
        yield return new WaitForSeconds( _attackDamageCooldown);
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
