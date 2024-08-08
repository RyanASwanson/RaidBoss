using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_AlchemistPotion : HeroProjectileFramework
{
    [SerializeField] private float _moveTime;
    [SerializeField] private float _idleLifetime;
    [Space]

    [SerializeField] private PotionTypes _potionType;
    [SerializeField] private float _buffStrength;
    [SerializeField] private float _secondaryBuffStrength;
    [SerializeField] private float _buffDuration;
    

    [Space]
    [SerializeField] private GeneralHeroHealArea _healArea;

    [Space]

    [SerializeField] private Animator _animator;
    private const string _removePotionAnimTrigger = "RemovePotion";

    private SH_Alchemist _alchemist;

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);

        
    }

    public void AdditionalSetup(Vector3 targetLocation)
    {
        PotionTypeSetup();
        StartCoroutine(MovePotionToEndLocation(targetLocation));
        StartCoroutine(RemovePotionTimer());
    }

    private void PotionTypeSetup()
    {
        _alchemist = (SH_Alchemist)_mySpecificHero;

        _healArea.GetEnterEvent().AddListener(ActivateAlchemistPassive);

        switch (_potionType)
        {
            case (PotionTypes.DamagePotion):
                _healArea.GetEnterEvent().AddListener(DamageBuff);
                return;
            case (PotionTypes.StaggerPotion):
                _healArea.GetEnterEvent().AddListener(StaggerBuff);
                return;
            case (PotionTypes.SpeedPotion):
                _healArea.GetEnterEvent().AddListener(SpeedBuff);
                return;
            case (PotionTypes.UtilityPotion):
                _healArea.GetEnterEvent().AddListener(UtilityBuff);
                return;

        }
        
    }

    public IEnumerator MovePotionToEndLocation( Vector3 targetLocation)
    {
        Vector3 startingPotionLocation = transform.position;
        float lerpProgress = 0;

        while (lerpProgress < 1)
        {
            lerpProgress += Time.deltaTime / _moveTime;
            transform.position = Vector3.Lerp(startingPotionLocation, targetLocation, lerpProgress);
            yield return null;
        }

        transform.position = targetLocation;
        ReachedEndLocation();
    }

    private void ReachedEndLocation()
    {
        _healArea.ToggleProjectileCollider(true);
    }


    public void ActivateAlchemistPassive(Collider collider)
    {
        _alchemist.ActivatePassiveAbilities(collider.transform.position);
    }

    private void DamageBuff(Collider collider)
    {
        ApplyBuffToHero(collider.GetComponentInParent<HeroBase>().GetHeroStats(),
            HeroGeneralAdjustableStats.DamageMultiplier);
    }

    private void StaggerBuff(Collider collider)
    {
        ApplyBuffToHero(collider.GetComponentInParent<HeroBase>().GetHeroStats(),
            HeroGeneralAdjustableStats.StaggerMultiplier);
    }

    private void SpeedBuff(Collider collider)
    {
        ApplyBuffToHero(collider.GetComponentInParent<HeroBase>().GetHeroStats(),
            HeroGeneralAdjustableStats.SpeedMultiplier);
    }

    private void UtilityBuff(Collider collider)
    {
        ApplyBuffToHero(collider.GetComponentInParent<HeroBase>().GetHeroStats(),
            HeroGeneralAdjustableStats.HealingRecievedMultiplier);
    }

    private void ApplyBuffToHero(HeroStats heroStats, HeroGeneralAdjustableStats stat)
    {
        heroStats.ApplyStatChangeForDuration(stat, _buffStrength,_secondaryBuffStrength, _buffDuration);
    }

    private IEnumerator RemovePotionTimer()
    {
        yield return new WaitForSeconds(_idleLifetime);
        RemovePotionAnimation();
    }

    private void RemovePotionAnimation()
    {
        _animator.SetTrigger(_removePotionAnimTrigger);
    }
}
