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
    [SerializeField] private UnityEvent _lifeTimeEndEvent;
    [Space]
    [SerializeField] private GameObject _hitCenteredVFX;

    [Header("Enter")]
    [SerializeField] private float _enterDamage;
    [SerializeField] private float _enterStagger;
    [SerializeField] private UnityEvent<Collider> _enterEvent;

    [Header("Stay")]
    [SerializeField] private float _stayDamagePerTick;
    [SerializeField] private float _stayStaggerPerTick;
    [SerializeField] private float _stayDamageTickRate;
    [SerializeField] private UnityEvent<Collider> _stayEvent;

    [Header("Exit")]
    [SerializeField] private float _exitDamage;
    [SerializeField] private float _exitStagger;
    [SerializeField] private UnityEvent<Collider> _exitEvent;

    private HeroBase _myHeroBase;

    private void Start()
    {
        if (_hasLifeTime)
            StartCoroutine(LifeTimeDestruction());
    }

    private IEnumerator LifeTimeDestruction()
    {
        yield return new WaitForSeconds(_lifeTime);
        _lifeTimeEndEvent?.Invoke();
        Destroy(gameObject);
    }

    public void SetUpDamageArea(HeroBase heroBase)
    {
        _myHeroBase = heroBase;
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
        if (HitBoss(collision, _stayEvent,
            _stayDamagePerTick, _stayStaggerPerTick) && (_stayDamageTickRate > 0))
        {
            StartCoroutine(DisableColliderForDuration(_stayDamageTickRate));
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        HitBoss(collision, _exitEvent, _exitDamage, _exitStagger);
    }

    private bool HitBoss(Collider collision, UnityEvent<Collider> hitEvent, float abilityDamage, float abilityStagger)
    {
        if(DoesColliderBelongToBoss(collision))
        {
            hitEvent?.Invoke(collision);

            DealDamageAndStagger(abilityDamage, abilityStagger);

            return true;
        }
        return false;
    }

    private void DealDamageAndStagger(float abilityDamage, float abilityStagger)
    {
        if (_myHeroBase == null)
            Debug.Log("Cant Find Hero Base");

        if (abilityDamage > 0)
            _myHeroBase.GetSpecificHeroScript().DamageBoss(abilityDamage);
            //bossBase.GetBossStats().DealDamageToBoss(abilityDamage);

        if (abilityStagger > 0)
            _myHeroBase.GetSpecificHeroScript().StaggerBoss(abilityStagger);
            //bossBase.GetBossStats().DealStaggerToBoss(abilityStagger);
    }

    public void ToggleProjectileCollider(bool colliderEnabled)
    {
        _damageCollider.enabled = colliderEnabled;
    }

    private IEnumerator DisableColliderForDuration(float duration)
    {
        ToggleProjectileCollider(false);
        yield return new WaitForSeconds(duration);
        ToggleProjectileCollider(true);
    }
    #endregion

    public void CreateHitDestructionVFX()
    {
        if (_hitCenteredVFX == null) return;
        Instantiate(_hitCenteredVFX, transform.position, Quaternion.identity);
    }

    public void DestroyProjectile()
    {
        CreateHitDestructionVFX();
        Destroy(gameObject);
    }

    #region Getters
    public UnityEvent<Collider> GetEnterEvent() => _enterEvent;
    public UnityEvent<Collider> GetStayEvent() => _stayEvent;
    public UnityEvent<Collider> GetExitEvent() => _exitEvent;
    #endregion
}
