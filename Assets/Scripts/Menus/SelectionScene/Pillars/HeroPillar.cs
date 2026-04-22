using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Provides the functionality for the hero pillars on the selection scene
/// </summary>
public class HeroPillar : MonoBehaviour
{
    [SerializeField] private GameObject _heroSpawnPointHolder;
    [SerializeField] private GameObject _heroSpawnPoint;
    [SerializeField] private GameObject _previewBase;

    [Space] 
    [SerializeField] private GeneralVFXFunctionality _selectedHeroParticles;
    
    [Space]
    [SerializeField] private Animator _heroSpawnAnimator;

    private GameObject _currentHeroVisual;

    private HeroSO _heroSelectedOnPillar;
    private HeroSO _storedHero;
    
    [Space]
    [SerializeField] private Animator _pillarAnimator;

    private const string HERO_PILLAR_MOVE_ANIM_BOOL = "PillarUp";
    private const string HERO_PILLAR_PREVIEW_SHOW_ANIM_BOOL = "PreviewShow";

    private const string NEW_HERO_HOVER_ANIM_TRIGGER = "NewHover";
    private const string REMOVE_HERO_ON_PILLAR_ANIM_TRIGGER = "RemoveHero";

    private Animator _heroSpecificAnimator;
    
    private const string HERO_PILLAR_SELECTED_ANIM_BOOL = "HeroSelected";
    
    [SerializeField] private Canvas _pillarHeroDeselectCanvas;

    private void Start()
    {
        SetUpHeroDeselectCanvas();
        SetHeroPreviewAnimation(false);
    }

    public void MovePillar(bool moveUp)
    {
        _pillarAnimator.SetBool(HERO_PILLAR_MOVE_ANIM_BOOL, moveUp);
    }

    /// <summary>
    /// Displays a hero on the pillar
    /// </summary>
    /// <param name="heroSO"></param>
    public void ShowHeroOnPillar(HeroSO heroSO, bool newHero)
    {
        ShowHeroOnPillar(heroSO, newHero, false);
    }

    public void ShowHeroOnPillar(HeroSO heroSO, bool newHero, bool heroAlreadySelectedOverride)
    {
        //If there is a hero on the pillar remove them
        if (!_currentHeroVisual.IsUnityNull())
        {
            RemoveHeroOnPillar();
        }

        //Spawn the hero onto the pillar
        _currentHeroVisual = Instantiate(heroSO.GetHeroPrefab(), _heroSpawnPoint.transform);
        
        _heroSpecificAnimator = _currentHeroVisual.GetComponent<Animator>();
        
        //Rotates the hero
        _currentHeroVisual.transform.eulerAngles += new Vector3(0,180,0);
        //Sets the stored hero
        _storedHero = heroSO;

        if (_heroSelectedOnPillar == heroSO || heroAlreadySelectedOverride)
        {
            SetHeroPreviewAnimation(true);
            PlayHeroIdleAnimation();
            
            PlayParticlesOfHeroOnPillar();
        }
        else
        {
            SetHeroPreviewAnimation(!newHero);
        }

        if (!newHero)
        {
            HeroSelectedOnPillar();
            return;
        }

        PlayHeroHoverAnimation();
        _heroSpawnAnimator.ResetTrigger(REMOVE_HERO_ON_PILLAR_ANIM_TRIGGER);
    }

    public void HeroSelectedOnPillar()
    {
        _heroSelectedOnPillar = _storedHero;
        
        StartHeroSelectedAnimation();
        PlayHeroIdleAnimation();
        
        PlayParticlesOfHeroOnPillar();
    }

    /// <summary>
    /// Removes the current hero from the pillar
    /// </summary>
    public void RemoveHeroOnPillar()
    {
        _storedHero = null;
        SetHeroPreviewAnimation(false);
        Destroy(_currentHeroVisual);
        StopParticlesOfHeroOnPillar();
    }

    public void HeroOnPillarDeselected()
    {
        _storedHero = null;
        _heroSelectedOnPillar = null;
        SetHeroPreviewAnimation(false);
        StopParticlesOfHeroOnPillar();
    }

    public void DeselectHeroOnPillar()
    {
        if (_storedHero.IsUnityNull())
        {
            return;
        }
        
        SelectionController.Instance.ForceHeroButtonPressFromID(_storedHero.GetHeroID());

        if (_storedHero.IsUnityNull())
        {
            AnimateOutHeroOnPillar();
        }
    }

    private void PlayParticlesOfHeroOnPillar()
    {
        _selectedHeroParticles.SetStartColor(_storedHero.GetHeroSelectionParticleColors()[0], _storedHero.GetHeroSelectionParticleColors()[1]);
        _selectedHeroParticles.PlayAllParticleSystems();
    }

    private void StopParticlesOfHeroOnPillar()
    {
        _selectedHeroParticles.StopAllParticleSystems();
    }

    private void SetUpHeroDeselectCanvas()
    {
        _pillarHeroDeselectCanvas.worldCamera = SelectionController.Instance.GetHeroCamera();
    }

    public void AnimateOutHeroOnPillar()
    {
        _heroSpawnAnimator.ResetTrigger(NEW_HERO_HOVER_ANIM_TRIGGER);
        _heroSpawnAnimator.SetTrigger(REMOVE_HERO_ON_PILLAR_ANIM_TRIGGER);
    }

    public void PlayHeroHoverAnimation()
    {
        _heroSpawnAnimator.SetTrigger(NEW_HERO_HOVER_ANIM_TRIGGER);
    }
    
    public void SetHeroPreviewAnimation(bool isHeroSelected)
    {
        _heroSpawnAnimator.SetBool(HERO_PILLAR_SELECTED_ANIM_BOOL,isHeroSelected);
    }
    
    public void StartHeroSelectedAnimation()
    {
        //_heroSpecificAnimator.SetTrigger(HERO_SPECIFIC_SELECTED_ANIM_TRIGGER);
        _heroSpecificAnimator.SetTrigger(HeroVisuals.HERO_SPECIFIC_SELECTED_ANIM_TRIGGER);
    }
    
    public void PlayHeroIdleAnimation()
    {
        //_heroSpecificAnimator.SetBool(HERO_IDLE_ANIM_BOOL,true);
        _heroSpecificAnimator.SetBool(HeroVisuals.HERO_IDLE_ANIM_BOOL,true);
    }

    #region PreviewPillar

    public void ShowPreviewPillar(bool shouldShow)
    {
        //_previewBase.SetActive(shouldShow);
        _pillarAnimator.SetBool(HERO_PILLAR_PREVIEW_SHOW_ANIM_BOOL, shouldShow);
    }

    #endregion

    #region Getters
    public GameObject GetHeroSpawnPoint() => _heroSpawnPoint;
    public HeroSO GetStoredHero() => _storedHero;
    public bool HasStoredHero() => _storedHero != null;
    #endregion
}
