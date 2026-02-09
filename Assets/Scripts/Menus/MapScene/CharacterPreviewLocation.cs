using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterPreviewLocation : MonoBehaviour
{
    [SerializeField] private CurveProgression _scaleProgression;
    
    private GameObject _currentCharacterPreview;

    private Animator _characterAnimator;

    private bool _hasCreatedCharacterPreview;


    public void PreviewCharacter(CharacterSO characterSO)
    {
        //If there is a hero on the pillar remove them
        if (!_currentCharacterPreview.IsUnityNull())
        {
            RemoveCharacterPreview();
        }

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
        
        /*
        //Rotates the hero
        _currentHeroVisual.transform.eulerAngles += new Vector3(0,180,0);
        //Sets the stored hero
        _storedHero = heroSO;

        if (_heroSelectedOnPillar == heroSO || heroAlreadySelectedOverride)
        {
            SetHeroPreviewAnimation(true);
            PlayHeroIdleAnimation();
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
        _heroSpawnAnimator.ResetTrigger(REMOVE_HERO_ON_PILLAR_ANIM_TRIGGER);*/
    }
    
    public void RemoveCharacterPreview()
    {
        _scaleProgression.StartMovingDownOnCurve();
        //SetHeroPreviewAnimation(false);
    }
}
