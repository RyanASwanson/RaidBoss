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

    [SerializeField] private RectTransform _abilityChargedOrigin;
    [SerializeField] private Animator _abilityRechargedHolder;

    private const string _showAbilityRechargedHolderBool = "ShowRechargedIcon";

    [Space]
    [SerializeField] private RectTransform _damageNumbersOrigin;
    [SerializeField] private RectTransform _healingNumbersOrigin;
    [SerializeField] private RectTransform _buffDebuffOrigin;
    [SerializeField] private RectTransform _abilityReChargedPopupIconOrigin;

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

    #region Health Status
    private void HeroDamaged(float damage)
    {
        CreateHeroDamagedVFX();
        HeroDamagedAnimation();
    }

    private void HeroHealed(float healing)
    {
        CreateHeroHealedVFX();
        HeroHealedAnimation();
    }

    private void CreateHeroDamagedVFX()
    {
        Instantiate(_heroDamagedVFX, transform.position, Quaternion.identity);
    }

    private void CreateHeroHealedVFX()
    {
        Instantiate(_heroHealedVFX, transform.position, Quaternion.identity);
    }

    private void HeroDied()
    {
        HeroDeathAnimation();
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

    #endregion


    #region Hero Animation
    /// <summary>
    /// Activates a trigger on the hero general animator
    /// </summary>
    /// <param name="animationTrigger"></param>
    private void HeroGeneralAnimationTrigger(string animationTrigger)
    {
        _heroGeneralAnimator.SetTrigger(animationTrigger);
    }

    /// <summary>
    /// Sets a bool on the hero general animator
    /// </summary>
    /// <param name="animationTrigger"></param>
    /// <param name="boolStatus"></param>
    private void HeroGeneralAnimationBool(string animationTrigger, bool boolStatus)
    {
        _heroGeneralAnimator.SetBool(animationTrigger, boolStatus);
    }


    /// <summary>
    /// Activates a trigger on the hero specific animator
    /// </summary>
    /// <param name="animationTrigger"></param>
    public void HeroSpecificAnimationTrigger(string animationTrigger)
    {
        _heroSpecificAnimator.SetTrigger(animationTrigger);
    }

    /// <summary>
    /// Sets a bool on the hero specific animator
    /// </summary>
    /// <param name="animationBool"></param>
    /// <param name="boolStatus"></param>
    public void HeroSpecificAnimationBool(string animationBool, bool boolStatus)
    {
        _heroSpecificAnimator.SetBool(animationBool, boolStatus);
    }


    public void HeroLevelIntroAnimation()
    {
        HeroGeneralAnimationTrigger(_levelIntroTriggerAnim);
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

    /// <summary>
    /// Causes the general hero damage animation to play
    /// </summary>
    private void HeroDamagedAnimation()
    {
        HeroGeneralAnimationTrigger(_heroDamagedTriggerAnim);
    }

    /// <summary>
    /// Causes the general hero heal animation to play
    /// </summary>
    private void HeroHealedAnimation()
    {
        HeroGeneralAnimationTrigger(_heroHealedTriggerAnim);
    }

    /// <summary>
    /// Causes the hero death animation to play
    /// </summary>
    private void HeroDeathAnimation()
    {
        HeroGeneralAnimationTrigger(_heroDeathTriggerAnim);
    }

    #endregion

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
        AssignHeroSpecificAnimator();

        StartHeroSpecificIdleAnimation();
    }

    /// <summary>
    /// Shows the control shape under the hero
    /// </summary>
    private void HeroControlStart()
    {
        _controlIcon.enabled = true;
    }

    /// <summary>
    /// Hides the control shape under the hero
    /// </summary>
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
    public RectTransform GetAbilityChargedOrigin() => _abilityChargedOrigin;
    public RectTransform GetDamageNumbersOrigin() => _damageNumbersOrigin;
    public RectTransform GetHealingNumbersOrigin() => _healingNumbersOrigin;
    public RectTransform GetBuffDebuffOrigin() => _buffDebuffOrigin;
    public RectTransform GetAbilityReChargedPopupIconOrigin() => _abilityReChargedPopupIconOrigin;
    #endregion
}
