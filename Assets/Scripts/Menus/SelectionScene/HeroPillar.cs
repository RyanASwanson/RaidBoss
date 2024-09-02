using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPillar : MonoBehaviour
{
    [SerializeField] private GameObject _heroSpawnPoint;
    [SerializeField] private Animator _heroSpawnAnimator;

    private GameObject _currentHeroVisual;

    private HeroSO _storedHero;
    [Space]

    [SerializeField] private Animator _pillarAnimator;

    private const string HERO_PILLAR_MOVE_ANIM_BOOL = "PillarUp";

    private const string NEW_HERO_HOVER_ANIM_TRIGGER = "NewHover";
    private const string REMOVE_HERO_ON_PILLAR_ANIM_TRIGGER = "RemoveHero";

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
        //If there is a hero on the pillar remove them
        if (_currentHeroVisual != null)
            RemoveHeroOnPillar();

        //Spawn the hero onto the pillar
        _currentHeroVisual = Instantiate(heroSO.GetHeroPrefab(), _heroSpawnPoint.transform);
        
        //Rotates the hero
        _currentHeroVisual.transform.eulerAngles += new Vector3(0,180,0);
        //Sets the stored hero
        _storedHero = heroSO;

        if (!newHero) return;

        _heroSpawnAnimator.SetTrigger(NEW_HERO_HOVER_ANIM_TRIGGER);
        _heroSpawnAnimator.ResetTrigger(REMOVE_HERO_ON_PILLAR_ANIM_TRIGGER);
    }

    /// <summary>
    /// Removes the current hero from the pillar
    /// </summary>
    public void RemoveHeroOnPillar()
    {
        _storedHero = null;
        Destroy(_currentHeroVisual);
    }

    public void AnimateOutHeroOnPillar()
    {
        _heroSpawnAnimator.ResetTrigger(NEW_HERO_HOVER_ANIM_TRIGGER);
        _heroSpawnAnimator.SetTrigger(REMOVE_HERO_ON_PILLAR_ANIM_TRIGGER);
    }

    #region Getters
    public GameObject GetHeroSpawnPoint() => _heroSpawnPoint;
    public HeroSO GetStoredHero() => _storedHero;
    public bool HasStoredHero() => _storedHero != null;
    #endregion
}
