using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_IsolationTargetZone : BossProjectileFramework
{
    [SerializeField] private Transform _fadingHopeRayHolder;

    [Space] 
    [SerializeField] private GameObject _damageTargetZone;
    [SerializeField] private GameObject _rayTargetZone;
    
    [Space]
    [SerializeField] private BossSharedSafeAndTargetZone _storedSafeAndTargetZone;
    [SerializeField] private BossAbilitySafeZone _safeZone;
    [SerializeField] private BossTargetZoneParent _rayBuffTargetZoneParent;
    [SerializeField] private SBP_RayOfHopeTargetZone _rayOfHope;
    [SerializeField] private FollowObject _followObject;
    
    private SBP_RayOfHopeTargetZone _associatedRay;

    public void AdditionalSetUp(HeroBase heroTarget)
    {
        _rayOfHope.SetUpProjectile(_myBossBase,SB_VoidLord.Instance.GetFadingHope().GetAbilityID());
        HeroesNoLongerInSafeZone();
        _followObject.StartFollowingObject(heroTarget.gameObject);

        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    public void SpawnRay()
    {
        _rayOfHope.SpawnRayOfHope();
    }

    public void RemoveTargetZones()
    {
        _storedSafeAndTargetZone.RemoveAllZones();
        _rayBuffTargetZoneParent.RemoveBossTargetZones();
    }

    public void HeroesInSafeZone()
    {
        _damageTargetZone.gameObject.SetActive(false);
        _rayTargetZone.gameObject.SetActive(true);
    }

    public void HeroesNoLongerInSafeZone()
    {
        _damageTargetZone.gameObject.SetActive(true);
        _rayTargetZone.gameObject.SetActive(false);
    }

    private void SubscribeToEvents()
    {
        _safeZone.GetOnTargetZoneSetToHeroInRange().AddListener(HeroesInSafeZone);
        _safeZone.GetOnTargetZoneSetToNoHeroInRange().AddListener(HeroesNoLongerInSafeZone);
    }

    private void UnsubscribeFromEvents()
    {
        _safeZone.GetOnTargetZoneSetToHeroInRange().RemoveListener(HeroesInSafeZone);
        _safeZone.GetOnTargetZoneSetToNoHeroInRange().RemoveListener(HeroesNoLongerInSafeZone);
    }
    
    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
    }
    #endregion

    #region Getters

    public BossSharedSafeAndTargetZone GetStoredSafeAndTargetZone() => _storedSafeAndTargetZone;

    #endregion
}
