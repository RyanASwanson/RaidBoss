using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the functionality for the reapers manual attack
/// After the projectile is created it will move towards the reaper
///     dealing damage along its path
///     The initial location is done by the reaper script
/// </summary>
public class SHP_ReaperManualProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileSpeed;

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
    }

    public void AdditionalSetup()
    {
        StartCoroutine(MoveProjectile());
    }

    private IEnumerator MoveProjectile()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                _ownerHeroBase.gameObject.transform.position, _projectileSpeed * Time.deltaTime);

            transform.LookAt(_ownerHeroBase.gameObject.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            //transform.position +=  * Time.deltaTime;
            yield return null;
        }
    }

    /*private void StartDamageCooldown()
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
    }*/
}
