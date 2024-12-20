using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroVisuals : HeroChildrenFunctionality
{
    [SerializeField] private MeshRenderer _controlIcon;
    [SerializeField] private GameObject _healthStatusIcon;
    [Space]

    [SerializeField] private GameObject _heroDamagedVFX;
    [SerializeField] private GameObject _heroHealedVFX;
    [Space]

    [SerializeField] private RectTransform _abilityChargedOrigin;
    [SerializeField] private Animator _abilityChargedAnimator;
    [SerializeField] private Image _abilityChargedManualImage;

    [Space]
    [SerializeField] private RectTransform _heroControlledOrigin;
    [SerializeField] private Animator _heroControlledAnimator;
    [SerializeField] private Image _heroControlledIcon;

    [Space]
    [SerializeField] private RectTransform _damageNumbersOrigin;
    [SerializeField] private RectTransform _healingNumbersOrigin;
    [SerializeField] private RectTransform _buffDebuffOrigin;
    [SerializeField] private RectTransform _abilityReChargedPopupIconOrigin;

    [Space]
    [SerializeField] private Transform _recentHealthOrigin;
    [SerializeField] private Image _recentHealthPopUp;
    [SerializeField] private Gradient _recentHealthPopUpGradient;
    [SerializeField] private AnimationCurve _scaleCurve;
    [SerializeField] private Animator _recentHealthPopupAnimator;

    private const string SHOW_RECENT_HEALTH_POP_UP_ANIM_TRIGGER = "PopUpRecentHealth";

    private const string GROUND_HEALTH_STATUS_ANIM_INT = "HealthStatus";

    [Space]
    [SerializeField] private Animator _heroGeneralAnimator;

    private const string LEVEL_INTO_ANIM_TRIGGER = "LevelIntroTrigger";
    private const string HERO_DAMAGED_ANIM_TRIGGER = "HeroDamaged";
    private const string HERO_HEALED_ANIM_TRIGGER = "HeroHealed";
    private const string HERO_DEATH_ANIM_TRIGGER = "HeroDeath";


    private Animator _heroSpecificAnimator;

    private const string HERO_IDLE_ANIM_TRIGGER = "G_HeroIdle";
    private const string HERO_WALKING_ANIM_BOOL = "G_HeroWalking";

    private const string HERO_BASIC_ANIM_TRIGGER = "G_HeroBasic";
    private const string HERO_MANUAL_ANIM_TRIGGER = "G_HeroManual";
    private const string HERO_PASSIVE_ANIM_TRIGGER = "G_HeroPassive";

    [Space]
    [SerializeField] private float _outlineWidth;
    [SerializeField] private Outline.Mode _outlineMode;
    private Outline _addedOutline;


    #region Health Status
    private void HeroDamaged(float damage)
    {
        CreateHeroDamagedVFX();
        HeroDamagedAnimation();
        ShowHealthPopUp();
    }

    private void HeroHealed(float healing)
    {
        CreateHeroHealedVFX();
        HeroHealedAnimation();
        ShowHealthPopUp();
    }

    private void CreateHeroDamagedVFX()
    {
        Instantiate(_heroDamagedVFX, transform.position, Quaternion.identity);
    }

    private void CreateHeroHealedVFX()
    {
        Instantiate(_heroHealedVFX, transform.position, Quaternion.identity);
    }

    private void ShowHealthPopUp()
    {
        float healthPercent = _myHeroBase.GetHeroStats().GetHeroHealthPercentage();

        _recentHealthPopUp.color = DetermineHealthPopUpColor(healthPercent);
        _recentHealthPopupAnimator.SetTrigger(SHOW_RECENT_HEALTH_POP_UP_ANIM_TRIGGER);

        float newScale = DetermineRecentHealthPopUpScale(healthPercent);
        _recentHealthOrigin.transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    private Color DetermineHealthPopUpColor(float percent)
    {
        return _recentHealthPopUpGradient.Evaluate(percent);
    }

    private float DetermineRecentHealthPopUpScale(float percent)
    {
        return 1 + _scaleCurve.Evaluate(percent);
    }

    private void HeroDied()
    {
        HeroDeathAnimation();
    }

    
    private void HeroHealthAboveHalf()
    {
        _healthStatusIcon.GetComponent<Animator>().SetInteger(GROUND_HEALTH_STATUS_ANIM_INT, 0);
    }

    private void HeroInjured()
    {
        _healthStatusIcon.GetComponent<Animator>().SetInteger(GROUND_HEALTH_STATUS_ANIM_INT, 1);
    }

    private void HeroCritical()
    {
        _healthStatusIcon.GetComponent<Animator>().SetInteger(GROUND_HEALTH_STATUS_ANIM_INT, 2);
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
        HeroGeneralAnimationTrigger(LEVEL_INTO_ANIM_TRIGGER);
    }


    /// <summary>
    /// Tells the hero specific animator to their animations by entering with the idle animation
    /// </summary>
    private void StartHeroSpecificIdleAnimation()
    {
        HeroSpecificAnimationTrigger(HERO_IDLE_ANIM_TRIGGER);
    }


    /// <summary>
    /// Tells the hero specific animator to start performing their walk animation
    /// </summary>
    public void StartHeroSpecificWalkingAnimation()
    {
        HeroSpecificAnimationBool(HERO_WALKING_ANIM_BOOL, true);
    }

    /// <summary>
    /// Tells the hero specific animator to stop performing their walk animation
    /// </summary>
    public void StopHeroSpecificWalkingAnimation()
    {
        HeroSpecificAnimationBool(HERO_WALKING_ANIM_BOOL, false);
    }

    /// <summary>
    /// Tells the hero specific animator to start their basic attack animation
    /// </summary>
    public void TriggerBasicAbilityAnimation()
    {
        HeroSpecificAnimationTrigger(HERO_BASIC_ANIM_TRIGGER);
    }

    /// <summary>
    /// Tells the hero specific animator to start their manual ability animation
    /// </summary>
    public void TriggerManualAbilityAnimation()
    {
        HeroSpecificAnimationTrigger(HERO_MANUAL_ANIM_TRIGGER);
    }

    /// <summary>
    /// Tells the hero specific animator to start their passive ability animation
    /// </summary>
    public void TriggerPassiveAbilityAnimation()
    {
        HeroSpecificAnimationTrigger(HERO_PASSIVE_ANIM_TRIGGER);
    }

    /// <summary>
    /// Causes the general hero damage animation to play
    /// </summary>
    private void HeroDamagedAnimation()
    {
        HeroGeneralAnimationTrigger(HERO_DAMAGED_ANIM_TRIGGER);
    }

    /// <summary>
    /// Causes the general hero heal animation to play
    /// </summary>
    private void HeroHealedAnimation()
    {
        HeroGeneralAnimationTrigger(HERO_HEALED_ANIM_TRIGGER);
    }

    /// <summary>
    /// Causes the hero death animation to play
    /// </summary>
    private void HeroDeathAnimation()
    {
        HeroGeneralAnimationTrigger(HERO_DEATH_ANIM_TRIGGER);
    }

    #endregion

    #region Hero Control
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
    #endregion

    #region Hero Movement
    private void HeroStartedMoving()
    {
        StartHeroSpecificWalkingAnimation();
    }

    private void HeroStoppedMoving()
    {
        StopHeroSpecificWalkingAnimation();
    }
    #endregion

    #region Hero Outline
    private void AddOutlineToHero()
    {
        _addedOutline = _myHeroBase.GetAssociatedHeroObject().AddComponent<Outline>();

        _addedOutline.OutlineWidth = _outlineWidth;
        _addedOutline.OutlineMode = _outlineMode;
    }
    #endregion


    private void AssignHeroSpecificAnimator()
    {
        _heroSpecificAnimator = _myHeroBase.GetSpecificHeroScript().GetSpecificHeroAnimator();
    }

    #region Base Hero
    public override void ChildFuncSetup(HeroBase heroBase)
    {
        base.ChildFuncSetup(heroBase);

        HeroLevelIntroAnimation();
    }

    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();

        _myHeroBase.GetHeroControlledBeginEvent().AddListener(HeroControlStart);
        _myHeroBase.GetHeroControlledEndEvent().AddListener(HeroControlStop);

        _myHeroBase.GetHeroDamagedEvent().AddListener(HeroDamaged);
        _myHeroBase.GetHeroHealedEvent().AddListener(HeroHealed);
        _myHeroBase.GetHeroDiedEvent().AddListener(HeroDied);

        _myHeroBase.GetHeroHealedAboveHalfEvent().AddListener(HeroHealthAboveHalf);
        _myHeroBase.GetHeroHealedAboveQuarterEvent().AddListener(HeroInjured);
        _myHeroBase.GetHeroDamagedUnderHalfEvent().AddListener(HeroInjured);
        _myHeroBase.GetHeroDamagedUnderQuarterEvent().AddListener(HeroCritical);

        _myHeroBase.GetHeroStartedMovingEvent().AddListener(HeroStartedMoving);
        _myHeroBase.GetHeroStoppedMovingEvent().AddListener(HeroStoppedMoving);
    }

    protected override void HeroSOAssigned(HeroSO heroSO)
    {
        AssignHeroSpecificAnimator();

        StartHeroSpecificIdleAnimation();

        AddOutlineToHero();

        base.HeroSOAssigned(heroSO);
    }
    #endregion

    #region Getters
    public RectTransform GetAbilityChargedOrigin() => _abilityChargedOrigin;
    public Animator GetAbilityChargedAnimator() => _abilityChargedAnimator;
    public Image GetAbilityChargedManualImage() => _abilityChargedManualImage;

    public RectTransform GetHeroControlledOrigin() => _heroControlledOrigin;
    public Animator GetHeroControlledAnimator() => _heroControlledAnimator;
    public Image GetHeroControlledIcon() => _heroControlledIcon;

    public RectTransform GetDamageNumbersOrigin() => _damageNumbersOrigin;
    public RectTransform GetHealingNumbersOrigin() => _healingNumbersOrigin;
    public RectTransform GetBuffDebuffOrigin() => _buffDebuffOrigin;
    public RectTransform GetAbilityReChargedPopupIconOrigin() => _abilityReChargedPopupIconOrigin;
    #endregion
}
