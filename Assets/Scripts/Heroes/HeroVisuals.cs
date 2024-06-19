using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroVisuals : HeroChildrenFunctionality
{
    [SerializeField] private MeshRenderer _controlIcon;
    [SerializeField] private GameObject _healthStatusIcon;

    [SerializeField] private RectTransform _damageNumbersOrigin;
    [SerializeField] private RectTransform _healingNumbersOrigin;
    [SerializeField] private RectTransform _abilityChargedIconOrigin;

    private const string _healthStatusIntAnim = "HealthStatus";

    public override void ChildFuncSetup(HeroBase heroBase)
    {
        base.ChildFuncSetup(heroBase);
    }


    private void HeroHealthAboveHalf()
    {
        _healthStatusIcon.GetComponent<Animator>().SetInteger(_healthStatusIntAnim, 0);
    }

    private void HeroInjured()
    {
        _healthStatusIcon.GetComponent<Animator>().SetInteger(_healthStatusIntAnim, 1);
    }

    private void HeroCritical()
    {
        _healthStatusIcon.GetComponent<Animator>().SetInteger(_healthStatusIntAnim, 2);
    }

    #region Events
    public override void SubscribeToEvents()
    {
        myHeroBase.GetSOSetEvent().AddListener(HeroSOAssigned);
        myHeroBase.GetHeroControlledBeginEvent().AddListener(HeroControlStart);
        myHeroBase.GetHeroControlledEndEvent().AddListener(HeroControlStop);

        myHeroBase.GetHeroHealedAboveHalfEvent().AddListener(HeroHealthAboveHalf);
        myHeroBase.GetHeroHealedAboveQuarterEvent().AddListener(HeroInjured);
        myHeroBase.GetHeroDamagedUnderHalfEvent().AddListener(HeroInjured);
        myHeroBase.GetHeroDamagedUnderQuarterEvent().AddListener(HeroCritical);


        myHeroBase.GetHeroStartedMovingEvent().AddListener(HeroStartedMoving);
        myHeroBase.GetHeroStoppedMovingEvent().AddListener(HeroStoppedMoving);
    }
    private void HeroSOAssigned(HeroSO heroSO)
    {
        //Debug.Log("Hero Assigned SO Event");
        //_meshAgent.speed = heroSO.GetMoveSpeed();

    }
    private void HeroControlStart()
    {
        _controlIcon.enabled = true;
    }

    private void HeroControlStop()
    {
        _controlIcon.enabled = false;
    }

    private void HeroStartedMoving()
    {
        
    }

    private void HeroStoppedMoving()
    {

    }
    #endregion

    #region Getters
    public RectTransform GetDamageNumbersOrigin() => _damageNumbersOrigin;
    public RectTransform GetHealingNumbersOrigin() => _healingNumbersOrigin;
    public RectTransform GetAbilityChargedIconOrigin() => _abilityChargedIconOrigin;
    #endregion
}
