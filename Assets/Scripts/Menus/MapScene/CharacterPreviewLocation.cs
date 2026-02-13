using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterPreviewLocation : MonoBehaviour
{
    [SerializeField] private CurveProgression _scaleProgression;
    
    private CharacterSO _currentCharacterSO;
    private GameObject _currentCharacterPreview;

    private Animator _characterAnimator;

    private bool _hasCreatedCharacterPreview;

    public void PreviewCharacterAndPerformAnimations(CharacterSO characterSO, bool doesSelectedAnim, bool doesIdleAnim)
    {
        PreviewCharacter(characterSO);
        
        if (doesSelectedAnim)
        {
            PerformCharacterSelectedAnimation();
        }

        if (doesIdleAnim)
        {
            PerformCharacterIdleAnimation();
        }
        
    }
    
    public void PreviewCharacter(CharacterSO characterSO)
    {
        //If there is a hero on the pillar remove them
        if (!_currentCharacterPreview.IsUnityNull())
        {
            RemoveCharacterPreview();
        }

        _currentCharacterSO = characterSO;
        if (characterSO is BossSO bossSO)
        {
            PreviewBoss(bossSO);
        }
        else if (characterSO is HeroSO heroSO)
        {
            PreviewHero(heroSO);
        }

        _hasCreatedCharacterPreview = true;
        
        _scaleProgression.StartMovingUpOnCurve();
    }

    private void PreviewBoss(BossSO bossSO)
    {
        if (!_hasCreatedCharacterPreview)
        {
            _currentCharacterPreview = Instantiate(bossSO.GetBossPrefab(), transform);
        }
        
        _characterAnimator = _currentCharacterPreview.GetComponentInChildren<Animator>();
    }
    
    public void PreviewHero(HeroSO heroSO)
    {
        if (!_hasCreatedCharacterPreview)
        {
            //Spawn the hero
            _currentCharacterPreview = Instantiate(heroSO.GetHeroPrefab(), transform);
        }
        
        _characterAnimator = _currentCharacterPreview.GetComponent<Animator>();
        
    }

    public void PerformCharacterSelectedAnimation()
    {
        if (_characterAnimator.IsUnityNull())
        {
            return;
        }
        
        if (_currentCharacterSO is BossSO)
        {
            _characterAnimator.SetTrigger(BossVisuals.BOSS_SPECIFIC_SELECTED_ANIM_TRIGGER);
        }
        else if(_currentCharacterSO is HeroSO)
        {
            _characterAnimator.SetTrigger(HeroVisuals.HERO_SPECIFIC_SELECTED_ANIM_TRIGGER);
        }
        
    }
    
    public void PerformCharacterIdleAnimation()
    {
        if (_characterAnimator.IsUnityNull())
        {
            return;
        }
        
        if (_currentCharacterSO is BossSO)
        {
            _characterAnimator.SetBool(BossVisuals.SPECIFIC_BOSS_IDLE_ANIM_BOOL,true);
        }
        else if(_currentCharacterSO is HeroSO)
        {
            _characterAnimator.SetBool(HeroVisuals.HERO_IDLE_ANIM_BOOL,true);
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PerformCharacterSelectedAnimation();
        }
    }

    public void RemoveCharacterPreview()
    {
        _scaleProgression.StartMovingDownOnCurve();
        //SetHeroPreviewAnimation(false);
    }
}
