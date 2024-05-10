using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_ReaperBasicProjectile : HeroProjectileFramework 
{
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private Vector2 _movementVariability;
    [SerializeField] private float _attackDamageCooldown;

    private Collider _projCollider;
    [Space]
    [SerializeField] private GameObject _childObject;


    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
        _projCollider = GetComponentInChildren<Collider>();
    }

    public void AdditionalSetup()
    {
        StartCoroutine(MoveProjectile(_projectileSpeed,_movementVariability));
    }

    private IEnumerator MoveProjectile(float projectileSpeed, Vector2 movementVariability)
    {
        float time = 4.75f;
        float xPos, zPos;

        while (true)
        {
            transform.position = _ownerHeroBase.transform.position;

            time += Time.deltaTime;
            xPos = Mathf.Cos(time);
            zPos = Mathf.Sin(2 * time) / 2;
            _childObject.transform.localPosition = new Vector3((xPos * movementVariability.x),
                _ownerHeroBase.transform.position.y, (zPos * movementVariability.y));
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
