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
    [SerializeField] private Animator _heroSpawnAnimator;

    private GameObject _currentHeroVisual;

    private HeroSO _heroSelectedOnPillar;
    private HeroSO _storedHero;
    
    private const string HERO_SPECIFIC_SELECTED_ANIM_TRIGGER = "G_HeroSelected";
    
    [Space]
    [SerializeField] private Animator _pillarAnimator;

    private const string HERO_PILLAR_MOVE_ANIM_BOOL = "PillarUp";
    private const string HERO_PILLAR_PREVIEW_SHOW_ANIM_BOOL = "PreviewShow";

    private const string NEW_HERO_HOVER_ANIM_TRIGGER = "NewHover";
    private const string REMOVE_HERO_ON_PILLAR_ANIM_TRIGGER = "RemoveHero";
    
    private const string HERO_SELECTED_ANIM_BOOL = "HeroSelected";

    private void Start()
    {
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
        
        //Rotates the hero
        _currentHeroVisual.transform.eulerAngles += new Vector3(0,180,0);
        //Sets the stored hero
        _storedHero = heroSO;

        if (_heroSelectedOnPillar == heroSO || heroAlreadySelectedOverride)
        {
            SetHeroPreviewAnimation(true);
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
        _heroSpawnAnimator.SetTrigger(NEW_HERO_HOVER_ANIM_TRIGGER);
        _heroSpawnAnimator.ResetTrigger(REMOVE_HERO_ON_PILLAR_ANIM_TRIGGER);
    }

    public void HeroSelectedOnPillar()
    {
        _heroSelectedOnPillar = _storedHero;
        
        if(_currentHeroVisual.TryGetComponent<Animator>(out Animator animator))
        {
            StartHeroSelectedAnimation(animator);
        }
    }

    /// <summary>
    /// Removes the current hero from the pillar
    /// </summary>
    public void RemoveHeroOnPillar()
    {
        _storedHero = null;
        SetHeroPreviewAnimation(false);
        Destroy(_currentHeroVisual);
    }

    public void DeselectHeroOnPillar()
    {
        _storedHero = null;
        _heroSelectedOnPillar = null;
        SetHeroPreviewAnimation(false);
    }

    public void AnimateOutHeroOnPillar()
    {
        _heroSpawnAnimator.ResetTrigger(NEW_HERO_HOVER_ANIM_TRIGGER);
        _heroSpawnAnimator.SetTrigger(REMOVE_HERO_ON_PILLAR_ANIM_TRIGGER);
    }
    
    public void SetHeroPreviewAnimation(bool isHeroSelected)
    {
        _heroSpawnAnimator.SetBool(HERO_SELECTED_ANIM_BOOL,isHeroSelected);
    }
    
    public void StartHeroSelectedAnimation(Animator animator)
    {
        animator.SetTrigger(HERO_SPECIFIC_SELECTED_ANIM_TRIGGER);
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
