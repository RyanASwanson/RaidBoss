using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Provides the functionality for the potions created by the Alchemist
/// </summary>
public class SHP_AlchemistPotion : HeroProjectileFramework
{
    [SerializeField] private float _moveTime;
    [SerializeField] private AnimationCurve _moveCurve;
    [SerializeField] private float _idleLifetime;
    [SerializeField] private float _idleLifetimeWarning;
    private WaitForSeconds _potionIdleWait;
    private WaitForSeconds _potionIdleWarningWait;
        
    [Space]
    [SerializeField] private PotionTypes _potionType;
    [SerializeField] private HeroAdjustableStatGroup _statChanges;
    [SerializeField] private Color[] _potionPickUpColors;

    [Space] 
    [SerializeField] private float _potionPickUpEffectScale;
    [SerializeField] private GameObject _potionPickUpVisualEffect;
    [SerializeField] private GameObject _potionBuffFollowVisualEffect;
    
    [Space]
    [SerializeField] private GeneralHeroHealArea _healArea;

    [SerializeField] private CurveProgression _lifeTimeWarningCurve;
    [SerializeField] private MeshRenderer _potionMesh;

    [Space]
    [SerializeField] private Animator _animator;

    private const string REMOVE_POTION_ANIM_TRIGGER = "RemovePotion";

    private SH_Alchemist _alchemist;

    /// <summary>
    /// Sets up the potion differently depending on what type of potion it is
    /// </summary>
    private void PotionTypeSetup()
    {
        _alchemist = (SH_Alchemist)_mySpecificHero;

        if (_potionIdleWait.IsUnityNull())
        {
            _potionIdleWait = new WaitForSeconds(_idleLifetime);
        }

        if (_potionIdleWarningWait.IsUnityNull())
        {
            _potionIdleWarningWait = new WaitForSeconds(_idleLifetimeWarning);
        }

        _healArea.GetEnterEvent().AddListener(PotionGeneralPickUp);

        if (_potionType == PotionTypes.HealingPotion)
        {
            return;
        }
        
        _healArea.GetEnterEvent().AddListener(ApplyBuffToHero);
    }

    /// <summary>
    /// The process by which the potion moves from where it's created to it's end location
    /// </summary>
    /// <param name="targetLocation"></param>
    /// <returns></returns>
    public IEnumerator MovePotionToEndLocation(Vector3 targetLocation)
    {
        Vector3 startingPotionLocation = transform.position;
        float lerpProgress = 0;

        while (lerpProgress < 1)
        {
            lerpProgress += Time.deltaTime / _moveTime;
            transform.position = Vector3.Lerp(startingPotionLocation, targetLocation, _moveCurve.Evaluate(lerpProgress));
            yield return null;
        }

        transform.position = targetLocation;
        ReachedEndLocation();
    }

    /// <summary>
    /// Called when the potion reaches the end of it's path
    /// </summary>
    private void ReachedEndLocation()
    {
        PlayPotionLandedAudio();
        
        _healArea.ToggleProjectileCollider(true);
    }

    public void PotionGeneralPickUp(Collider collider)
    {
        ActivateAlchemistPassive(collider);
        PlayPotionPickUpAudio();
        CreatePotionPickUpEffect();
        RemovePickedUpPotion();
    }

    private void RemovePickedUpPotion()
    {
        
    }
    
    /// <summary>
    /// Tells the alchemist script to use its passive ability
    /// </summary>
    /// <param name="collider"></param> The object that picked up the potion
    public void ActivateAlchemistPassive(Collider collider)
    {
        _alchemist.ActivatePassiveAbilities(collider.transform.position);
    }
    
    private void ApplyBuffToHero(Collider hero)
    {
        HeroBase heroBase = hero.GetComponentInParent<HeroBase>();
        
        PlayPotionBuffActivatedAudio();
        
        CreatePotionFollowEffect(heroBase);
        
        heroBase.GetHeroStats().ApplyStatChangesForDuration(_statChanges);
    }

    /// <summary>
    /// The process that delays the removal of the potion
    /// </summary>
    /// <returns></returns>
    private IEnumerator RemovePotionTimer()
    {
        yield return _potionIdleWait;
        RemovePotionAnimation();
    }

    private void RemovePotionAnimation()
    {
        _animator.SetTrigger(REMOVE_POTION_ANIM_TRIGGER);
    }
    
    private IEnumerator PotionRemoveWarningTimer()
    {
        yield return _potionIdleWarningWait;
        PotionRemoveWarning();
    }

    private void PotionRemoveWarning()
    {
        _lifeTimeWarningCurve.StartMovingUpOnCurve();
    }

    private void CreatePotionPickUpEffect()
    {
        GeneralVFXFunctionality vfxFunctionality =
            Instantiate(_potionPickUpVisualEffect, transform.position, Quaternion.identity)
                .GetComponent<GeneralVFXFunctionality>();

        vfxFunctionality.gameObject.transform.localScale *= _potionPickUpEffectScale;
        vfxFunctionality.SetStartColor(_potionPickUpColors[0], _potionPickUpColors[1]);
        vfxFunctionality.PlayAllParticleSystems();
    }
    private void CreatePotionFollowEffect(HeroBase heroBase)
    {
        AlchemistManualFollowEffect alchemistManualFollowEffect = 
            Instantiate(_potionBuffFollowVisualEffect, heroBase.transform.position, Quaternion.identity)
                .GetComponent<AlchemistManualFollowEffect>();
        
        alchemistManualFollowEffect.SetUp(heroBase.gameObject,_potionMesh.material, _statChanges.BuffDuration);
    }

    private void PlayPotionLandedAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(AudioManager.Instance.AllSpecificHeroAudio
                [_myHeroBase.GetHeroSO().GetHeroID()]
            .MiscellaneousHeroAudio[SH_Alchemist.ALCHEMIST_POTION_LANDED_AUDIO_ID]);
    }

    private void PlayPotionPickUpAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(AudioManager.Instance.AllSpecificHeroAudio
                [_myHeroBase.GetHeroSO().GetHeroID()]
            .MiscellaneousHeroAudio[SH_Alchemist.ALCHEMIST_POTION_PICKED_UP_AUDIO_ID]);
    }

    private void PlayPotionBuffActivatedAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(AudioManager.Instance.AllSpecificHeroAudio
                [_myHeroBase.GetHeroSO().GetHeroID()]
            .MiscellaneousHeroAudio[SH_Alchemist.ALCHEMIST_POTION_BUFF_ACTIVATED_AUDIO_ID]);
    }
    
    #region Base Ability
    public void AdditionalSetup(Vector3 targetLocation)
    {
        PotionTypeSetup();
        StartCoroutine(MovePotionToEndLocation(targetLocation));
        StartCoroutine(RemovePotionTimer());
        StartCoroutine(PotionRemoveWarningTimer());
    }
    #endregion
}
