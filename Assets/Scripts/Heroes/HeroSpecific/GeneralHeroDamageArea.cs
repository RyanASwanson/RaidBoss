using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains the functionality that is generally used for hero attacks to do damage
/// </summary>
public class GeneralHeroDamageArea : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Collider _damageCollider;
    [SerializeField] private bool _hasLifeTime;
    [SerializeField] private float _lifeTime;

    [Header("Enter")]
    [SerializeField] private float _enterDamage;
    [SerializeField] private float _enterStagger;
    [SerializeField] private UnityEvent<Collider> _enterEvent;

    [Header("Stay")]
    [SerializeField] private float _stayDamagePerSecond;
    [SerializeField] private float _stayStaggerPerSecond;
    [SerializeField] private UnityEvent<Collider> _stayEvent;

    [Header("Exit")]
    [SerializeField] private float _exitDamage;
    [SerializeField] private float _exitStagger;
    [SerializeField] private UnityEvent<Collider> _exitEvent;

    private void Start()
    {
        if (_hasLifeTime)
            Destroy(gameObject, _lifeTime);
    }

    #region Collision
    private bool DoesColliderBelongToBoss(Collider collision)
    {
        return collision.gameObject.CompareTag(TagStringData.GetBossHitboxTagName());
    }

    private void OnTriggerEnter(Collider collision)
    {
        HitBoss(collision, _enterEvent, _enterDamage, _enterStagger);
    }

    private void OnTriggerStay(Collider collision)
    {
        HitBoss(collision, _stayEvent, 
            _stayDamagePerSecond * Time.deltaTime, _stayStaggerPerSecond * Time.deltaTime);
    }

    private void OnTriggerExit(Collider collision)
    {
        HitBoss(collision, _exitEvent, _exitDamage, _exitStagger);
    }

    private void HitBoss(Collider collision, UnityEvent<Collider> hitEvent, float abilityDamage, float abilityStagger)
    {
        if(DoesColliderBelongToBoss(collision))
        {
            hitEvent?.Invoke(collision);

            DealDamageAndStagger(collision.GetComponentInParent<BossBase>(),abilityDamage, abilityStagger);
        }
    }

    private void DealDamageAndStagger(BossBase bossBase, float abilityDamage, float abilityStagger)
    {
        if (abilityDamage >= 0)
            bossBase.GetBossStats().DealDamageToBoss(abilityDamage);

        if (abilityStagger >= 0)
            bossBase.GetBossStats().DealStaggerToBoss(abilityStagger);
    }

    public void ToggleProjectileCollider(bool colliderEnabled)
    {
        _damageCollider.enabled = colliderEnabled;
    }
    #endregion

}