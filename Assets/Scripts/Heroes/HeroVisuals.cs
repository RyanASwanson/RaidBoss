using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroVisuals : HeroChildrenFunctionality
{
    [SerializeField] private MeshRenderer _controlIcon;
    [SerializeField] private GameObject _healthStatusIcon;
    [Space]

    [SerializeField] private GameObject _heroDamagedVFX;
    [SerializeField] private GameObject _heroHealedVFX;
    [Space]

    [SerializeField] private RectTransform _damageNumbersOrigin;
    [SerializeField] private RectTransform _healingNumbersOrigin;
    [SerializeField] private RectTransform _abilityChargedIconOrigin;

    private const string _healthStatusIntAnim = "HealthStatus";

    [Space]
    [SerializeField] private Animator _heroGeneralAnimator;

    private const string _levelIntroTriggerAnim = "LevelIntroTrigger";
    private const string _heroDamagedTriggerAnim = "HeroDamaged";
    private const string _heroHealedTriggerAnim = "HeroHealed";
    private const string _heroDeathTriggerAnim = "HeroDeath";

    public override void ChildFuncSetup(HeroBase heroBase)
    {
        base.ChildFuncSetup(heroBase);

        HeroLevelIntroAnimation();
    }


    private void HeroDamaged(float damage)
    {
        Instantiate(_heroDamagedVFX, transform.position, Quaternion.identity);
        HeroDamagedAnimation();
    }

    private void HeroHealed(float healing)
    {
        Instantiate(_heroHealedVFX, transform.position, Quaternion.identity);
        HeroHealedAnimation();
    }

    private void HeroDied()
    {
        HeroDeathAnimation();
    }

    #region Health Status
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
    #endregion

    public void HeroLevelIntroAnimation()
    {
        _heroGeneralAnimator.SetTrigger(_levelIntroTriggerAnim);
    }

    private void HeroDamagedAnimation()
    {
        _heroGeneralAnimator.SetTrigger(_heroDamagedTriggerAnim);
    }

    private void HeroHealedAnimation()
    {
        _heroGeneralAnimator.SetTrigger(_heroHealedTriggerAnim);
    }

    private void HeroDeathAnimation()
    {
        _heroGeneralAnimator.SetTrigger(_heroDeathTriggerAnim);
    }

    #region Events
    public override void SubscribeToEvents()
    {
        myHeroBase.GetSOSetEvent().AddListener(HeroSOAssigned);
        myHeroBase.GetHeroControlledBeginEvent().AddListener(HeroControlStart);
        myHeroBase.GetHeroControlledEndEvent().AddListener(HeroControlStop);

        myHeroBase.GetHeroDamagedEvent().AddListener(HeroDamaged);
        myHeroBase.GetHeroHealedEvent().AddListener(HeroHealed);
        myHeroBase.GetHeroDiedEvent().AddListener(HeroDied);

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
