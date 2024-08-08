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
    [SerializeField] private RectTransform _buffDebuffOrigin;
    [SerializeField] private RectTransform _abilityChargedIconOrigin;

    private const string _healthStatusIntAnim = "HealthStatus";

    [Space]
    [SerializeField] private Animator _heroGeneralAnimator;

    private const string _levelIntroTriggerAnim = "LevelIntroTrigger";
    private const string _heroDamagedTriggerAnim = "HeroDamaged";
    private const string _heroHealedTriggerAnim = "HeroHealed";
    private const string _heroDeathTriggerAnim = "HeroDeath";


    [Space]
    private Animator _heroSpecificAnimator;

    private const string _heroIdleAnimTrigger = "G_HeroIdle";
    private const string _heroWalkingAnimBool = "G_HeroWalking";

    private const string _heroBasicAnimTrigger = "G_HeroBasic";
    private const string _heroManualAnimTrigger = "G_HeroManual";
    private const string _heroPassiveAnimTrigger = "G_HeroPassive";

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

    private void HeroGeneralAnimationTrigger(string animationTrigger)
    {
        _heroGeneralAnimator.SetTrigger(animationTrigger);
    }

    private void HeroGeneralAnimationBool(string animationTrigger, bool boolStatus)
    {
        _heroGeneralAnimator.SetBool(animationTrigger, boolStatus);
    }

    public void HeroLevelIntroAnimation()
    {
        HeroGeneralAnimationTrigger(_levelIntroTriggerAnim);
    }

    private void HeroDamagedAnimation()
    {
        HeroGeneralAnimationTrigger(_heroDamagedTriggerAnim);
    }

    private void HeroHealedAnimation()
    {
        HeroGeneralAnimationTrigger(_heroHealedTriggerAnim);
    }

    private void HeroDeathAnimation()
    {
        HeroGeneralAnimationTrigger(_heroDeathTriggerAnim);
    }


    /// <summary>
    /// Activates a trigger on the hero specific animator
    /// </summary>
    /// <param name="animationTrigger"></param>
    private void HeroSpecificAnimationTrigger(string animationTrigger)
    {
        _heroSpecificAnimator.SetTrigger(animationTrigger);
    }

    /// <summary>
    /// Sets a bool on the hero specific animator
    /// </summary>
    /// <param name="animationBool"></param>
    /// <param name="boolStatus"></param>
    private void HeroSpecificAnimationBool(string animationBool, bool boolStatus)
    {
        _heroSpecificAnimator.SetBool(animationBool, boolStatus);
    }


    /// <summary>
    /// Tells the hero specific animator to their animations by entering with the idle animation
    /// </summary>
    private void StartHeroSpecificIdleAnimation()
    {
        HeroSpecificAnimationTrigger(_heroIdleAnimTrigger);
    }


    /// <summary>
    /// Tells the hero specific animator to start performing their walk animation
    /// </summary>
    public void StartHeroSpecificWalkingAnimation()
    {
        HeroSpecificAnimationBool(_heroWalkingAnimBool, true);
    }

    /// <summary>
    /// Tells the hero specific animator to stop performing their walk animation
    /// </summary>
    public void StopHeroSpecificWalkingAnimation()
    {
        HeroSpecificAnimationBool(_heroWalkingAnimBool, false);
    }

    /// <summary>
    /// Tells the hero specific animator to start their basic attack animation
    /// </summary>
    public void TriggerBasicAbilityAnimation()
    {
        HeroSpecificAnimationTrigger(_heroBasicAnimTrigger);
    }

    /// <summary>
    /// Tells the hero specific animator to start their manual ability animation
    /// </summary>
    public void TriggerManualAbilityAnimation()
    {
        HeroSpecificAnimationTrigger(_heroManualAnimTrigger);
    }

    /// <summary>
    /// Tells the hero specific animator to start their passive ability animation
    /// </summary>
    public void TriggerPassiveAbilityAnimation()
    {
        HeroSpecificAnimationTrigger(_heroPassiveAnimTrigger);
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
        AssignHeroSpecificAnimator();

        StartHeroSpecificIdleAnimation();
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
        StartHeroSpecificWalkingAnimation();
    }

    private void HeroStoppedMoving()
    {
        StopHeroSpecificWalkingAnimation();
    }

    private void AssignHeroSpecificAnimator()
    {
        _heroSpecificAnimator = myHeroBase.GetSpecificHeroScript().GetSpecificHeroAnimator();
    }
    #endregion

    #region Getters
    public RectTransform GetDamageNumbersOrigin() => _damageNumbersOrigin;
    public RectTransform GetHealingNumbersOrigin() => _healingNumbersOrigin;
    public RectTransform GetBuffDebuffOrigin() => _buffDebuffOrigin;
    public RectTransform GetAbilityChargedIconOrigin() => _abilityChargedIconOrigin;
    #endregion
}
